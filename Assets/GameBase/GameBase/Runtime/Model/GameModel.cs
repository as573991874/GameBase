// 游戏基础信息
public class GameModel : BaseModel {
    // 游戏是否完成初始化
    public bool GameInitialize = false;

    // 游戏配置
    public GameSetting GameSetting;

    // 资源包
    public AssetModel AssetModel;
}
