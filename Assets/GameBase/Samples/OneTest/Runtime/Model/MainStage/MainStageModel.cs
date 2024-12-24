// 主阶段
public class MainStageModel
{
    // 当前阶段
    public MainStageEnum stage;
    // 关卡 Id
    public int missionId;

    public string GetStageInfo() {
        switch (stage)
        {
            case MainStageEnum.Campsite:
                return "营地";
            case MainStageEnum.Journey:
                return "旅途";
            default:
                return "";
        }
    }
}
