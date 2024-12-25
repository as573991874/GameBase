using UnityEngine;
using System.Collections;
using UnityEngine.UI;
using System;

public class LoadAsset : BaseUI {
    public override string path => "LoadAsset";

    // model
    private AssetModel loadAssetModel;

    // view
    private Text txtInfo;
    private Button btnRetry;
    private Button btnDownload;
    private Button btnEnter;

    public void Inject(AssetModel model) {
        this.loadAssetModel = model;
    }

    // 界面加载
    protected override IEnumerator OnLoad() {
        var load = this.canvas.transform.Find("Loading");
        var go = GameObject.Instantiate(load.gameObject);
        this.gameObject = go;
        this.transform = this.gameObject.transform as RectTransform;
        this.transform.SetParent(this.canvas.transform);
        this.transform.offsetMin = Vector2.zero;
        this.transform.offsetMax = Vector2.one;
        this.gameObject.name = path;
        LoadFinish();
        yield return null;
    }

    // 初始化
    protected override void OnInit() {
        txtInfo = this.transform.Find("Text (Legacy)").GetComponent<Text>();
        this.transform.Find("List").gameObject.SetActive(true);
        btnRetry = this.transform.Find("List/BtnRetry").GetComponent<Button>();
        btnRetry.onClick.AddListener(this.OnRetry);
        btnDownload = this.transform.Find("List/BtnDownload").GetComponent<Button>();
        btnDownload.onClick.AddListener(this.OnDownload);
        btnEnter = this.transform.Find("List/BtnEnter").GetComponent<Button>();
        btnEnter.onClick.AddListener(this.OnEnter);
    }

    private void OnRetry() {
        if (this.loadAssetModel.PackageUpdateManifest) {
            this.eventRunner.Dispath(new AppLoadPackageEvent());
        } else {
            this.eventRunner.Dispath(new AppInitPackageEvent());
        }
    }

    private void OnDownload() {
        this.eventRunner.Dispath(new AppLoadPackageEvent());
    }

    private void OnEnter() {
        this.eventRunner.Dispath(new AppAssetLoadFinishEvent());
    }

    // 打开
    protected override void OnOpen() {
        txtInfo.text = $"正在启动资源加载器";
        this.btnRetry.gameObject.SetActive(false);
        this.btnDownload.gameObject.SetActive(false);
        this.btnEnter.gameObject.SetActive(false);
    }

    // 关闭
    protected override void OnClose() {
    }

    // 更新
    protected override void OnTick() {
        var model = this.loadAssetModel;
        if (model.PackageDownloadFinish) {
            // 已经完成资源更新
            this.btnRetry.gameObject.SetActive(false);
            this.btnDownload.gameObject.SetActive(false);
            this.btnEnter.gameObject.SetActive(true);
            txtInfo.text = $"资源更新完成";
        } else if (model.PackageUpdateManifest) {
            // 已经完成清单更新
            if (!model.PackageDownloading) {
                if (model.PackageDownloadFail) {
                    this.btnRetry.gameObject.SetActive(true);
                    this.btnDownload.gameObject.SetActive(false);
                    this.btnEnter.gameObject.SetActive(false);
                    txtInfo.text = $"资源更新失败";
                } else {
                    this.btnRetry.gameObject.SetActive(false);
                    this.btnDownload.gameObject.SetActive(model.PackageNeedDownload);
                    this.btnEnter.gameObject.SetActive(model.PackageNeedDownload);
                    txtInfo.text = model.PackageNeedDownload ? $"检测到有资源需要更新" : "正在检查资源文件";
                }
            } else {
                this.btnRetry.gameObject.SetActive(false);
                this.btnDownload.gameObject.SetActive(false);
                this.btnEnter.gameObject.SetActive(false);

                var downloader = model.PackageDownloader;
                string currentSizeMB = (downloader.CurrentDownloadBytes / 1048576f).ToString("f1");
                string totalSizeMB = (downloader.TotalDownloadBytes / 1048576f).ToString("f1");
                var info = $"{downloader.CurrentDownloadCount}/{downloader.TotalDownloadCount} {currentSizeMB}MB/{totalSizeMB}MB";
                txtInfo.text = info;
            }
        } else {
            var fail = model.PackageInitializeFail || model.PackageUpdateVersionFail || model.PackageUpdateManifestFail;
            if (model.PackageIniting) {
                txtInfo.text = $"正在启动资源加载器";
                this.btnRetry.gameObject.SetActive(false);
            } else if (fail) {
                txtInfo.text = $"资源初始化失败";
                this.btnRetry.gameObject.SetActive(true);
            } else {
                txtInfo.text = $"正在启动资源加载器";
                this.btnRetry.gameObject.SetActive(false);
            }
        }
    }
}
