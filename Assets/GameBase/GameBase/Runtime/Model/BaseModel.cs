// 支持 dirty check
using System;
using System.Collections.Generic;
using System.Runtime.CompilerServices;
using UnityEngine;
using UnityEngine.UI;

public class BaseModel {
    // 当前触发的脏数据
    private HashSet<string> triggetDirty = new HashSet<string>();
    // 当前设置的脏数据
    private HashSet<string> setDirty = new HashSet<string>();

    // 切换
    public void Tick() {
        var dirty = triggetDirty;
        triggetDirty = setDirty;
        setDirty = dirty;
        setDirty.Clear();
    }

    // 设置脏标记
    protected bool SetField<T>(ref T field, T value, [CallerMemberName] string propertyName = null) {
        if (EqualityComparer<T>.Default.Equals(field, value)) return false;
        field = value;
        setDirty.Add(propertyName);
        return true;
    }

    // 脏检查
    public bool DirtyCheck(string propertyName = null) {
        if (string.IsNullOrEmpty(propertyName)) {
            return triggetDirty.Count > 0;
        }
        return triggetDirty.Contains(propertyName);
    }

    public void DirtyCheck(Action func) {
        if (this.DirtyCheck()) {
            func();
        }
    }

    public void DirtyCheck<T>(Action<T> func) where T : BaseModel {
        if (this.DirtyCheck()) {
            func(this as T);
        }
    }

    public void DirtyCheck(string propertyName, Action func) {
        if (this.DirtyCheck(propertyName)) {
            func();
        }
    }

    public void DirtyCheck<T>(string propertyName, Action<T> func, T value) {
        if (this.DirtyCheck(propertyName)) {
            func(value);
        }
    }

    public void DirtyCheck(string propertyName, GameObject obj, bool value) {
        if (this.DirtyCheck(propertyName)) {
            obj.SetActive(value);
        }
    }

    public void DirtyCheck(string propertyName, Text text, string value) {
        if (this.DirtyCheck(propertyName)) {
            text.text = value;
        }
    }

    public void DirtyCheck(string propertyName, Animator animator, bool value, string key = null) {
        if (this.DirtyCheck(propertyName)) {
            animator.SetBool(key ?? propertyName, value);
        }
    }

    public void DirtyCheck(string propertyName, Animator animator, float value, string key = null) {
        if (this.DirtyCheck(propertyName)) {
            animator.SetFloat(key ?? propertyName, value);
        }
    }

    public void DirtyCheck(string propertyName, Animator animator, int value, string key = null) {
        if (this.DirtyCheck(propertyName)) {
            animator.SetInteger(key ?? propertyName, value);
        }
    }

    public void DirtyCheck(string propertyName, Animator animator, string value, string key = null) {
        if (this.DirtyCheck(propertyName)) {
            animator.SetTrigger(value);
        }
    }
}
