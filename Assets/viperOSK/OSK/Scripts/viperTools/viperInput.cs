/////////////////////////////////////
///  viperInput abstracts basic input between Input Manager and Input System
///  (C) vipercode corp
///  
/////////////////////////////////////


using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

#if ENABLE_INPUT_SYSTEM
using UnityEngine.InputSystem;
#endif



namespace viperTools
{
    public enum AXIS_INPUT
    {
        DPAD_X,
        DPAD_Y,
        LEFTSTICK_X,    // also doubles for main joystick
        LEFTSTICK_Y,    // also doubles for main joystick
        RIGHTSTICK_X,
        RIGHTSTICK_Y,
    }




    /// <summary>
    /// Class to create input abstraction wrapper between legacy Input Manager and Input System
    /// </summary>
    public class viperInput : MonoBehaviour
    {

        public void Start()
        {
            
        }

        public static void RegisterKeyStrokeCallback(Action<char> action, bool enable)
        {
#if ENABLE_INPUT_SYSTEM
            if(enable && Keyboard.current !=null && action != null)
            {
                
                Keyboard.current.onTextInput -= action;
                Keyboard.current.onTextInput += action;
            } else
            {
                Keyboard.current.onTextInput -= action;
            }         
#endif
        }

#if ENABLE_INPUT_SYSTEM
        public static Key ConvertKeyCodeToKey(KeyCode k)
        {
            switch (k)
            {
                case KeyCode.Return: return Key.Enter;
                case KeyCode.KeypadEnter: return Key.NumpadEnter;
                case KeyCode.CapsLock: return Key.CapsLock;
                case KeyCode.LeftShift: return Key.LeftShift;
                case KeyCode.RightShift: return Key.RightShift;
                //case KeyCode.DownArrow: return Key.DownArrow;

                default:
                    return (Key)System.Enum.Parse(typeof(Key), k.ToString());

            }
        }
#endif

        /// <summary>
        /// Key down abstraction for Input Manager vs Input System
        /// </summary>
        /// <param name="k"></param>
        public static bool KeyDown(KeyCode k)
        {

#if ENABLE_INPUT_SYSTEM
            Key c = ConvertKeyCodeToKey(k);
            return Keyboard.current[c].wasPressedThisFrame;
#else
            return Input.GetKeyDown(k);
#endif

        }


        /// <summary>
        /// Key up abstraction for Input Manager vs Input System
        /// </summary>
        /// <param name="k"></param>
        public static bool KeyUp(KeyCode k)
        {

#if ENABLE_INPUT_SYSTEM
            Key c = ConvertKeyCodeToKey(k);
            return Keyboard.current[c].wasPressedThisFrame;
#else
            return Input.GetKeyUp(k);
#endif

        }


        /// <summary>
        /// Key up abstraction for Input Manager vs Input System
        /// </summary>
        /// <param name="k">keycode</param>
        public static bool KeyPress(KeyCode k)
        {
#if ENABLE_INPUT_SYSTEM
            Key c = ConvertKeyCodeToKey(k);
            return Keyboard.current[c].IsPressed();
#else
            return Input.GetKey(k);
#endif

        }

        public static bool PointerDown(int mouseBtn = 0)
        {
#if ENABLE_INPUT_SYSTEM

            return Pointer.current.press.isPressed;
#else
            return Input.GetMouseButtonDown(mouseBtn);
#endif
        }

        public static bool PointerUp(int mouseBtn = 0)
        {
#if ENABLE_INPUT_SYSTEM

            return Pointer.current.press.wasReleasedThisFrame;
#else
            return Input.GetMouseButtonUp(mouseBtn);
#endif
        }

        /// <summary>
        /// abstraction of user hitting the "A" Button (X button for PS4)
        /// </summary>
        /// <returns>returns true if A Button is pressed</returns>
        public static bool Fire1()
        {

#if ENABLE_INPUT_SYSTEM
            // v3.4 failsafe for calling this function without a connected controller
            if (Gamepad.all.Count == 0)
                return false;

            return Gamepad.current.buttonSouth.wasPressedThisFrame;
#else
            bool fire = (Input.GetButtonDown("Fire1") || Input.GetButtonDown("Submit")) && !viperInput.PointerDown(0) && !viperInput.KeyDown(KeyCode.Space);
            fire = fire || Input.GetKeyUp(KeyCode.JoystickButton14) || Input.GetKeyUp(KeyCode.JoystickButton15);    //for appleTV remote touchpad press or play btn press
            return fire;
            
#endif
        }

