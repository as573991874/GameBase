// 系统事件
public struct AppInitEvent : IEventData {
    public int frameCount;
}

public struct AppStageSwitchEvent : IEventData {
    public AppStageEnum stage;
    public int frameCount;
}

public struct AppStageSwitchFinishEvent : IEventData
{
    public AppStageEnum stage;
    public int frameCount;
}

public struct AppAssetLoadFinishEvent : IEventData {
    public int frameCount;
}
