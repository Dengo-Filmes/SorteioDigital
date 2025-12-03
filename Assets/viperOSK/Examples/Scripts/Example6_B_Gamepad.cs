////////////////////////////////////////////
///
/// Example 6.B: this example is designed for multiple controller to illustrate one way multiple controllers could be used with viperOSK
/// In the example, players *simultaneously* use the same keyboard to enter input, this input could be for the same input field (receiver)
/// or separate one
/// 
/// Note also that the internal OSK_Keyboard controller support is bypassed and the control is handled in the OSK_GamepadHelper class
/// 
/// © vipercode corp
/// 2023
/// Please use this asset according to the attached license
/// Attributions, mentions and reviews are always welcomed
///
///////////////////////////////////////////////////////////////////////////////////////


using System.Collections;
using System.Collections.Generic;
using TMPro;
using UnityEngine;
using UnityEngine.Events;
using viperOSK;



namespace viperOSK_Examples
{

    public class Example6_B_Gamepad : MonoBehaviour
    {
        public OSK_Keyboard keyboard;
        public OSK_Receiver receiver1;
        public OSK_Receiver receiver2;

        public ParticleSystem particle;

        public TMP_Text message;
        bool isWarning;

        public AudioClip successSound;
        public AudioClip invalidSound;

#if UNITY_2019
        public UE2019_EventInt onSuccessEntry;
        public UE2019_EventInt onFailEntry;
#else
        public UnityEvent<int> onSuccessEntry;
        public UnityEvent<int> onFailEntry;
#endif

        private void Awake()
        {
#if UNITY_2019
            onSuccessEntry = new UE2019_EventInt();
            onFailEntry = new UE2019_EventInt();
#endif
        }

        public void PlayerOneSubmit()
        {
            // enter your conditions for a what's considered a proper entry here
            if (receiver1.Text().Length > 0)
            {
                particle.transform.position = receiver1.transform.position;
                particle.Emit(10);

                onSuccessEntry.Invoke(1);

            }
            else
            {
                particle.transform.position = receiver1.transform.position;
                particle.Emit(1);

                onFailEntry.Invoke(1);
            }

        }

        public void PlayerTwoSubmit()
        {
            // enter your conditions for a what's considered a proper entry here
            if (receiver2.Text().Length > 0)
            {
                particle.transform.position = receiver2.transform.position;
                particle.Emit(10);

                onSuccessEntry.Invoke(2);

            }
            else
            {
                particle.transform.position = receiver2.transform.position;
                particle.Emit(1);

                onFailEntry.Invoke(2);
            }
        }

        public void SuccessEntry(int playernum)
        {
            if(playernum == 1)
            {
                receiver1.OnFocusLost();
            }

            if (playernum == 2)
            {
                receiver2.OnFocusLost();
            }

            if(!receiver1.isFocused() && !receiver2.isFocused())
            {
                // if both names are in, reset fields. Developers can put script here to go to the next screen or load a scene
                receiver1.ClearText();
                receiver2.ClearText();
                receiver1.OnFocus();
                receiver2.OnFocus();
            }

        }

        public void FailDing()
        {
            this.GetComponent<AudioSource>().PlayOneShot(invalidSound);
        }

        public void SuccessDing()
        {
            this.GetComponent<AudioSource>().PlayOneShot(successSound);
        }

        public void TwoControllerWarning()
        {
            // already warning - turn off
            if(isWarning)
            {
                message.fontStyle = FontStyles.Normal;
                message.color = Color.white;
                isWarning = false;
            } else
            {
                isWarning = true;
                message.fontStyle = FontStyles.Bold;
                message.color = Color.red;
                Invoke("TwoControllerWarning", 3f);

            }

            Debug.Log("twocontroler");
            
        }

        // Start is called before the first frame update
        void Start()
        {
            if (keyboard == null)
            {
                Debug.LogError("keyboard needs to be assigned an OSK_Keyboard, you can do this in the inspector");
                
                    
            } 


            string[] joysticks = Input.GetJoystickNames();
            Debug.Log("num of joysticks:" + joysticks.Length);
            foreach(string n in joysticks)
            {
                Debug.Log("joy:" + n);
            }
            
            if(joysticks.Length < 2)
            {
                Invoke("TwoControllerWarning", 1f);
            }

            Invoke("InputFieldsFocus", 1f);

         }

        void InputFieldsFocus()
        {
            receiver1.OnFocus();
            receiver2.OnFocus();
        }




    }

}
