using UnityEngine;
using System.Collections.Generic;

namespace viperOSK
{

    [CreateAssetMenu(fileName = "viperOSK_LanguagePackage", menuName = "ScriptableObjects/viperOSK_LanguagePackage", order = 1)]
    public class OSK_LanguagePackage: ScriptableObject
    {
        [SerializeField]
        [TextArea(15, 6)]
        public string keyboardLayout;

        [Space]

        [SerializeField]
        [TextArea(15, 6)]
        public string altKeyboardLayout;

        [Space]
        public OSK_AccentAssetObj accentPackage;
    }
}
