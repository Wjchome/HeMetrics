using System.Collections.Generic;
using UnityEngine;

public class BondUILogic
{
    public void ChangeCharacter(Dictionary<BondType, List<Character>> myActiveBonds, Dictionary<BondType,List<Character>> enemyActiveBonds)
    {
        Core.UIMgr.GetUI<BondShowWin>().UpdateBondList(myActiveBonds,enemyActiveBonds);
    }
}