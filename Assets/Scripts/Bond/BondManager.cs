
    using System.Collections.Generic;
    using UnityEngine;

    public class BondManager:MonoBehaviour
    {
        Dictionary<BondType,List<int>>  bondsConfig = new Dictionary<BondType, List<int>>();
        Dictionary<BondType,List<int>>  currentBonds = new Dictionary<BondType, List<int>>();
        Dictionary<BondType,List<int>>  currentEnemyBonds = new Dictionary<BondType, List<int>>();

        public void AddBondConfig(BondType bondType, int id)
        {
            if (!bondsConfig.TryGetValue(bondType, out List<int> list))
            {
                list = new List<int>();
                bondsConfig[bondType] = list; 
            }
            list.Add(id); 
        }
        public void AddBondConfig(List<BondType> bondTypes, int id)
        {
            foreach (var bondType in bondTypes)
            {
                AddBondConfig(bondType, id);
            }
        }

        public void AddBond(BondType bondType, int id,bool isMine)
        {
            Dictionary<BondType,List<int>> foo =null;
            if (isMine)
            {
                foo = currentBonds;
            }
            else
            {
                foo = currentEnemyBonds;
            }
            
            if (!foo.TryGetValue(bondType, out List<int> list))
            {
                list = new List<int>();
                foo[bondType] = list; 
            }
            list.Add(id); 
        }
        public void RemoveBond(BondType bondType, int id,bool isMine)
        {
            Dictionary<BondType,List<int>> foo =null;
            if (isMine)
            {
                foo = currentBonds;
            }
            else
            {
                foo = currentEnemyBonds;
            }
            foo[bondType].Remove(id);
            if (foo[bondType].Count == 0)
            {
                foo.Remove(bondType);
            }
        }

        public void Init()
        {
            foreach (var data in Core.dataMgr.CharacterData().DataList)
            {
                Core.bondMgr.AddBondConfig(data.BondList,data.Id);
            }
        }


        public void ChangeCharacter(Character character, bool isHex)
        {
            if (isHex)
            {
                foreach (var bondType in character.bondTypes)
                {
                    AddBond(bondType,character.id,character.isMine);
                }
            }
            else
            {
                foreach (var bondType in character.bondTypes)
                {
                    RemoveBond(bondType,character.id,character.isMine);
                }
            }
            Core.LogicMgr.bondLogic.ChangeCharacter(character,isHex);
        }
        
      
        public Dictionary<BondType, int> GetMyActiveBonds()
        {
            Dictionary<BondType, int> activeBonds = new Dictionary<BondType, int>();
            foreach (var kvp in currentBonds)
            {
                activeBonds[kvp.Key] = kvp.Value.Count;
            }
            return activeBonds;
        }
        
        public Dictionary<BondType, int> GetEnemyActiveBonds()
        {
            Dictionary<BondType, int> activeBonds = new Dictionary<BondType, int>();
            foreach (var kvp in currentEnemyBonds)
            {
                activeBonds[kvp.Key] = kvp.Value.Count;
            }
            return activeBonds;
        }
        
        /// <summary>
        /// 获取羁绊配置中该羁绊类型需要的总角色数量
        /// </summary>
        public int GetBondConfigCount(BondType bondType)
        {
            if (bondsConfig.TryGetValue(bondType, out List<int> list))
            {
                return list.Count;
            }
            return 0;
        }
    }
