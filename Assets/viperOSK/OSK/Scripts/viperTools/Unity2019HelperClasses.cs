using System;
using UnityEngine;
using UnityEngine.Events;

namespace viperOSK
{

    /// (v3.4) classes necessary for backwards compatibility with Unity 2019.4


#if UNITY_2019

    [System.Serializable]
    public class UE2019_Key : UnityEvent<OSK_KeyCode, OSK_Receiver>
    {
    }

    [System.Serializable]
    public class UE2019_Rec : UnityEvent<string>
    {
    }

    [System.Serializable]
    public class UE2019_LongPress : UnityEvent<OSK_LongPressPacket>
    {
    }
    
    [System.Serializable]
    public class UE2019_EventInt : UnityEvent<int>
    {
    }

#endif

}
