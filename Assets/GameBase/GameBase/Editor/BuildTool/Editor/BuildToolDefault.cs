// System
using System.IO;

// Unity
using UnityEditor;
using UnityEngine;

public partial class BuildTool {
    /// <summary>
    /// 默认打包方案
    /// </summary>
    static void Build2Default() {
        AssetDatabase.Refresh();
        string[] args = System.Environment.GetCommandLineArgs();

        for (int i = 0; i < args.Length; i++) {
            Debug.LogFormat("args[{0}]={1}\n", i, args[i]);
        }

        string buildType = args[10];
        string packageName = args[11];

        // 打包路径
        string path = args[12];
        bool isDebug = (args.Length > 13) ? bool.Parse(args[13]) : false;

        Debug.LogFormat(" ---------- BeginBuild ---------- ");
        EnableAutoRotate(false);
        PlayerSettings.productName = packageName;

        //todo 每次build删除之前的残留
        BuildTarget target = BuildTarget.Android;
        BuildOptions options = BuildOptions.None;
        BuildTargetGroup group = BuildTargetGroup.Android;

        if (isDebug) {
            options |= BuildOptions.Development;
            PlayerSettings.enableInternalProfiler = true;
        }

        switch (buildType) {
            case "Android":
                target = BuildTarget.Android;

                // 生成Android APK
                EditorUserBuildSettings.androidBuildSubtarget = MobileTextureSubtarget.ETC;
                EditorUserBuildSettings.androidBuildSystem = AndroidBuildSystem.Gradle;
                path = Path.Combine(path, "android", packageName + ".apk");
                break;
            case "iOS":
                target = BuildTarget.iOS;
                group = BuildTargetGroup.iOS;
                SignedIOS();
                path = Path.Combine(path, "build", packageName);
                break;
            default:
                group = BuildTargetGroup.Standalone;
                target = BuildTarget.StandaloneWindows64;
                path = Path.Combine(path, packageName, packageName + ".exe");
                break;
        }

        EditorUserBuildSettings.SwitchActiveBuildTarget(group, target);
        BuildPipeline.BuildPlayer(GetBuildScenes(), path, target, options);
        Debug.LogFormat("----EndBuildAndroid---------- outputpath:{0}", path);
    }
}
