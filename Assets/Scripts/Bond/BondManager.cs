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

    public List<int> GetAllIdFromBond(BondType bondType)
    {
        return bondsConfig[bondType];
    }

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


    private Dictionary<BondType, List<Character>> GetBondDict(bool isMine)
    {
        return isMine ? currentBonds : currentEnemyBonds;
    }

    private void AddBond(BondType bondType, Character character, bool isMine)
    {
        var bondDict = GetBondDict(isMine);

        if (!bondDict.TryGetValue(bondType, out List<Character> list))
        {
            list = new List<Character>();
            bondDict[bondType] = list;
        }

        list.Add(character);
    }

    private void RemoveBond(BondType bondType, Character character, bool isMine)
    {
        var bondDict = GetBondDict(isMine);

        if (bondDict.TryGetValue(bondType, out List<Character> list))
        {
            list.Remove(character);
            if (list.Count == 0)
            {
                bondDict.Remove(bondType);
            }
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

        // 重新计算羁绊数量（去重，因为相同的角色不能算2个羁绊）
        // 注意：Clear()必须在循环外部，否则每次循环都会清空整个字典！
        currentBondCount.Clear();
        currentEnemyBondCount.Clear();

        // 计算我方羁绊数量（去重）
        CalculateBondCount(currentBonds, currentBondCount);

        // 计算敌方羁绊数量（去重）
        CalculateBondCount(currentEnemyBonds, currentEnemyBondCount);

        List<Character> battleCharacters = Core.CharacterMgr.GetBattleCharacters();

        // 处理羁绊Buff
        // 重新计算羁绊 - 先清除所有角色的羁绊buff
        foreach (var battleCharacter in battleCharacters)
        {
            if (battleCharacter.buffManager == null)
            {
                battleCharacter.buffManager = new BuffManager(battleCharacter);
            }
            else
            {
                // 移除所有羁绊来源的Buff（通过BuffManager）
                battleCharacter.buffManager.RemoveBuffsBySource("羁绊");
            }
        }

        // 应用羁绊Buff（通过BuffManager）
        ApplyBondBuffsFromBuffList(currentBondCount, currentBonds);
        ApplyBondBuffsFromBuffList(currentEnemyBondCount, currentEnemyBonds);

        // 更新所有角色的属性
        foreach (var battleCharacter in battleCharacters)
        {
            battleCharacter.UpdateAttributes();
        }

        //更新UI
        Core.LogicMgr.BondUILogic.ChangeCharacter(currentBondCount, currentEnemyBondCount);
    }

    private void ApplyBondBuffsFromBuffList(Dictionary<BondType, List<int>> bonds,
        Dictionary<BondType, List<Character>> bondCharacter)
    {
        foreach (var (bondType, characters) in bondCharacter)
        {
            BondData bondData = Core.dataMgr.BondData()[bondType];


            // 计算羁绊等级（找到满足条件的最高等级）
            int bondLevel = GetBondLevel(bondData, bonds[bondType].Count);

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
    /// 计算羁绊数量（去重，使用HashSet提高性能）
    /// </summary>
    private void CalculateBondCount(Dictionary<BondType, List<Character>> bondDict,
        Dictionary<BondType, List<int>> bondCountDict)
    {
        foreach (var kv in bondDict)
        {
            BondType bondType = kv.Key;
            List<Character> characters = kv.Value;

            // 使用HashSet去重，提高性能
            HashSet<int> uniqueIds = new HashSet<int>();
            foreach (var character in characters)
            {
                if (character != null && !character.isDead)
                {
                    uniqueIds.Add(character.id);
                }
            }

            // 转换为List存储
            bondCountDict[bondType] = new List<int>(uniqueIds);
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