///////////////////////////////////////////////////////////////////////////////////////
///
/// OSK_UI_Keyboard Class - this is the main class for UI widgets - ensure that all necessary references are made in the Unity inspector
/// ensure that receivers of OSK_UI_Keyboard have a OSK_UI_InputReceiver or OSK_UI_CustomReceiver
/// Note in this UI class, the text being edited is help in the OSK_UI_InputReceiver or the OSK_UI_CustomReceiver
/// this component does not have a "text" variable
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
using TMPro;
using System;
using System.Linq;
using viperTools;
using UnityEngine.UI;
using UnityEngine.EventSystems;

namespace viperOSK
{


    [ExecuteInEditMode]
    public class OSK_UI_Keyboard : OSK_Keyboard
    {

        // general map layout of keys so user can navigate with joystick
        new List<List<OSK_UI_Key>> keyLayout = new List<List<OSK_UI_Key>>();


        // currently selected OSK_UI_Key (useful for gamepad)
        private OSK_UI_Key currentSelUIKey;

        private OSK_UI_Key nextKey;

        private Transform keyboardAssets;

        public override Vector3 SpanTopLeft()
        {
            return keySpanTopLeft;
        }

        public override Vector3 SpanBottomRight()
        {
            return keySpanBottomRight;
        }

        public void ShowHideKeyboard(bool show)
        {
            keyboardAssets.gameObject.SetActive(show);
        }

        public override void SetInteractable(bool isInteractable)
        {
            base.SetInteractable(isInteractable);

            keyboardAssets.GetComponent<CanvasGroup>().interactable = isInteractable;
        }

        /// <summary>
        /// checks if a certain key is on the OSK
        /// </summary>
        /// <param name="k">keyCode to be checked</param>
        /// <returns>return true if a key with this keycode is on the OSK, false otherwise</returns>
        public override bool HasKey(OSK_KeyCode k)
        {
            return keyDict != null && keyDict.ContainsKey(k);
        }

        public override void Reset()
        {
            OSK_UI_Key[] list = this.transform.GetComponentsInChildren<OSK_UI_Key>();
            if (list.Length > 0)
            {
                foreach (OSK_UI_Key p in list)
                {
                    if(Application.isEditor)
                    {
                        DestroyImmediate(p.gameObject);
                    } else
                    {
                        Destroy(p.gameObject);
                    }
                    
                }
            }

            keyDict.Clear();

            keyLayout.Clear();
        }

