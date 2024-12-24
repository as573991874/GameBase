using System.Collections;
using UnityEngine;
using System;

// 样例系统
public class JourneyActorSystem : BaseSystem {
    // model
    private JourneyModel journeyModel;
    private PlayerModel playerModel;
    private MonsterModel monsterModel;

    // view
    private JourneyActor actor;
    private JourneyMonster monster;

    // system

    // 新增注入
    public void Inject(JourneyModel journeyModel, PlayerModel playerModel, JourneyActor journeyActor) {
        this.journeyModel = journeyModel;
        this.playerModel = playerModel;
        this.actor = journeyActor;
    }

    // 加载
    protected override IEnumerator OnLoad() {
        // model
        // view
        // system
        yield return null;
        // 加载完成
        this.LoadFinish();
    }

    // 加载完成
    protected override void OnInit() {
        // 监听事件
    }

    // 打开系统
    protected override void OnStart() {
        // model
        this.journeyModel.CurSplinePercent = 0f;
        this.journeyModel.CurSpeed = 0f;
        this.journeyModel.IsIdle = true;
        this.journeyModel.IsFinish = false;
        this.journeyModel.IsRun = false;

        // view
        var splinePath = this.journeyModel.splinePath;
        var pos = splinePath.GetPointPosition(0);
        this.actor.SetPosition(pos);
        var nextPos = splinePath.GetPointPosition(splinePath.pointCount - 1);
        this.actor.SetForward(nextPos - pos);

        this.Idle();
    }

    // 关闭系统
    protected override void OnStop() {
        // 关闭表现
    }

    // 战斗
    public void Idle() {
        this.journeyModel.IsBattle = false;
        this.journeyModel.IsMeet = false;
        this.journeyModel.IsRun = false;
        this.journeyModel.IsIdle = true;
        this.journeyModel.CurSpeed = 0f;
        this.actor.testActor.Idle();
    }

    // 出发
    public void Go() {
        this.journeyModel.IsIdle = false;
        this.journeyModel.IsRun = false;
        this.actor.testActor.RefreshBody(0);
        this.actor.testActor.Walk();
    }

    // 慢下来
    public void Slow() {
        this.journeyModel.IsRun = false;
        this.actor.testActor.Walk();
    }

    // 跑起来
    public void Rush() {
        this.journeyModel.IsRun = true;
        this.actor.testActor.Move();
    }

    // 相遇
    public void Meet() {
        this.journeyModel.IsIdle = false;
        this.journeyModel.IsRun = false;
        this.journeyModel.IsMeet = true;
        this.journeyModel.CurSpeed = 0f;
        this.actor.testActor.Idle();
    }

    // 战斗
    public void Battle(MonsterModel monsterModel, JourneyMonster monster) {
        var index = UnityEngine.Random.Range(0, this.playerModel.weapons.Count);
        this.playerModel.curWeapon = this.playerModel.weapons[index];
        this.actor.testActor.RefreshBody(this.playerModel.curWeapon.index);
        this.actor.testActor.Standby();
        this.journeyModel.IsBattle = true;
        this.journeyModel.battleMonsterModel = monsterModel;
        this.journeyModel.battleActor = this.actor.transform;
        this.journeyModel.battleEnemy = monster.transform;
        this.journeyModel.IsNear = false;
        this.journeyModel.IsMeet = false;
        this.journeyModel.IsDead = false;
        this.monsterModel = monsterModel;
        this.monster = monster;
    }

    // 强制下一步
    public void Next() {
        if (this.journeyModel.IsDead) {
            if (this.playerModel.Health <= 0) {
                this.playerModel.Health = (int)(this.playerModel.maxHealth * 0.5f);
            }
            this.Battle(this.monsterModel, this.monster);
        } else {
            this.Win();
        }
    }

    // 强制胜利
    public void Win() {
        this.journeyModel.IsIdle = true;
        this.journeyModel.IsRun = false;
        this.journeyModel.IsMeet = false;
        this.journeyModel.IsBattle = false;
        this.actor.testActor.Standby();
        this.monster.animator.SetTrigger("Dead");
        this.monster = null;
    }

    // 死掉了
    public void Dead() {
        this.journeyModel.IsDead = true;
        this.actor.testActor.Dead();
        this.monster.animator.SetFloat("Velocity", 0f);
    }

    // 回血
    public void Treat() {
        this.playerModel.Health = Math.Min(
            this.playerModel.Health + 5, this.playerModel.maxHealth
        );
        this.eventRunner.Dispath(new JourneyActorHealthEvent() { health = 5, transform = this.actor.transform });
    }

    void Finish() {
        this.journeyModel.IsFinish = true;
        this.journeyModel.IsRun = false;
        this.journeyModel.IsIdle = true;
        this.actor.testActor.Idle();
    }

    // 系统运行时
    protected override void OnTick() {
        if (this.journeyModel.IsFinish) {
            return;
        }

        // 相遇中
        if (this.journeyModel.IsBattle) {
            this.CheckBattle();
            return;
        }

        // 相遇中
        if (this.journeyModel.IsMeet) {
            this.actor.actorController.SimpleMove(Vector3.zero);
            return;
        }

        // 待机
        if (this.journeyModel.IsIdle) {
            return;
        }

        // 往前走
        this.RunForward();

        // 检查是否完成
        if (this.journeyModel.CurSplinePercent >= 0.99f) {
            this.Finish();
        }
    }

