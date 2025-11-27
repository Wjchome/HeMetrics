using System.Collections.Generic;
using UnityEngine;

public class BondUILogic
{
    public void ChangeCharacter(Dictionary<BondType, List<int>> myActiveBonds, Dictionary<BondType,List<int>> enemyActiveBonds)
    {
        Core.UIMgr.GetUI<BondShowWin>().UpdateBondList(myActiveBonds,enemyActiveBonds);
    }
}