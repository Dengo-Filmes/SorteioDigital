///////////////////////////////////////////////////////////////////////////////////////
///
/// Example 4 - this example showcases the viperOSK 2.0 functionalities for non-UI implementation
/// You can now have multiple input fields in your scene and select or "tab" between them
/// the OSK_Receiver is a new component that manages how the OSK keyboard input is handled by the input field
/// this also includes how it shows on screen, see the how the password field is hidden with '*'s. 
/// This is controled by the charMask string, leave the char empty ("") if you want the text to show as-is on screen
/// 
/// Each OSK_Receiver has it's own OnSubmit unity even that can be called, in this example if the user hits "send" while still in
/// the username field it calls a Tab instead
/// Whereas when the user is in the password field it will attempt to submit the username/password combination
///
/// © vipercode corp
/// 2022
/// Please use this asset according to the attached license
/// Attributions, mentions and reviews are always welcomed
///
///////////////////////////////////////////////////////////////////////////////////////

using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using TMPro;
using viperOSK;
using UnityEngine.SceneManagement;

namespace viperOSK_Examples
{
    public class Example4 : MonoBehaviour
    {
        // list of input fields for OSK
        public List<OSK_Receiver> inputFields;

        // current selected field
        int currentField;

        public OSK_Keyboard keyboard;

        string username, password;

        public TextMeshPro usernameTMP;
        public TextMeshPro pwdTMP;

        // Start is called before the first frame update
        void Start()
        {
            username = "";
            password = "";
        }

        public void TabKey()
        {
            if(currentField == 0 && username.Length == 0)
            {
                username = inputFields[currentField].Text();
            }
            currentField++;
            currentField = currentField % inputFields.Count;

            keyboard.SetOutput(inputFields[currentField]);

        }

        public void UsernameSubmit(string txt)
        {
            if (txt == null || txt.Length == 0)
                return;

            // if there is a username, save it and then tab over to the password
            username = txt;

            if (txt.Equals("switch", StringComparison.InvariantCultureIgnoreCase))
            {
                int currentSceneIndex = SceneManager.GetActiveScene().buildIndex;
                if (currentSceneIndex < SceneManager.sceneCountInBuildSettings - 1)
                {
                    SceneManager.LoadScene(currentSceneIndex + 1);
                } else
                {
                    SceneManager.LoadScene(0);
                }
                return;
            }


            TabKey();
        }

        public void PasswordSubmit(string txt)
        {
            if (txt == null || txt.Length == 0)
                return;

            password = txt;


            
            if(inputFields[0].Text().Length == 0)
            {
                usernameTMP.text = "[missing username]";
                return;
            }
            username = inputFields[0].Text();

            usernameTMP.text = username;
            pwdTMP.text = password;

            inputFields[0].ClearText();
            inputFields[1].ClearText();

            // return to username
            TabKey();

            Invoke("Cleanup", 10f);
        }

        void Cleanup()
        {
            usernameTMP.text = "";
            pwdTMP.text = "";
        }


        // Update is called once per frame
        void Update()
        {

        }
    }
}