        /// <summary>
        /// v3.5 checks if A (or X on PS4) pressed down
        /// </summary>
        /// <returns></returns>
        public static bool AButtonDown()
        {
#if ENABLE_INPUT_SYSTEM
            // v3.4 failsafe for calling this function without a connected controller
            if (Gamepad.all.Count == 0)
                return false;

            return Gamepad.current.buttonSouth.isPressed;
#else
            bool fire = (Input.GetButtonDown("Fire1") || Input.GetButtonDown("Submit")) && !viperInput.PointerDown(0) && !viperInput.KeyDown(KeyCode.Space);
            fire = fire || Input.GetKeyDown(KeyCode.JoystickButton14) || Input.GetKeyDown(KeyCode.JoystickButton15);    //for appleTV remote touchpad press or play btn press
            return fire;
            
#endif
        }

        /// <summary>
        /// v3.5 checks if A (or X on PS4) was released
        /// </summary>
        /// <returns></returns>
        public static bool AButtonUp()
        {
#if ENABLE_INPUT_SYSTEM
            // v3.4 failsafe for calling this function without a connected controller
            if (Gamepad.all.Count == 0)
                return false;

            return Gamepad.current.buttonSouth.wasReleasedThisFrame;
#else
            bool fire = (Input.GetButtonUp("Fire1") || Input.GetButtonUp("Submit")) && !viperInput.PointerUp(0) && !viperInput.KeyUp(KeyCode.Space);
            fire = fire || Input.GetKeyUp(KeyCode.JoystickButton14) || Input.GetKeyUp(KeyCode.JoystickButton15);    //for appleTV remote touchpad press or play btn press
            return fire;
            
#endif
        }

        /// <summary>
        /// v3.5 checks if B (or O on PS4) pressed down
        /// </summary>
        /// <returns></returns>
        public static bool BButtonDown()
        {
#if ENABLE_INPUT_SYSTEM
            // v3.4 failsafe for calling this function without a connected controller
            if (Gamepad.all.Count == 0)
                return false;

            return Gamepad.current.buttonEast.isPressed;
#else
            bool fire = (Input.GetButtonDown("Cancel") && !viperInput.PointerDown(0) && !viperInput.KeyDown(KeyCode.Space));
            fire = fire || Input.GetKeyDown(KeyCode.JoystickButton14) || Input.GetKeyDown(KeyCode.JoystickButton15);    //for appleTV remote touchpad press or play btn press
            return fire;
            
#endif
        }

        /// <summary>
        /// v3.5 checks if B (or O on PS4) was released
        /// </summary>
        /// <returns></returns>
        public static bool BButtonUp()
        {
#if ENABLE_INPUT_SYSTEM
            // v3.4 failsafe for calling this function without a connected controller
            if (Gamepad.all.Count == 0)
                return false;

            return Gamepad.current.buttonEast.wasReleasedThisFrame;
#else
            bool fire = (Input.GetButtonDown("Cancel") && !viperInput.PointerDown(0) && !viperInput.KeyDown(KeyCode.Space));
            fire = fire || Input.GetKeyDown(KeyCode.JoystickButton14) || Input.GetKeyDown(KeyCode.JoystickButton15);    //for appleTV remote touchpad press or play btn press
            return fire;
            
#endif
        }

        /// <summary>
        /// abstraction of any physical key being pressed
        /// </summary>
        /// <returns>true if a key is pressed</returns>
        public static bool AnyPhysicalKey()
        {
#if ENABLE_INPUT_SYSTEM
            // v3.4 failsafe for calling this function without a connected controller
            if (InputSystem.devices.Count == 0)
                return false;


            return Keyboard.current.anyKey.wasPressedThisFrame;
#else
            return Input.anyKeyDown;
#endif
        }

        /// <summary>
        /// Receives the input string from the physical keyboard
        /// </summary>
        /// <returns>string input of frame from physical keyboard</returns>
        public static string GetPhysicalKey()
        {
#if ENABLE_LEGACY_INPUT_MANAGER
            return Input.inputString;
#else
            return "";// handled through callback
#endif
        }

