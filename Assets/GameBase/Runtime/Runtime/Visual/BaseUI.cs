using UnityEngine;
using System.Collections;

public abstract class BaseUI : BaseVisual {
    // 对象
    private Transform parent { get; set; }
    protected RectTransform transform;
    protected Canvas canvas;
    protected Camera camera;

    // 基础系统
    public void Inject(AssetSystem assetSystem, EventRunner eventRunner, Transform parent, Canvas canvas, Camera camera) {
        base.Inject(assetSystem, eventRunner);
        this.parent = parent;
        this.canvas = canvas;
        this.camera = camera;
    }

    // 界面加载
    protected override IEnumerator OnLoad() {
        var assetPath = $"{path}";
        var request = assetSystem.LoadAsset<GameObject>(assetPath);
        yield return request.WaitLoad();
        var go = GameObject.Instantiate(request.Asset);
        this.gameObject = go;
        this.transform = this.gameObject.transform as RectTransform;
        this.transform.SetParent(this.parent);
        this.transform.offsetMin = Vector2.zero;
        this.transform.offsetMax = Vector2.one;
        this.gameObject.name = path;
        LoadFinish();
    }
}
