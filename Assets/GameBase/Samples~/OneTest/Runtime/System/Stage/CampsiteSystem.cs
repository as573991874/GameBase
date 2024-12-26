using System.Collections;
using UnityEngine;

// 营地系统
public class CampsiteSystem : BaseSystem {
    // model
    private PlayerModel playerModel;

    // view
    private CampsiteUI campsiteUI;
    private CampsiteScene campsiteScene;

    // system

    public void Inject(PlayerModel model) {
        this.playerModel = model;
    }

    // 加载
    protected override IEnumerator OnLoad() {
        // model
        // view
        this.campsiteUI = this.OpenUI<CampsiteUI>();
        this.campsiteUI.Inject(this.playerModel);
        yield return this.campsiteUI.WaitLoad();
        this.campsiteScene = this.OpenScene<CampsiteScene>();
        yield return this.campsiteScene.WaitLoad();
        // 加载完成
        this.LoadFinish();
    }

    // 加载完成
    protected override void OnInit() {
        // 监听事件
    }

    // 打开系统
    protected override void OnStart() {
        this.campsiteUI?.Open();
        this.campsiteScene?.Open();
    }

    // 关闭系统
    protected override void OnStop() {
        this.campsiteUI?.Close();
        this.campsiteScene?.Close();
    }

    // 系统运行时
    protected override void OnTick() {
        // 表现
        this.campsiteUI?.Tick();
        this.campsiteScene?.Tick();
    }
}
