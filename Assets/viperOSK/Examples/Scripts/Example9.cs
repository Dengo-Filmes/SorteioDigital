/// 
/// © vipercode corp
/// 2024
/// Please use this asset according to the attached license
/// Attributions, mentions and reviews are always welcomed
///
///////////////////////////////////////////////////////////////////////////////////////

using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.SceneManagement;
using viperOSK;

namespace viperOSK_Examples
{

    public class Example9 : MonoBehaviour
    {

        public OSK_Keyboard keyboard;

        public OSK_AccentConsole accentConsole;

        public OSK_LanguagePackage altKeyboard;

        bool isAltLayout;

        public void ToggleAltLanguageAsset()
        {
            if (altKeyboard.altKeyboardLayout != null && altKeyboard.altKeyboardLayout != string.Empty)
            {
                StartCoroutine(ToggleLanguageCoroutine());
            }
        }

        /// <summary>
        /// use this as a template approach to change languages. The coroutine ensures that Unity does not through errors when buttons are erased and replaced with their
        /// other counterparts
        /// </summary>
        IEnumerator ToggleLanguageCoroutine()
        {
            isAltLayout = !isAltLayout;
            if (isAltLayout)
            {
                keyboard.layout = altKeyboard.altKeyboardLayout;
            }
            else
            {

                keyboard.layout = altKeyboard.keyboardLayout;
            }

            // this is needed especially for UI versions so Selectable objects are not destroyed while Unity is still accessing them
            yield return new WaitForEndOfFrame();

            keyboard.Reset();

            yield return new WaitForEndOfFrame();

            keyboard.AutoCorrectLayout();
            keyboard.Generate();
        }

        public void Submit(string text)
        {
            Debug.Log("SUBMITTED TEXT: " + text);

            // script to load another example for mobile testing
            if (text.Equals("switch", StringComparison.InvariantCultureIgnoreCase))
            {
                int currentSceneIndex = SceneManager.GetActiveScene().buildIndex;
                Debug.Log("current scene = " + currentSceneIndex + " / " + SceneManager.sceneCountInBuildSettings);
                if (currentSceneIndex < SceneManager.sceneCountInBuildSettings - 1)
                {
                    SceneManager.LoadScene(currentSceneIndex + 1);
                }
                else
                {
                    SceneManager.LoadScene(0);
                }
                return;
            }
        }

        // Start is called before the first frame update
        void Start()
        {
            if (altKeyboard.accentPackage != null) accentConsole.LoadAccentMap(altKeyboard.accentPackage);
        }

        // Update is called once per frame
        void Update()
        {

        }
    }

}
