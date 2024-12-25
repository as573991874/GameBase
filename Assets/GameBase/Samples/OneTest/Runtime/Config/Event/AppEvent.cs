// 系统事件
public struct AppInitEvent : IEventData {
    public int frameCount;
}

// 场景切换
public struct AppStageSwitchEvent : IEventData {
    public AppStageEnum stage;
    public int frameCount;
}

// 场景切换完成
public struct AppStageSwitchFinishEvent : IEventData {
    public AppStageEnum stage;
    public int frameCount;
}

// 开始初始化资源包
public struct AppInitPackageEvent : IEventData { }

// 开始更新资源包
public struct AppLoadPackageEvent : IEventData { }

// 资源准备完成
public struct AppAssetLoadFinishEvent : IEventData {
    public int frameCount;
}
