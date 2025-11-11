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

    public void Init()
    {
        for (int i = 0; i < standNum; i++)
        {
            myStandCells.Add(Instantiate(standCellPrefab, Vector3.Lerp(
                startPoint.position, endPoint.position, (float)i / standNum), Quaternion.identity, transform));
            if (i == 0)
            {
                Core.CharacterMgr.characters.Add(Instantiate(characterpre, myStandCells[0].transform.position,
                    Quaternion.identity));
            }
        }
    }
}