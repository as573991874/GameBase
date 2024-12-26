using System.Collections;
using System.Collections.Generic;
using UnityEngine;

// 旅途系统
public class JourneySystem : BaseSystem {
    // model
    private PlayerModel playerModel;
    private JourneyModel journeyModel;
    private List<MonsterModel> monsterModels;

    // view
    private JourneyUI journeyUI;
    private JourneyScene journeyScene;
    private JourneyActor journeyActor;
    private List<JourneyMonster> journeyMonsters;
    private JourneyCamera journeyCamera;

    public void Inject(PlayerModel model) {
        this.playerModel = model;
    }

    // 加载
    protected override IEnumerator OnLoad() {
        // model
        this.journeyModel = this.CreateModel<JourneyModel>();
        this.monsterModels = new List<MonsterModel>();
        // view
        this.journeyUI = this.OpenUI<JourneyUI>();
        this.journeyUI.Inject(this.playerModel, this.journeyModel);
        yield return this.journeyUI.WaitLoad();
        this.journeyScene = this.OpenScene<JourneyScene>();
        yield return this.journeyScene.WaitLoad();
        this.journeyActor = this.OpenActor<JourneyActor>();
        this.journeyActor.Inject(this.journeyModel, this.playerModel);
        yield return this.journeyActor.WaitLoad();
        this.journeyCamera = this.OpenCamera<JourneyCamera>();
        this.journeyCamera.Inject(this.journeyActor, this.journeyModel);
        yield return this.journeyCamera.WaitLoad();
        this.journeyMonsters = new List<JourneyMonster>();
        // 加载完成
        this.LoadFinish();
    }

    // 加载完成
    protected override void OnInit() {
        // 监听事件
        this.eventRunner.Listen<JourneyNextEvent>();
        this.eventRunner.Listen<JourneyTreatEvent>();
        this.eventRunner.Listen<JourneyMeetEvent>();
        this.eventRunner.Listen<JourneyActorHealthEvent>();
    }

    // 打开系统
    protected override void OnStart() {
        // 物理设置
        Physics.autoSyncTransforms = true;

        this.journeyUI?.Open();
        this.journeyScene?.Open();
        this.journeyActor?.Open();
        this.journeyCamera?.Open();
    }

    // 关闭系统
    protected override void OnStop() {
        this.journeyCamera?.Close();
        this.journeyActor?.Close();
        this.journeyScene?.Close();
        this.journeyUI?.Close();
    }

    // 系统运行时
    protected override void OnTick() {
        this.eventRunner.HandleOne<JourneyNextEvent>(JourneyNext);
        this.eventRunner.HandleOne<JourneyTreatEvent>(JourneyTreat);
        this.eventRunner.Handle<JourneyActorHealthEvent>(JourneyActorHealth);
        this.eventRunner.HandleOne<JourneyMeetEvent>(JourneyMeet);

        // 镜头 -> 后续改到系统
        if (this.journeyModel.IsMeet || this.journeyModel.IsBattle) {
            this.journeyModel.CameraIndex = 2;
        } else if (this.journeyModel.CurSplinePercent > 0.43f) {
            this.journeyModel.CameraIndex = 0;
        } else if (this.journeyModel.CurSplinePercent > 0.33f) {
            this.journeyModel.CameraIndex = 1;
        } else {
            this.journeyModel.CameraIndex = 0;
        }

        this.journeyUI?.Tick();
        this.journeyScene?.Tick();
        this.journeyActor?.Tick();
        for (var i = 0; i < this.journeyMonsters.Count; i++) {
            this.journeyMonsters[i].Tick();
        }
        this.journeyCamera?.Tick();
    }

    // 按钮下一步
    private void JourneyNext(JourneyNextEvent eventData) {
        if (this.journeyModel.IsFinish) {
            this.eventRunner.Dispath(new MainStageSwitchEvent() { stage = MainStageEnum.Campsite });
        } else if (this.journeyModel.IsMeet) {
        } else if (this.journeyModel.IsBattle) {
        } else if (this.journeyModel.IsIdle) {
        } else if (this.journeyModel.IsRun) {
        } else {
        }
    }

    // 治疗
    private void JourneyTreat(JourneyTreatEvent eventData) {
    }

    // 血量变化
    private void JourneyActorHealth(JourneyActorHealthEvent eventData) {
        this.journeyUI?.PlayHealthVFX(eventData.transform, eventData.health);
    }

    // 遇上
    private void JourneyMeet(JourneyMeetEvent eventData) {
        this.StartCoroutine(this.JourneyMeetFlow());
    }

    private IEnumerator JourneyMeetFlow() {
        var model = this.monsterModels[this.journeyModel.NextEnergyIndex];
        var monster = this.journeyMonsters[this.journeyModel.NextEnergyIndex];
        this.journeyModel.NextEnergyIndex += 1;
        this.journeyCamera?.SetLookAt(monster.transform);
        yield return new WaitForSeconds(1.2f);
    }
}
