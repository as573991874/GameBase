using UnityEngine;
using System.Collections;
using UnityEngine.UI;
using System;

public class LoadStage : BaseUI {
    public override string path => "Loading";

    // model
    private MainStageModel stageModel;

    // view
    private Text txtInfo;

    public void Inject(MainStageModel model){
        this.stageModel = model;
    }

    // 初始化
    protected override void OnInit() {
        txtInfo = this.transform.Find("Text (Legacy)").GetComponent<Text>();
    }

    // 打开
    protected override void OnOpen() {
        txtInfo.text = $"正在前往{this.stageModel.GetStageInfo()}";
    }

    // 关闭
    protected override void OnClose() {
    }

    // 更新
    protected override void OnTick() {
    }
}
