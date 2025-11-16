using System.Collections.Generic;
using UnityEngine;

public class BondLogic
{
    public void ChangeCharacter(Character character, bool isHex)
    {
        // 获取当前激活的羁绊信息
        var myActiveBonds = Core.bondMgr.GetMyActiveBonds();
        var enemyActiveBonds = Core.bondMgr.GetEnemyActiveBonds();
        
        
        Core.UIMgr.bondShow.UpdateBondList(myActiveBonds,enemyActiveBonds);
        
    }
}