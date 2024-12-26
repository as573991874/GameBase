// 玩家数据
using System.Collections.Generic;

public class PlayerModel : BaseModel {
    // 昵称
    private string nick;
    public string Nick { get => nick; set => SetField(ref nick, value); }

    // 当前血量
    private int health;
    public int Health { get => health; set => SetField(ref health, value); }

    // 最大血量
    public int maxHealth;
    // 当前武器
    public WeaponModel curWeapon;
    // 持有武器
    public List<WeaponModel> weapons;

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

// 武器数据
public class WeaponModel {
    // 武器名称
    public string name;
    // 武器标示
    public int index;
    // 武器攻击力
    public int attack;
    // 攻击硬直率
    public float attackStunRate = 1f;
}
