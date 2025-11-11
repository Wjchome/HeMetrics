using System;
using System.Collections.Generic;
using UnityEngine;

public class CharacterManager
{
    public List<Character> characters= new List<Character>();

    public void UpdateFrame()
    {
        if (Core.GameMgr.gameState != GameState.Fighting)
            return;
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
}