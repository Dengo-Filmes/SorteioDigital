///////////////////////////////////////////////////////////////////////////////////////
/// © vipercode corp
/// 2022
/// Please use this asset according to the attached license
/// Attributions, mentions and reviews are always welcomed
///
///////////////////////////////////////////////////////////////////////////////////////

using System;
using System.Collections;
using System.Collections.Generic;
using TMPro;
using UnityEngine;
using UnityEngine.Events;


namespace viperOSK
{
    public class OSK_Key : MonoBehaviour, I_OSK_Key
    {
        public OSK_KeyCode key;

        // (v3.4 backward compatibility with Unity 2019)
#if UNITY_2019
        public UE2019_Key callBack;
#else
        public UnityEvent<OSK_KeyCode, OSK_Receiver> callBack;
#endif
        /// <summary>
        /// alternative action, for example for mini-keyboard use of OSK_Key
        /// </summary>
        Action<string, OSK_Receiver> altAction;

        public OSK_KEY_TYPES keyType;

        OSK_Receiver tmpOutput;

        public TextMeshPro keyName;
        public SpriteRenderer bk;

        public bool isPressed;
        float lastPressed;  // in Unity Time.time, when was the last pressed instance
        public bool isLongPress;
        Coroutine longPressCoroutine; // longpress coroutine detector

        // originally set color of the key
        Color bk_baseColor;

        /// <summary>
        /// location x,y in keylayout
        /// </summary>
        Vector2Int layoutLoc = new Vector2Int();

        /// <summary>
        /// string containing the type of device used to press button. Options are "pointer" "keyboard" "joystick" (applies for gamepads as well)
        /// </summary>
        string keyPressType = "pointer";

        public OSK_KeyCode GetKeyCode()
        {
            return key;
        }

        public object GetObject()
        {
            return this;
        }

        public string GetKeyName()
        {
            return keyName.text;
        }

        public float LastPressed()
        {
            return lastPressed;
        }

        public OSK_KEY_TYPES KeyType()
        {
            return keyType;
        }

        public Transform GetKeyTransform()
        {
            return this.transform;
        }

        void Awake()
        {
#if UNITY_2019
            callBack = new UE2019_Key();
#endif
        }
        /// <summary>
        /// v3.5 assigns a special action Action<string> to this Key. It 
        /// </summary>
        /// <param name="action"></param>
        public void AssignSpecialAction(Action<string, OSK_Receiver> action)
        {
            altAction += action;
        }

        public void Assign(OSK_KeyCode newKey, OSK_KEY_TYPES ktype, string key_name = "")
        {
            key = newKey;

            if (key_name.Length == 0)
            {
                this.keyName.text = ((char)key).ToString();


            }
            else
            {
                this.keyName.text = key_name;
            }

            keyType = ktype;

        }

        /// <summary>
        /// sets location in x (col) and y (row) in the key layout on the OSK keyboard
        /// </summary>
        /// <param name="x"></param>
        /// <param name="y"></param>
        public void SetLayoutLocation(int x, int y)
        {
            layoutLoc.x = x;
            layoutLoc.y = y;
        }

        /// <summary>
        /// provides the layout location x (col) and y (row) for keyLayout, necessary to align the selected key to location
        /// </summary>
        /// <returns></returns>
        public Vector2Int GetLayoutLocation()
        {
            return layoutLoc;
        }

        public void KeyFont(TMP_FontAsset keyfont)
        {
            keyName.font = keyfont;
        }

        public void SetColors(Color bk_color, Color label_color)
        {
            bk_baseColor = bk_color;
            bk.color = bk_color;

            keyName.color = label_color;
        }

        public void BackScale(Vector3 scale)
        {
            bk.size = scale * .95f;
            this.GetComponent<BoxCollider>().size = scale;
            keyName.rectTransform.sizeDelta = scale * .95f;
        }

        /// <summary>
        /// v3.5 Y-size for layout placement
        /// </summary>
        /// <returns>y-size based on collider</returns>
        public float getYSize()
        {
            return this.GetComponent<BoxCollider>().size.y;
        }

        public float getXSize()
        {
            return this.GetComponent<BoxCollider>().size.x;
        }

        public GameObject GetGameObject()
        {
            return this.gameObject;
        }

        public void OnPressed()
        {
            if (!isPressed)
            {
                this.transform.localPosition = this.transform.localPosition + new Vector3(.025f, -.05f, 0f);
                Color c = bk.color + new Color(.1f, .1f, .1f);
                bk.color = c;
                isPressed = true;

                // v4.0 launch coroutine to catch longpress
                isLongPress = false;

                // check if an longpressaction is available 
                if (key != OSK_KeyCode._MINIKEYBOARD_)
                {
                    if (OSK_Settings.instance.longPressAction != null)
                    {
                        longPressCoroutine = StartCoroutine(LongPressCheck());
                    }
                }

            }
        }

        public void OnDepressed()
        {
            // If the coroutine is still running, stop it
            if (longPressCoroutine != null)
            {
                StopCoroutine(longPressCoroutine);
                longPressCoroutine = null;
            }

            if (!isLongPress && isPressed)
            {
                // this is where the key pressed callBack is called

                lastPressed = Time.time;
                this.transform.localPosition = this.transform.localPosition - new Vector3(.025f, -.05f, 0f);
                //Color c = bk.color + new Color(-.1f, -.1f, -.1f);
                bk.color = bk_baseColor;



                if (callBack != null)
                    callBack.Invoke(key, tmpOutput);
                if (altAction != null)
                    altAction?.Invoke(keyName.text, tmpOutput);
            }

            isPressed = false;
            isLongPress = false;
        }


