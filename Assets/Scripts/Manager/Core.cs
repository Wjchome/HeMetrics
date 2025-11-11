using System;
using UnityEngine;

public class Core : MonoBehaviour
{
    public static Core I;
    public static CharacterManager CharacterMgr;
    public static HexMapManager HexMapMgr;
    public static NetManager NetMgr;
    public static StandManager StandMgr;
    public static UIManager UIMgr;
    public static GameManager GameMgr;
    public static CursorManager CursorMgr;
    public static Object2DClickHandler RayCastHandler;
    private void Awake()
    {
        Application.targetFrameRate = 20;

        I = this;
        CharacterMgr = new CharacterManager();
        HexMapMgr = GetComponent<HexMapManager>();
        NetMgr = new NetManager();
        StandMgr = GetComponent<StandManager>();
        UIMgr = new UIManager();
        GameMgr = new GameManager();
        CursorMgr = GetComponent<CursorManager>();
        RayCastHandler = new Object2DClickHandler();
    }

    private void Start()
    {
        NetMgr.Init();
        HexMapMgr.Init();
        StandMgr.Init();
    }

    /// <summary>
    /// 测试，后边需要改成联网
    /// </summary>
    public void Update()
    {
        if (Input.GetKeyDown(KeyCode.Q))
        {
            GameMgr.gameState = (GameState)((int)(GameMgr.gameState + 1) % 2);
            Debug.Log("GameMgr.gameState = " + GameMgr.gameState);
        }

        UpdateFrame();
    }

    private void UpdateFrame()
    {
        NetMgr.UpdateFrame();
        CharacterMgr.UpdateFrame();
    }
}