using UnityEngine;

public class JourneyActor : BaseActor {
    public override string path => "Actor_Actor";

    // model
    private JourneyModel journeyModel;
    private PlayerModel playerModel;

    // view
    public CharacterController actorController;

    public void Inject(JourneyModel model, PlayerModel playerModel) {
        this.journeyModel = model;
        this.playerModel = playerModel;
    }

    // 初始化
    protected override void OnInit() {
        this.actorController = this.gameObject.GetComponent<CharacterController>();
    }

    // 打开
    protected override void OnOpen() {
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
        this.eventRunner.Dispath(new JourneyActorHealthEvent() { health = -attack, transform = this.transform });
    }

    // 更新
    protected override void OnTick() {
    }
}
