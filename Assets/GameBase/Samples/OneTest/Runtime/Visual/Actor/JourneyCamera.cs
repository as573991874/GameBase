using UnityEngine;
using Cinemachine;

public class JourneyCamera : BaseActor {
    public override string path => "CM_JourneyCamera";

    // model
    private JourneyActor playerActor;
    private JourneyModel journeyModel;

    // view
    private Animator cameraAnimator;
    private CinemachineVirtualCamera walkCamera;
    private CinemachineVirtualCamera walkCamera2;
    private CinemachineVirtualCamera battleCamera;
    private CinemachineTargetGroup cinemachineTargetGroup;

    public void Inject(JourneyActor playerActor, JourneyModel journeyModel) {
        this.playerActor = playerActor;
        this.journeyModel = journeyModel;
    }

    // 初始化
    protected override void OnInit() {
        this.cameraAnimator = this.gameObject.GetComponent<Animator>();
        this.walkCamera = this.transform.Find("WalkCamera").GetComponent<CinemachineVirtualCamera>();
        this.walkCamera2 = this.transform.Find("WalkCamera2").GetComponent<CinemachineVirtualCamera>();
        this.battleCamera = this.transform.Find("BattleCamera").GetComponent<CinemachineVirtualCamera>();
        this.cinemachineTargetGroup = this.transform.Find("BattleGroup").GetComponent<CinemachineTargetGroup>();
    }

    protected override void OnOpen() {
        this.SetFollow(playerActor.transform);
        this.cameraAnimator?.SetInteger("state", this.journeyModel.CameraIndex);
    }

    // 设置镜头跟随
    public void SetFollow(Transform actor) {
        walkCamera.Follow = actor;
        walkCamera2.Follow = actor;
    }

    // 设置战斗镜头朝向
    public void SetLookAt(Transform target){
        this.cinemachineTargetGroup.m_Targets = null;
        this.cinemachineTargetGroup.AddMember(walkCamera.Follow, 1, 1);
        this.cinemachineTargetGroup.AddMember(target, 1, 1);
    }

    protected override void OnTick() {
        this.journeyModel.DirtyCheck("CameraIndex", cameraAnimator, journeyModel.CameraIndex, "state");
    }
}
