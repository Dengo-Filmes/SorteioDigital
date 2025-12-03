using UnityEngine;
using System.Collections.Generic;

namespace viperOSK
{

    [CreateAssetMenu(fileName = "viperOSK_AccentMap", menuName = "ScriptableObjects/viperOSK_AccentMap", order = 1)]
    public class OSK_AccentAssetObj : ScriptableObject
    {
        public List<AccentEntry> entries = new List<AccentEntry>();
    }

    [System.Serializable]
    public class AccentEntry
    {
        public string baseCharacter;
        public List<string> accentedCharacters = new List<string>();
    }
}
