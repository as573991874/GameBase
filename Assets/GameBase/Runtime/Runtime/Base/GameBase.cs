using System.Collections;
using UnityEngine;

public class GameBase : MonoBehaviour
{
    private CoroutineRunner coroutine;
    private EventRunner eventRunner;
    private AssetSystem assetSystem;
    private UISystem uiSystem;

    void Awake()
    {
        DontDestroyOnLoad(this);
        eventRunner = new EventRunner(this);
        coroutine = new CoroutineRunner();
        coroutine.Start(Load());
    }

    // 创建系统
    T CreateSystem<T>() where T : BaseSystem, new()
    {
        var system = new T();
        system.Inject(eventRunner);
        system.Inject(assetSystem, uiSystem);
        return system;
    }

    // 异步加载
    protected virtual IEnumerator Load()
    {
        // model

        // system
        this.assetSystem = CreateSystem<AssetSystem>();
        this.uiSystem = CreateSystem<UISystem>();

        // 初始化资源系统
        yield return this.assetSystem.WaitLoad();
        this.assetSystem.Start();

        // 初始化表现节点
        var request = this.assetSystem.LoadAsset<GameObject>("App/Base");
        yield return request.WaitLoad();
        GameObject baseObj = GameObject.Instantiate(request.Asset, this.transform);
        baseObj.name = "Base";

        var ui = baseObj.transform.Find("Canvas");
        var camera = baseObj.transform.Find("Main Camera");
        this.uiSystem.Inject(ui, camera);
        yield return this.uiSystem.WaitLoad();
        this.uiSystem.Start();

        // 初始化
        Init();
    }

    protected virtual void Init()
    {
        // 监听事件
        this.eventRunner.Listen<GameBaseInitEvent>();
        // 派发事件
        this.eventRunner.Dispath(new GameBaseInitEvent() { frameCount = Time.frameCount });
    }

    // Update is called once per frame
    protected virtual void Update()
    {
        // 基础
        this.coroutine.Tick();
        // 事件
        this.eventRunner.HandleOne<GameBaseInitEvent>(InitFinish);
        // 系统
        this.assetSystem?.Tick();
        this.uiSystem?.Tick();
    }

    protected virtual void InitFinish(GameBaseInitEvent eventData)
    {
    }

    protected virtual void FixedUpdate()
    {
    }

    protected virtual void LateUpdate()
    {
    }

    protected virtual void OnDestroy()
    {
        this.assetSystem = null;
        this.uiSystem = null;
    }

    void OnApplicationQuit()
    {
        OnDestroy();
    }
}
