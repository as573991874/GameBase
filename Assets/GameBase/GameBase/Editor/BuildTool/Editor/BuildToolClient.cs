// System
using System.IO;
// Unity
using UnityEditor;
using UnityEngine;

public partial class BuildTool {
    /// <summary>
    /// 用于客户端的打包方案
    /// </summary>
    public static void Build2ClientByEnvir() {
        string[] lineArgs = System.Environment.GetCommandLineArgs();
        var args = LoadJson<BuildArgs>(lineArgs[9]);
        if (args == null) {
            Debug.Log("Error : Args parse failed.");
            return;
        }

        buildArgs = args;
        Build2Client();
    }

    public static void Build2Client() {
        Debug.Log("Tip : Begin AssetDatabase.Refresh.");
        AssetDatabase.Refresh();

        // 后续再集成资源打包

        // 打包路径
        string path = buildArgs.path;
        GenerateVersionInfo(buildArgs.buildNum);

        Debug.LogFormat(" ---------- BeginBuild ---------- ");
        // EnableAutoRotate(false);
        if (buildArgs.apkTag.Length > 0) {
            PlayerSettings.productName = buildArgs.packageName + "_" + buildArgs.apkTag;
        } else {
            PlayerSettings.productName = buildArgs.packageName;
        }

        //todo 每次build删除之前的残留
        BuildTarget target = BuildTarget.Android;
        BuildOptions options = BuildOptions.None;
        BuildTargetGroup group = BuildTargetGroup.Android;

        if (buildArgs.isDebug) {
            options |= (BuildOptions.Development | BuildOptions.AllowDebugging);
            // | BuildOptions.ConnectWithProfiler
            PlayerSettings.enableInternalProfiler = true;
            Debug.Log("Tip : Enable Debug Mode.");
        } else {
            PlayerSettings.enableInternalProfiler = false;
        }

        // icon
        string iconPath = AssetDatabase.GetAssetPath(buildSetting.PackageIcon);
        switch (buildArgs.buildType) {
            case "Android":
                target = BuildTarget.Android;
                // options |= BuildOptions.AcceptExternalModificationsToPlayer;
                group = BuildTargetGroup.Android;
                path = Path.Combine(path, "android", buildArgs.packageName + ".apk");
                SetIconsForAndroid(iconPath, UnityEditor.Android.AndroidPlatformIconKind.Adaptive, 1);
                SetIconsForAndroid(iconPath, UnityEditor.Android.AndroidPlatformIconKind.Legacy, 0);
                SetIconsForAndroid(iconPath, UnityEditor.Android.AndroidPlatformIconKind.Round, 0);
                break;
            case "iOS":
                target = BuildTarget.iOS;
                group = BuildTargetGroup.iOS;
                SignedIOS();
                path = Path.Combine(path, "build", buildArgs.packageName);
                break;
            case "StandaloneOSX":
                target = BuildTarget.StandaloneOSX;
                group = BuildTargetGroup.Standalone;
                // PlayerSettings.fullScreenMode = FullScreenMode.Windowed;
                // PlayerSettings.resizableWindow = true;
                // PlayerSettings.defaultScreenHeight = 1334;
                // PlayerSettings.defaultScreenWidth = 750;
                path = Path.Combine(path, "macOS", buildArgs.packageName + ".app");
                break;
            default:
                group = BuildTargetGroup.Standalone;
                target = BuildTarget.StandaloneWindows64;
                path = Path.Combine(path, buildArgs.packageName, buildArgs.packageName + ".exe");
                break;
        }
        SetIconsForTargetGroup(group, iconPath);

        // 删除文件
        if (File.Exists(path)) {
            File.Delete(path);
        }

        // 删除文件夹
        if (Directory.Exists(path)) {
            Directory.Delete(path, true);
        }

        if (buildArgs.isIL2CPP) {
            PlayerSettings.SetScriptingBackend(group, ScriptingImplementation.IL2CPP);
        } else {
            PlayerSettings.SetScriptingBackend(group, ScriptingImplementation.Mono2x);
        }

        Debug.Log("Tip : Begin SwitchActiveBuildTarget.");
        EditorUserBuildSettings.SwitchActiveBuildTarget(group, target);

        // AddressableAssetSettings.BuildPlayerContent();

        Debug.Log("Tip : Begin BuildPipeline BuildPlayer.");

        if (!string.IsNullOrEmpty(buildSetting.BootScene)) {
            string[] scenes = new string[] { buildSetting.BootScene };
            BuildPipeline.BuildPlayer(scenes, path, target, options);
        } else {
            BuildPipeline.BuildPlayer(GetBuildScenes(), path, target, options);
        }

        Debug.LogFormat("----EndBuildClient---------- outputpath:{0}", path);
    }
}
