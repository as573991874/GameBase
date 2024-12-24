using UnityEngine;
using System.Collections;
using Dreamteck.Splines;

public class JourneyScene : BaseScene {
    public override string path => "Scene1211/Journey";

    // model
    public Vector3 startPos;
    public Vector3 forward;
    public SplineComputer splinePath;

    // view

    // 初始化
    protected override void OnInit() {
        var camera = GameObject.Find("Camera");
        var zs = camera.transform.Find("ZS");
        startPos = zs.position;
        forward = zs.forward;
        camera.SetActive(false);
        var wayPoints = GameObject.Find("WayPoints");
        splinePath = wayPoints.GetComponent<SplineComputer>();
    }
}