        /// <summary>
        /// Routine to generate the keyboard keys based on the layout. Can be called from Editor or as runtime script
        /// </summary>
        public override void Generate()
        {

            //first check and clear all keys
            Reset();


            List<string> layoutRows = layout.Split('\n').ToList();


            RectTransform k_transform = KeyPrefab.GetComponent<RectTransform>();
            Vector3 keyOffset = new Vector3(keySize.x * k_transform.rect.width, keySize.y * k_transform.rect.height, keySize.z);

            Vector3 v = topLeft.localPosition;

            keySpanBottomRight = new Vector3(float.MinValue, float.MaxValue, 0f);
            keySpanTopLeft = new Vector3(float.MaxValue, float.MinValue, 0f);

            int lx = 0;
            int ly = 0;
            keyLayout.Add(new List<OSK_UI_Key>());

            for (int rowIdx = 0; rowIdx < layoutRows.Count; rowIdx++)
            {
                string[] chars = layoutRows[rowIdx].Split(' ');

                for (int i = 0; i < chars.Length; i++)
                {
                    if (chars[i].Contains("Skip"))
                    {
                        // Skip command allows modification of how the keys are offset
                        string ns = string.Join("", chars[i].ToCharArray().Where(item => char.IsDigit(item) || item == '.' || item == '-'));
                        if (ns.Length > 0)
                        {
                            float n;
                            if (float.TryParse(ns, out n))
                            {
                                v.x = v.x + keyOffset.x * n;
                            }
                            else
                            {
                                v.x = v.x + keyOffset.x;
                            }
                        }

                    }
                    else if (chars[i].Length >= 1)
                    {
                        GameObject k = Instantiate(KeyPrefab, keyboardAssets) as GameObject;


                        k_transform = k.GetComponent<RectTransform>();
                        OSK_UI_Key thisKey = k.GetComponent<OSK_UI_Key>();

                        if (keyFont != null)
                        {
                            thisKey.KeyFont(keyFont);
                        }

                        k.name = chars[i];

                        OSK_KeyCode key = GetOSKKeyCode(chars[i]);

                        OSK_SpecialKeys special = specialKeys.Find(item => item.keycode == key);



                        OSK_KEY_TYPES keyType = OSK_KeyTypeMeta.KeyType(key);

                        if (special != null)
                        {

                            thisKey.Assign(key, keyType, special.name);
                            thisKey.SetColors(special.col, keyLabelColor);

                            Vector3 keysz = keyOffset;
                            keysz.x *= special.x_size;
                            thisKey.x_size = special.x_size;

                            thisKey.BackScale(keysz);
                            v.x = v.x + keysz.x * .5f;
                            k_transform.localPosition = v;
                            v.x = v.x + keysz.x * .5f;

                            thisKey.SetLayoutLocation(lx, ly);
                            for (int j = 0; j < special.x_size; j++)
                            {
                                keyLayout[ly].Add(thisKey);
                                lx++;
                            }

                            if (special.keySoundCode >= 0 && !keySounds.ContainsKey(key))
                                keySounds.Add(key, special);

                            if (special.specialAction.GetPersistentEventCount() == 0)
                            {
                                thisKey.callBack.AddListener(KeyCallBase);
                                thisKey.callBack.AddListener(KeyCall);
                            }
                            else
                            {

                                thisKey.callBack = special.specialAction;
                                thisKey.callBack.AddListener(KeyCallBase);
                            }

                        }
                        else
                        {
                            if (key > OSK_KeyCode.__CUSTOM__)
                            {
                                thisKey.Assign(key, keyType, chars[i]);
                            }
                            else
                            {
                                thisKey.Assign(key, keyType);
                            }
                            OSK_KeyTypeMeta ktColor = keyTypeMeta.Find(item => item.keyType == keyType);

                            if (ktColor != null) thisKey.SetColors(ktColor.col, keyLabelColor);

                            thisKey.x_size = 1f;
                            thisKey.BackScale(keyOffset);
                            v.x = v.x + keyOffset.x * .5f;
                            k_transform.localPosition = v;
                            v.x = v.x + keyOffset.x * .5f;

                            keyLayout[ly].Add(thisKey);
                            thisKey.SetLayoutLocation(lx, ly);
                            lx++;

                            thisKey.callBack.AddListener(KeyCallBase);
                            thisKey.callBack.AddListener(KeyCall);

                        }

                        // (v3.5) added variables to retain the topleft & bottomright
                        try
                        {

                            keySpanTopLeft.x = Mathf.Min(keySpanTopLeft.x, k_transform.position.x + k_transform.rect.xMin);
                            keySpanTopLeft.y = Mathf.Max(keySpanTopLeft.y, k_transform.position.y + k_transform.rect.yMax);
                            keySpanBottomRight.x = Mathf.Max(keySpanBottomRight.x, k_transform.position.x + k_transform.rect.xMax);
                            keySpanBottomRight.y = Mathf.Min(keySpanBottomRight.y, k_transform.position.y + k_transform.rect.yMin);

                        }
                        catch (Exception e)
                        {
                            Debug.LogError("Key " + thisKey.name + " does not have a RectTransform and may not function properly\n" + e.Message);
                        }

                        keyDict.Add(key, thisKey);

                        //set Button UI param when highlighted
                        ColorBlock keyColors = thisKey.bk.colors;
                        keyColors.selectedColor = highlighterColor;
                        thisKey.bk.colors = keyColors;

                        if (Application.isEditor && shift) thisKey.ShiftUp();
                    }

                }//word

                if (rowIdx != layoutRows.Count - 1)
                {
                    //Debug.Log("Com " + ly + " count=" + keyLayout[ly].Count);
                    v.x = topLeft.localPosition.x;
                    v.y -= keyOffset.y;
                    ly++;
                    lx = 0;
                    keyLayout.Add(new List<OSK_UI_Key>());
                }

            }//row

            if (Application.isPlaying)
            {
                if (shift || caps)
                    BroadcastMessage("ShiftUp");
                else
                    BroadcastMessage("ShiftDown");

            }

            GamepadWrapNavigation();

        }

