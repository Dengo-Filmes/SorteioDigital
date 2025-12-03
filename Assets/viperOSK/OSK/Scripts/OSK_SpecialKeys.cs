///////////////////////////////////////////////////////////////////////////////////////
/// © vipercode corp
/// 2022
/// Please use this asset according to the attached license
/// Attributions, mentions and reviews are always welcomed
///
///////////////////////////////////////////////////////////////////////////////////////


using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using System;
using UnityEngine.Events;

namespace viperOSK
{
    /// <summary>
    /// OSK_SpecialKeys provides the structure to control the look, label and sound of user-defined special keys
    /// these keys can be set using the Inspector
    /// </summary>
    [Serializable]
    public class OSK_SpecialKeys
    {
        public OSK_KeyCode keycode;
        public string name;
        public Color col;
        public float x_size;
        public int keySoundCode;

#if UNITY_2019
        public UE2019_Key specialAction;
#else

        public UnityEvent<OSK_KeyCode, OSK_Receiver> specialAction;
#endif

        public OSK_SpecialKeys(OSK_KeyCode k, string n, Color c, float s)
        {
            keycode = k;
            name = n;
            col = c;
            x_size = s;

            //(v3.4) backward compatibility
#if UNITY_2019
            specialAction = new UE2019_Key();
#endif
        }
    }

}