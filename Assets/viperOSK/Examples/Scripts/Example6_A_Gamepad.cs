////////////////////////////////////////////
///
/// Example 6.A: this example is designed for multiple controller to illustrate one way multiple controllers could be used with viperOSK
/// In the example, players take turn to enter their name, when a player hits "GO" the input is switched to the other player
/// note the routines that run from the Inspector of the OSK_Receiver that call the actions to switch players
/// Note also that the internal OSK_Keyboard controller support is bypassed and the control is handled here in this script
/// to handle the alternating players and change of the highlighted key color
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
using viperOSK;
#if REWIRED
using Rewired;
#endif

namespace viperOSK_Examples
{

    public class Example6_A_Gamepad : MonoBehaviour
    {
        public OSK_Keyboard keyboard;
        public OSK_GamepadHelper player1;
        public OSK_GamepadHelper player2;

        public ParticleSystem particle;

        public TMP_Text message;
        bool isWarning;

        public int currentPlayer = 1;

        public float inputReactiveness;
        float t;

        Vector2 joy;

        public void NextPlayer()
        {
            // this if statement ensures that the player did not leave their name blank. One could make it more fancy and ensure they have a certain
            // number of letters/digits and not just symbols or spaces
            if ((currentPlayer == 1 && player1.receiver.Text().Length > 0) || (currentPlayer == 2 && player2.receiver.Text().Length > 0))
            {

                currentPlayer = currentPlayer == 1 ? 2 : 1;

                if (currentPlayer == 1)
                {
                    PlayerOne();
                }
                else
                {
                    PlayerTwo();
                }
            } else
            {
                // this is what happens when the player did not fill out a name, the cursor stays where it is and we emit one particle as visual reminder
                if (currentPlayer == 1)
                    particle.transform.position = player1.receiver.transform.position;
                else
                    particle.transform.position = player2.receiver.transform.position;

                particle.Emit(1);
            }
        }

        public void PlayerOne()
        {
            // change the key highlighter color to player 1's color
            keyboard.highlighterColor = new Color(240f, 0f, 0f);

            player1.Activate();
            player2.DeActivate();

            particle.transform.position = player1.receiver.transform.position;
            particle.Emit(10);
        }

        public void PlayerTwo()
        {
            // change the key highlighter color to player 2's color
            keyboard.highlighterColor = new Color(0f, 240f, 0f);

            player1.DeActivate();
            player2.Activate();

            particle.transform.position = player2.receiver.transform.position;
            particle.Emit(10);
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

            
            
        }

        // Start is called before the first frame update
        void Start()
        {
            if (keyboard == null)
            {
                keyboard = GameObject.FindObjectOfType<OSK_Keyboard>();
                if (keyboard == null)
                    Debug.LogError("keyboard needs to be assigned an OSK_Keyboard, you can do this in the inspector");
                else
                    keyboard.bypassDefaultInput = true;
            }

            joy = new Vector2();

            string[] joysticks = viperTools.viperInput.GetControllerNames();
            Debug.Log("num of joysticks:" + joysticks.Length);
            foreach(string n in joysticks)
            {
                Debug.Log("joy:" + n);
            }
            
            if(joysticks.Length < 2)
            {
                player2.DeActivate();
                Invoke("TwoControllerWarning", 1f);

            }

            PlayerOne();
        }

        // Update is called once per frame
        void FixedUpdate()
        {

        }

        private void Update()
        {

        }
    }

}
