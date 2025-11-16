using System.Collections.Generic;
using UnityEngine;

public class BondLogic
{
    public void ChangeCharacter(Character character, bool isHex)
    {
        // 获取当前激活的羁绊信息
        var activeBonds = Core.bondMgr.GetActiveBonds();
        
        // 更新UI显示
        if (Core.UIMgr.bondShow != null)
        {
            Core.UIMgr.bondShow.UpdateBondList(activeBonds);
        }
    }
}