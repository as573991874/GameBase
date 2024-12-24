using UnityEngine;

public class SampleScene : BaseScene {
    public override string path => "Sample/Scene";

    // model
    private BaseModel sampleModel;

    // view
    private Camera camera;

    public void Inject(BaseModel model) {
        this.sampleModel = model;
    }

    // 初始化
    protected override void OnInit() {
    }

    // 打开
    protected override void OnOpen() {
        // test
    }

    // 关闭
    protected override void OnClose() {
    }

    // 更新
    protected override void OnTick() {
    }
}
