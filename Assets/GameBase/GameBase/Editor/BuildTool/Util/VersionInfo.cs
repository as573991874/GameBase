using UnityEngine;
using System.IO;

/// <summary>
/// 用于写入版本信息
/// </summary>
public class VersionInfo {
    public int buildNum;

    public void Save() {
        string saveRoot = Path.Combine(Application.dataPath, "Resources");
        string savePath = Path.Combine(saveRoot, nameof(VersionInfo) + ".txt");
        string strJson = JsonUtility.ToJson(this);

        if (!Directory.Exists(saveRoot)) {
            Directory.CreateDirectory(saveRoot);
        }
        File.WriteAllText(savePath, strJson);
    }
}
