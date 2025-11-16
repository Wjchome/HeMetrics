using System.Collections.Generic;
using UnityEngine;

public class BondLogic
{
    public void ChangeCharacter(Dictionary<BondType, int> myActiveBonds, Dictionary<BondType, int> enemyActiveBonds)
    {
        Core.UIMgr.bondShow.UpdateBondList(myActiveBonds,enemyActiveBonds);
    }
}