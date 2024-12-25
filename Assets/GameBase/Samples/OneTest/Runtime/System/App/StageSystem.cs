using System.Collections;
using UnityEngine;

// 流程系统，应用流程切换
public class StageSystem : BaseSystem {
    // model
    private AppStageModel appStageModel;

    // view
    private Transform root;
    private Loading loadingUI;

    // system
    private LoadStageSystem loadStageSystem;
    private LoginStageSystem loginStageSystem;
    private MainStageSystem mainStageSystem;
    private BaseSystem curSystem;

    public void Inject(Transform root) {
        this.root = root;
    }

    protected override IEnumerator OnLoad() {
        // 准备 model
        this.appStageModel = new AppStageModel();
        this.appStageModel.stage = AppStageEnum.Init;

        // 准备 view
        this.loadingUI = this.OpenUI<Loading>();
        yield return this.loadingUI.WaitLoad();

        // 准备系统
        this.loadStageSystem = CreateSystem<LoadStageSystem>();
        this.loginStageSystem = CreateSystem<LoginStageSystem>();
        this.mainStageSystem = CreateSystem<MainStageSystem>();

        // 加载完成
        this.LoadFinish();
    }

    protected override void OnInit() {
        this.eventRunner.Listen<AppAssetLoadFinishEvent>();
    }

    protected override void OnStart() {
        SwitchStage(AppStageEnum.Load);
    }

    // 资源加载完成
    void AssetLoadFinish(AppAssetLoadFinishEvent eventData) {
        SwitchStage(AppStageEnum.Main);
    }

    // 切换情景
    private void SwitchStage(AppStageEnum stage) {
        if (stage == this.appStageModel.stage) {
            return;
        }
        this.eventRunner.Dispath(new AppStageSwitchEvent() { stage = stage, frameCount = Time.frameCount });
        if (this.appStageModel.switchCoroutine != null) {
            this.StopCoroutine(this.appStageModel.switchCoroutine);
        }
        this.appStageModel.switchCoroutine = this.StartCoroutine(SwitchStageFlow(stage));
    }

    // 切换流程
    private IEnumerator SwitchStageFlow(AppStageEnum curStage) {
        // var lastStage = this.appStageModel.stage;
        var lastTime = Time.time;
        this.appStageModel.stage = curStage;
        this.appStageModel.switching = true;
        // 打开 loading 界面
        this.loadingUI.Inject(this.appStageModel);
        this.loadingUI.Open();
        // 推出之前的情景
        this.curSystem?.Stop();
        // 进入新的情景
        switch (curStage) {
            case AppStageEnum.Load:
                this.curSystem = this.loadStageSystem;
                break;
            case AppStageEnum.Login:
                this.curSystem = this.loginStageSystem;
                break;
            case AppStageEnum.Main:
                this.curSystem = this.mainStageSystem;
                break;
            default:
                Debug.LogError("switch stage error");
                break;
        }
        yield return this.curSystem?.WaitLoad();
        // 至少等待 1s
        var diff = lastTime + 1 - Time.time;
        if (diff > 0f) {
            yield return new WaitForSeconds(diff);
        }
        // 完成切换
        this.curSystem?.Start();
        this.loadingUI.Close();
        this.appStageModel.switching = false;
        this.appStageModel.switchCoroutine = null;
        this.eventRunner.Dispath(new AppStageSwitchFinishEvent() { stage = curStage, frameCount = Time.frameCount });
    }

    protected override void OnTick() {
        // 事件
        this.eventRunner.HandleOne<AppAssetLoadFinishEvent>(AssetLoadFinish);
        // 系统
        this.loadingUI?.Tick();
        this.loadStageSystem?.Tick();
        this.loginStageSystem?.Tick();
        this.mainStageSystem?.Tick();
    }

    public void FixedTick() {
    }

    public void LateTick() {
    }
}
