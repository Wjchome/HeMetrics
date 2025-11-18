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
    public static BulletManager BulletMgr;
    public static Bullet1Manager Bullet1Mgr;
    public static LuBanDataManager dataMgr;
    public static BondManager bondMgr;
    public static LogicManager LogicMgr;
    
    public Bullet bulletPrefab;
    public Bullet1 bullet1Prefab;
    
    // 统一的Character预制体（不再需要3个不同的预制体）
    public Character characterPrefab;
    private void Awake()
    {
        Application.targetFrameRate = 20;

        I = this;
        dataMgr = new LuBanDataManager();
        CharacterMgr = new CharacterManager();
        HexMapMgr = GetComponent<HexMapManager>();
        NetMgr = new NetManager();
        StandMgr = GetComponent<StandManager>();
        UIMgr =GetComponent<UIManager>();
        GameMgr = new GameManager();
        CursorMgr = GetComponent<CursorManager>();
        RayCastHandler = new Object2DClickHandler();
        BulletMgr = new BulletManager();
        Bullet1Mgr = new Bullet1Manager();
        bondMgr = GetComponent<BondManager>();
        LogicMgr = new LogicManager();
    }

    private void Start()
    {
        dataMgr.Init();
        NetMgr.Init();
        HexMapMgr.Init();
        StandMgr.Init();
        bondMgr.Init();
        UIMgr.Init();
        LogicMgr.Init();
        
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

        // 更新点击和拖动处理
        RayCastHandler.Update();
        
        UpdateFrame();
    }

    private void UpdateFrame()
    {
        NetMgr.UpdateFrame();
        BulletMgr.UpdateFrame();
        Bullet1Mgr.UpdateFrame();
        CharacterMgr.UpdateFrame();
    }
}