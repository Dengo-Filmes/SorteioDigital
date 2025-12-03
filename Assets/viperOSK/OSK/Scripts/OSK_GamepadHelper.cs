///////////////////////////////////////////
/// This class is a helper class for the 3d version of viperOSK (not the UI one)
/// It allows developers to setup multiple controller support on the same keyboard
/// it can also be setup so that each controler has its own receiver as in Example 6.B in the asset package
/// 
/// To use Rewired controls, please define REWIRED in the Player Settings
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
using UnityEngine.Events;
#if REWIRED
using Rewired;
#endif

namespace viperOSK
{

    public class OSK_GamepadHelper : MonoBehaviour
    {

        public OSK_Keyboard keyboard;

        /// <summary>
        /// you can select a receiver for this gamepad, a null value would use the OSK's output receiver
        /// </summary>
        public OSK_Receiver receiver;

        /// <summary>
        /// graphical gameobject that goes over the key to indicate the selected key
        /// </summary>
        public GameObject selectionMarker;

        /// <summary>
        /// gamepad or joystick number between 1 to 8. A value of 0 means the component will catch all joystick/gamepad input
        /// </summary>
        public int gamepadNum;

        /// <summary>
        /// represents the present state of the x and y on the joystick axis
        /// </summary>
        Vector2 joy;

        /// <summary>
        /// when true, the character selected will repeat when the user holds a gamepad button down.
        /// </summary>
        public bool allowRepeatButton;

        public bool invertY;

        protected OSK_Key selectedKey;

        /// <summary>
        /// how reactive moving the joystick in terms of seconds (2x for buttons)
        /// </summary>
        public float inputReactiveness;
        float t;
        float tBtn;
        /// <summary>
        /// to prevent characters from repeating too quickly if a user holds a button down
        /// </summary>
        bool aBtnPressed = false;
        bool bBtnPressed = false;


        bool active = true;
        bool connected = false;

        /// <summary>
        /// Unity events to run when Activate() is called
        /// </summary>
        public UnityEvent onActivate;

        /// <summary>
        /// Unity events to run when DeActivate() is called
        /// </summary>
        public UnityEvent onDeactivate;

        /// <summary>
        /// Preparations for gamepad functionality, must be called right after OSK_Keyboard.Generate()
        /// </summary>
        public void GamepadPrep()
        {

            if (keyboard == null)
            {
                keyboard = GameObject.FindObjectOfType<OSK_Keyboard>();
                if (keyboard == null)
                    Debug.LogError("keyboard needs to be assigned an OSK_Keyboard, you can do this in the inspector");
                else
                    keyboard.bypassDefaultInput = true;
            } else
            {
                keyboard.bypassDefaultInput = true;
            }

            if(receiver == null)
            {
                receiver = GameObject.FindObjectOfType<OSK_Receiver>();
                if (receiver == null)
                    Debug.Log("OSK_Receiver needs to be assigned if you want to use separate input fields than the one attached to the keyboard");
            }

            connected = true;

            if(gamepadNum < 0 || viperTools.viperInput.NumControllers() < gamepadNum)
            {
                // developers can replace this part with a routine to ask the user to connect additional controllers
                Debug.LogError("Note enough controllers connected. This controller #"+gamepadNum+" is not in the "+ viperTools.viperInput.NumControllers()+"  gamepad/joysticks connected");
                active = false;
                connected = false;

            }

            if(selectedKey == null)
            {
                selectedKey = keyboard.gameObject.GetComponentInChildren<OSK_Key>();
            }

        }

        public OSK_Key GetSelectedKey()
        {
            return selectedKey;
        }

        public void SetSelectedKey(OSK_Key k)
        {
            selectedKey = k;
        }

        public void SetSelectedKey(string k)
        {
            selectedKey = keyboard.GetOSKKey(k);
        }

        /// <summary>
        /// allows input from controllers, it does not hide the selection marker, if you wish to do so you can add that as a UnityEvent (see Example 6.B)
        /// </summary>
        public void Activate()
        {
            active = true;
            onActivate.Invoke();
        }

        /// <summary>
        /// blocks input from controllers, it does not hide the selection marker, if you wish to do so you can add that as a UnityEvent (see Example 6.B)
        /// </summary>
        public void DeActivate()
        {
            active = false;
            onDeactivate.Invoke();
        }

        // Start is called before the first frame update
        void Start()
        {

            GamepadPrep();
        }

        Vector2 JoystickInput()
        {

#if REWIRED
            
            return ReInput.controllers.GetJoystick(gamepadNum-1).GetAxis2D(0);
#else
            return viperTools.viperInput.GetPlayerJoystickInput(gamepadNum);
#endif
        }

        bool JoystickButtonA()
        {

#if REWIRED

            return ReInput.controllers.GetJoystick(gamepadNum - 1).GetButton(0);
#else

            return viperTools.viperInput.GetPlayerAButton(gamepadNum);
#endif
        }

        bool JoystickButtonB()
        {

#if REWIRED
            return ReInput.controllers.GetJoystick(gamepadNum - 1).GetButton(1);
#else
            return viperTools.viperInput.GetPlayerBButton(gamepadNum);
#endif
        }

        // Update is called once per frame
        void FixedUpdate()
        {
            if (!active || !connected)
                return;

            t += Time.fixedDeltaTime;
            // this code dampens (through script) the feedback from the controller so the selected key moves at a reasonable speed
            // the input speed could be managed in the Input Manager or Input System
            // this solution is implement to accomodate different types of input setups but could be forgone depending on how you setup your input system

            if (t > inputReactiveness)
            {
                t = 0f;
                joy = JoystickInput();

                if (Mathf.Abs(joy.x) > .15f || Mathf.Abs(joy.y) > .15f)
                {
                    if (invertY)
                        joy.y = -joy.y;
                    selectedKey = keyboard.SelectedKeyMove(joy, selectedKey.GetLayoutLocation());
                    
                }

                // if you have a selection widget then it's used, otherwise the key is highlighted using the OSK default
                if (selectionMarker != null)
                {
                    selectionMarker.transform.position = selectedKey.GetKeyTransform().position;
                } else
                {
                    keyboard.SetSelectedKey(selectedKey.key);
                }

            }
        }

        private void Update()
        {

            if (!active || !connected)
                return;

            // resets the button press if allowRepeatButton is true so the characters will type repeatedly at 2x the pace of inputReactiveness
            if (allowRepeatButton)
            {
                tBtn += Time.deltaTime;
                if (tBtn > inputReactiveness * 2f)
                {
                    tBtn = 0f;
                    aBtnPressed = false;
                    bBtnPressed = false;
                }
            }

            if (JoystickButtonA())
            {
                if(!aBtnPressed)
                {
                    aBtnPressed = true;
                    if (selectedKey != null)
                        selectedKey.JoystickPressDown(receiver);
                } 
            }
            else
            {
                aBtnPressed = false;
            }


            if (JoystickButtonB())
            {
                if (!bBtnPressed)
                {
                    bBtnPressed = true;
                    // change this to the behaviour you wish for B button presses
                    OSK_Key back = keyboard.GetOSKKey("Backspace");
                    if (back != null)
                        back.JoystickPressUp(receiver);
                }

            }
            else
            {
                bBtnPressed = false;
            }

        }
    }

}

