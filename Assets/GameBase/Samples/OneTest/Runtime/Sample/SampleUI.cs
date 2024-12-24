using UnityEngine;
using UnityEngine.UI;

public class SampleUI: BaseUI {
    public override string path => "Sample";

    // model
    private BaseModel sampleModel;

    // view
    private Text txtInfo;

    public void Inject(BaseModel model)
    {
        this.sampleModel = model;
    }

    // 初始化
    protected override void OnInit()
    {
        txtInfo = this.transform.Find("Sample").GetComponent<Text>();
    }

    // 打开
    protected override void OnOpen()
    {
        txtInfo.text = "init";
    }

    // 关闭
    protected override void OnClose()
    {
    }

    // 更新
    protected override void OnTick()
    {
        txtInfo.text = $"frame: {Time.frameCount}";
    }
}
