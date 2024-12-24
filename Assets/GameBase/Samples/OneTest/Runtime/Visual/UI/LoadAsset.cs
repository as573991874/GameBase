using UnityEngine;
using System.Collections;
using UnityEngine.UI;
using System;

public class LoadAsset : BaseUI {
    public override string path => "Loading";

    // model
    private LoadAssetModel loadAssetModel;

    // view
    private Text txtInfo;

    public void Inject(LoadAssetModel model){
        this.loadAssetModel = model;
    }

    // 初始化
    protected override void OnInit() {
        txtInfo = this.transform.Find("Text (Legacy)").GetComponent<Text>();
    }

    // 打开
    protected override void OnOpen() {
        txtInfo.text = $"资源加载中 ({this.loadAssetModel.curIndex + 1} / {this.loadAssetModel.loadCount})";
    }

    // 关闭
    protected override void OnClose() {
    }

    // 更新
    protected override void OnTick() {
        if (this.loadAssetModel.isDone){
            txtInfo.text = "资源加载完成";
        } else {
            int count = (int)(Time.frameCount / 30f);
            string info = $"资源加载中 ({this.loadAssetModel.curIndex + 1} / {this.loadAssetModel.loadCount})";
            txtInfo.text = info + new string('.', count % 4);
        }
    }
}
