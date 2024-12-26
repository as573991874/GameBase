// 怪物数据
using UnityEngine;

public class MonsterModel : BaseModel {
    // 昵称
    public string nick;
    // 标示
    public string sign;
    // 血量
    private int health;
    public int Health { get { return health; } set => SetField(ref health, value); }
    public int maxHealth;
    // 攻击力
    public int attack;
    // 攻击硬直率
    public float attackStunRate = 1f;
    // 出生位置(百分比)
    public float spawnPercent;
    public Quaternion curForward;
    public Vector3 position;

    // 待机中
    public bool isIdle = false;
    // 攻击中
    public bool isAtk = false;
    // 攻击有效期
    public bool isAtkValid = false;
    // 受伤中
    public bool isHurt = false;
    // 随机等待
    public int waitCount = 0;
}
