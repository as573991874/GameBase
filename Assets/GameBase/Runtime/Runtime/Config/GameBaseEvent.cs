// 游戏初始化完成
public struct GameBaseInitEvent : IEventData {
    public int frameCount;
}

// 资源包初始化失败
public struct GamePackageInitFailEvent : IEventData { }

// 资源包初始化完成
public struct GamePackageInitEvent : IEventData { }

// 资源下载失败事件
public struct WebFileDownloadFailEvent : IEventData {
    public string FileName;
    public string Error;
}

// 资源包下载失败事件
public struct GamePackageLoadFailEvent : IEventData { }

// 资源包下载完成事件
public struct GamePackageLoadFinishEvent : IEventData { }