        public override void Traverse()
        {

            OSK_UI_Key[] list = this.transform.GetComponentsInChildren<OSK_UI_Key>();

            keyDict.Clear();
            keyLayout.Clear();

            foreach (OSK_UI_Key k in list)
            {
                keyDict.Add(k.key, k);
            }

            List<string> layoutRows = layout.Split('\n').ToList();

            keyLayout.Clear();
            int lx = 0;
            int ly = 0;

            keySpanBottomRight = new Vector3(float.MinValue, float.MaxValue, 0f);
            keySpanTopLeft = new Vector3(float.MaxValue, float.MinValue, 0f);

            keyLayout.Add(new List<OSK_UI_Key>());

            for (int rowIdx = 0; rowIdx < layoutRows.Count; rowIdx++)
            {
                string[] chars = layoutRows[rowIdx].Split(' ');

                for (int i = 0; i < chars.Length; i++)
                {

                    if (chars[i].Contains("Skip"))
                    {
                        //do nothing as keyboard is already visually generated
                    }
                    else if (chars[i].Length >= 1)
                    {

                        OSK_KeyCode key = GetOSKKeyCode(chars[i]);

                        OSK_UI_Key thisKey = list.First(item => item.key == key);
                        if (thisKey != null)
                        {
                            thisKey.SetLayoutLocation(lx, ly);
                            for (int j = 0; j < thisKey.getXSize(); j++)
                            {
                                keyLayout[ly].Add(thisKey);
                                lx++;
                            }


                            OSK_SpecialKeys special = specialKeys.Find(item => item.keycode == key);
                            if (special != null)
                            {

                                if (special.keySoundCode >= 0 && !keySounds.ContainsKey(key))
                                    keySounds.Add(key, special);

                                if (special.specialAction.GetPersistentEventCount() == 0)
                                {
                                    thisKey.callBack.AddListener(KeyCallBase);
                                    thisKey.callBack.AddListener(KeyCall);
                                }
                                else
                                {
                                    thisKey.callBack = special.specialAction;
                                    thisKey.callBack.AddListener(KeyCallBase);
                                }
                            }
                            else
                            {
                                thisKey.callBack.AddListener(KeyCallBase);
                                thisKey.callBack.AddListener(KeyCall);
                            }


                        }

                        // (v3.5) added variables to retain the topleft & bottomright
                        try
                        {
                            RectTransform k_transform = thisKey.gameObject.GetComponent<RectTransform>();
                            keySpanTopLeft.x = Mathf.Min(keySpanTopLeft.x, k_transform.position.x+ k_transform.rect.xMin);
                            keySpanTopLeft.y = Mathf.Max(keySpanTopLeft.y, k_transform.position.y+k_transform.rect.yMax);
                            keySpanBottomRight.x = Mathf.Max(keySpanBottomRight.x, k_transform.position.x+k_transform.rect.xMax);
                            keySpanBottomRight.y = Mathf.Min(keySpanBottomRight.y, k_transform.position.y+k_transform.rect.yMin);

                        }
                        catch (Exception e)
                        {
                            Debug.LogError("Key " + thisKey.name + " does not have a RectTransform and may not function properly\n" + e.Message);
                        }

                    }
                }//word

                if (rowIdx != layoutRows.Count - 1)
                {
                    ly++;
                    lx = 0;
                    keyLayout.Add(new List<OSK_UI_Key>());
                }

            }//row

            if (Application.isPlaying)
            {
                if (shift || caps)
                    BroadcastMessage("ShiftUp");
                else
                    BroadcastMessage("ShiftDown");

            }

            GamepadWrapNavigation();
        }



