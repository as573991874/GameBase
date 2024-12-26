using System.Collections;
using UnityEngine;

// 账号登陆阶段
public class LoginStageSystem : BaseSystem
{
    protected override IEnumerator OnLoad()
    {
        // 准备数据（测试延迟）
        for (var i = 0; i < 5; i++)
        {
            yield return null;
        }
        this.LoadFinish();
    }

    protected override void OnTick()
    {
    }
}