    private void RunForward() {
        // 速度
        var speed = this.journeyModel.IsRun ? this.journeyModel.runSpeed : this.journeyModel.walkSpeed;
        var disSpd = speed - this.journeyModel.CurSpeed;
        if (Math.Abs(disSpd) < this.journeyModel.acceleration * Time.deltaTime) {
            this.journeyModel.CurSpeed = speed;
        } else {
            this.journeyModel.CurSpeed += disSpd > 0 ? this.journeyModel.acceleration * Time.deltaTime : -this.journeyModel.acceleration * Time.deltaTime;
        }
        // 动画
        this.actor.testActor.animator.SetFloat("Velocity", this.journeyModel.CurSpeed / speed);
        // 位置
        var percent = this.journeyModel.splinePath.Travel(this.journeyModel.CurSplinePercent, this.journeyModel.CurSpeed);
        var pos = this.journeyModel.splinePath.EvaluatePosition(percent);
        var lastPos = this.actor.transform.position;
        var forward = pos - lastPos;
        forward.y = 0;
        this.actor.actorController.SimpleMove(forward.normalized * this.journeyModel.CurSpeed);
        var sample = this.journeyModel.splinePath.Project(this.actor.transform.position);
        this.journeyModel.CurSplinePercent = sample.percent;
        // 朝向
        this.actor.SetForward(this.actor.transform.position - lastPos);
    }

    private void CheckBattle() {
        if (this.journeyModel.IsDead) {
        } else if (this.journeyModel.IsNear) {
            this.CheckPlayer();
            this.CheckMonster();
            this.CheckResult();
        } else {
            this.CheckNear();
        }
    }

    private void CheckNear() {
        var p1 = this.monster.transform.position;
        var p2 = this.actor.transform.position;
        var dis = Vector3.Distance(p1, p2);
        var minDis = this.monsterModel.sign == "EnemyMinionMonkey" ? 2f : 2.2f;
        if (dis > minDis) {
            var forward = p1 - p2;
            forward.y = 0;
            this.actor.testActor.Move();
            this.actor.actorController.SimpleMove(forward.normalized * this.journeyModel.walkSpeed);
            this.actor.SetForward(p1 - this.actor.transform.position);
            forward = p2 - p1;
            forward.y = 0;
            this.monster.animator.SetFloat("Velocity", 1f);
            this.monster.actorController.SimpleMove(forward.normalized * this.journeyModel.walkSpeed);
            this.monster.SetForward(p2 - this.monster.transform.position);
        } else {
            this.journeyModel.IsNear = true;
            this.actor.testActor.Standby();
            this.monster.animator.SetFloat("Velocity", 0f);
            this.playerModel.isIdle = true;
            this.playerModel.isAtk = false;
            this.playerModel.isAtkValid = false;
            this.playerModel.isHurt = false;
            this.playerModel.waitCount = 0;
            this.monsterModel.isIdle = true;
            this.monsterModel.isAtk = false;
            this.monsterModel.isAtkValid = false;
            this.monsterModel.isHurt = false;
            this.monsterModel.waitCount = 0;
            this.actor.testActor.body.GetComponent<TestAnimationEvent>().playerModel = this.playerModel;
            this.monster.body.GetComponent<TestAnimationEvent>().monsterModel = this.monsterModel;
        }
    }

    private void CheckPlayer() {
        if (this.playerModel.isHurt) {
        } else if (this.playerModel.isIdle) {
            if (this.playerModel.waitCount-- > 0) {
                return;
            }
            this.playerModel.isIdle = false;
            this.playerModel.isAtk = true;
            this.actor.testActor.animator.SetTrigger("Attack1");
            this.playerModel.waitCount = UnityEngine.Random.Range(20, 75);
        } else if (this.playerModel.isAtk) {
            // 检查攻击是否有效
            if (this.playerModel.isAtkValid) {
                this.playerModel.isAtkValid = false;
                this.monster.Hurt(this.playerModel.curWeapon.attack, this.playerModel.curWeapon.attackStunRate);
            }
        }
    }

    private void CheckMonster() {
        if (this.monsterModel.isHurt) {
        } else if (this.monsterModel.isIdle) {
            if (this.monsterModel.waitCount-- > 0) {
                return;
            }
            this.monsterModel.isIdle = false;
            this.monsterModel.isAtk = true;
            this.monster.animator.SetTrigger("Attack1");
            this.monsterModel.waitCount = UnityEngine.Random.Range(20, 75);
        } else if (this.monsterModel.isAtk) {
            // 检查攻击是否有效
            if (this.monsterModel.isAtkValid) {
                this.monsterModel.isAtkValid = false;
                this.actor.Hurt(this.monsterModel.attack, this.monsterModel.attackStunRate);
            }
        }
    }

    private void CheckResult() {
        if (this.playerModel.Health <= 0) {
            this.Dead();
        } else if (this.monsterModel.Health <= 0) {
            this.Win();
        }
    }

    private bool CheckIdle(Animator animator, string name) {
        AnimatorStateInfo currentState = animator.GetCurrentAnimatorStateInfo(0);
        if (currentState.IsName(name)) {
            return true;
        }
        return false;
    }
}
