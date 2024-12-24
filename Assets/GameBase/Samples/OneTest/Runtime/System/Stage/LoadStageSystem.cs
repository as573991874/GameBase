using System.Collections;
using UnityEngine;

// 资源加载阶段
public class LoadStageSystem : BaseSystem {
    // model
    private LoadAssetModel loadAssetModel;

    // view
    private LoadAsset loadingUI;

    protected override IEnumerator OnLoad() {
        // model
        this.loadAssetModel = new LoadAssetModel();
        this.loadAssetModel.isDone = false;
        this.loadAssetModel.curIndex = 0;
        this.loadAssetModel.loadCount = 5;

        // 准备 view
        this.loadingUI = this.OpenUI<LoadAsset>();
        yield return this.loadingUI.WaitLoad();

        // 完成
        this.LoadFinish();
    }

    protected override void OnStart() {
        this.loadingUI.Inject(this.loadAssetModel);
        this.loadingUI.Open();
        this.StartCoroutine(LoadAsset());
    }

    private IEnumerator LoadAsset() {
        for (var i = this.loadAssetModel.curIndex; i < this.loadAssetModel.loadCount; i++) {
            this.loadAssetModel.curIndex = i;
            yield return new WaitForSeconds(0.45f);
        }
        this.loadAssetModel.isDone = true;
        yield return new WaitForSeconds(0.5f);
        this.eventRunner.Dispath(new AppAssetLoadFinishEvent(){frameCount = Time.frameCount});
    }

    protected override void OnStop() {
        this.loadingUI?.Close();
    }

    protected override void OnTick() {
        this.loadingUI?.Tick();
    }
}
