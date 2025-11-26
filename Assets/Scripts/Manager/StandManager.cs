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

            var data = Core.dataMgr.CharacterData()[i + 1];
            
            // 使用统一的Character预制体，攻击行为会根据CharacterType自动创建
            Character character = Instantiate(Core.I.characterPrefab, myStandCells[i].transform.position,
                Quaternion.identity);
            character.Init(i + 1);
        
            Core.CharacterMgr.AddStandCharacter(character);
            character.currentCell = myStandCells[i];
            myStandCells[i].characterOn = character;
            character.isMine = i % 2 == 0;
            if (character.isMine)
                character.GetComponent<Renderer>().material.color = new Color(1,1,1,0.5f);
            else
                character.GetComponent<Renderer>().material.color = new Color(1,1,1,1);
           
        }
    }
}