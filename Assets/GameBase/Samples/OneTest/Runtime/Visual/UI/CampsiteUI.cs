using UnityEngine;
using UnityEngine.UI;

public class CampsiteUI : BaseUI {
    public override string path => "Campsite";

    // model
    private PlayerModel playerModel;

    // view
    private Text txtNick;
    private Text txtHealth;
    private Button btnReady;

    public void Inject(PlayerModel model) {
        this.playerModel = model;
    }

    // 初始化
    protected override void OnInit() {
        txtNick = this.transform.Find("GoLeft/GoNick/TxtNick").GetComponent<Text>();
        txtHealth = this.transform.Find("GoLeft/GoHealth/TxtHealth").GetComponent<Text>();
        btnReady = this.transform.Find("BtnReady").GetComponent<Button>();
        btnReady.onClick.AddListener(this.OnReady);
    }

    // 打开
    protected override void OnOpen() {
        txtNick.text = $"昵称: {playerModel.Nick}";
        txtHealth.text = $"血量: {playerModel.Health}";
    }

    private void OnReady(){
        this.eventRunner.Dispath(new MainStageSwitchEvent(){stage = MainStageEnum.Journey});
    }

    // 关闭
    protected override void OnClose() {
    }

    // 更新
    protected override void OnTick() {
    }
}
