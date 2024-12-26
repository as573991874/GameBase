using System.Collections;

// 应用状态
public class AppStageModel {
    // 当前阶段
    public AppStageEnum stage = AppStageEnum.Init;
    // 切换中
    public bool switching = false;
    // 正在切换的协程
    public IEnumerator switchCoroutine = null;

    public string GetStageInfo(){
        switch (stage){
            case AppStageEnum.Load:
                return "资源加载中";
            case AppStageEnum.Login:
                return "账号登陆中";
            case AppStageEnum.Main:
                return "正在进入游戏";
            default:
                return "";
        }
    }
}
