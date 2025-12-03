using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace viperOSK
{

    public interface I_OSK_Key
    {

        OSK_KeyCode GetKeyCode();

        void Click(string keyDevice, OSK_Receiver inputfield = null);

        void OnKeyPress(string keyDevice, OSK_Receiver inputfield = null);

        void OnKeyDepress(string keyDevice, OSK_Receiver inputfield = null);

        void Highlight(bool hi, Color c);

        OSK_KEY_TYPES KeyType();

        Transform GetKeyTransform();

        Vector2Int GetLayoutLocation();

        string GetKeyName();

        object GetObject();

        GameObject GetGameObject();

        float getXSize();

        float getYSize();
    }
}
