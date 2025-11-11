using System;
using UnityEngine;

public class Core : MonoBehaviour
{
    public static Core I;
    public static CharacterManager CharacterMgr;
    public static HexMapManager HexMapMgr;
    public static NetManager NetMgr;
    public static StandManager standMgr;
    private void Awake()
    {
        Application.targetFrameRate = 20;

        I = this;
        CharacterMgr = new CharacterManager();
        HexMapMgr = GetComponent<HexMapManager>();
        NetMgr = new NetManager();
        standMgr = GetComponent<StandManager>();
    }

    private void Start()
    {
        NetMgr.Init();
        HexMapMgr.Init();
        standMgr.Init();
    }

    /// <summary>
    /// 测试，后边需要改成联网
    /// </summary>
    public void Update()
    {
        UpdateFrame();
    }

    private void UpdateFrame()
    {
        NetMgr.UpdateFrame();
        CharacterMgr.UpdateFrame();
    }
}