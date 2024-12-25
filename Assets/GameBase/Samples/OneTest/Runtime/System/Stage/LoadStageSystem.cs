using System.Collections;
using UnityEngine;

// 资源加载阶段
public class LoadStageSystem : BaseSystem {
    // model
    private GameModel gameModel;

    // view
    private LoadAsset loadingUI;

    public void Inject(GameModel gameModel) {
        this.gameModel = gameModel;
    }

    protected override IEnumerator OnLoad() {
        // 准备 view
        this.loadingUI = this.OpenUI<LoadAsset>();
        yield return this.loadingUI.WaitLoad();

        // 完成
        this.LoadFinish();
    }

    protected override void OnStart() {
        this.loadingUI.Inject(this.gameModel.AssetModel);
        this.loadingUI.Open();
        this.eventRunner.Dispath(new AppInitPackageEvent());
    }

    protected override void OnStop() {
        this.loadingUI?.Close();
    }

    protected override void OnTick() {
        this.loadingUI?.Tick();
        var model = this.gameModel.AssetModel;
        model.DirtyCheck("PackageUpdateManifest", UpdateManifest, model.PackageUpdateManifest);
    }

    void UpdateManifest(bool finish){
        Debug.Log($"{finish} zsk {this.gameModel.AssetModel.PackageNeedDownload}");
        if(finish && !this.gameModel.AssetModel.PackageNeedDownload){
            this.eventRunner.Dispath(new AppAssetLoadFinishEvent());
        }
    }
}
