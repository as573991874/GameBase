using System.Collections;

public class App : GameBase {
    // system
    private StageSystem stageSystem;


    // 异步加载
    protected override IEnumerator Load() {
        // base
        yield return base.LoadBase();

        // system
        this.stageSystem = CreateSystem<StageSystem>();
        this.stageSystem.Inject(this.transform, this.gameModel);
        yield return this.stageSystem.WaitLoad();

        // 初始化
        Init();
    }

    protected override void Init() {
        base.Init();
        this.eventRunner.Listen<AppInitPackageEvent>();
        this.eventRunner.Listen<AppLoadPackageEvent>();
    }

    // Update is called once per frame
    protected override void Update() {
        base.Update();
        this.eventRunner.HandleOne<AppInitPackageEvent>(InitPackage);
        this.eventRunner.HandleOne<AppLoadPackageEvent>(LoadPackage);
        this.stageSystem?.Tick();
    }

    protected override void InitFinish(GameBaseInitEvent eventData) {
        // 初始化完成进入打开场景切换系统
        this.stageSystem.Start();
    }

    // 初始化资源包
    private void InitPackage(AppInitPackageEvent eventData) {
        this.coroutine.Start(this.assetSystem.StartInitPackage());
    }

    // 更新资源包
    private void LoadPackage(AppLoadPackageEvent eventData) {
        this.coroutine.Start(this.assetSystem.BeginDownload());
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
