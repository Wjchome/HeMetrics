
    using System.Collections.Generic;
    using UnityEngine;

    public class BondManager:MonoBehaviour
    {
        Dictionary<BondType,List<int>>  bondsConfig = new Dictionary<BondType, List<int>>();
        Dictionary<BondType,List<Character>>  currentBonds = new Dictionary<BondType, List<Character>>();
        Dictionary<BondType,List<Character>>  currentEnemyBonds = new Dictionary<BondType, List<Character>>();

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

        public void AddBond(BondType bondType, Character id,bool isMine)
        {
            Dictionary<BondType,List<Character>> foo =null;
            if (isMine)
            {
                foo = currentBonds;
            }
            else
            {
                foo = currentEnemyBonds;
            }
            
            if (!foo.TryGetValue(bondType, out List<Character> list))
            {
                list = new List<Character>();
                foo[bondType] = list; 
            }
            list.Add(id); 
        }
        public void RemoveBond(BondType bondType, Character id,bool isMine)
        {
            Dictionary<BondType,List<Character>> foo =null;
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
                    AddBond(bondType,character,character.isMine);
                }
            }
            else
            {
                foreach (var bondType in character.bondTypes)
                {
                    RemoveBond(bondType,character,character.isMine);
                }
            }
            // 获取当前激活的羁绊信息
            var myActiveBonds = GetMyActiveBonds();
            var enemyActiveBonds = GetEnemyActiveBonds();

            List<Character> battleCharacters = Core.CharacterMgr.GetBattleCharacters();
            //重新算羁绊
            foreach (var battleCharacter in battleCharacters)
            {
                battleCharacter.attributeManager = new AttributeManager();
                
            }

            foreach (var kv in currentBonds)
            {
                BondData bondData = Core.dataMgr.BondData()[kv.Key];
                int lay = -1;
                foreach (var level in bondData.Level )
                {
                    if (kv.Value.Count >= level)
                    {
                        lay = level;
                    }
                }

                if (lay != -1)
                {
                    foreach (var _character in kv.Value)
                    {
                        _character.attributeManager.Add(kv.Key.ToString(),"羁绊",lay);
                    }
                }

                 
                
            }

            

           
            Core.LogicMgr.BondUILogic.ChangeCharacter(myActiveBonds,enemyActiveBonds);
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
        
        
    }
