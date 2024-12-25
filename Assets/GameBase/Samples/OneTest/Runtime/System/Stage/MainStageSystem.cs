using System.Collections;
using System.Collections.Generic;
using UnityEngine;

// 游戏主场景
public class MainStageSystem : BaseSystem {
    // model
    private MainStageModel mainStageModel;
    private PlayerModel playerModel;
    private BackpackModel backpackModel;

    // view
    private LoadStage loadStageUI;

    // system
    private CampsiteSystem campsiteSystem;
    private JourneySystem journeySystem;

    // 加载
    protected override IEnumerator OnLoad() {
        // model
        this.mainStageModel = new MainStageModel();
        this.playerModel = this.CreateModel<PlayerModel>();
        this.playerModel.Nick = "aboy";
        this.playerModel.maxHealth = 25;
        this.playerModel.Health = this.playerModel.maxHealth;
        this.playerModel.weapons = new List<WeaponModel>(){
            new WeaponModel(){name = "弓", index = 1, attack = 5, attackStunRate = 1.5f},
            new WeaponModel(){name = "杖", index = 2, attack = 3, attackStunRate = 2f},
            new WeaponModel(){name = "棒", index = 3, attack = 4, attackStunRate = 1f}
        };
        this.playerModel.curWeapon = this.playerModel.weapons[0];
        this.backpackModel = new BackpackModel();

        // 准备 view
        this.loadStageUI = this.OpenUI<LoadStage>();
        this.loadStageUI.Inject(this.mainStageModel);
        yield return this.loadStageUI.WaitLoad();

        // system
        this.campsiteSystem = CreateSystem<CampsiteSystem>();
        this.campsiteSystem.Inject(this.playerModel);
        this.journeySystem = CreateSystem<JourneySystem>();
        this.journeySystem.Inject(this.playerModel);

        // 加载完成
        this.LoadFinish();
    }

    // 加载完成
    protected override void OnInit() {
        this.eventRunner.Listen<MainStageSwitchEvent>();
    }

    // 打开系统
    protected override void OnStart() {
        // 前往营地
        this.StartCoroutine(GoCampsite());
    }

    // 关闭系统
    protected override void OnStop() {
        this.loadStageUI?.Close();
    }

    // 系统运行时
    protected override void OnTick() {
        // 事件
        this.eventRunner.HandleOne<MainStageSwitchEvent>(SwitchStage);
        // 子系统更新
        this.campsiteSystem?.Tick();
        this.journeySystem?.Tick();
        // 表现
        this.loadStageUI?.Tick();
    }

    private void SwitchStage(MainStageSwitchEvent eventData) {
        switch (eventData.stage) {
            case MainStageEnum.Campsite:
                this.StartCoroutine(GoCampsite());
                break;
            case MainStageEnum.Journey:
                this.StartCoroutine(GoJourney());
                break;
            default:
                break;
        }
    }

    // 前往营地
    private IEnumerator GoCampsite() {
        this.mainStageModel.stage = MainStageEnum.Campsite;
        this.loadStageUI.Open();
        this.journeySystem.Stop();
        yield return this.campsiteSystem.WaitLoad();
        this.campsiteSystem.Start();
        this.loadStageUI.Close();
    }

    // 前往旅途
    private IEnumerator GoJourney() {
        this.mainStageModel.stage = MainStageEnum.Journey;
        this.loadStageUI.Open();
        this.campsiteSystem.Stop();
        yield return this.journeySystem.WaitLoad();
        this.journeySystem.Start();
        this.loadStageUI.Close();
    }
}
