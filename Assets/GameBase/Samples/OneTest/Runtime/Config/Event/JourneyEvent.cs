// 旅行玩法事件
using UnityEngine;

public struct JourneyNextEvent : IEventData
{
}

// 治疗事件
public struct JourneyTreatEvent : IEventData
{
}

// 血量变化事件
public struct JourneyActorHealthEvent : IEventData {
    public int health;
    public Transform transform;
}

// 相遇事件
public struct JourneyMeetEvent: IEventData{
    public int index;
}