        public static string ConvertToLegacyAxis(AXIS_INPUT axis)
        {
            switch(axis)
            {
                // you can setup specific DPAD axes in the legacy Input Manager (using joystick 6th and 7th axis)
                case AXIS_INPUT.DPAD_X: return "Horizontal";
                case AXIS_INPUT.DPAD_Y: return "Vertical";

                case AXIS_INPUT.LEFTSTICK_X: return "Horizontal";
                case AXIS_INPUT.LEFTSTICK_Y: return "Vertical";
                case AXIS_INPUT.RIGHTSTICK_X: return "Horizontal";
                case AXIS_INPUT.RIGHTSTICK_Y: return "Vertical";

                default:
                    return "Horizontal";
            }
        }

        public static string[] GetControllerNames()
        {

#if ENABLE_INPUT_SYSTEM

            string[] s = new string[Gamepad.all.Count];
            for (int i = 0; i < Gamepad.all.Count; i++)
            {
                s[i] = Gamepad.all[i].displayName;
            }

            return s;
            
#else
            return Input.GetJoystickNames();
#endif


        }

        public static float GetAllAxis()
        {

#if ENABLE_INPUT_SYSTEM
            // v3.4 failsafe for calling this function without a connected controller
            if (Gamepad.all.Count == 0)
                return 0f;

            return Gamepad.current.leftStick.ReadValue().sqrMagnitude + Gamepad.current.dpad.ReadValue().sqrMagnitude;

#else
            return GetAxis(AXIS_INPUT.LEFTSTICK_X) + GetAxis(AXIS_INPUT.LEFTSTICK_Y) + GetAxis(AXIS_INPUT.DPAD_X) + GetAxis(AXIS_INPUT.DPAD_Y);
#endif
        }

        public static float GetAxis(AXIS_INPUT axis)
        {
#if ENABLE_INPUT_SYSTEM

            // v3.4 failsale for calling this function without a connected controller
            if (Gamepad.all.Count == 0)
                return 0f;
            
            switch(axis)
            {
                case AXIS_INPUT.DPAD_X: return (Gamepad.current.dpad.right.isPressed ? 1f : 0f) + (Gamepad.current.dpad.left.isPressed ? -1f : 0f);

                case AXIS_INPUT.DPAD_Y: return (Gamepad.current.dpad.up.isPressed ? 1f : 0f) + (Gamepad.current.dpad.down.isPressed ? -1f : 0f);

                case AXIS_INPUT.LEFTSTICK_X: return Mathf.Abs(Gamepad.current.leftStick.ReadValue().x) >= .9f ? Mathf.Sign(Gamepad.current.leftStick.ReadValue().x) : 0f;

                case AXIS_INPUT.LEFTSTICK_Y: return Mathf.Abs(Gamepad.current.leftStick.ReadValue().y) >= .9f ? Mathf.Sign(Gamepad.current.leftStick.ReadValue().y) : 0f ;

                case AXIS_INPUT.RIGHTSTICK_X: return Gamepad.current.rightStick.ReadValue().x;

                case AXIS_INPUT.RIGHTSTICK_Y: return Gamepad.current.rightStick.ReadValue().y;

                default:
                    return (Gamepad.current.dpad.right.isPressed ? 1f : 0f) + (Gamepad.current.dpad.left.isPressed ? -1f : 0f);
            }
#else
      
            return Input.GetAxis(ConvertToLegacyAxis(axis));

#endif
        }

