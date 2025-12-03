/// 
/// © vipercode corp
/// 2024
/// Please use this asset according to the attached license
/// Attributions, mentions and reviews are always welcomed
///
///////////////////////////////////////////////////////////////////////////////////////

using System.Collections;
using System.Collections.Generic;
using TMPro;
using UnityEngine;
using UnityEngine.UI;
using viperOSK;

namespace viperOSK_Examples
{

    public class Example8UI : MonoBehaviour
    {
        public OSK_UI_Keyboard keyboard;

        public OSK_AccentConsole accentConsole;

        public TMP_Text outputText;

        public OSK_LanguagePackage currentLanguage;
        bool isAltLayout;

        public Image currentLangHighlight;

        public void SetOutputText(string text)
        {
            outputText.text = text;
        }

        /// <summary>
        /// sets the highlighter the btn language pressed
        /// </summary>
        /// <param name="btn">selected btn</param>
        public void HighlightLanguageBtn(Button btn)
        {
            currentLangHighlight.rectTransform.localPosition = btn.transform.localPosition;
        }


        public void LoadLanguageAsset(OSK_LanguagePackage languageAsset)
        {
            currentLanguage = languageAsset;
            keyboard.layout = languageAsset.keyboardLayout;
            if(languageAsset.accentPackage != null) accentConsole.LoadAccentMap(languageAsset.accentPackage);
            keyboard.AutoCorrectLayout();
            keyboard.Generate();

            isAltLayout = false;
        }

        public void ToggleAltLanguageAsset()
        {
            if(currentLanguage.altKeyboardLayout != null && currentLanguage.altKeyboardLayout != string.Empty)
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
                keyboard.layout = currentLanguage.altKeyboardLayout;
            }
            else
            {

                keyboard.layout = currentLanguage.keyboardLayout;
            }

            // this is needed especially for UI versions so Selectable objects are not destroyed while Unity is still accessing them
            yield return new WaitForEndOfFrame();

            keyboard.Reset();

            yield return new WaitForEndOfFrame();

            keyboard.AutoCorrectLayout();
            keyboard.Generate();
        }

        // Start is called before the first frame update
        void Start()
        {

        }

        // Update is called once per frame
        void Update()
        {

        }
    }

}
