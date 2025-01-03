using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public abstract class BaseSystem {
    // 基础
    protected EventRunner eventRunner;
    private CoroutineRunner coroutine;
    private List<WeakReference<BaseModel>> models = new List<WeakReference<BaseModel>>();
    protected AssetSystem assetSystem;
    protected UISystem uiSystem;
    protected BaseScene baseScene;

    // 是否初始化完成
    public bool isInit { get; private set; }
    // 是否打开
    public bool enabled { get; private set; }

    // 创建数据
    protected T CreateModel<T>() where T : BaseModel, new() {
        var model = new T();
        var weakModel = new WeakReference<BaseModel>(model);
        models.Add(weakModel);
        return model;
    }
    protected T CreateModel<T>(T model) where T : BaseModel, new() {
        var weakModel = new WeakReference<BaseModel>(model);
        models.Add(weakModel);
        return model;
    }

    // 创建系统
    protected T CreateSystem<T>() where T : BaseSystem, new() {
        var system = new T();
        system.Inject(eventRunner);
        system.Inject(assetSystem, uiSystem);
        system.Inject(baseScene);
        return system;
    }

    // 打开 UI
    protected T OpenUI<T>() where T : BaseUI, new() {
        return this.uiSystem.OpenUI<T>(eventRunner);
    }

    // 加载场景
    protected T OpenScene<T>() where T : BaseScene, new() {
        var scene = new T();
        scene.Inject(assetSystem, eventRunner);
        this.baseScene = scene;
        return scene;
    }

    // 打开 actor
    protected T OpenActor<T>() where T : BaseActor, new() {
        if (this.baseScene == null) {
            Debug.LogError("base scene is null");
        }
        var actor = new T();
        actor.Inject(assetSystem, eventRunner, this.baseScene.actorNode);
        return actor;
    }

    // 打开 camera actor
    protected T OpenCamera<T>() where T : BaseActor, new() {
        if (this.baseScene == null) {
            Debug.LogError("base scene is null");
        }
        var actor = new T();
        actor.Inject(assetSystem, eventRunner, this.baseScene.cameraNode);
        return actor;
    }

    // 事件
    public void Inject(EventRunner parent) {
        this.eventRunner = new EventRunner(this, parent);
    }

    // 基础系统
    public void Inject(AssetSystem assetSystem, UISystem uiSystem) {
        this.assetSystem = assetSystem;
        this.uiSystem = uiSystem;
    }

    // 事件
    public void Inject(BaseScene baseScene) {
        this.baseScene = baseScene;
    }

    // 等待加载完成（外部延迟、支持重复加载）
    public IEnumerator WaitLoad() {
        yield return OnLoad();
    }

    protected virtual IEnumerator OnLoad() {
        yield return null;
        this.LoadFinish();
    }
    protected void LoadFinish() {
        if (this.isInit) {
            return;
        }
        this.OnInit();
        this.isInit = true;
    }
    // 初始化只会有一次
    protected virtual void OnInit() { }

    public void Start() {
        if (!this.isInit) {
            Debug.LogError("system not init");
            return;
        }
        if (this.enabled) {
            return;
        }
        this.enabled = true;
        this.eventRunner?.Clear();
        OnStart();
    }
    protected virtual void OnStart() { }

    // 协程
    protected IEnumerator StartCoroutine(IEnumerator enumerator) {
        if (this.coroutine == null) {
            this.coroutine = new CoroutineRunner();
        }
        coroutine.Start(enumerator);
        return enumerator;
    }
    protected void StopCoroutine(IEnumerator enumerator) {
        coroutine?.Stop(enumerator);
    }

    public void Tick() {
        if (this.enabled) {
            for (var i = models.Count - 1; i >= 0; i--) {
                BaseModel model;
                if(models[i].TryGetTarget(out model)){
                    model.Tick();
                } else {
                    models.RemoveAt(i);
                }
            }
            coroutine?.Tick();
            OnTick();
        }
    }
    protected virtual void OnTick() { }

    // 尽量不注销只开关
    public void Stop() {
        if (!this.enabled) {
            return;
        }
        this.enabled = false;
        OnStop();
    }
    protected virtual void OnStop() { }
}