        /// <summary>
        /// providers specific controller X and Y axis (on left stick)
        /// Note the naming convention for Unity's old input system is based on the default, 
        /// if you had changed the names in Input Manager then adjust these accordingly
        /// </summary>
        /// <param name="p">joystick number 1-8 joysticks</param>
        /// <returns>return vector with x and y controller movement</returns>
        public static Vector2 GetPlayerJoystickInput(int p)
        {

#if ENABLE_INPUT_SYSTEM

            p = Mathf.Clamp(p - 1, 0, Gamepad.all.Count);

            Vector2 v = new Vector2();
            v.x = (Gamepad.all[p].dpad.right.isPressed ? 1f : 0f) + (Gamepad.all[p].dpad.left.isPressed ? -1f : 0f);
            v.y = (Gamepad.all[p].dpad.up.isPressed ? 1f : 0f) + (Gamepad.all[p].dpad.down.isPressed ? -1f : 0f);

            v.x += Gamepad.all[p].leftStick.ReadValue().x;
            v.y += Gamepad.all[p].leftStick.ReadValue().y;

            return v;


#else
            p = Mathf.Clamp(p, 1, 8);
            Vector2 v = new Vector2(Input.GetAxis("Joy"+p+"Axis1"), Input.GetAxis("Joy" + p + "Axis2"));

            return v;

#endif
        }

        /// <summary>
        /// Captures the joystick button 0 (normally, A button or cross) for a particular controller
        /// </summary>
        /// <param name="p">controller number</param>
        /// <returns></returns>
        public static bool GetPlayerAButton(int p)
        {
#if ENABLE_INPUT_SYSTEM
            p = Mathf.Clamp(p - 1, 0, Gamepad.all.Count);

            return Gamepad.all[p] != null ? Gamepad.all[p].buttonSouth.wasPressedThisFrame : Gamepad.current.buttonSouth.wasPressedThisFrame;
#else
            switch (p)
                        {
                            case 1:
                                return Input.GetKey(KeyCode.Joystick1Button0);
                            case 2:
                                return Input.GetKey(KeyCode.Joystick2Button0);
                            case 3:
                                return Input.GetKey(KeyCode.Joystick3Button0);
                            case 4:
                                return Input.GetKey(KeyCode.Joystick4Button0);
                            case 5:
                                return Input.GetKey(KeyCode.Joystick5Button0);
                            case 6:
                                return Input.GetKey(KeyCode.Joystick6Button0);
                            case 7:
                                return Input.GetKey(KeyCode.Joystick7Button0);
                            case 8:
                                return Input.GetKey(KeyCode.Joystick8Button0);

                            default:
                                return Input.GetKey(KeyCode.JoystickButton0);

                        }
            

#endif

        }

        /// <summary>
        /// Captures the joystick button 1 (normally,  B button or circle) for a particular controller
        /// </summary>
        /// <param name="p">controller number</param>
        /// <returns></returns>
        public static bool GetPlayerBButton(int p)
        {
#if ENABLE_INPUT_SYSTEM
            p = Mathf.Clamp(p - 1, 0, Gamepad.all.Count);

            return Gamepad.all[p] != null ? Gamepad.all[p].buttonEast.wasPressedThisFrame : Gamepad.current.buttonEast.wasPressedThisFrame;


#else
            switch (p)
            {
                case 1:
                    return Input.GetKey(KeyCode.Joystick1Button1);
                case 2:
                    return Input.GetKey(KeyCode.Joystick2Button1);
                case 3:
                    return Input.GetKey(KeyCode.Joystick3Button1);
                case 4:
                    return Input.GetKey(KeyCode.Joystick4Button1);
                case 5:
                    return Input.GetKey(KeyCode.Joystick5Button1);
                case 6:
                    return Input.GetKey(KeyCode.Joystick6Button1);
                case 7:
                    return Input.GetKey(KeyCode.Joystick7Button1);
                case 8:
                    return Input.GetKey(KeyCode.Joystick8Button1);

                default:
                    return Input.GetKey(KeyCode.JoystickButton1);

            }

#endif
        }

        /// <summary>
        /// detects the number of joystick/gamepads attached to the playing device
        /// </summary>
        /// <returns></returns>
        public static int NumControllers()
        {
#if ENABLE_INPUT_SYSTEM
            return Gamepad.all.Count;

#else
            return Input.GetJoystickNames().Length;
#endif
        }

        public static void ResetAllAxis()
        {
#if ENABLE_LEGACY_INPUT_MANAGER

            Input.ResetInputAxes();
#else
      
            
#endif
        }

        public static Vector2 GetPointerPos()
        {
#if ENABLE_INPUT_SYSTEM
            Vector2 pos = Vector2.zero;

            if(Pointer.current != null)
            {
                pos = Pointer.current.position.ReadValue();
            }

            return pos;
            
#else
            return Input.mousePosition;
#endif
        }


    }

}
