using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class JourneyUI : BaseUI {
    public override string path => "UI_Journey";

    // model
    private PlayerModel playerModel;
    private JourneyModel journeyModel;

    // view
    private Text txtNick;
    private Text txtHealth;
    private Text txtNext;
    private GameObject goHealthTemp;
    private GameObject goVFXHealthTemp;
    private List<RectTransform> healthList;
    private Button btnBack;
    private Button btnNext;
    private Button btnSkill;

    public void Inject(PlayerModel model, JourneyModel journeyModel) {
        this.playerModel = model;
        this.journeyModel = journeyModel;
    }

    // 初始化
    protected override void OnInit() {
        txtNick = this.transform.Find("GoLeft/GoNick/TxtNick").GetComponent<Text>();
        txtHealth = this.transform.Find("GoLeft/GoHealth/TxtHealth").GetComponent<Text>();
        goHealthTemp = this.transform.Find("HealthTemp").gameObject;
        goVFXHealthTemp = this.transform.Find("VFXHealthTemp").gameObject;
        healthList = new List<RectTransform>();
        btnBack = this.transform.Find("BtnBack").GetComponent<Button>();
        btnBack.onClick.AddListener(this.OnBack);
        btnNext = this.transform.Find("BtnNext").GetComponent<Button>();
        btnNext.onClick.AddListener(this.OnNext);
        txtNext = btnNext.GetComponentInChildren<Text>();
        btnSkill = this.transform.Find("BtnSkill").GetComponent<Button>();
        btnSkill.onClick.AddListener(this.OnSkill);
    }

    // 打开
    protected override void OnOpen() {
        txtNick.text = $"昵称: {playerModel.Nick}";
        txtHealth.text = $"血量: {playerModel.Health}";
        btnSkill.gameObject.SetActive(false);
        this.renderTxtNext();
    }

    private void OnBack() {
        this.eventRunner.Dispath(new MainStageSwitchEvent() { stage = MainStageEnum.Campsite });
    }

    private void OnNext() {
        this.eventRunner.Dispath(new JourneyNextEvent() { });
    }

    private void OnSkill() {
        this.eventRunner.Dispath(new JourneyTreatEvent() { });
    }

    // 关闭
    protected override void OnClose() {
    }

    // 更新
    protected override void OnTick() {
        this.playerModel.DirtyCheck("Health", txtHealth, $"血量: {playerModel.Health}");
        this.journeyModel.DirtyCheck(this.renderTxtNext);
        this.journeyModel.DirtyCheck("IsBattle", this.RenderHealth, this.journeyModel.IsBattle);
        this.RenderHealthState();
    }

    // 飘血
    public void PlayHealthVFX(Transform transform, int value){
        var vfx = GameObject.Instantiate(this.goVFXHealthTemp, this.transform);
        vfx.SetActive(true);
        var text = vfx.transform.Find("TxtHealth").GetComponent<Text>();
        text.color = value > 0 ? Color.green : Color.red;
        text.text = value.ToString();
        var screenPosition = camera.WorldToScreenPoint(transform.position + new Vector3(0, 1.2f, 0));
        screenPosition += new Vector3(0, 20, 0);
        Vector2 localPoint;
        bool isInside = RectTransformUtility.ScreenPointToLocalPointInRectangle(
            this.transform, screenPosition, null, out localPoint
        );
        var rectTransform = vfx.transform as RectTransform;
        rectTransform.anchoredPosition = localPoint;
        rectTransform.localScale = new Vector3(1.3f, 1.3f, 1.3f);
    }

    // 渲染血条状态
    private void RenderHealthState() {
        if (healthList.Count > 0 && this.journeyModel.IsBattle) {
            var health = healthList[0];
            var value = (float)this.playerModel.Health / (float)this.playerModel.maxHealth;
            this.RenderHealthState(health, value, this.journeyModel.battleActor.position);
            health = healthList[1];
            value = (float)this.journeyModel.battleMonsterModel.Health / (float)this.journeyModel.battleMonsterModel.maxHealth;
            this.RenderHealthState(health, value, this.journeyModel.battleEnemy.position);
        }
    }

    private void RenderHealthState(RectTransform health, float value, Vector3 pos) {
        pos += new Vector3(0, 1.15f, 0);
        health.GetComponentInChildren<Slider>().value = value;
        var screenPosition = camera.WorldToScreenPoint(pos);
        Vector2 localPoint;
        bool isInside = RectTransformUtility.ScreenPointToLocalPointInRectangle(
            transform, screenPosition, null, out localPoint
        );
        health.anchoredPosition = localPoint;
    }

    private void RenderHealth(bool isBattle) {
        if (isBattle) {
            if (healthList.Count == 0) {
                // 角色
                var health = GameObject.Instantiate(this.goHealthTemp, this.transform);
                health.SetActive(true);
                var image = health.transform.Find("Slider/Fill Area/Fill").GetComponent<Image>();
                image.color = Color.green;
                healthList.Add(health.transform as RectTransform);
                // 怪物
                health = GameObject.Instantiate(this.goHealthTemp, this.transform);
                health.SetActive(true);
                image = health.transform.Find("Slider/Fill Area/Fill").GetComponent<Image>();
                image.color = Color.red;
                healthList.Add(health.transform as RectTransform);
            }
        } else {
            if (healthList.Count > 0) {
                for (var i = 0; i < healthList.Count; i++) {
                    GameObject.Destroy(healthList[i].gameObject);
                }
                healthList.Clear();
            }
            healthList.Clear();
        }
    }

    private void renderTxtNext() {
        if (this.journeyModel.IsFinish) {
            this.txtNext.text = "回家";
        } else if (this.journeyModel.IsBattle) {
            this.txtNext.text = "战斗...";
            this.btnSkill.gameObject.SetActive(true);
        } else if (this.journeyModel.IsMeet) {
            this.txtNext.text = "遇袭...";
        } else if (this.journeyModel.IsIdle) {
            this.txtNext.text = "出发";
            this.btnSkill.gameObject.SetActive(false);
        } else if (this.journeyModel.IsRun) {
            this.txtNext.text = "慢慢来";
        } else {
            this.txtNext.text = "赶紧的";
        }
    }
}
