using System;
using System.Collections.Generic;
using System.Reflection;
using System.Runtime.CompilerServices;
using System.Text;

public interface IEventData {
}

// 事件触发器，支持冒泡
public class EventRunner {
    private static bool OpenLog = true;

    // 事件容器
    private Dictionary<Type, List<IEventData>> eventDatas = new Dictionary<Type, List<IEventData>>();

    // 父对象
    private Type parentType;
    private EventRunner parent;

    public EventRunner(object data, EventRunner parent = null) {
        this.parentType = data.GetType();
        this.parent = parent;
    }

    // 监听数据
    public void Listen<T>() where T : struct, IEventData {
        var type = typeof(T);
        this.eventDatas.Add(type, new List<IEventData>());
    }

    // 派发事件
    public void Dispath<T>(T data, bool buddle = false, [CallerMemberName] string methodName = null) where T : struct, IEventData {
        var type = typeof(T);
        if (OpenLog && !buddle) {
            LogDispatch<T>(data, methodName);
        }
        // 添加事件
        if (this.eventDatas.ContainsKey(type)) {
            this.eventDatas[type].Add(data);
        }
        // 冒泡
        if (this.parent != null) {
            this.parent.Dispath<T>(data, true, methodName);
        }
    }

    // 触发事件(逐个触发)
    public void Handle<T>(Action<T> handler) where T : struct, IEventData {
        var type = typeof(T);
        if (!this.eventDatas.ContainsKey(type)) {
            return;
        }
        var eventDatas = this.eventDatas[type];
        if (eventDatas.Count == 0) {
            return;
        }
        if (OpenLog) {
            LogHandleList<T>(eventDatas, handler.GetMethodInfo().Name);
        }
        for (var i = 0; i < eventDatas.Count; i++) {
            handler((T)eventDatas[i]);
        }
        eventDatas.Clear();
    }

    // 触发事件(最后一个)
    public void HandleOne<T>(Action<T> handler) where T : struct, IEventData {
        var type = typeof(T);
        if (!this.eventDatas.ContainsKey(type)) {
            return;
        }
        var eventDatas = this.eventDatas[type];
        if (eventDatas.Count == 0) {
            return;
        }
        if (OpenLog) {
            LogHandleOne<T>(eventDatas[eventDatas.Count - 1], handler.GetMethodInfo().Name);
        }
        for (var i = 0; i < eventDatas.Count; i++) {
            handler((T)eventDatas[i]);
        }
        eventDatas.Clear();
    }

    // 触发事件（列表）
    public void HandleList<T>(Action<List<IEventData>> handler) where T : struct, IEventData {
        var type = typeof(T);
        if (!this.eventDatas.ContainsKey(type)) {
            return;
        }
        var eventDatas = this.eventDatas[type];
        if (eventDatas.Count == 0) {
            return;
        }
        if (OpenLog) {
            LogHandleList<T>(eventDatas, handler.GetMethodInfo().Name);
        }
        for (var i = 0; i < eventDatas.Count; i++) {
            handler(eventDatas);
        }
        eventDatas.Clear();
    }

    void LogHandleList<T>(List<IEventData> datas, string methodName) {
        var info = new StringBuilder();
        var eventType = typeof(T);
        info.AppendLine($"{parentType}.{methodName} =>");
        for (var index = 0; index < datas.Count; index++) {
            var data = datas[index];
            info.Append($"[{index}] => ");
            FieldInfo[] fields = eventType.GetFields();
            foreach (FieldInfo field in fields) {
                object value = field.GetValue(data);
                info.Append($"{field.Name}: {value},");
            }
            info.AppendLine("");
        }
        UnityEngine.Debug.Log(info.ToString());
    }

    void LogHandleOne<T>(IEventData data, string methodName) {
        var info = new StringBuilder();
        var eventType = typeof(T);
        info.AppendLine($"{parentType}.{methodName} =>");
        FieldInfo[] fields = eventType.GetFields();
        foreach (FieldInfo field in fields) {
            object value = field.GetValue(data);
            info.Append($"{field.Name}: {value},");
        }
        UnityEngine.Debug.Log(info.ToString());
    }

    void LogDispatch<T>(IEventData data, string methodName) {
        var info = new StringBuilder();
        var eventType = typeof(T);
        info.AppendLine($"{parentType}.{methodName}.{eventType} =>");
        FieldInfo[] fields = eventType.GetFields();
        foreach (FieldInfo field in fields) {
            object value = field.GetValue(data);
            info.Append($"{field.Name}: {value},");
        }
        info.AppendLine("");
        UnityEngine.Debug.Log(info.ToString());
    }

    // 每帧清除
    public void Clear() {
        foreach (var item in this.eventDatas.Values) {
            item.Clear();
        }
    }
}
