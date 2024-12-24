using System.Collections;
using System.Collections.Generic;
using UnityEngine;

// 遭遇系统，用于把怪物创建出来，和触发战斗
public class JourneyMeetSystem : BaseSystem {
    // model
    private JourneyModel journeyModel;
    private List<MonsterModel> monsterModels;

    // view
    private List<JourneyMonster> journeyMonsters;

    // system

    // 新增注入
    public void Inject(JourneyModel model, List<MonsterModel> monsterModels, List<JourneyMonster> journeyMonsters) {
        this.journeyModel = model;
        this.monsterModels = monsterModels;
        this.journeyMonsters = journeyMonsters;
    }

    // 加载
    protected override IEnumerator OnLoad() {
        this.journeyModel.NextEnergyIndex = 0;
        yield return null;
        // 加载完成
        this.LoadFinish();
    }

    private void CreateMonster(MonsterModel model) {
        this.monsterModels.Add(model);
        var monster = this.OpenActor<JourneyMonster>();
        monster.Inject(model);
        var pos = this.journeyModel.splinePath.EvaluatePosition(model.spawnPercent);
        monster.SetPosition(pos + new Vector3(0, 5, 0));
        var lastPos = this.journeyModel.splinePath.EvaluatePosition(model.spawnPercent - 0.01);
        var forward = lastPos - pos;
        monster.SetForward(forward);
        monster.Open();
        this.journeyMonsters.Add(monster);
    }

    // 加载完成
    protected override void OnInit() {
        // 监听事件
    }

    // 打开系统
    protected override void OnStart() {
        this.CreateMonster(this.CreateModel(new MonsterModel() {
            nick = "小羊羔", sign = "EnemyBallSlime", Health = 10, maxHealth = 10, attack = 2, attackStunRate = 1f, spawnPercent = 0.05f
        }));
        this.CreateMonster(this.CreateModel(new MonsterModel() {
            nick = "羊咩咩", sign = "EnemyBallSlime", Health = 20, maxHealth = 20, attack = 3, attackStunRate = 1.2f, spawnPercent = 0.1f
        }));
        this.CreateMonster(this.CreateModel(new MonsterModel() {
            nick = "羊咩咩", sign = "EnemyBallSlime", Health = 20, maxHealth = 20, attack = 4, attackStunRate = 1.4f, spawnPercent = 0.2f
        }));
        this.CreateMonster(this.CreateModel(new MonsterModel() {
            nick = "爪牙猴子", sign = "EnemyMinionMonkey", Health = 40, maxHealth = 40, attack = 5, attackStunRate = 1.5f, spawnPercent = 0.3f
        }));
        this.CreateMonster(this.CreateModel(new MonsterModel() {
            nick = "羊咩咩", sign = "EnemyBallSlime", Health = 30, maxHealth = 30, attack = 4, attackStunRate = 1.3f, spawnPercent = 0.4f
        }));
        this.CreateMonster(this.CreateModel(new MonsterModel() {
            nick = "羊咩咩", sign = "EnemyBallSlime", Health = 30, maxHealth = 30, attack = 5, attackStunRate = 1.5f, spawnPercent = 0.5f
        }));
        this.CreateMonster(this.CreateModel(new MonsterModel() {
            nick = "爪牙猴子", sign = "EnemyMinionMonkey", Health = 50, maxHealth = 50, attack = 6, attackStunRate = 1.8f, spawnPercent = 0.6f
        }));
    }

    // 关闭系统
    protected override void OnStop() {
        // 关闭表现
    }

    // 系统运行时
    protected override void OnTick() {
        if (this.journeyModel.IsFinish || this.journeyModel.IsIdle || this.journeyModel.IsBattle || this.journeyModel.IsMeet) {
            return;
        }

        if (this.monsterModels.Count <= this.journeyModel.NextEnergyIndex) {
            return;
        }

        var nextTriggerPercent = this.monsterModels[this.journeyModel.NextEnergyIndex].spawnPercent;
        var dis = this.journeyModel.splinePath.CalculateLength(this.journeyModel.CurSplinePercent, nextTriggerPercent);
        if (dis < 7.5f) {
            this.eventRunner.Dispath(new JourneyMeetEvent() { index = this.journeyModel.NextEnergyIndex });
        }
    }
}
