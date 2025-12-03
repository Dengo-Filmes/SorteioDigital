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
using UnityEngine.EventSystems;
using UnityEngine.UI;

namespace viperOSK
{
    public class OSK_UI_Key : MonoBehaviour, IPointerDownHandler, IPointerUpHandler, I_OSK_Key
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

        public TextMeshProUGUI keyName;

        // v3.5 change from Button to Selectable, so that OSK_UI_Key can handle longpress
        public Selectable bk;

        public bool isPressed;
        public bool isLongPress;
        Coroutine longPressCoroutine;

        float lastPressed;  // in Unity Time.time, when was the last pressed instance

        // size of the key in factor of keysize
        public float x_size;

        // originally set color of the key
        Color bk_baseColor;

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
            return this.gameObject;
        }

        public GameObject GetGameObject()
        {
            return this.gameObject;
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


        public void AssignSpecialAction(Action<string, OSK_Receiver> action)
        {
            altAction += action;
        }

        public void Assign(OSK_KeyCode newKey, OSK_KEY_TYPES ktype, string name = "")
        {
            key = newKey;

            if (name.Length == 0)
            {
                this.keyName.text = ((char)key).ToString();


            }
            else
            {
                this.keyName.text = name;
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

            SetBkColor(bk_color);
            keyName.color = label_color;
        }

        public void SetBkColor(Color bk_color, bool reset_base_color = true)
        {
            if(reset_base_color) bk_baseColor = bk_color;
            ColorBlock cb = bk.colors;
            cb.normalColor = bk_color;
            cb.selectedColor = bk_color;
            cb.highlightedColor = bk_color + new Color(.1f, .1f, .1f, 1f);
            cb.pressedColor = bk_color - new Color(.1f, .1f, .1f, 0f);

            bk.colors = cb;
        }

        public void BackScale(Vector3 scale)
        {
            //bk.size = scale * .9f;
            bk.image.rectTransform.sizeDelta = scale;
           // bk.image.rectTransform.anchorMax = new Vector3 (bk.image.rectTransform.localPosition.x-scale.x * 30f, bk.image.rectTransform.localPosition.x-30f);
           // bk.image.rectTransform.anchorMin = new Vector3(bk.image.rectTransform.localPosition.x + scale.x * 30f, bk.image.rectTransform.localPosition.x+30f);

            this.GetComponent<BoxCollider>().size = scale;
           // keyName.rectTransform.sizeDelta = scale * .9f;
        }

        public float getYSize()
        {
            return this.GetComponent<BoxCollider>().size.y;
        }

        public float getXSize()
        {
            return x_size;
        }

        public void OnPointerDown(PointerEventData eventData)
        {
            keyPressType = "pointer";
            OnPressed();
        }

        public void OnPointerUp(PointerEventData eventData)
        {
            keyPressType = "pointer";
            OnDepressed();
        }

        /// <summary>
        /// Handle button  'down'
        /// input is handled through the UI Button object, therefore the UnityEngine's delegate is not needed
        /// </summary>
        public void OnPressed()
        {

            if (!isPressed)
            {
                this.transform.localPosition = this.transform.localPosition + new Vector3(.025f, -.05f, 0f);
                isPressed = true;

                // v3.5 launch coroutine to catch longpress
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

        /// <summary>
        /// Handle button  'up'
        /// input is handled through the UI Button object, therefore the UnityEngine's delegate is not needed
        /// </summary>
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
                lastPressed = Time.time;
                this.transform.localPosition = this.transform.localPosition - new Vector3(.025f, -.05f, 0f);

                // this is where the key pressed callBack is called

                if (callBack != null)
                    callBack.Invoke(key, tmpOutput);
                if (altAction != null)
                    altAction?.Invoke(keyName.text, tmpOutput);



            }

            isPressed = false;
            isLongPress = false;

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
            }

            isPressed = false;
            isLongPress = false;

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

        public void OnKeyPress(string keyDevice, OSK_Receiver inputfield = null)
        {
            keyPressType = keyDevice;
            tmpOutput = inputfield;
            OnPressed();
        }

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
            if(!isPressed)
            {
                keyPressType = keyDevice;
                tmpOutput = inputfield;
                OnPressed();
                StartCoroutine(ClickCoroutine());
            }
        }

        /*
        public void Click()
        {
            if (!isPressed)
            {

                tmpOutput = null;
                OnPressed();
                StartCoroutine(ClickCoroutine());
            }
        }*/

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
            if (hi)
            {
                SetBkColor(c, false);
            }
            else
            {
                SetBkColor(bk_baseColor, false);
            }
        }

        OSK_UI_Key Dir(int x, int y)
        {
            OSK_UI_Key u = null;
            if (x > 0)
            {
                u = bk.FindSelectableOnRight().GetComponent<OSK_UI_Key>();
            }

            if (x < 0)
            {
                u = bk.FindSelectableOnLeft().GetComponent<OSK_UI_Key>();
            }

            if (y > 0)
            {
                u = bk.FindSelectableOnUp().GetComponent<OSK_UI_Key>();
            }

            if (x < 0)
            {
                u = bk.FindSelectableOnDown().GetComponent<OSK_UI_Key>();
            }

            return u;
        }

        void Awake()
        {
#if UNITY_2019
            // (v3.4) backward compatibility with 2019.4
            callBack = new UE2019_Key();
#endif

            if (keyName == null) keyName = this.transform.GetComponentInChildren<TextMeshProUGUI>();
           if (bk == null) bk = this.transform.GetComponent<Selectable>();

           if (callBack == null) callBack.AddListener(this.transform.GetComponentInParent<OSK_UI_Keyboard>().KeyCall);

        }

        // Update is called once per frame
        void Update()
        {

        }
    }

}
