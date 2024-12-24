using UnityEngine;
using System.Collections;

public abstract class BaseActor : BaseVisual {
    // 对象
    private Transform parent { get; set; }
    public Transform transform;

    public void Inject(AssetSystem assetSystem, EventRunner eventRunner, Transform parent) {
        base.Inject(assetSystem, eventRunner);
        this.parent = parent;
    }

    // 界面加载
    protected override IEnumerator OnLoad() {
        var assetPath = $"{path}";
        var request = assetSystem.LoadAsset<GameObject>(assetPath);
        yield return request.WaitLoad();
        this.gameObject = GameObject.Instantiate(request.Asset);
        this.transform = this.gameObject.transform;
        this.transform.SetParent(this.parent);
        this.transform.position = Vector3.zero;
        this.gameObject.name = path;
        LoadFinish();
    }
}
