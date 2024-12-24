using UnityEditor.Animations;
using UnityEngine;

public class JourneyActor : BaseActor {
    public override string path => "Actor/Actor";

    // model
    private JourneyModel journeyModel;
    private PlayerModel playerModel;

    // view
    public TestActor testActor;
    public CharacterController actorController;

    public void Inject(JourneyModel model, PlayerModel playerModel) {
        this.journeyModel = model;
        this.playerModel = playerModel;
    }

    // 初始化
    protected override void OnInit() {
        this.testActor = this.gameObject.GetComponent<TestActor>();
        this.actorController = this.gameObject.GetComponent<CharacterController>();
    }

    // 打开
    protected override void OnOpen() {
        this.testActor?.RefreshBody(0);
        this.journeyModel.curForward = Quaternion.LookRotation(this.testActor.body.forward);
    }

    // 关闭
    protected override void OnClose() {
    }

    // 设置位置
    public void SetPosition(Vector3 position) {
        this.transform.position = position;
    }

    // 设置朝向
    public void SetForward(Vector3 forward) {
        this.journeyModel.curForward = Quaternion.LookRotation(forward);
    }

    // 受伤
    public void Hurt(int attack, float spd) {
        this.playerModel.Health -= attack;
        this.playerModel.isAtk = false;
        this.playerModel.isAtkValid = false;
        this.playerModel.isIdle = false;
        this.playerModel.isHurt = true;
        this.testActor.animator.SetTrigger("Hurt");
        // 设置速度
        var controller = this.testActor.animator.runtimeAnimatorController as AnimatorController;
        for (var i = 0; i < controller.layers[0].stateMachine.states.Length; i++) {
            var state = controller.layers[0].stateMachine.states[i].state;
            if (state.name == "BeHurt") {
                state.speed = 1 / spd;
            }
        }
        this.eventRunner.Dispath(new JourneyActorHealthEvent() { health = -attack, transform = this.transform });
    }

    // 更新
    protected override void OnTick() {
        if (this.testActor.body.rotation != this.journeyModel.curForward) {
            this.testActor.body.rotation = Quaternion.Lerp(
                this.testActor.body.rotation,
                this.journeyModel.curForward,
                Time.deltaTime * 5
            );
        }
    }
}
