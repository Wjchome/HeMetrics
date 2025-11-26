using System.Collections.Generic;
using UnityEngine;

public class BondManager : MonoBehaviour
{
    Dictionary<BondType, List<int>> bondsConfig = new Dictionary<BondType, List<int>>();
    Dictionary<BondType, List<Character>> currentBonds = new Dictionary<BondType, List<Character>>();
    Dictionary<BondType, List<Character>> currentEnemyBonds = new Dictionary<BondType, List<Character>>();

    private void AddBondConfig(BondType bondType, int id)
    {
        if (!bondsConfig.TryGetValue(bondType, out List<int> list))
        {
            list = new List<int>();
            bondsConfig[bondType] = list;
        }

        list.Add(id);
    }

    private void AddBondConfig(List<BondType> bondTypes, int id)
    {
        foreach (var bondType in bondTypes)
        {
            AddBondConfig(bondType, id);
        }
    }

    private void AddBond(BondType bondType, Character id, bool isMine)
    {
        Dictionary<BondType, List<Character>> foo = null;
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

    private void RemoveBond(BondType bondType, Character id, bool isMine)
    {
        Dictionary<BondType, List<Character>> foo = null;
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
            AddBondConfig(data.BondList, data.Id);
        }
    }


    public void ChangeCharacter(Character character, bool isHex)
    {
        if (isHex)
        {
            foreach (var bondType in character.bondTypes)
            {
                AddBond(bondType, character, character.isMine);
            }
        }
        else
        {
            foreach (var bondType in character.bondTypes)
            {
                RemoveBond(bondType, character, character.isMine);
            }
        }

        // 获取当前激活的羁绊信息
        var myActiveBonds = GetMyActiveBonds();
        var enemyActiveBonds = GetEnemyActiveBonds();

        List<Character> battleCharacters = Core.CharacterMgr.GetBattleCharacters();

        // 重新计算羁绊 - 先清除所有角色的羁绊buff
        foreach (var battleCharacter in battleCharacters)
        {
          battleCharacter.attributeManager.RemoveBySourcePrefix("羁绊");
        }

        // 应用我方羁绊
        ApplyBondBuffsFromBuffList(currentBonds, battleCharacters);

        // 应用敌方羁绊
        ApplyBondBuffsFromBuffList(currentEnemyBonds, battleCharacters);

        // 更新所有角色的属性
        foreach (var battleCharacter in battleCharacters)
        {
            battleCharacter.UpdateAttributes();
        }


        Core.LogicMgr.BondUILogic.ChangeCharacter(myActiveBonds, enemyActiveBonds);
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
    /// 应用羁绊Buff到角色列表（使用BondData.BuffList）
    /// </summary>
    private void ApplyBondBuffsFromBuffList(Dictionary<BondType, List<Character>> bonds,
        List<Character> allBattleCharacters)
    {
        foreach (var kv in bonds)
        {
            BondType bondType = kv.Key;
            List<Character> characters = kv.Value;

            if (characters == null || characters.Count == 0)
            {
                continue;
            }

            BondData bondData = Core.dataMgr.BondData()[bondType];
            if (bondData == null)
            {
                continue;
            }

            // 计算羁绊等级（找到满足条件的最高等级）
            int bondLevel = GetBondLevel(bondData, characters.Count);

            if (bondLevel > 0)
            {
                // 为每个角色应用羁绊Buff（从BuffList中读取）
                foreach (var character in characters)
                {
                    if (character == null || character.isDead)
                    {
                        continue;
                    }

                    BondBuffHandler.ApplyBondBuffs(character, bondData, bondLevel);
                }
            }
        }
    }

    /// <summary>
    /// 根据羁绊数量和配置确定羁绊等级
    /// </summary>
    private int GetBondLevel(BondData bondData, int bondCount)
    {
        if (bondData == null || bondData.Level == null || bondData.Level.Count == 0)
        {
            return 0;
        }

        // 找到满足条件的最高等级
        int maxLevel = 0;
        for (int i = 0; i < bondData.Level.Count; i++)
        {
            if (bondCount >= bondData.Level[i])
            {
                maxLevel = i + 1; // 等级从1开始
            }
            else
            {
                break;
            }
        }

        return maxLevel;
    }
}