using UnityEngine;
using System.Collections;
using UnityEngine.UI;
using System;

public class Loading : BaseUI {
    public override string path => "Loading";

    // model
    private AppStageModel appStageModel;

    // view
    private Text txtInfo;

    public void Inject(AppStageModel model){
        this.appStageModel = model;
    }

    // 初始化
    protected override void OnInit() {
        txtInfo = this.transform.Find("Text (Legacy)").GetComponent<Text>();
    }

    // 打开
    protected override void OnOpen() {
        txtInfo.text = this.appStageModel.GetStageInfo();
    }

    // 关闭
    protected override void OnClose() {
    }

    // 更新
    protected override void OnTick() {
        int count = (int)(Time.frameCount / 20f);
        txtInfo.text = this.appStageModel.GetStageInfo() + new string('.', count % 4);
    }
}
