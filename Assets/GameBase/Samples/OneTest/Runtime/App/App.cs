using System.Collections;
using UnityEngine;

public class App : MonoBehaviour {
    private CoroutineRunner coroutine;
    private EventRunner eventRunner;

    // model
    private AppVersion appVersion;

    // system
    private AssetSystem assetSystem;
    private UISystem uiSystem;
    private StageSystem stageSystem;

    void Awake() {
        DontDestroyOnLoad(this);
        eventRunner = new EventRunner(this);
        coroutine = new CoroutineRunner();
        coroutine.Start(Load());
    }

    // 创建系统
    T CreateSystem<T>() where T : BaseSystem, new() {
        var system = new T();
        system.Inject(eventRunner);
        system.Inject(assetSystem, uiSystem);
        return system;
    }

    // 异步加载
    IEnumerator Load() {
        // model
        this.appVersion = new AppVersion();
        this.appVersion.Load();

        // system
        this.assetSystem = CreateSystem<AssetSystem>();
        this.uiSystem = CreateSystem<UISystem>();
        this.stageSystem = CreateSystem<StageSystem>();

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

        this.stageSystem.Inject(this.transform);
        yield return this.stageSystem.WaitLoad();

        // 初始化
        Init();
    }

    void Init() {
        // 监听事件
        this.eventRunner.Listen<AppInitEvent>();
        // 派发事件
        this.eventRunner.Dispath(new AppInitEvent() { frameCount = Time.frameCount });
    }

    // Update is called once per frame
    void Update() {
        // 基础
        this.coroutine.Tick();
        // 事件
        this.eventRunner.HandleOne<AppInitEvent>(AppInitFinish);
        // 系统
        this.assetSystem?.Tick();
        this.uiSystem?.Tick();
        this.stageSystem?.Tick();
    }

    void AppInitFinish(AppInitEvent eventData) {
        // 初始化完成进入打开场景切换系统
        // Debug.Log($"App Init Finish: {eventData.frameCount}");
        this.stageSystem.Start();
    }

    void FixedUpdate() {
        this.stageSystem?.FixedTick();
    }

    void LateUpdate() {
        this.stageSystem?.LateTick();
    }

    void OnDestroy() {
        this.assetSystem = null;
        this.uiSystem = null;
        this.stageSystem = null;
    }

    void OnApplicationQuit() {
        OnDestroy();
    }
}
