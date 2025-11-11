using System;
using System.Collections.Generic;
using UnityEngine;

public class CharacterManager
{
    private List<Character> characters= new List<Character>();
    private List<Character> battleCharacters= new List<Character>();
    private List<Character> standCharacters= new List<Character>();

    public void UpdateFrame()
    {
        if (Core.GameMgr.gameState != GameState.Fighting)
            return;
        for (int i = 0; i < battleCharacters.Count; i++)
        {
            battleCharacters[i].UpdateFrame();
        }
        
        for (int i = battleCharacters.Count - 1; i >= 0; i--)
        {
            if (battleCharacters[i].isDead)
            {
                battleCharacters[i].LogicDead();
                battleCharacters.RemoveAt(i); // 用 RemoveAt 更高效（无需查找元素）
            }
        }
    }

    public Character GetNearestCharacter(Character character)
    {
        Character nearestCharacter = null;
        int nearestDistance = int.MaxValue;
        foreach (Character character1 in battleCharacters)
        {
            if (character.isMine != character1.isMine)
            {
                int dis = Core.HexMapMgr.GetHexDistance(character.currentCell as HexCell, character1.currentCell as HexCell);
                if (dis < nearestDistance)
                {
                    nearestDistance = dis;
                    nearestCharacter = character1;
                }
            }
        }

        return nearestCharacter;
    }

    public void AddStandCharacter(Character character)
    {
        standCharacters.Add(character);
    }

    public void RemoveStandCharacter(Character character)
    {
        standCharacters.Remove(character);
    }
    public void AddBattleCharacter(Character character)
    {
        battleCharacters.Add(character);
    }

    public void RemoveBattleCharacter(Character character)
    {
        battleCharacters.Remove(character);
    }

    public void ChangeCharacter(Character character,bool isHex)
    {
        if (isHex)
        {
            battleCharacters.Add(character);
            standCharacters.Remove(character);
        }
        else
        {
            battleCharacters.Remove(character);
            standCharacters.Add(character);
        }
    }
}