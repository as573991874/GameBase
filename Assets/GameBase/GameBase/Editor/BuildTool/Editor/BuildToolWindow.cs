using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEditor;
using Newtonsoft.Json;
using Newtonsoft.Json.Linq;

public class BuildToolWindow : EditorWindow {
    private string[] helpInfos = new string[] {
            "一个好的程序员是那种过单行线马路都要往两边看的人。",
            "程序有问题时不要担心。如果所有东西都没问题，你就失业了。",
            "程序员的麻烦在于，你无法弄清他在捣腾什么，当你最终弄明白时，也许已经晚了。",
            "编程时要保持这种心态：就好象将来要维护你这些代码的人是一位残暴的精神病患者，而且他知道你住在哪。",
            "如果建筑工人像程序员写软件那样盖房子，那第一只飞来的啄木鸟就能毁掉人类文明。",
            "如果没能一次成功，那就叫它1.0版吧。",
            "工作进度上越早落后，你就会有越充足的时间赶上。",
            "为什么我们没有时间把事情做对，却总有时间把事情做过头？",
            "傻瓜都能写出计算机能理解的程序。优秀的程序员写出的是人类能读懂的代码。",
            "软件就像做爱。一次犯错，你需要用余下一生来维护支持。",
            "软件设计最困难的部分…是阻挡新功能的引入。",
            "理论上，理论和实践是没有差异的。但实践中，是有的。",
            "评估一个事情要比去理解你评估了什么容易。",
            "拷贝-粘贴是一种设计错误。",
            "这不是个bug——这一个未注明的功能特征。",
            "小心上面代码中的bug；我只知道这些代码是正确的，但没有试过。"
        };
    private int helpIndex = 0;

    // 打包类型
    private string[] buildObjects = new string[] { "客户端", "服务端" };
    private int buildObjectIndex;
    private string[] buildClientTypes = new string[] { "Android", "iOS", "StandaloneOSX", "StandaloneWindows64" };
    private string[] buildServerTypes = new string[] { "StandaloneOSX", "Linux64" };
    private int buildIndex;

    // 默认打包版本号
    private int buildNum;

    // 是否是调试模式
    private bool isDebug;

    // 是否是IL2CPP模式
    private bool isIL2CPP;

    // 打包名称
    private string packageName;

    // 打包路径
    private string path = "";

    // 打包标签
    private string apkTag;

    [MenuItem("工具集/项目打包工具/打开打包配置", false, 803)]
    internal static void OpenBuildToolWindow() {
        GetWindow<BuildToolWindow>("项目打包工具");
    }

    private void OnEnable() {
        Refresh();
    }

    private void Refresh() {
        BuildTool.Refresh();
        buildIndex = EditorPrefs.GetInt("BuildTool_BuildIndex", 0);
        buildObjectIndex = EditorPrefs.GetInt("BuildTool_BuildObjectIndex", 0);
        buildNum = EditorPrefs.GetInt("BuildTool_BuildNum", 1);
        isIL2CPP = EditorPrefs.GetBool("BuildTool_IL2CPP", false);
        isDebug = EditorPrefs.GetBool("BuildTool_Debug", false);
        packageName = EditorPrefs.GetString("BuildTool_PackageName", BuildTool.buildSetting.PackageName);
        apkTag = EditorPrefs.GetString("BuildTool_ApkTag", "test");
        path = EditorPrefs.GetString("BuildTool_Path", System.Environment.CurrentDirectory + "/Build/");
        helpIndex = Random.Range(0, helpInfos.Length);
    }

    private void OnGUI() {
        EditorGUILayout.HelpBox(helpInfos[helpIndex], MessageType.Info);

        EditorGUILayout.BeginHorizontal();
        int i = EditorGUILayout.Popup("打包类型：", buildIndex, buildObjectIndex == 0 ? buildClientTypes : buildServerTypes);
        if(i != buildIndex) {
            buildIndex = i;
            EditorPrefs.SetInt("BuildTool_BuildIndex", i);
        }
        i = EditorGUILayout.Popup(buildObjectIndex, buildObjects);
        if (i != buildObjectIndex) {
            buildObjectIndex = i;
            buildIndex = 0;
            EditorPrefs.SetInt("BuildTool_BuildObjectIndex", buildObjectIndex);
            EditorPrefs.SetInt("BuildTool_BuildIndex", 0);
        }
        EditorGUILayout.EndHorizontal();

        EditorGUILayout.BeginHorizontal();
        var name = EditorGUILayout.TextField("打包名称：", packageName);
        if (name != packageName) {
            packageName = name;
            EditorPrefs.SetString("BuildTool_PackageName", packageName);
        }
        EditorGUILayout.EndHorizontal();

        EditorGUILayout.BeginHorizontal();
        var tag = EditorGUILayout.TextField("打包标签：", apkTag);
        if (tag != apkTag) {
            apkTag = tag;
            EditorPrefs.SetString("BuildTool_ApkTag", apkTag);
        }
        EditorGUILayout.EndHorizontal();

        EditorGUILayout.BeginHorizontal();
        var packPath = EditorGUILayout.TextField("打包路径：", path);
        if (packPath != path) {
            path = packPath;
            EditorPrefs.SetString("BuildTool_Path", path);
        }
        EditorGUILayout.EndHorizontal();

        EditorGUILayout.BeginHorizontal();
        var num = EditorGUILayout.IntField("打包版本号：", buildNum);
        if (num != buildNum) {
            buildNum = num;
            EditorPrefs.SetInt("BuildTool_BuildNum", buildNum);
        }
        EditorGUILayout.EndHorizontal();

        EditorGUILayout.BeginHorizontal();
        var il2cpp = EditorGUILayout.Toggle("使用 IL2CPP：", isIL2CPP);
        if (il2cpp != isIL2CPP){
            isIL2CPP = il2cpp;
            EditorPrefs.SetBool("BuildTool_IL2CPP", isIL2CPP);
        }
        EditorGUILayout.EndHorizontal();

        EditorGUILayout.BeginHorizontal();
        var debug = EditorGUILayout.Toggle("是否 DEBUG：", isDebug);
        if (debug != isDebug){
            isDebug = debug;
            EditorPrefs.SetBool("BuildTool_Debug", isDebug);
        }
        EditorGUILayout.EndHorizontal();

        if (GUILayout.Button("虔诚祈祷，打包无忧")) {
            Debug.Log("打包中...");
            helpIndex = Random.Range(0, helpInfos.Length);
            BuildTool.buildArgs.buildNum = buildNum;
            BuildTool.buildArgs.isDebug = isDebug;
            BuildTool.buildArgs.isIL2CPP = isIL2CPP;
            BuildTool.buildArgs.packageName = packageName;
            BuildTool.buildArgs.apkTag = apkTag;
            BuildTool.buildArgs.path = path;
            BuildTool.buildArgs.buildType = buildObjectIndex == 0 ? buildClientTypes[buildIndex] : buildServerTypes[buildIndex];
            if (buildObjectIndex == 0) {
                BuildTool.Build2Client();
            } else {
                BuildTool.Build2Server();
            }
        }
    }
}
