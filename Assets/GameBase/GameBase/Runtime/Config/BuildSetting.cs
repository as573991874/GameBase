using UnityEngine;

// 打包配置
[CreateAssetMenu(fileName = "BuildSetting", menuName = "GameBase/BuildSetting")]
public class BuildSetting : ScriptableObject {
    [Header("基础包名")]
    public string PackageName;
    [Header("启动场景")]
    public string BootScene;
    [Header("打包图标")]
    public Sprite PackageIcon;
    [Header("IOS AppID")]
    public string IOSAppID;
    [Header("IOS DeveloperID")]
    public string IOSDeveloperID;
    [Header("IOS ProfileID")]
    public string IOSProfileID;
}
