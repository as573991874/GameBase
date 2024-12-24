using System.Collections;
using System.Collections.Generic;
using UnityEngine;

// 协程封装
internal class Coroutine {
    public Coroutine prev;
    public Coroutine next;
    public IEnumerator coroutine;

    public Coroutine(IEnumerator coroutine) {
        this.coroutine = coroutine;
        this.prev = null;
        this.next = null;
    }
}

// 通用等待时间
public class WaitForSeconds : IEnumerator {
    // 用于记录开始时间
    private float startTime;
    private float delayTime;
    // 用于标记是否已经等待完毕
    private bool isDone = false;

    // 实现Current属性，按照IEnumerator接口要求，这里返回null，因为我们不需要返回具体元素
    public object Current => null;

    // 构造函数，初始化开始时间
    public WaitForSeconds(float delayTime) {
        this.startTime = Time.time;
        this.delayTime = delayTime;
    }

    // 实现MoveNext方法，用于控制协程的执行流程，这里通过判断是否达到等待时间来决定是否继续执行
    public bool MoveNext() {
        if (isDone) {
            return false;
        }

        // 计算已经过去的时间
        if (Time.time - startTime >= delayTime) {
            isDone = true;
            return false;
        }

        return true;
    }

    // 实现Reset方法，按照IEnumerator接口要求，这里简单实现，在这个示例中其实不太需要重置逻辑
    public void Reset() {
        startTime = Time.time;
        isDone = false;
    }
}

// 协程管理器类
public class CoroutineRunner {
    // 使用列表来存储正在运行的协程
    private List<Coroutine> runningCoroutines = new List<Coroutine>();

    // 启动协程的方法
    public void Start(IEnumerator coroutine) {
        var c = new Coroutine(coroutine);
        runningCoroutines.Add(c);
        TickCoroutine(c);
    }

    // 更新协程的方法，通常在游戏循环或者定时更新的地方调用
    public void Tick() {
        // 遍历正在运行的协程列表
        for (int i = runningCoroutines.Count - 1; i >= 0; i--) {
            Coroutine coroutine = runningCoroutines[i];
            if (TickCoroutine(coroutine)) {
                runningCoroutines.RemoveAt(i);
            }
        }
    }

    bool TickCoroutine(Coroutine coroutine) {
        while (coroutine.next != null) {
            coroutine = coroutine.next;
        }
        IEnumerator enumerator = coroutine.coroutine;
        if (enumerator.MoveNext()) {
            var result = enumerator.Current as IEnumerator;
            if (result != null) {
                var next = new Coroutine(result);
                coroutine.next = next;
                next.prev = coroutine;
                return TickCoroutine(next);
            } else {
                return false;
            }
        } else {
            if (coroutine.prev != null) {
                coroutine = coroutine.prev;
                coroutine.next = null;
                return TickCoroutine(coroutine);
            } else {
                return true;
            }
        }
    }

    // 停止指定协程的方法
    public void Stop(IEnumerator coroutine) {
        for (var i = 0; i < runningCoroutines.Count; i++) {
            if (runningCoroutines[i].coroutine == coroutine) {
                runningCoroutines.RemoveAt(i);
                break;
            }
        }
    }
}
