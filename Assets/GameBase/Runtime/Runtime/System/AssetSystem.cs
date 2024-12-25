using System.Collections;
using UnityEngine;
using UnityEngine.SceneManagement;
using YooAsset;

public class AssetRequest<T> where T : Object {
    private AssetHandle handle;
    public AssetRequest(AssetHandle handle) {
        this.handle = handle;
    }

    public IEnumerator WaitLoad() {
        while (!handle.IsDone) {
            yield return null;
        }
    }

    public T Asset {
        get {
            return handle.AssetObject as T;
        }
    }
}


// 资源系统，管理资源加载
public class AssetSystem : BaseSystem {
    private GameModel gameModel;
    private AssetModel assetModel;
    private GameSetting gameSetting;

    private ResourcePackage package;

    public void Inject(GameModel model) {
        this.gameModel = model;
        this.gameSetting = model.GameSetting;
    }

    protected override IEnumerator OnLoad() {
        this.assetModel = this.CreateModel<AssetModel>();
        this.gameModel.AssetModel = this.assetModel;

        YooAssets.Initialize();
        var packageName = gameSetting.PackageName;
        this.package = YooAssets.TryGetPackage(packageName);
        if (this.package == null)
            this.package = YooAssets.CreatePackage(packageName);
        YooAssets.SetDefaultPackage(this.package);
        yield return null;
        this.LoadFinish();
    }

    // 初始化包流程
    public IEnumerator StartInitPackage() {
        if (this.assetModel.PackageIniting) {
            yield break;
        }
        this.assetModel.PackageIniting = true;
        if (!this.assetModel.PackageInitialize) {
            yield return InitPackage();
        } else if (!this.assetModel.PackageUpdateVersion) {
            yield return UpdatePackageVersion();
        } else if (!this.assetModel.PackageUpdateManifest) {
            yield return UpdateManifest();
        } else {
            // all done
            this.assetModel.PackageIniting = false;
        }
    }

    // 初始化资源包
    private IEnumerator InitPackage() {
        var playMode = gameSetting.AssetPlayMode;
        var packageName = gameSetting.PackageName;
        var buildPipeline = EDefaultBuildPipeline.BuiltinBuildPipeline.ToString();

        this.assetModel.PackageInitializeFail = false;

        // 编辑器下的模拟模式
        InitializationOperation initializationOperation = null;
        if (playMode == EPlayMode.EditorSimulateMode) {
            var simulateBuildResult = EditorSimulateModeHelper.SimulateBuild(buildPipeline, packageName);
            var createParameters = new EditorSimulateModeParameters();
            createParameters.EditorFileSystemParameters = FileSystemParameters.CreateDefaultEditorFileSystemParameters(simulateBuildResult);
            initializationOperation = package.InitializeAsync(createParameters);
        }

        // 单机运行模式
        if (playMode == EPlayMode.OfflinePlayMode) {
            var createParameters = new OfflinePlayModeParameters();
            createParameters.BuildinFileSystemParameters = FileSystemParameters.CreateDefaultBuildinFileSystemParameters();
            initializationOperation = package.InitializeAsync(createParameters);
        }

        // 联机运行模式
        if (playMode == EPlayMode.HostPlayMode) {
            string defaultHostServer = GetHostServerURL();
            string fallbackHostServer = GetHostServerURL();
            IRemoteServices remoteServices = new RemoteServices(defaultHostServer, fallbackHostServer);
            var createParameters = new HostPlayModeParameters();
            createParameters.BuildinFileSystemParameters = FileSystemParameters.CreateDefaultBuildinFileSystemParameters();
            createParameters.CacheFileSystemParameters = FileSystemParameters.CreateDefaultCacheFileSystemParameters(remoteServices);
            initializationOperation = package.InitializeAsync(createParameters);
        }

        // WebGL运行模式
        if (playMode == EPlayMode.WebPlayMode) {
            var createParameters = new WebPlayModeParameters();
            createParameters.WebFileSystemParameters = FileSystemParameters.CreateDefaultWebFileSystemParameters();
            initializationOperation = package.InitializeAsync(createParameters);
        }

        yield return initializationOperation;

        // 如果初始化失败弹出提示界面
        if (initializationOperation.Status != EOperationStatus.Succeed) {
            this.assetModel.PackageIniting = false;
            this.assetModel.PackageInitializeFail = true;
            this.eventRunner.Dispath(new GamePackageInitFailEvent());
        } else {
            this.assetModel.PackageInitialize = true;
            yield return UpdatePackageVersion();
        }
    }

    // 更新资源包版本
    private IEnumerator UpdatePackageVersion() {
        this.assetModel.PackageUpdateVersionFail = false;
        yield return new WaitForSeconds(0.5f);

        var operation = package.RequestPackageVersionAsync();
        yield return operation;

        if (operation.Status != EOperationStatus.Succeed) {
            this.assetModel.PackageIniting = false;
            this.assetModel.PackageUpdateVersionFail = true;
            this.eventRunner.Dispath(new GamePackageInitFailEvent());
        } else {
            this.assetModel.PackageVersion = operation.PackageVersion;
            this.assetModel.PackageUpdateVersion = true;
            yield return UpdateManifest();
        }
    }

