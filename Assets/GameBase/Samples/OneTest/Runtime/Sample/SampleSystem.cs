using System.Collections;
using UnityEngine;

// 样例系统
public class SampleSystem : BaseSystem {
    // model
    // view
    // system

    // 新增注入
    public void Inject() {
        // xxx
    }

    // 加载
    protected override IEnumerator OnLoad() {
        // model
        // view
        // system
        yield return new WaitForSeconds(1f);
        // 加载完成
        this.LoadFinish();
    }

    // 加载完成
    protected override void OnInit() {
        // 监听事件
    }

    // 打开系统
    protected override void OnStart() {
        // 处理一个流程
        this.StartCoroutine(SampleFlow());
    }

    // 关闭系统
    protected override void OnStop() {
        // 关闭表现
    }

    // 系统运行时
    protected override void OnTick() {
        // 事件
        // 子系统
        // 表现
    }

    // 前往营地
    private IEnumerator SampleFlow() {
        // 流程逻辑
        yield return new WaitForSeconds(1f);
        // 完成流程
    }
}