        void GamepadWrapNavigation()
        {
            // setup gamepad navigation
            if (gamepadKeyboardWrap != KEYBOARD_WRAP.NO_WRAP)
            {
                // process x-wrapping
                if (gamepadKeyboardWrap == KEYBOARD_WRAP.X_WRAP)
                {
                    for (int j = 0; j < keyLayout.Count; j++)
                    {
                        Navigation left = keyLayout[j][0].bk.navigation;
                        Navigation right = keyLayout[j][keyLayout[j].Count - 1].bk.navigation;

                        left.mode = Navigation.Mode.Explicit;
                        right.mode = Navigation.Mode.Explicit;

                        left.selectOnLeft = keyLayout[j][keyLayout[j].Count - 1].bk;
                        left.selectOnRight = keyLayout[j][0].bk.FindSelectableOnRight().gameObject.GetComponent<Selectable>();//keyLayout[j][Mathf.Clamp(1,0,keyLayout[j].Count-1)].bk;
                        left.selectOnUp = keyLayout[Mathf.Clamp(j - 1, 0, keyLayout.Count - 1)][0].bk;
                        left.selectOnDown = keyLayout[Mathf.Clamp(j + 1, 0, keyLayout.Count - 1)][0].bk;

                        right.selectOnRight = keyLayout[j][0].bk;
                        right.selectOnLeft = keyLayout[j][keyLayout[j].Count - 1].bk.FindSelectableOnLeft().gameObject.GetComponent<Selectable>();// keyLayout[j][Mathf.Clamp(keyLayout[j].Count - 2, 0, keyLayout[j].Count - 1)].bk;
                        right.selectOnUp = keyLayout[Mathf.Clamp(j - 1, 0, keyLayout.Count - 1)][keyLayout[j].Count - 1].bk;
                        right.selectOnDown = keyLayout[Mathf.Clamp(j + 1, 0, keyLayout.Count - 1)][keyLayout[j].Count - 1].bk;

                        keyLayout[j][0].bk.navigation = left;
                        keyLayout[j][keyLayout[j].Count - 1].bk.navigation = right;
                    }
                }

                // process y-wrapping
                if (gamepadKeyboardWrap == KEYBOARD_WRAP.XY_WRAP || gamepadKeyboardWrap == KEYBOARD_WRAP.Y_WRAP)
                {

                    Debug.LogWarning("Keyboard Wrapping XY_WRAP and Y_WRAP not *presently* supported in UI mode");
                    /*
                    for (int j = 0; j < keyLayout[0].Count; j++)
                    {
                        Navigation top = keyLayout[0][j].bk.navigation;
                        Navigation bottom = keyLayout[keyLayout.Count - 1][Mathf.Clamp(j, 0, keyLayout[keyLayout.Count - 1].Count)].bk.navigation;

                        top.selectOnUp = keyLayout[keyLayout.Count - 1][Mathf.Clamp(j, 0, keyLayout[keyLayout.Count - 1].Count)].bk;
                        bottom.selectOnRight = keyLayout[0][j].bk;
                    }
                    */
                }

                // process x-cascade
                if (gamepadKeyboardWrap == KEYBOARD_WRAP.X_CASCADE)
                {
                    for (int j = 0; j < keyLayout.Count; j++)
                    {
                        Navigation left = keyLayout[j][0].bk.navigation;
                        Navigation right = keyLayout[j][keyLayout[j].Count - 1].bk.navigation;

                        left.mode = Navigation.Mode.Explicit;
                        right.mode = Navigation.Mode.Explicit;

                        left.selectOnLeft = keyLayout[Mathf.Clamp(j - 1, 0, keyLayout.Count - 1)][keyLayout[j].Count - 1].bk;
                        left.selectOnRight = keyLayout[j][0].bk.FindSelectableOnRight().gameObject.GetComponent<Selectable>();//keyLayout[j][Mathf.Clamp(1,0,keyLayout[j].Count-1)].bk;
                        left.selectOnUp = keyLayout[Mathf.Clamp(j - 1, 0, keyLayout.Count - 1)][0].bk;
                        left.selectOnDown = keyLayout[Mathf.Clamp(j + 1, 0, keyLayout.Count - 1)][0].bk;

                        right.selectOnRight = keyLayout[Mathf.Clamp(j + 1, 0, keyLayout.Count - 1)][0].bk;
                        right.selectOnLeft = keyLayout[j][keyLayout[j].Count - 1].bk.FindSelectableOnLeft().gameObject.GetComponent<Selectable>();// keyLayout[j][Mathf.Clamp(keyLayout[j].Count - 2, 0, keyLayout[j].Count - 1)].bk;
                        right.selectOnUp = keyLayout[Mathf.Clamp(j - 1, 0, keyLayout.Count - 1)][keyLayout[j].Count - 1].bk;
                        right.selectOnDown = keyLayout[Mathf.Clamp(j + 1, 0, keyLayout.Count - 1)][keyLayout[j].Count - 1].bk;

                        keyLayout[j][0].bk.navigation = left;
                        keyLayout[j][keyLayout[j].Count - 1].bk.navigation = right;
                    }
                }
            }
        }

