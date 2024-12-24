using Dreamteck.Splines;
using UnityEngine;

public class JourneyModel : BaseModel {
    // 移动路径
    public SplineComputer splinePath;

    // 镜头
    private int cameraIndex = 0;
    public int CameraIndex { get { return cameraIndex; } set => SetField(ref cameraIndex, value); }

    // 当前位置百分比
    private double curSplinePercent = 0f;
    public double CurSplinePercent { get { return curSplinePercent; } set => SetField(ref curSplinePercent, value); }

    // 移动速度
    public float walkSpeed = 3f;

    // 奔跑速度
    public float runSpeed = 8f;

    // 当前速度
    private float curSpeed = 0f;
    public float CurSpeed { get { return curSpeed; } set => SetField(ref curSpeed, value); }

    // 加速度
    public float acceleration = 3.5f;

    // 当前朝向
    public Quaternion curForward;

    // 当前是否待机
    private bool isIdle = false;
    public bool IsIdle { get { return isIdle; } set => SetField(ref isIdle, value); }

    // 是否快速移动
    private bool isRun = true;
    public bool IsRun { get { return isRun; } set => SetField(ref isRun, value); }

    // 遇到敌人阶段
    private bool isMeet = false;
    public bool IsMeet { get { return isMeet; } set => SetField(ref isMeet, value); }

    // 战斗中
    private bool isBattle = false;
    public bool IsBattle { get { return isBattle; } set => SetField(ref isBattle, value); }

    // 是否靠近
    public bool IsNear = false;

    // 是否等待重生
    public bool IsDead = false;

    // 是否完成路径
    private bool isFinish = false;
    public bool IsFinish { get { return isFinish; } set => SetField(ref isFinish, value); }

    // 下一波遭遇的敌人索引
    private int nextEnergyIndex;
    public int NextEnergyIndex { get { return nextEnergyIndex; } set => SetField(ref nextEnergyIndex, value); }

    // 战斗数据关联（先这样放）
    public MonsterModel battleMonsterModel;
    public Transform battleActor;
    public Transform battleEnemy;
}
