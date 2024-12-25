using UnityEngine;
using System.Collections;

public abstract class BaseScene : BaseVisual {
    // 对象
    protected Transform transform;
    public Transform actorNode;
    public Transform cameraNode;

    // 界面加载
    protected override IEnumerator OnLoad() {
        var scenePath = $"{path}";
        yield return assetSystem.LoadScene(scenePath);
        this.gameObject = new GameObject("__Root");
        this.transform = this.gameObject.transform;
        var go = new GameObject("ActorNode");
        go.transform.SetParent(this.transform);
        this.actorNode = go.transform;
        go = new GameObject("CameraNode");
        go.transform.SetParent(this.transform);
        this.cameraNode = go.transform;
        LoadFinish();
    }
}