        /// <summary>
        /// this base KeyCall callback is invoked for all key presses whether the key has a UnityEvent attached or not.
        /// </summary>
        /// <param name="k">keycode of key pressed</param>
        /// <param name="ktype"></param>
        public override void KeyCallBase(OSK_KeyCode k, OSK_Receiver receiver)
        {

            if (!hasFocus)
                return;

            if (!HasKey(k))
                return;


            if (keySounds.ContainsKey(k))
            {
                ClickSound(keySounds[k].keySoundCode);
            }
            else
            {

                OSK_KeyTypeMeta ktm = keyTypeMeta.Find(item => item.keyType == keyDict[k].KeyType());
                if (ktm != null)
                {
                    ClickSound(ktm.keySoundCode);
                }
                else
                {
                    ClickSound(0);
                }


            }
        }


        /// <summary>
        /// Called when user strikes a key (keyup) or hits the A button on a gamepad
        /// you can also write your own code for other types of control keys
        /// </summary>
        /// <param name="k">OSK_KeyCode of the key pressed</param>
        /// <param name="ktype">Key Type (digit, letter, punctuation, control) of the key pressed</param>
        public override void KeyCall(OSK_KeyCode k, OSK_Receiver receiver)
        {


            if (!hasFocus)
                return;

            

            if (!keyDict.ContainsKey(k))
                return;

            

            if (k == OSK_KeyCode.Backspace)
            {
                KeyBackspace(receiver);
                return;
            }

            if (k == OSK_KeyCode.Delete)
            {
                KeyDelete(receiver);
                return;
            }

            if (k == OSK_KeyCode.Return)
            {
                if (receiver != null)
                    receiver.Submit();
                else if (output != null)
                    output.Submit();
                return;
            }

            if (k == OSK_KeyCode.LeftShift || k == OSK_KeyCode.RightShift)
            {
                KeyShift();
                return;
            }

            if (k == OSK_KeyCode.CapsLock)
            {
                caps = !caps;
                shift = !caps;
                KeyShift();
                return;
            }

            if (k == OSK_KeyCode.LeftControl || k == OSK_KeyCode.RightControl)
            {
                // place your own code here to handle your approach for Controls
                return;
            }

            if (k == OSK_KeyCode.LeftAlt || k == OSK_KeyCode.RightAlt)
            {
                // place your own code here to handle your approach for Alt
                return;
            }


            string n;
            if (k < OSK_KeyCode.__CUSTOM__)
            {
                n = shift || caps ? ((char)k).ToString().ToUpper() : ((char)k).ToString();

            }
            else
            {
                n = keyDict[k].GetKeyName();
            }

            OutputTextUpdate(n, receiver);

            if (!caps && shift)
                KeyShift();

        }

