using System.Collections.Generic;
using UnityEngine;

public class BondManager : MonoBehaviour
{
    //每种羁绊对应的角色id
    Dictionary<BondType, List<int>> bondsConfig = new Dictionary<BondType, List<int>>();

    //现在的羁绊对应的棋盘上的角色
    Dictionary<BondType, List<Character>> currentBonds = new Dictionary<BondType, List<Character>>();
    Dictionary<BondType, List<Character>> currentEnemyBonds = new Dictionary<BondType, List<Character>>();

    //现在的羁绊已经触发了几个,id是什么
    Dictionary<BondType, List<int>> currentBondCount = new Dictionary<BondType, List<int>>();
    Dictionary<BondType, List<int>> currentEnemyBondCount = new Dictionary<BondType, List<int>>();

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


    public void Init()
    {
        foreach (var data in Core.dataMgr.CharacterData().DataList)
        {
            AddBondConfig(data.BondList, data.Id);
        }
    }


    private void AddBond(BondType bondType, Character character, bool isMine)
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

        list.Add(character);
    }

    private void RemoveBond(BondType bondType, Character character, bool isMine)
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

        foo[bondType].Remove(character);
        if (foo[bondType].Count == 0)
        {
            foo.Remove(bondType);
        }
    }


    //这个角色增加到棋盘上或者离开棋盘
    public void ChangeCharacter(Character character, bool isHex)
    {
        //处理Bond->list<character>
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
        //处理Bond->list<int>  list<int>不重复,因为相同的角色不能算2个羁绊
        foreach (var kv in currentBonds)
        {
            currentBondCount.Clear();
            if (!currentBondCount.ContainsKey(kv.Key))
            {
                currentBondCount.Add(kv.Key, new List<int>());
            }

            foreach (var _character in kv.Value)
            {
                if (currentBondCount[kv.Key].Contains(_character.id))
                {
                }
                else
                {
                    currentBondCount[kv.Key].Add(_character.id);
                }
            }
        }
        foreach (var kv in currentEnemyBonds)
        {
            currentEnemyBondCount.Clear();
            if (!currentEnemyBondCount.ContainsKey(kv.Key))
            {
                currentEnemyBondCount.Add(kv.Key, new List<int>());
            }

            foreach (var _character in kv.Value)
            {
                if (currentEnemyBondCount[kv.Key].Contains(_character.id))
                {
                }
                else
                {
                    currentEnemyBondCount[kv.Key].Add(_character.id);
                }
            }
        }

        List<Character> battleCharacters = Core.CharacterMgr.GetBattleCharacters();

        // 处理羁绊
        // 重新计算羁绊 - 先清除所有角色的羁绊buff
        foreach (var battleCharacter in battleCharacters)
        {
            battleCharacter.attributeManager.RemoveBySourcePrefix("羁绊");
        }
        ApplyBondBuffsFromBuffList(currentBondCount, currentBonds,battleCharacters);
        ApplyBondBuffsFromBuffList(currentEnemyBondCount, currentEnemyBonds,battleCharacters);
        // 更新所有角色的属性
        foreach (var battleCharacter in battleCharacters)
        {
            battleCharacter.UpdateAttributes();
        }

        //更新UI
        Core.LogicMgr.BondUILogic.ChangeCharacter(currentBondCount, currentEnemyBondCount);
    }


    /// <summary>
    /// 应用羁绊Buff到角色列表（使用BondData.BuffList）
    /// </summary>
    private void ApplyBondBuffsFromBuffList(Dictionary<BondType, List<int>> bonds,Dictionary<BondType, List<Character>> bondCharacter,
        List<Character> allBattleCharacters)
    {
        foreach (var kv in bondCharacter)
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
            int bondLevel = GetBondLevel(bondData,  bonds[bondType].Count);

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