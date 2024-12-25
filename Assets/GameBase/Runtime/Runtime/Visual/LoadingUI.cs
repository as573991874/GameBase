using UnityEngine;
using System.Collections;
using UnityEngine.UI;

public class LoadingUI : BaseUI {
    public override string path => "Loading";

    // view
    private Text txtInfo;

    // 初始化
    protected override void OnInit() {
        txtInfo = this.transform.Find("Text (Legacy)").GetComponent<Text>();
        this.transform.Find("List").gameObject.SetActive(false);
    }

    // 界面加载
    protected override IEnumerator OnLoad() {
        this.transform = this.canvas.transform.Find("Loading") as RectTransform;
        this.gameObject = this.transform.gameObject;
        this.gameObject.name = path;
        LoadFinish();
        yield return null;
    }

    // 打开
    protected override void OnOpen() {
        txtInfo.text = "加载中";
    }

    // 关闭
    protected override void OnClose() {
    }

    // 更新
    protected override void OnTick() {
        int count = (int)(Time.frameCount / 20f);
        txtInfo.text = "加载中" + new string('.', count % 4);
    }
}