        /// <summary>
        /// Joystick or GamePad button 
        /// GamePad A button
        /// or on PS4 the X button
        /// </summary>
        public override void ButtonA()
        {
            // override and do nothing, in UI, input click is handled by Unity
        }

        public OSK_UI_Key SelectedKey()
        {
            return currentSelUIKey;
        }

        /// <summary>
        /// sets the highlighted key (for gamepad controllers) to the keycode specified if available
        /// </summary>
        /// <param name="k">keyCode for the specified key</param>
        public override void SetSelectedKey(OSK_KeyCode k)
        {

            if (HasKey(k))
            {

                GameObject keyGameObj = keyDict[k].GetObject() as GameObject;
                currentSelUIKey = keyGameObj.GetComponent<OSK_UI_Key>();

                if (currentSelUIKey == null)
                    return;

                if (EventSystem.current.alreadySelecting)
                {

                    nextKey = currentSelUIKey;
                    StartCoroutine(SelectKey(nextKey));
                }
                else
                {
                    StartCoroutine(SelectKey(currentSelUIKey));
                }


            }
        }

        IEnumerator SelectKey(OSK_UI_Key selKey)
        {
            yield return new WaitForEndOfFrame();
            yield return new WaitForSecondsRealtime(.15f);
            EventSystem.current.SetSelectedGameObject(selKey.gameObject);
        }

        public override void SetSelectedKey(string c)
        {
            OSK_KeyCode k = GetOSKKeyCode(c);
            SetSelectedKey(k);
        }

        /*
        /// <summary>
        /// sets the highlighted key (for gamepad controllers) to the key corresponding to the string provided
        /// </summary>
        /// <param name="k">string representation of the keycode</param>
        public void SetSelectedKey(string c)
        {

            OSK_KeyCode k;
            if (OSK_Keymap.chartoKeycode.TryGetValue(c, out k))
            {
                SetSelectedKey(k);
            }
            else
            {
                try
                {
                    string prefix = "";
                    if (c[0] >= '0' && c[0] <= '9')
                        prefix = "Alpha";
                    k = (OSK_KeyCode)System.Enum.Parse(typeof(OSK_KeyCode), prefix + c[0]);
                }
                catch (Exception e)
                {
                    Debug.LogError("OSK_Keyboard could not parse the layout for this key (" + c + ") make sure you are using KeyCode names for the characters\nException error: " + e.Message);
                }

                SetSelectedKey(k);
            }
        }*/

        public void SetSelectedKey(OSK_UI_Key k)
        {
            if (currentSelUIKey != null) currentSelUIKey.Highlight(false, Color.white);
            currentSelUIKey = k;
            EventSystem.current.SetSelectedGameObject(currentSelUIKey.gameObject);
        }