        public void OnMouseDown()
        {
            keyPressType = "pointer";
            OnPressed();

        }


        public void OnMouseUp()
        {
            
            keyPressType = "pointer";
            OnDepressed();

        }



        /// <summary>
        /// v4.0 coroutine to catch long-press and generate the accent console
        /// </summary>
        IEnumerator LongPressCheck()
        {
            yield return new WaitForSecondsRealtime(OSK_Settings.instance.longPressDelay); // Long-press threshold - adjust this to your liking

            isLongPress = true;

            // initiate longpress action as in OSK_Keyboard.Settings
            if (OSK_Settings.instance.longPressAction != null)
            {
                OSK_Settings.instance.longPressAction.Invoke(new OSK_LongPressPacket(this.keyName.text, this.key, this.gameObject, this.keyPressType));
                this.transform.localPosition = this.transform.localPosition - new Vector3(.025f, -.05f, 0f);

            }

            isLongPress = false;
            isPressed = false;


        }

        /// <summary>
        /// v3.5 press down to also capture longpress from gamepads
        /// </summary>
        /// <param name="inputfield">receiver for multi-input setups</param>
        public void JoystickPressDown(OSK_Receiver inputfield = null)
        {
            keyPressType = "joystick";
            tmpOutput = inputfield;
            OnPressed();

        }

        /// <summary>
        /// v3.5 press up / depress
        /// </summary>
        /// <param name="inputfield">receiver for multi-input setups</param>
        public void JoystickPressUp(OSK_Receiver inputfield = null)
        {
            keyPressType = "joystick";
            tmpOutput = inputfield;
            OnDepressed();

        }

        /// <summary>
        /// v3.5 generalized for all devices on key press
        /// </summary>
        public void OnKeyPress(string keyDevice, OSK_Receiver inputfield = null)
        {
            keyPressType = keyDevice;
            tmpOutput = inputfield;
            OnPressed();
        }

        /// <summary>
        /// v3.5 generalized for all devices on key press up / depress
        /// </summary>
        public void OnKeyDepress(string keyDevice, OSK_Receiver inputfield = null)
        {
            keyPressType = keyDevice;
            tmpOutput = inputfield;
            OnDepressed();
        }

        /// <summary>
        /// this function is used with raycasting or alternative keyboards to simulate a button down then up a fraction of a sector later
        /// used in Input System and gamepad/joystick controls
        /// </summary>
        public void Click(string keyDevice, OSK_Receiver inputfield = null)
        {

            if (!isPressed)
            {
                keyPressType = keyDevice;

                tmpOutput = inputfield;
                OnPressed();
                //Invoke("OnMouseUp", .15f); // removed in viperOSK v3.4 and replaced with Coroutine to avoid issues for devs using TimeScale=0 on pause menus
                StartCoroutine(ClickCoroutine());
            }
        }

        /// <summary>
        /// delay and then depress the key using OnMouseUp
        /// </summary>
        IEnumerator ClickCoroutine()
        {
            yield return new WaitForSecondsRealtime(0.15f);
            OnDepressed();
        }


        public void ShiftUp()
        {
            if (keyType == OSK_KEY_TYPES.LETTER)
            {
                // change this condition if you have Letters that are special keys and how you want shift up/down to affect them
                // this implementation only affects the 1st letter.
                if(keyName.text.Length <= 1)
                {
                    keyName.text = keyName.text.ToUpper();
                } else
                {
                    keyName.text = char.ToUpper(keyName.text[0]) + keyName.text.Substring(1);
                }
                
            }
            // you can also change what happens to other key types here with your own implementation
            // keep in mind that the callback will still receive the assigned OSK_KeyCode 
            // the best approach would be to handle the impact of Shift+Key in the callback (KeyCall in OSK_Keyboard), changing the OSK_KeyCode assigned to a key here is not recommended
        }

        public void ShiftDown()
        {
            // change this condition if you have Letters that are special keys and how you want shift up/down to affect them
            // this implementation only affects the 1st letter.
            if (keyType == OSK_KEY_TYPES.LETTER)
            {
                if (keyName.text.Length <= 1)
                {
                    keyName.text = keyName.text.ToLower();
                }
                else
                {
                    keyName.text = char.ToLower(keyName.text[0]) + keyName.text.Substring(1);
                }
            }
            // you can also change what happens to other key types here with your own implementation
            // keep in mind that the callback will still receive the assigned OSK_KeyCode 
            // the best approach would be to handle the impact of Shift+Key in the callback (KeyCall in OSK_Keyboard), changing the OSK_KeyCode assigned to a key here is not recommended

        }


        /// <summary>
        /// highlights a key background based. Used for gamepad/joystick controls
        /// </summary>
        /// <param name="hi">highlight bool, true=highlighted</param>
        /// <param name="c">highlight color of the background key sprite</param>
        public void Highlight(bool hi, Color c)
        {
            if (hi && c.a > 0f)
            {
                bk.color = c;
                
            }
            else
            {
                bk.color = bk_baseColor;
            }
        }

        void Start()
        {
            if (keyName == null) keyName = this.transform.GetComponentInChildren<TextMeshPro>();
            if (bk == null) bk = this.transform.GetComponentInChildren<SpriteRenderer>();

            if (callBack == null) callBack.AddListener(this.transform.GetComponentInParent<OSK_Keyboard>().KeyCall);

            if (bk != null) bk_baseColor = bk.color;
        }

        // Update is called once per frame
        void Update()
        {

        }
    }

}
