// System
using System.IO;

// Unity
using UnityEditor;
using UnityEngine;

#pragma warning disable CS0618 // Type or member is obsolete

public partial class BuildTool {
    /// <summary>
    /// 用于打战场服务器包的方案
    /// </summary>
    public static void Build2ServerByEnvir() {
        string[] lineArgs = System.Environment.GetCommandLineArgs();
        var args = LoadJson<BuildArgs>(lineArgs[9]);
        if (args == null) {
            Debug.Log("Error : Args parse failed.");
            return;
        }
        buildArgs = args;
        Build2Server();
    }

    public static void Build2Server() {
        string packageName = buildArgs.packageName;

        // 打包路径
        string path = buildArgs.path;
        GenerateVersionInfo(buildArgs.buildNum);

        Debug.LogFormat(" ---------- BeginBuildServer ---------- ");
        PlayerSettings.productName = packageName;

        BuildTarget target = BuildTarget.StandaloneLinux64;
        BuildOptions options = BuildOptions.None;
        BuildTargetGroup group = BuildTargetGroup.Standalone;

        if (buildArgs.isDebug) {
            options |= (BuildOptions.Development | BuildOptions.AllowDebugging);
        }

        switch (buildArgs.buildType) {
            case "StandaloneOSX":
                target = BuildTarget.StandaloneOSX;
                options = BuildOptions.None;
                // 考虑区分平台(目前都由重签名实现)
                PlayerSettings.fullScreenMode = FullScreenMode.Windowed;
                path = Path.Combine(path, "macOS", buildArgs.packageName + ".app");
                break;
            case "Linux64":
                target = BuildTarget.StandaloneLinux64;
                options |= BuildOptions.EnableHeadlessMode;

                PlayerSettings.fullScreenMode = FullScreenMode.Windowed;
                path = Path.Combine(path, $"{packageName}/{packageName}");
                break;
            default:
                target = BuildTarget.StandaloneWindows;
                path = Path.Combine(path, buildArgs.packageName, buildArgs.packageName + ".exe");
                break;
        }

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

        if (!string.IsNullOrEmpty(buildSetting.BootScene)) {
            string[] scenes = new string[] { buildSetting.BootScene };
            BuildPipeline.BuildPlayer(scenes, path, target, options);
        } else {
            BuildPipeline.BuildPlayer(GetBuildScenes(), path, target, options);
        }

        switch (buildArgs.buildType) {
            case "StandaloneOSX": {
                    EditorPlist(path + "/Contents/");
                    var sourcePath = Path.Combine(Application.dataPath, "Generated/TempletBattle/Binary");
                    var TargetPath = Path.Combine(path, "Contents", "Generated/TempletBattle/Binary");
                    FolderCopy(sourcePath, TargetPath);
                }
                break;
            case "Linux64": {
                    var sourcePath = Path.Combine(Application.dataPath, "Generated/TempletBattle/Binary");
                    var TargetPath = Path.Combine(path + "_Data", "Generated/TempletBattle/Binary");
                    FolderCopy(sourcePath, TargetPath);
                }
                break;
            default:
                break;
        }

        Debug.LogFormat("----EndBuildServer---------- outputpath:{0}", path);
    }
}
