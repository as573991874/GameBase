using System.Collections;
using UnityEngine;

public class GameBase : MonoBehaviour {
    protected GameModel gameModel;
    protected CoroutineRunner coroutine;
    protected EventRunner eventRunner;
    protected AssetSystem assetSystem;
    protected UISystem uiSystem;

    void Awake() {
        DontDestroyOnLoad(this);
        eventRunner = new EventRunner(this);
        coroutine = new CoroutineRunner();
        coroutine.Start(Load());
    }

    // 创建系统
    protected T CreateSystem<T>() where T : BaseSystem, new() {
        var system = new T();
        system.Inject(eventRunner);
        system.Inject(assetSystem, uiSystem);
        return system;
    }

    // 异步加载
    protected virtual IEnumerator Load() {
        yield return LoadBase();
        Init();
    }

    // 异步加载
    protected IEnumerator LoadBase() {
        // model
        this.gameModel = new GameModel();
        this.gameModel.GameSetting = Resources.Load<GameSetting>("GameSetting");

        // node
        var assetObj = Resources.Load<GameObject>(this.gameModel.GameSetting.BaseNode);
        GameObject baseObj = GameObject.Instantiate(assetObj, this.transform);
        baseObj.name = "Base";

        // asset system
        this.assetSystem = CreateSystem<AssetSystem>();
        this.assetSystem.Inject(this.gameModel);
        yield return this.assetSystem.WaitLoad();
        this.assetSystem.Start();

        // ui system
        var ui = baseObj.transform.Find("Canvas");
        var camera = baseObj.transform.Find("Main Camera");
        this.uiSystem = CreateSystem<UISystem>();
        this.uiSystem.Inject(ui, camera);
        yield return this.uiSystem.WaitLoad();
        this.uiSystem.Start();
    }

    protected virtual void Init() {
        // 监听事件
        this.eventRunner.Listen<GameBaseInitEvent>();
        // 派发事件
        this.eventRunner.Dispath(new GameBaseInitEvent() { frameCount = Time.frameCount });
    }

    // Update is called once per frame
    protected virtual void Update() {
        // 基础
        this.gameModel.Tick();
        this.coroutine.Tick();
        // 事件
        this.eventRunner.HandleOne<GameBaseInitEvent>(InitFinish);
        // 系统
        this.assetSystem?.Tick();
        this.uiSystem?.Tick();
    }

    protected virtual void InitFinish(GameBaseInitEvent eventData) {
    }

    protected virtual void FixedUpdate() {
    }

    protected virtual void LateUpdate() {
    }

    protected virtual void OnDestroy() {
        this.assetSystem = null;
        this.uiSystem = null;
    }

    void OnApplicationQuit() {
        OnDestroy();
    }
}
