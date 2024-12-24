using UnityEditor.Animations;
using UnityEngine;

public class JourneyMonster : BaseActor {
    public override string path => $"Actor/{this.monsterModel.sign}";

    // model
    private MonsterModel monsterModel;

    // view
    public Animator animator;
    public Transform body;
    public CharacterController actorController;

    public void Inject(MonsterModel model) {
        this.monsterModel = model;
    }

    // 初始化
    protected override void OnInit() {
        this.body = this.transform.Find("Body");
        this.animator = body.GetComponent<Animator>();
        this.actorController = this.gameObject.GetComponent<CharacterController>();
    }

    // 打开
    protected override void OnOpen() {
        this.transform.position = this.monsterModel.position;
        this.body.rotation = this.monsterModel.curForward;
    }

    // 关闭
    protected override void OnClose() {
    }

    // 设置位置
    public void SetPosition(Vector3 position) {
        if (this.transform) {
            this.transform.position = position;
        }
        this.monsterModel.position = position;
    }

    // 设置朝向
    public void SetForward(Vector3 forward) {
        this.monsterModel.curForward = Quaternion.LookRotation(forward);
    }

    // 受伤
    public void Hurt(int attack, float spd) {
        this.monsterModel.Health -= attack;
        this.monsterModel.isAtk = false;
        this.monsterModel.isAtkValid = false;
        this.monsterModel.isIdle = false;
        this.monsterModel.isHurt = true;
        this.animator.SetTrigger("Hurt");
        // 设置速度
        var controller = this.animator.runtimeAnimatorController as AnimatorController;
        for (var i = 0; i < controller.layers[0].stateMachine.states.Length; i++) {
            var state = controller.layers[0].stateMachine.states[i].state;
            if (state.name == "f_hurt") {
                state.speed = 1 / spd;
            }
        }
        this.eventRunner.Dispath(new JourneyActorHealthEvent(){health = -attack, transform = this.transform});
    }

    // 更新
    protected override void OnTick() {
        if (this.body.rotation != this.monsterModel.curForward) {
            this.body.rotation = Quaternion.Lerp(
                this.body.rotation,
                this.monsterModel.curForward,
                Time.deltaTime * 5
            );
        }

        if (this.actorController) {
            this.actorController.SimpleMove(Vector3.zero);
        }
    }
}
