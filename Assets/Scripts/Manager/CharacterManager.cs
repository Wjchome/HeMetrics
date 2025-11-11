using System;
using System.Collections.Generic;
using UnityEngine;

public class CharacterManager
{
    public List<Character> characters= new List<Character>();

    public void UpdateFrame()
    {
        for (int i = 0; i < characters.Count; i++)
        {
            characters[i].UpdateFrame();
        }
    }

    public Character GetNearestCharacter(Character character)
    {
        Character nearestCharacter = null;
        int nearestDistance = int.MaxValue;
        foreach (Character character1 in characters)
        {
            if (character.isMine != character1.isMine)
            {
                int dis = Core.HexMapMgr.GetHexDistance(character.currentCell, character1.currentCell);
                if (dis < nearestDistance)
                {
                    nearestDistance = dis;
                    nearestCharacter = character1;
                }
            }
        }

        return nearestCharacter;
    }
}