    // 更新资源清单
    private IEnumerator UpdateManifest() {
        this.assetModel.PackageUpdateManifestFail = false;
        yield return new WaitForSeconds(0.5f);

        var packageVersion = this.assetModel.PackageVersion;
        var operation = package.UpdatePackageManifestAsync(packageVersion);
        yield return operation;

        if (operation.Status != EOperationStatus.Succeed) {
            this.assetModel.PackageIniting = false;
            this.assetModel.PackageUpdateManifestFail = true;
            this.eventRunner.Dispath(new GamePackageInitFailEvent());
        } else {
            int downloadingMaxNum = 10;
            int failedTryAgain = 3;
            this.assetModel.PackageDownloader = package.CreateResourceDownloader(downloadingMaxNum, failedTryAgain);
            this.assetModel.PackageNeedDownload = this.assetModel.PackageDownloader.TotalDownloadCount > 0;
            this.assetModel.PackageIniting = false;
            this.assetModel.PackageUpdateManifest = true;
            this.eventRunner.Dispath(new GamePackageInitEvent());
        }
    }

    // 更新资源
    public IEnumerator BeginDownload() {
        if (this.assetModel.PackageDownloading) {
            yield break;
        }
        this.assetModel.PackageDownloading = true;
        this.assetModel.PackageDownloadFail = false;

        var downloader = this.assetModel.PackageDownloader;
        downloader.OnDownloadErrorCallback = WebFileDownloadFailed;
        downloader.OnDownloadProgressCallback = DownloadProgressUpdate;
        downloader.BeginDownload();
        yield return downloader;

        // 检测下载结果
        if (downloader.Status != EOperationStatus.Succeed) {
            this.assetModel.PackageDownloading = false;
            this.assetModel.PackageDownloadFail = true;
            this.eventRunner.Dispath(new GamePackageLoadFailEvent());
        } else {
            var operation = package.ClearUnusedBundleFilesAsync();
            operation.Completed += ClearUnusedBundleFilesFinish;
        }
    }

    // 资源下载失败
    void WebFileDownloadFailed(string fileName, string error) {
        this.eventRunner.Dispath(new WebFileDownloadFailEvent() { FileName = fileName, Error = error });
    }

    // 资源下载进度
    void DownloadProgressUpdate(int totalDownloadCount, int currentDownloadCount, long totalDownloadSizeBytes, long currentDownloadSizeBytes) {
        // var msg = new DownloadProgressUpdate();
        // msg.TotalDownloadCount = totalDownloadCount;
        // msg.CurrentDownloadCount = currentDownloadCount;
        // msg.TotalDownloadSizeBytes = totalDownloadSizeBytes;
        // msg.CurrentDownloadSizeBytes = currentDownloadSizeBytes;
    }

    // 清理废弃资源
    private void ClearUnusedBundleFilesFinish(AsyncOperationBase obj) {
        this.assetModel.PackageDownloading = false;
        this.assetModel.PackageDownloadFinish = true;
        this.eventRunner.Dispath(new GamePackageLoadFinishEvent());

    }

    /// <summary>
    /// 获取资源服务器地址
    /// </summary>
    private string GetHostServerURL() {
        string hostServerIP = gameSetting.AssetServerIP;
        string appVersion = gameSetting.Version;

#if UNITY_EDITOR
        if (UnityEditor.EditorUserBuildSettings.activeBuildTarget == UnityEditor.BuildTarget.Android)
            return $"{hostServerIP}/CDN/Android/{appVersion}";
        else if (UnityEditor.EditorUserBuildSettings.activeBuildTarget == UnityEditor.BuildTarget.iOS)
            return $"{hostServerIP}/CDN/IPhone/{appVersion}";
        else if (UnityEditor.EditorUserBuildSettings.activeBuildTarget == UnityEditor.BuildTarget.WebGL)
            return $"{hostServerIP}/CDN/WebGL/{appVersion}";
        else
            return $"{hostServerIP}/CDN/PC/{appVersion}";
#else
        if (Application.platform == RuntimePlatform.Android)
            return $"{hostServerIP}/CDN/Android/{appVersion}";
        else if (Application.platform == RuntimePlatform.IPhonePlayer)
            return $"{hostServerIP}/CDN/IPhone/{appVersion}";
        else if (Application.platform == RuntimePlatform.WebGLPlayer)
            return $"{hostServerIP}/CDN/WebGL/{appVersion}";
        else
            return $"{hostServerIP}/CDN/PC/{appVersion}";
#endif
    }

    /// <summary>
    /// 远端资源地址查询服务类
    /// </summary>
    private class RemoteServices : IRemoteServices {
        private readonly string _defaultHostServer;
        private readonly string _fallbackHostServer;

        public RemoteServices(string defaultHostServer, string fallbackHostServer) {
            _defaultHostServer = defaultHostServer;
            _fallbackHostServer = fallbackHostServer;
        }
        string IRemoteServices.GetRemoteMainURL(string fileName) {
            return $"{_defaultHostServer}/{fileName}";
        }
        string IRemoteServices.GetRemoteFallbackURL(string fileName) {
            return $"{_fallbackHostServer}/{fileName}";
        }
    }

    // 加载资源
    public AssetRequest<T> LoadAsset<T>(string path) where T : Object {
        var handle = YooAssets.LoadAssetAsync<T>(path);
        return new AssetRequest<T>(handle);
    }

    // 加载资源
    public IEnumerator LoadScene(string path) {
        var handle = package.LoadSceneAsync(path);
        while (!handle.IsDone) {
            yield return null;
        }
    }

    protected override void OnTick() {
    }
}
