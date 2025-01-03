// System
using System.Collections.Generic;
using System.IO;
using Newtonsoft.Json;
using Unity.VisualScripting;


// Unity
using UnityEditor;
using UnityEngine;

/// <summary>
/// 打包工具
/// </summary>
public partial class BuildTool {
    /// <summary>
    /// 打包参数
    /// </summary>
    public static BuildArgs buildArgs = new BuildArgs();

    // 打包配置
    public static BuildSetting buildSetting;

    public static void Refresh() {
        // 获取配置
        var configs = AssetDatabase.FindAssets("BuildSetting");
        if (configs.Length > 0) {
            var path = AssetDatabase.GUIDToAssetPath(configs[0]);
            BuildSetting setting = (BuildSetting)AssetDatabase.LoadAssetAtPath(path, typeof(BuildSetting));
            buildSetting = setting;
        } else {
            Debug.LogWarning($"Error : BuildSetting.");
        }
    }

    /// <summary>
    /// 读文件
    /// </summary>
    /// <param name="path"></param>
    /// <returns></returns>
    static string ReadFile(string path) {
        if (!File.Exists(path)) {
            return "";
        }

        StreamReader reader = File.OpenText(path);
        string outText = reader.ReadToEnd();
        reader.Close();
        reader.Dispose();
        return outText;
    }

    /// <summary>
    /// 加载并反序列化json文件
    /// </summary>
    /// <param name="path"></param>
    /// <typeparam name="T"></typeparam>
    /// <returns></returns>
    static T LoadJson<T>(string path) {
        try {
            string jsonRepo = ReadFile(path);
            if (jsonRepo.Length > 0) {
                return JsonConvert.DeserializeObject<T>(jsonRepo);
            }
        } catch {
            Debug.LogError($"Error : LoadJson(\"{path}\").");
        }

        return default(T);
    }

    /// <summary>
    /// 获取打包场景
    /// </summary>
    /// <returns></returns>
    static string[] GetBuildScenes() {
        List<string> names = new List<string>();
        foreach (EditorBuildSettingsScene e in EditorBuildSettings.scenes) {
            if (e == null) {
                continue;
            }
            if (e.enabled) {
                names.Add(e.path);
            }
        }
        return names.ToArray();
    }

    /// <summary>
    /// 生成版本信息文件
    /// </summary>
    /// <param name="buildNum"></param>
    static void GenerateVersionInfo(int buildNum) {
        try {
            var versionInfo = new VersionInfo();
            versionInfo.buildNum = buildNum;
            versionInfo.Save();
        } catch {
        }
    }

    /// <summary>
    /// 设置包体图标
    /// </summary>
    /// <param name="platform"></param>
    /// <param name="pathIcon"></param>
    static void SetIconsForTargetGroup(BuildTargetGroup platform, string pathIcon) {
        if (string.IsNullOrEmpty(pathIcon)) {
            Debug.Log("Tip : SetIconsForTargetGroup - icon empty.");
            return;
        }

        int[] iconSize = PlayerSettings.GetIconSizesForTargetGroup(platform);
        Debug.Log($"Tip : SetIconsForTargetGroup - icon size {iconSize.Length}.");
        if (iconSize.Length <= 0) {
            return;
        }

        Texture2D defaultIcon = AssetDatabase.LoadAssetAtPath<Texture2D>(pathIcon);
        if (defaultIcon == null) {
            Debug.LogError($"Error : SetIconsForTargetGroup failed - {pathIcon}.");
            return;
        }

        Texture2D[] icons = new Texture2D[iconSize.Length];
        for (int i = 0; i < iconSize.Length; i++) {
            icons[i] = defaultIcon;
        }

        PlayerSettings.SetIconsForTargetGroup(platform, icons);
        Debug.Log($"Tip : SetIconsForTargetGroup [{pathIcon}] success.");
    }

