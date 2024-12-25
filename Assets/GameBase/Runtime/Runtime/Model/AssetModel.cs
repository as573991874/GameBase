using YooAsset;

// 资源包信息
public class AssetModel : BaseModel
{
    // 资源包是否完成初始化
    private bool packageInitialize = false;
    public bool PackageInitialize { get { return packageInitialize; } set => SetField(ref packageInitialize, value); }

    // 包初始化中
    public bool PackageIniting = false;

    // 资源包是否初始化失败
    private bool packageInitializeFail = false;
    public bool PackageInitializeFail { get { return packageInitializeFail; } set => SetField(ref packageInitializeFail, value); }

    // 资源包是否完成版本更新
    private bool packageUpdateVersion = false;
    public bool PackageUpdateVersion { get { return packageUpdateVersion; } set => SetField(ref packageUpdateVersion, value); }

    // 资源包是否更新版本失败
    private bool packageUpdateVersionFail = false;
    public bool PackageUpdateVersionFail { get { return packageUpdateVersionFail; } set => SetField(ref packageUpdateVersionFail, value); }

    // 资源包版本
    private string packageVersion;
    public string PackageVersion { get { return packageVersion; } set => SetField(ref packageVersion, value); }

    // 资源包是否完成清单更新
    private bool packageUpdateManifest = false;
    public bool PackageUpdateManifest { get { return packageUpdateManifest; } set => SetField(ref packageUpdateManifest, value); }

    // 资源包是否更新清单失败
    private bool packageUpdateManifestFail = false;
    public bool PackageUpdateManifestFail { get { return packageUpdateManifestFail; } set => SetField(ref packageUpdateManifestFail, value); }

    // 资源包是否需要更新
    private bool packageNeedDownload = false;
    public bool PackageNeedDownload { get { return packageNeedDownload; } set => SetField(ref packageNeedDownload, value); }

    // 下载器
    public ResourceDownloaderOperation PackageDownloader;

    // 包下载中
    public bool PackageDownloading = false;

    // 资源包是否下载完成
    private bool packageDownloadFinish = false;
    public bool PackageDownloadFinish { get { return packageDownloadFinish; } set => SetField(ref packageDownloadFinish, value); }

    // 资源包是否下载完成
    private bool packageDownloadFail = false;
    public bool PackageDownloadFail { get { return packageDownloadFail; } set => SetField(ref packageDownloadFail, value); }
}
