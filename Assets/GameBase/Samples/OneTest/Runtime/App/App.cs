using System.Collections;
using UnityEngine;

public class App : GameBase {
    // model
    private AppVersion appVersion;

    // system
    private StageSystem stageSystem;


    // 异步加载
    protected override IEnumerator Load() {
        // base
        yield return base.LoadBase();

        // model
        this.appVersion = new AppVersion();
        this.appVersion.Load();

        // system
        this.stageSystem = CreateSystem<StageSystem>();
        this.stageSystem.Inject(this.transform);
        yield return this.stageSystem.WaitLoad();

        // 初始化
        Init();
    }

    protected override void Init() {
        base.Init();
    }

    // Update is called once per frame
    protected override void Update() {
        base.Update();
        this.stageSystem?.Tick();
    }

    protected override void InitFinish(GameBaseInitEvent eventData) {
        // 初始化完成进入打开场景切换系统
        this.stageSystem.Start();
    }

    protected override void FixedUpdate() {
        base.FixedUpdate();
        this.stageSystem?.FixedTick();
    }

    protected override void LateUpdate() {
        base.LateUpdate();
        this.stageSystem?.LateTick();
    }

    protected override void OnDestroy() {
        base.OnDestroy();
        this.stageSystem = null;
    }
}
