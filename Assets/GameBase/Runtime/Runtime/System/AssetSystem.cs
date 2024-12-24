using System.Collections;
using UnityEngine;
using UnityEngine.SceneManagement;

public class AssetRequest<T> where T : Object {
    private ResourceRequest request;
    public AssetRequest(ResourceRequest request) {
        this.request = request;
    }

    public IEnumerator WaitLoad() {
        while (!request.isDone) {
            yield return null;
        }
    }

    public T Asset {
        get {
            return request.asset as T;
        }
    }
}


// 资源系统，管理资源加载
public class AssetSystem : BaseSystem {
    protected override IEnumerator OnLoad() {
        // 准备数据（测试延迟）
        for (var i = 0; i < 5; i++) {
            yield return null;
        }
        this.LoadFinish();
    }

    // 加载资源
    public AssetRequest<T> LoadAsset<T>(string path) where T : Object {
        ResourceRequest request = Resources.LoadAsync<T>(path);
        return new AssetRequest<T>(request);
    }

    // 加载资源
    public IEnumerator LoadScene(string path) {
        AsyncOperation asyncLoad = SceneManager.LoadSceneAsync(path);
        asyncLoad.allowSceneActivation = true;
        while (!asyncLoad.isDone) {
            yield return null;
        }
    }

    protected override void OnTick() {
    }
}
