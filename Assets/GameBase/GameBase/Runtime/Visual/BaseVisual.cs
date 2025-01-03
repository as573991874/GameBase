using UnityEngine;
using System.Collections;

public abstract class BaseVisual {
    protected EventRunner eventRunner;
    private CoroutineRunner coroutine;
    // 后续替换成加载接口
    protected AssetSystem assetSystem;

    // 资源路径
    public abstract string path { get; }
    // 是否开始加载
    public bool isLoad { get; private set; }
    // 是否初始化完成
    public bool isInit { get; private set; }
    // 是否打开
    public bool isOpen { get; private set; }

    // 对象（在这边规划节点）
    protected GameObject gameObject;

    // 基础系统
    public void Inject(AssetSystem assetSystem, EventRunner eventRunner) {
        this.assetSystem = assetSystem;
        this.eventRunner = new EventRunner(this, eventRunner);
    }

    // 启动协程
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

    // 等待加载完成（外部延迟、不支持重复加载）
    public IEnumerator WaitLoad() {
        if (!this.isLoad) {
            this.isLoad = true;
            yield return OnLoad();
        }
    }

    // 界面加载
    protected virtual IEnumerator OnLoad() {
        yield return null;
        LoadFinish();
    }

    // 加载完成初始化
    protected void LoadFinish() {
        if (this.isInit) {
            return;
        }
        OnInit();
        isInit = true;
        if (isOpen) {
            this.gameObject?.SetActive(true);
            this.OnOpen();
        } else {
            this.gameObject?.SetActive(false);
            this.OnClose();
        }
    }
    protected virtual void OnInit() { }

    // 界面打开
    public void Open() {
        if (this.isOpen) {
            return;
        }
        this.isOpen = true;
        // 启动加载（内部延迟、不支持重复加载）
        if (!this.isLoad) {
            this.isLoad = true;
            StartCoroutine(OnLoad());
        }
        if (this.isInit) {
            this.gameObject?.SetActive(true);
            this.eventRunner?.Clear();
            this.OnOpen();
        }
    }
    protected virtual void OnOpen() { }

    // 界面关闭
    public void Close() {
        if (!this.isOpen) {
            return;
        }
        this.isOpen = false;
        if (this.isInit) {
            this.gameObject?.SetActive(false);
            this.OnClose();
        }
    }
    protected virtual void OnClose() { }

    // 每帧更新
    public void Tick() {
        // 加载阶段
        if (!this.isInit) {
            this.coroutine.Tick();
        } else if (this.isOpen) {
            // 打开过程
            OnTick();
        }
    }
    protected virtual void OnTick() { }
}
