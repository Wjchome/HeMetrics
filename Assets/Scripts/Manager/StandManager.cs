using System.Collections.Generic;
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

    public Character characterpre;

    public int characterNum = 3;

    public void Init()
    {
        for (int i = 0; i < standNum; i++)
        {
            myStandCells.Add(Instantiate(standCellPrefab, Vector3.Lerp(
                startPoint.position, endPoint.position, (float)i / standNum), Quaternion.identity, transform));
            if (i < characterNum)
            {
                var character = Instantiate(characterpre, myStandCells[i].transform.position,
                    Quaternion.identity);

                Core.CharacterMgr.AddStandCharacter(character);
                character.currentCell = myStandCells[i];
                myStandCells[i].characterOn = character;
                character.isMine = i % 2 == 0;
                if (character.isMine)
                    character.GetComponent<Renderer>().material.color = Color.cyan;
            }
        }
    }
}