        /// <summary>
        /// handles how the movement of the highlighted key is done when using a GamePad or Joystick. The movements on a stick are approximated to those of a Directional Pad (D-Pad)
        /// so that the key highlight is simple and elegant
        /// </summary>
        /// <param name="dir">Vector2D direction of movement on the screen</param>
        public override void DpadMove(Vector2 dir)
        {

            OSK_UI_Key k = keyLayout[(int)DpadSelection.y][(int)DpadSelection.x];
            k.Highlight(false, Color.white);

            // movement along y-axis
            if (dir.y != 0f)
            {
                float oldy = DpadSelection.y;
                DpadSelection.y = (DpadSelection.y + Mathf.RoundToInt(dir.y));
                if (DpadSelection.y < 0f)
                {
                    if (gamepadKeyboardWrap == KEYBOARD_WRAP.Y_WRAP || gamepadKeyboardWrap == KEYBOARD_WRAP.XY_WRAP)
                    {
                        DpadSelection.y = keyLayout.Count - 1;
                    }
                    else
                        DpadSelection.y = 0;
                }
                else if (DpadSelection.y >= keyLayout.Count)
                {
                    if (gamepadKeyboardWrap == KEYBOARD_WRAP.Y_WRAP || gamepadKeyboardWrap == KEYBOARD_WRAP.XY_WRAP)
                    {
                        DpadSelection.y = 0;
                    }
                    else
                        DpadSelection.y = keyLayout.Count - 1;
                }

            }


            // movement along x-axis
            if (dir.x != 0f)
            {
                float x = DpadSelection.x;

                do
                {
                    x += dir.x;

                    if (x < 0)
                    {
                        if (gamepadKeyboardWrap == KEYBOARD_WRAP.X_WRAP || gamepadKeyboardWrap == KEYBOARD_WRAP.XY_WRAP)
                        {
                            x = keyLayout[DpadSelection.y].Count - 1;
                        }
                        else if (gamepadKeyboardWrap == KEYBOARD_WRAP.X_CASCADE)
                        {
                            DpadSelection.y = Mathf.Clamp(DpadSelection.y - 1, 0, keyLayout.Count - 1);
                            x = keyLayout[DpadSelection.y].Count - 1;

                        }
                        else
                            x = 0;
                        break;
                    }
                    else if (x >= keyLayout[DpadSelection.y].Count)
                    {

                        if (gamepadKeyboardWrap == KEYBOARD_WRAP.X_WRAP || gamepadKeyboardWrap == KEYBOARD_WRAP.XY_WRAP)
                        {
                            x = 0;
                        }
                        else if (gamepadKeyboardWrap == KEYBOARD_WRAP.X_CASCADE)
                        {
                            DpadSelection.y = Mathf.Clamp(DpadSelection.y + 1, 0, keyLayout.Count - 1);
                            x = 0;

                        }
                        else
                            x = keyLayout[DpadSelection.y].Count - 1;

                        break;
                    }
                } while (keyLayout[(int)DpadSelection.y][(int)x].key == k.key);

                DpadSelection.x = (int)x;
            }

            currentSelUIKey = keyLayout[(int)DpadSelection.y][(int)DpadSelection.x];
            keyLayout[(int)DpadSelection.y][(int)DpadSelection.x].Highlight(true, highlighterColor);
            currentSelUIKey = keyLayout[DpadSelection.y][DpadSelection.x];
            
        }


        // Start is called before the first frame update
        void Awake()
        {
            if (this.GetComponent<OSK_Settings>() == null)
            {
                this.gameObject.AddComponent<OSK_Settings>();
            }

            PrepAssetGroup();
            Prep();
        }

        private void Start()
        {
            //AcceptPhysicalKeyboard(acceptPhysicalKeyboard);
        }

        public void PrepAssetGroup()
        {
            keyboardAssets = this.transform.GetChild(0);
        }

        /*void OnGUI()
        {
            if (!hasFocus || !Application.isPlaying)
                return;

            if (acceptPhysicalKeyboard)
            {
                if (Event.current.isKey && keyDict.ContainsKey(Event.current.keyCode))
                {
                    //KeyCall(Event.current.keyCode, OSK_KEY_TYPES.LETTER);
                    keyDict[Event.current.keyCode].Click(output);
                }
            }

        }*/

        private void Update()
        {
            if (!hasFocus || bypassDefaultInput || !Application.isPlaying || currentSelUIKey == null)
                return;

            if (viperInput.AButtonDown())
            {

                currentSelUIKey.JoystickPressDown();
            }

            if (viperInput.AButtonUp())
            {
                currentSelUIKey.JoystickPressUp();
            }

        }

        void FixedUpdate()
        {

            OSK_UI_Key ui_key;
            if (EventSystem.current.currentSelectedGameObject != null && EventSystem.current.currentSelectedGameObject.TryGetComponent<OSK_UI_Key>(out ui_key))
            {
                if (currentSelUIKey != null && currentSelUIKey != ui_key && viperInput.GetAllAxis() != 0f )
                {

                    SelectSound();

                }

                currentSelUIKey = ui_key;

            }
            else
                currentSelUIKey = null;
        }


    }


}