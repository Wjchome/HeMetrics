using System.Collections.Generic;
using Unity.VisualScripting;
using UnityEngine;


public class StandManager : MonoBehaviour
{
    public Transform startPoint;
    public Transform endPoint;
    public Transform startPoint1;
    public Transform endPoint1;
    public int standNum;
    public StandCell standCellPrefab;

    public List<StandCell> myStandCells = new List<StandCell>();
    public List<StandCell> otherStandCells;


    public void Init()
    {
        for (int i = 0; i < standNum; i++)
        {
            myStandCells.Add(Instantiate(standCellPrefab, Vector3.Lerp(
                startPoint.position, endPoint.position, (float)i / standNum), Quaternion.identity, transform));

            Character character = null;
            if (i % 4 == 0||i % 4 == 1)
            {
                character = Instantiate(Core.I.meleeCharacterPrefab, myStandCells[i].transform.position,
                    Quaternion.identity);
            }
            else
            {
                character = Instantiate(Core.I.rangedCharacterPrefab, myStandCells[i].transform.position,
                    Quaternion.identity);
            }


            Core.CharacterMgr.AddStandCharacter(character);
            character.currentCell = myStandCells[i];
            myStandCells[i].characterOn = character;
            character.isMine = i % 2 == 0;
            if (character.isMine)
                character.GetComponent<Renderer>().material.color = Color.cyan;
        }
    }
}