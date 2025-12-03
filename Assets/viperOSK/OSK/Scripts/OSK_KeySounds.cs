///////////////////////////////////////////////////////////////////////////////////////
///
/// OSK_Keyboard Class - this is the main class - ensure that all necessary references are made in the Unity inspector
///
/// © vipercode corp
/// 2022
/// Please use this asset according to the attached license
/// Attributions, mentions and reviews are always welcomed
///
///////////////////////////////////////////////////////////////////////////////////////

using System.Collections;
using System.Collections.Generic;
using UnityEngine;


namespace viperOSK
{
    /// <summary>
    /// OSK_KeySounds manages the various sounds keys make through a List of audioclips
    /// the scripts then reference this list based on the index of the sound to play
    /// </summary>
    public class OSK_KeySounds : MonoBehaviour
    {

        public List<AudioClip> keySounds = new List<AudioClip>();

        public AudioClip selectKeySound;

        // Start is called before the first frame update
        void Start()
        {

        }

        public void PlaySound(int k)
        {
            if (keySounds.Count > 0 && k >= 0 && k < keySounds.Count)
            {
                this.GetComponent<AudioSource>().PlayOneShot(keySounds[k]);
            }
        }

        public void PlaySelectKeySound()
        {
            if(selectKeySound != null)
                this.GetComponent<AudioSource>().PlayOneShot(selectKeySound);
        }

        // Update is called once per frame
        void Update()
        {

        }
    }

}