    /// <summary>
    /// 设置包体图标（android）
    /// </summary>
    /// <param name="pathIcon"></param>
    /// <param name="kind"></param>
    /// <param name="layer"></param>
    static void SetIconsForAndroid(string pathIcon, PlatformIconKind kind, int layer) {
        PlatformIcon[] androidIcons = PlayerSettings.GetPlatformIcons(BuildTargetGroup.Android, kind);
        Texture2D textureIcon = AssetDatabase.LoadAssetAtPath<Texture2D>(pathIcon);
        for (int i = 0; i < androidIcons.Length; i++) {
            androidIcons[i].SetTexture(textureIcon, layer);
        }

        PlayerSettings.SetPlatformIcons(BuildTargetGroup.Android, kind, androidIcons);
    }

    /// <summary>
    /// 设置是否自动切换横竖屏
    /// </summary>
    /// <param name="bEnable"></param>
    static void EnableAutoRotate(bool bEnable) {
        if (bEnable) {
            PlayerSettings.defaultInterfaceOrientation = UIOrientation.AutoRotation;
            PlayerSettings.allowedAutorotateToLandscapeLeft = true;
            PlayerSettings.allowedAutorotateToLandscapeRight = true;
            PlayerSettings.allowedAutorotateToPortraitUpsideDown = true;
        } else {
            PlayerSettings.defaultInterfaceOrientation = UIOrientation.Portrait;
            PlayerSettings.allowedAutorotateToLandscapeLeft = false;
            PlayerSettings.allowedAutorotateToLandscapeRight = false;
            PlayerSettings.allowedAutorotateToPortraitUpsideDown = false;
        }
    }

    /// <summary>
    /// 设置 ios 签名
    /// </summary>
    static void SignedIOS() {
        PlayerSettings.SetApplicationIdentifier(BuildTargetGroup.iOS, buildSetting.IOSAppID);
        PlayerSettings.iOS.appleEnableAutomaticSigning = false;
        PlayerSettings.iOS.appleDeveloperTeamID = buildSetting.IOSDeveloperID;
        PlayerSettings.iOS.iOSManualProvisioningProfileID = buildSetting.IOSProfileID;
        PlayerSettings.iOS.iOSManualProvisioningProfileType = ProvisioningProfileType.Distribution;
    }

    /// <summary>
    /// 修改 xcplist 文件
    /// </summary>
    /// <param name="filePath"></param>
    static void EditorPlist(string filePath) {
        XCPlist list = new XCPlist(filePath);
        string addPlist = @"    <key>LSUIElement</key> <true />";

        list.AddKey(addPlist);
        list.Save();
    }

    /// <summary>
    /// 文件夹拷贝
    /// </summary>
    /// <param name="sourceFolder"></param>
    /// <param name="targetFolder"></param>
    static void FolderCopy(string sourceFolder, string targetFolder) {
        try {
            if (!Directory.Exists(sourceFolder)) {
                return;
            }

            Debug.Log($"Tip : FolderCopy - [{sourceFolder}] => [{targetFolder}]");

            if (!Directory.Exists(targetFolder)) {
                Directory.CreateDirectory(targetFolder);
            }

            string[] files = Directory.GetFiles(sourceFolder);
            foreach (string file in files) {
                string name = Path.GetFileName(file);
                string dest = Path.Combine(targetFolder, name);
                File.Copy(file, dest);
            }

            string[] folders = Directory.GetDirectories(sourceFolder);
            foreach (string folder in folders) {
                string name = Path.GetFileName(folder);
                string dest = Path.Combine(targetFolder, name);
                FolderCopy(folder, dest);
            }
        } catch {
            Debug.Log("error : TempletDataCopy failed.");
        }
    }
}

/// <summary>
/// 打包参数(会覆盖打包配置)
/// </summary>
public class BuildArgs {
    public int buildNum = 9999999;
    public bool isDebug = false;
    public bool isIL2CPP = false;
    public string buildType = "";
    public string packageName = "";
    public string path = "";
    public string apkTag = "";

    /// <summary>
    /// 解析参数
    /// </summary>
    public void Print() {
        Debug.Log("Tip : ============= args info =============");
        Debug.Log($"buildNum : {buildNum}");
        Debug.Log($"isDebug : {isDebug}");
        Debug.Log($"isIL2CPP : {isIL2CPP}");
        Debug.Log($"buildType : {buildType}");
        Debug.Log($"packageName : {packageName}");
        Debug.Log($"path : {path}");
        Debug.Log($"apkTag : {apkTag}");
        Debug.Log("Tip : ============= args end  =============");
    }

}
