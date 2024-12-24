using System.Collections;
using UnityEngine;

// UI 系统，管理 ui 创建
public class UISystem : BaseSystem {
    // view
    private Transform root;
    private Transform cameraNode;
    private Canvas canvas;
    private Camera camera;

    public void Inject(Transform root, Transform camera) {
        this.root = root;
        this.canvas = this.root.GetComponent<Canvas>();
        this.cameraNode = camera;
        this.camera = camera.GetComponent<Camera>();
    }

    protected override IEnumerator OnLoad() {
        this.root.gameObject.SetActive(false);
        yield return null;
        this.LoadFinish();
    }

    protected override void OnStart() {
        this.root.gameObject.SetActive(true);
    }

    // 加载资源
    public T OpenUI<T>(EventRunner eventRunner) where T : BaseUI, new() {
        T ui = new T();
        ui.Inject(assetSystem, eventRunner, this.root, this.canvas, this.camera);
        return ui;
    }
}
