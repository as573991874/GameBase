using UnityEngine;
using System.Collections;

public class JourneyScene : BaseScene {
    public override string path => "GameBase/Samples/OneTest/Scene/Journey";

    // model
    public Vector3 startPos;
    public Vector3 forward;

    // view

    // 初始化
    protected override void OnInit() {
        var camera = GameObject.Find("Camera");
        var zs = camera.transform.Find("ZS");
        startPos = zs.position;
        forward = zs.forward;
        camera.SetActive(false);
    }
}
