
    using System.Collections.Generic;
    using UnityEngine;

    public class BondManager:MonoBehaviour
    {
        Dictionary<BondType,List<int>>  bondsConfig = new Dictionary<BondType, List<int>>();
        Dictionary<BondType,List<int>>  currentBonds = new Dictionary<BondType, List<int>>();

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

        public void AddBond(BondType bondType, int id)
        {
            if (!currentBonds.TryGetValue(bondType, out List<int> list))
            {
                list = new List<int>();
                currentBonds[bondType] = list; 
            }
            list.Add(id); 
        }
        public void RemoveBond(BondType bondType, int id)
        {
            currentBonds[bondType].Remove(id);
            if (currentBonds[bondType].Count == 0)
            {
                currentBonds.Remove(bondType);
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
                    AddBond(bondType,character.id);
                }
            }
            else
            {
                foreach (var bondType in character.bondTypes)
                {
                    RemoveBond(bondType,character.id);
                }
            }
            Core.LogicMgr.bondLogic.ChangeCharacter(character,isHex);
        }
        
        /// <summary>
        /// 获取当前激活的羁绊信息
        /// </summary>
        /// <returns>羁绊类型和对应的角色数量</returns>
        public Dictionary<BondType, int> GetActiveBonds()
        {
            Dictionary<BondType, int> activeBonds = new Dictionary<BondType, int>();
            foreach (var kvp in currentBonds)
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
