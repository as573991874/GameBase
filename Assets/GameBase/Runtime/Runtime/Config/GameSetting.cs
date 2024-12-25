using UnityEngine;
using YooAsset;

[CreateAssetMenu(fileName = "GameSetting", menuName = "GameBase/GameSetting")]
public class GameSetting : ScriptableObject {
    [Header("包版本")]
    public string Version;
    [Header("基础节点名")]
    public string BaseNode;
    [Header("基础包名")]
    public string PackageName;
    [Header("资源运行模式")]
    public EPlayMode AssetPlayMode;
    [Header("基础包名")]
    public string AssetServerIP;
}
