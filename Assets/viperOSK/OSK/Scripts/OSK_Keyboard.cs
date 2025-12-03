///////////////////////////////////////////////////////////////////////////////////////
///
/// OSK_Keyboard Class - this is the main class - ensure that all necessary references are made in the Unity inspector
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
using System.Linq;
using TMPro;
using UnityEngine;
using UnityEngine.Events;
using viperTools;


namespace viperOSK
{


    [ExecuteInEditMode]
    public class OSK_Keyboard : MonoBehaviour
    {



        /// <summary>
        /// bypass viperOSK's default input allows total bypass of viperOSK input management if you wish to tailor the input management to your liking or use the Unity UI default navigation
        /// </summary>
        public bool bypassDefaultInput = false;

        // when set to true, the keyboard will generate on Start()
        public bool generateOnStart;

        // when true, keyboard gets input.
        // This is for the user to set to true/false based on
        // whether you want a keyboard to receive input (in case you have more than one in the scene)
        // set to true by default
        protected bool hasFocus = true;

        // points to the text mesh where the output text is being displayed
        public OSK_Receiver output;

        public GameObject KeyPrefab;
        public Transform topLeft;
        public Color keyLabelColor = Color.white;

        // key prefab size
        public Vector3 keySize;
        public TMP_FontAsset keyFont;

        // is Caps Lock on?
        public bool caps;

        // is in shift up/down
        public bool shift;

        public bool acceptPhysicalKeyboard;


        [Header("Gamepad/Joystick")]
        // whether the keyboard accepts joystick typing
        public bool acceptGamePadInput;
        public enum KEYBOARD_WRAP
        {
            NO_WRAP,
            XY_WRAP,
            X_WRAP,
            Y_WRAP,
            X_CASCADE
        }

        /// <summary>
        /// how the keyboard selection wraps (ex: X_WRAP going off left or right keyboard wraps around to the other side)
        /// </summary>
        public KEYBOARD_WRAP gamepadKeyboardWrap = KEYBOARD_WRAP.NO_WRAP;

        public Color highlighterColor = new Color(.7f, .5f, .5f);



        // current selected key when using gamepads/joysticks
        private OSK_Key currentSelectedKey;

        // highlighted key in joystick selection based on the keymap
        protected Vector2Int DpadSelection;

        // timer to slow down inputs from dpad or joysticks
        protected float inputTimer;
        protected float inputTimerThreshold = .15f;

        [Header("Sound Effects")]
        public bool soundFX;
        protected Action<int> sound;
        protected Action selectSound;

        [Header("Keys Layout and Settings")]
        // general map layout of keys so user can navigate with joystick
        protected List<List<OSK_Key>> keyLayout = new List<List<OSK_Key>>();

        // For the layout to function, all characters or new line (\n) must have a space before and after
        // punctionations and other control keys must be named as per the example below in a manner that is parsed by OSK_KeyCode
        // use 'Skip#.#' to offset your keyboard, replace the #.# by a float denoting the size of the offset (with respect to a key size). For example: Skip1.5 would offset the next key by 1.5x a key width
        [SerializeField]
        [TextArea(15, 6)]
        public string layout = "1 2 3 4 5 6 7 8 9 0 \n Q W E R T Y U I O P \n A S D F G H J K L Exclaim \n Z X C V B N M Period Backspace \n LeftShift Space Return";

        // this list is to denote special keys, you get full control on how they are displayed (property 'name'), the color, and size
        public List<OSK_SpecialKeys> specialKeys = new List<OSK_SpecialKeys>()
        {
            new OSK_SpecialKeys(OSK_KeyCode.Backspace, "\x00AB", new Color(.93f, .77f, .22f), 2.0f),
            new OSK_SpecialKeys(OSK_KeyCode.Return, "Send", new Color(.21f,.57f,.74f), 2.0f),
            new OSK_SpecialKeys(OSK_KeyCode.LeftShift, "Shft", new Color(.93f, .77f, .22f), 2.0f),
            new OSK_SpecialKeys(OSK_KeyCode.Space, " ", new Color(.93f, .77f, .22f), 4.0f),
        };

        // this list has the general classification by type of key (digit, letter, punctuation or control) and allows you to set colors and key sounds (in the Unity inspector)
        public List<OSK_KeyTypeMeta> keyTypeMeta = new List<OSK_KeyTypeMeta>(4)
        {
            new OSK_KeyTypeMeta(OSK_KEY_TYPES.DIGIT, new Color(.4f, .4f, .4f)),
            new OSK_KeyTypeMeta(OSK_KEY_TYPES.LETTER, new Color(.35f, .35f, .35f)),
            new OSK_KeyTypeMeta(OSK_KEY_TYPES.PUNCTUATION, new Color(.45f, .45f, .45f)),
            new OSK_KeyTypeMeta(OSK_KEY_TYPES.CONTROLS, new Color(.55f, .55f, .55f)),

        };

        protected Dictionary<OSK_KeyCode, OSK_SpecialKeys> keySounds = new Dictionary<OSK_KeyCode, OSK_SpecialKeys>();

        // dictionary of all available keycodes for this OSK
        protected Dictionary<OSK_KeyCode, I_OSK_Key> keyDict = new Dictionary<OSK_KeyCode, I_OSK_Key>();

        // Keymapping helper for auto-correcting layout
        public OSK_Keymap osk_Keymap = new OSK_Keymap();

        /// <summary>
        /// (v3.5) the edges of the keyboard based on the furthest left,top,right and bottom
        /// these are used to potentially resize the keyboard background or other visual artifacts
        /// </summary>
        protected Vector3 keySpanTopLeft;
        protected Vector3 keySpanBottomRight;

        public virtual Vector3 SpanTopLeft()
        {
            return keySpanTopLeft;
        }

        public virtual Vector3 SpanBottomRight()
        {
            return keySpanBottomRight;
        }

        public void AutoCorrectLayout()
        {
            string corrected = osk_Keymap.AutoCorrectLayout(layout);
            if(corrected != layout)
            {
                layout = corrected;
            }
        }

        /// <summary>
        /// checks if a certain key is on the OSK
        /// </summary>
        /// <param name="k">keyCode to be checked</param>
        /// <returns>return true if a key with this keycode is on the OSK, false otherwise</returns>
        public virtual bool HasKey(OSK_KeyCode k)
        {
            return keyDict != null && keyDict.ContainsKey(k);
        }

        public virtual void AddText(string newText)
        {
            output.AddText(newText);
        }

        /// <summary>
        /// v3.5 add a string of characters to the output. The enables adding a word like ".com" through special keys custom actions
        /// </summary>
        /// <param name="multichar">string of characters to send the receiver</param>
        public virtual void AddString(string multichar)
        {
            foreach(char c in multichar)
            {
                AddText_ShftEnabled(c.ToString());
            }
        }

        public virtual void AddNewLine()
        {
            output.NewLine();
        }

        /// <summary>
        /// use this method for SpecialKeys that are standing in for special characters (for example, "é") so that when shift is enabled
        /// the capitalized version of that character is typed (for example, "É"). Use this method as the Special Action
        /// </summary>
        /// <param name="newText"></param>
        public virtual void AddText_ShftEnabled(string newText)
        {
            if (shift || caps)
            {
                output.AddText(newText.ToUpperInvariant());
            } else
            {
                output.AddText(newText);
            }
        }

        public virtual void InsertText(string newText, OSK_Receiver receiver)
        {

            if (shift || caps)
            {
                if (receiver != null)
                    receiver.AddText(newText.ToUpperInvariant());
                else
                    output.AddText(newText.ToUpperInvariant());
            }
            else
            {
                if (receiver != null)
                    receiver.AddText(newText);
                else
                    output.AddText(newText);
            }
        }

        public virtual string Text()
        {
            return output.Text();
        }

        public virtual void HasFocus(bool isFocus)
        {
            hasFocus = isFocus;
        }

        public virtual void SetInteractable(bool isInteractable)
        {
            HasFocus(isInteractable);
        }

        /// <summary>
        /// changes the output of the OSK_Keyboard to a new OSK_Receiver and gives it focus
        /// </summary>
        /// <param name="newOutput"></param>
        public virtual void SetOutput(OSK_Receiver newOutput)
        {
            
            if (output != null)
                output.OnFocusLost();

            output = newOutput;
            output.OnFocus();
        }

        /// <summary>
        /// function to setup callback on key strokes for new Input System
        /// </summary>
        /// <param name="accept">true=accept and register physical keyboard strokes</param>
        protected void AcceptPhysicalKeyboard(bool accept)
        {

            if (accept)
            {
                viperInput.RegisterKeyStrokeCallback(OnPhysicalKeyStroke, true);
            }
            else
            {
                viperInput.RegisterKeyStrokeCallback(OnPhysicalKeyStroke, false);
            }

        }

        protected void Prep()
        {
            


            if (output != null)
                output.OnFocus();

            OSK_KeySounds ks = this.GetComponent<OSK_KeySounds>();
            if (ks != null)
            {
                sound += ks.PlaySound;
                selectSound += ks.PlaySelectKeySound;

            }



            if (Application.isPlaying)
            {
                if(generateOnStart)
                {
                    Generate();
                }
                else
                {
                    Traverse();
                }

                currentSelectedKey = this.transform.GetComponentInChildren<OSK_Key>();

                //AcceptPhysicalKeyboard(acceptPhysicalKeyboard);
            }



        }

        /// <summary>
        /// Input method to set "layout" from script. This should be followed by a call to Generate() in order to reflect the new layout in the OSK
        /// </summary>
        /// <param name="lay">space delimited layout of the keyboard</param>
        public virtual void LoadLayout(string lay)
        {
            layout = lay;
        }

        /// <summary>
        /// converts OSK_KeyCode to keycode. If the character is outside of the regular OSK_KeyCode enum, KeyCode.AltGr is returned
        /// </summary>
        /// <param name="k">OSK_KeyCode enum</param>
        /// <returns>Unity.KeyCode corresponding or KeyCode.AltGr otherwise</returns>
        public static KeyCode OSK_to_KeyCode(OSK_KeyCode k)
        {
            KeyCode res = KeyCode.AltGr;
            if((int)k <= 512)
            {
                res = (KeyCode)k;
            }

            return res;
        }

        /// <summary>
        /// finds the keycode for the corresponding string representation of the keycode
        /// </summary>
        /// <param name="c">string representation of keycode</param>
        /// <returns>returns OSK_KeyCode or OSK_KeyCode.None if not found or invalid</returns>
        public OSK_KeyCode GetOSKKeyCode(string c)
        {
            OSK_KeyCode k = OSK_KeyCode.None;
            if (osk_Keymap.chartoKeycode.TryGetValue(c, out k))
            {
                return k;
            } else if(c.Length == 1 && OSK_Keymap.IsAccentedCharacter(c[0]))
            {
                // adds an accented character to the keymap chartokeycode. keeps tack of # of accented characters for each base char
                string baseChar = OSK_Keymap.BaseCharacter(c).ToLowerInvariant();

                if(!osk_Keymap.altAlphabeticalAssignment.ContainsKey(baseChar))
                {
                    osk_Keymap.altAlphabeticalAssignment.Add(baseChar, 1);

                }

                if(osk_Keymap.altAlphabeticalAssignment[baseChar] > 18)
                {
                    Debug.LogError("OSK_Keyboard could not parse the layout for this key (" + c + ") each base character can only handle up to 18 accented variants");
                    return OSK_KeyCode.None;
                }

                k = (OSK_KeyCode)1000 + ((int)(baseChar[0] - 'a')+1)*100 + osk_Keymap.altAlphabeticalAssignment[baseChar];
                osk_Keymap.chartoKeycode.Add(c, k);

                osk_Keymap.altAlphabeticalAssignment[baseChar]++;

                return k;
            }
            else
            {
                try
                {
                    string prefix = "";
                    if (c[0] >= '0' && c[0] <= '9')
                        prefix = "Alpha";

                    k = (OSK_KeyCode)System.Enum.Parse(typeof(OSK_KeyCode), prefix + c);

                }
                catch (Exception e)
                {
                    Debug.LogError("OSK_Keyboard could not parse the layout for this key (" + c + ") make sure you are using KeyCode names for the characters\nException error: " + e.Message);
                    return OSK_KeyCode.None;
                }

                return k;
            }
        }

        /// <summary>
        /// finds the keycode for the corresponding string representation of the keycode
        /// </summary>
        /// <param name="c">string representation of keycode</param>
        /// <returns>returns keycode or KeyCode.None if not found or invalid</returns>
        public KeyCode GetKeyCode(string c)
        {
            KeyCode k = KeyCode.None;
            OSK_KeyCode osk_code;
            if (osk_Keymap.chartoKeycode.TryGetValue(c, out osk_code))
            {
                k = (int)osk_code < 505 ? (KeyCode)osk_code : KeyCode.AltGr;
                return k;
            }
            else
            {
                try
                {
                    string prefix = "";
                    if (c[0] >= '0' && c[0] <= '9')
                        prefix = "Alpha";

                    k = (KeyCode)System.Enum.Parse(typeof(OSK_KeyCode), prefix + c);

                }
                catch (Exception e)
                {
                    Debug.LogError("OSK_Keyboard could not parse the layout for this key (" + c + ") make sure you are using OSK_KeyCode names for the characters\nException error: " + e.Message);
                    return KeyCode.None;
                }

                return k;
            }
        }

        /// <summary>
        /// v3.50 clears all keys on the keyboard, resets keylayout and dictionary of allowed keys
        /// </summary>
        public virtual void Reset()
        {
            //first check and clear all keys
            OSK_Key[] list = this.transform.GetComponentsInChildren<OSK_Key>();

            if (list.Length > 0)
            {
                foreach (OSK_Key p in list)
                {
                    DestroyImmediate(p.gameObject);
                }
            }


            keyDict.Clear();

            keyLayout.Clear();
        }


        /// <summary>
        /// Routine to generate the keyboard keys based on the layout. Can be called from Editor or as runtime script
        /// </summary>
        public virtual void Generate()
        {
            Reset();

            List<string> layoutRows = layout.Split('\n').ToList();


            int lx = 0;
            int ly = 0;

            keySpanBottomRight = new Vector3(float.MinValue, float.MaxValue, 0f);
            keySpanTopLeft = new Vector3(float.MaxValue, float.MinValue, 0f);

            Vector3 keyOffset = new Vector3(keySize.x, -keySize.y, keySize.z);

            Vector3 v = topLeft.localPosition;

            keyLayout.Add(new List<OSK_Key>());

            for (int rowIdx=0; rowIdx < layoutRows.Count; rowIdx++)
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
                                v.x = v.x + keySize.x * n;
                            }
                            else
                            {
                                v.x = v.x + keySize.x;
                            }
                        }

                    }
                    else if (chars[i].Length >= 1)
                    {
                        GameObject k = Instantiate(KeyPrefab, this.transform) as GameObject;
                        OSK_Key thisKey = k.GetComponent<OSK_Key>();
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

                            Vector3 keysz = keySize;
                            keysz.x *= special.x_size;

                            thisKey.BackScale(keysz);
                            v.x = v.x + keysz.x * .5f;
                            k.transform.localPosition = v;
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

                            thisKey.BackScale(keySize);
                            v.x = v.x + keySize.x * .5f;
                            k.transform.localPosition = v;
                            v.x = v.x + keySize.x * .5f;

                            keyLayout[ly].Add(thisKey);
                            thisKey.SetLayoutLocation(lx, ly);
                            lx++;

                            thisKey.callBack.AddListener(KeyCallBase);
                            thisKey.callBack.AddListener(KeyCall);

                        }

                        // (v3.5) added variables to retain the topleft & bottomright
                        try
                        {
                            BoxCollider collider = thisKey.GetComponent<BoxCollider>();
                            keySpanTopLeft.x = Mathf.Min(keySpanTopLeft.x, collider.bounds.min.x);
                            keySpanTopLeft.y = Mathf.Max(keySpanTopLeft.y, collider.bounds.max.y);
                            keySpanBottomRight.x = Mathf.Max(keySpanBottomRight.x, collider.bounds.max.x);
                            keySpanBottomRight.y = Mathf.Min(keySpanBottomRight.y, collider.bounds.min.y);

                        } catch(Exception e)
                        {
                            Debug.LogError("Key " + thisKey.name + " does not have a collider and may not function properly\n"+e.Message);
                        }
                        

                        keyDict.Add(thisKey.key, thisKey);


                        if (Application.isEditor && shift) thisKey.ShiftUp();
                    }

                } // for words
                  
                
                // done with the row, a new row is created

                if (rowIdx != layoutRows.Count - 1)
                {
                    //Debug.Log("Com " + ly + " count=" + keyLayout[ly].Count);
                    v.x = topLeft.localPosition.x;
                    v.y += keyOffset.y;
                    ly++;
                    lx = 0;
                    keyLayout.Add(new List<OSK_Key>());
                }


            } // for rows
            

            if (Application.isPlaying)
            {
                if (shift || caps)
                    BroadcastMessage("ShiftUp");
                else
                    BroadcastMessage("ShiftDown");

            }


        }

        /// <summary>
        /// This function re-establishes the dictionaries, layout and other links in an existing keyboard without deleting OSK_Key objects
        /// </summary>
        public virtual void Traverse()
        {
            OSK_Key[] list = this.transform.GetComponentsInChildren<OSK_Key>();

            if (list.Length == 0)
                return; // keyboard not generated, no sense in traversing the keys

            keyDict.Clear();
            keyLayout.Clear();

            foreach (OSK_Key k in list)
            {
                keyDict.Add(k.key, k);
            }

            List<string> layoutRows = layout.Split('\n').ToList();

            keyLayout.Clear();
            int lx = 0;
            int ly = 0;

            keySpanBottomRight = new Vector3(float.MinValue, float.MaxValue, 0f);
            keySpanTopLeft = new Vector3(float.MaxValue, float.MinValue, 0f);

            keyLayout.Add(new List<OSK_Key>());

            for (int rowIdx = 0; rowIdx < layoutRows.Count; rowIdx++)
            {
                string[] chars = layoutRows[rowIdx].Split(' ');


                for (int i = 0; i < chars.Length; i++)
                {
                    // when a newline is encountered, a new row is created
                    if (chars[i].Contains("Skip"))
                    {
                        // do nothing as the keyboard is already generated
                    }
                    else if (chars[i].Length >= 1)
                    {

                        OSK_KeyCode key = GetOSKKeyCode(chars[i]);



                        OSK_Key thisKey = list.First(item => item.key == key);
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
                            BoxCollider collider = thisKey.GetComponent<BoxCollider>();
                            keySpanTopLeft.x = Mathf.Min(keySpanTopLeft.x, collider.bounds.min.x);
                            keySpanTopLeft.y = Mathf.Max(keySpanTopLeft.y, collider.bounds.max.y);
                            keySpanBottomRight.x = Mathf.Max(keySpanBottomRight.x, collider.bounds.max.x);
                            keySpanBottomRight.y = Mathf.Min(keySpanBottomRight.y, collider.bounds.min.y);


                        }
                        catch (Exception e)
                        {
                            Debug.LogError("Key " + thisKey.name + " does not have a collider and may not function properly\n"+e.Message);
                        }



                    }
                } //words

                if (rowIdx != layoutRows.Count - 1)
                {
                    ly++;
                    lx = 0;
                    keyLayout.Add(new List<OSK_Key>());
                }


            }//rows

            if (Application.isPlaying)
            {
                if (shift || caps)
                    BroadcastMessage("ShiftUp");
                else
                    BroadcastMessage("ShiftDown");

            }
        }



        /// <summary>
        /// Plays sound based on the list of sounds
        /// </summary>
        /// <param name="keytypecode">index of the sound to be played from the KeySounds list</param>
        public void ClickSound(int keytypecode)
        {
            if (soundFX)
            {
                sound.Invoke(keytypecode);
            }

        }

        /// <summary>
        /// Plays sound when the selecting a key using a gamepad
        /// </summary>
        public void SelectSound()
        { 
            if(soundFX)
            {
                selectSound.Invoke();
            }
        
        }

        /// <summary>
        /// updates the output text
        /// </summary>
        protected void OutputTextUpdate(string newchar, OSK_Receiver receiver)
        {
            if (this.gameObject.activeSelf)
            {
                if(receiver == null)
                {
                    output.AddText(newchar);
                } else
                {
                    receiver.AddText(newchar);
                }
    
                
            }
        }

        /// <summary>
        /// this base KeyCall callback is invoked for all key presses whether the key has a UnityEvent attached or not.
        /// </summary>
        /// <param name="k">keycode of key pressed</param>
        /// <param name="ktype"></param>
        public virtual void KeyCallBase(OSK_KeyCode k, OSK_Receiver receiver)
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
        /// <param name="receiver">OSK_Receiver that will be outputing the keystroke</param>
        public virtual void KeyCall(OSK_KeyCode k, OSK_Receiver receiver)
        {
            //Debug.Log("keypressed=" + k.ToString());

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
                else if(output != null)
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

            if(k == OSK_KeyCode.LeftControl || k == OSK_KeyCode.RightControl)
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

            } else
            {
                n = keyDict[k].GetKeyName();
            }


            OutputTextUpdate(n, receiver);

            if (!caps && shift)
                KeyShift();

        }

        public virtual void  KeyBackspace(OSK_Receiver receiver)
        {
            if (receiver != null)
            {
                if (receiver.Text().Length > 0)
                    OutputTextUpdate("\x0008", receiver);
                return;
            }
            else
            {
                if (output.Text().Length > 0)
                    OutputTextUpdate("\x0008", null);
            }

        }

        public virtual void KeyDelete(OSK_Receiver receiver)
        {

            if (receiver != null)
            {
                if (receiver.Text().Length > 0)
                    OutputTextUpdate("\x007F", receiver);
                return;
            }
            else
            {
                if (output.Text().Length > 0)
                    OutputTextUpdate("\x007F", null);
            }

        }

        /// <summary>
        /// calls the UnityEvent and passes the output text, this is normally when the user presses Enter
        /// </summary>
        public virtual void Submit()
        {
            output.Submit();
        }

        /// <summary>
        /// Toggles the shift key
        /// </summary>
        public virtual void KeyShift()
        {
            shift = !shift;
            if (shift)
                BroadcastMessage("ShiftUp");
            else
                BroadcastMessage("ShiftDown");
        }

        /// <summary>
        /// Joystick or GamePad button 
        /// GamePad A button
        /// or on PS4 the X button
        /// </summary>
        public virtual void ButtonA()
        {
            
            I_OSK_Key k = keyLayout[DpadSelection.y][DpadSelection.x];
            if (k != null)
                k.Click("joystick");

            StartCoroutine(ReHighlightKey());
        }

        protected IEnumerator ReHighlightKey()
        {
            yield return new WaitForSecondsRealtime(0.2f);
            // when using a gamepad, this function will re-highlight the key
            currentSelectedKey.Highlight(true, highlighterColor);
        }

        /// <summary>
        /// sets the highlighted key (for gamepad controllers) to the keycode specified if available
        /// </summary>
        /// <param name="k">keyCode for the specified key</param>
        public virtual void SetSelectedKey(OSK_KeyCode k)
        {
            if (HasKey(k))
            {
                if (currentSelectedKey != null) currentSelectedKey.Highlight(false, Color.white);
                currentSelectedKey = (OSK_Key)keyDict[k];
                DpadSelection = currentSelectedKey.GetLayoutLocation();
                currentSelectedKey.Highlight(true, highlighterColor);
            }
        }

        /// <summary>
        /// sets the highlighted key (for gamepad controllers) to the key corresponding to the string provided
        /// </summary>
        /// <param name="k">string representation of the keycode</param>
        public virtual void SetSelectedKey(string c)
        {

            OSK_KeyCode k = GetOSKKeyCode(c);
            SetSelectedKey(k);          
        }

        public OSK_Key GetSelectedKey()
        {
            return currentSelectedKey;
        }

        /// <summary>
        /// finds the key corresponding to the string based on the 
        /// </summary>
        /// <param name="k"></param>
        /// <returns></returns>
        public OSK_Key GetOSKKey(string k)
        {
            OSK_KeyCode keycode = GetOSKKeyCode(k);
            OSK_Key key = null;
            if (HasKey(keycode))
                key = (OSK_Key)keyDict[keycode];

            return key;
        }

        /// <summary>
        /// handles how the movement of the highlighted key is done when using a GamePad or Joystick. The movements on a stick are approximated to those of a Directional Pad (D-Pad)
        /// so that the key highlight is simple and elegant
        /// </summary>
        /// <param name="dir">Vector2D direction of movement on the screen</param>
        public virtual void DpadMove(Vector2 dir)
        {
            OSK_Key k = keyLayout[DpadSelection.y][DpadSelection.x];
            k.Highlight(false, Color.white);
            dir.x = Mathf.RoundToInt(dir.x);
            dir.y = Mathf.RoundToInt(dir.y);
            // movement along y-axis
            if (dir.y != 0f)
            {
                
                DpadSelection.y = DpadSelection.y + (int) dir.y;
                if (DpadSelection.y < 0)
                {
                    if(gamepadKeyboardWrap == KEYBOARD_WRAP.Y_WRAP || gamepadKeyboardWrap == KEYBOARD_WRAP.XY_WRAP)
                    {
                        DpadSelection.y = keyLayout.Count - 1;
                    } else 
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
                int x = DpadSelection.x;

                do
                {
                    x += (int) dir.x;
                    if (x < 0)
                    {
                        if (gamepadKeyboardWrap == KEYBOARD_WRAP.X_WRAP || gamepadKeyboardWrap == KEYBOARD_WRAP.XY_WRAP)
                        {
                            x = keyLayout[DpadSelection.y].Count - 1;
                        } else if(gamepadKeyboardWrap == KEYBOARD_WRAP.X_CASCADE)
                        {
                            DpadSelection.y = Mathf.Clamp(DpadSelection.y - 1, 0, keyLayout.Count - 1);
                            x = keyLayout[DpadSelection.y].Count - 1;
                            
                        } else
                            x = 0;
                        break;
                    } else if(x >= keyLayout[DpadSelection.y].Count)
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
                } while (keyLayout[DpadSelection.y][x].key == k.key);

                DpadSelection.x = x;
            }

            try
            {

                DpadSelection.Clamp(new Vector2Int(0, 0), new Vector2Int(keyLayout[DpadSelection.y].Count - 1, keyLayout.Count - 1));
                keyLayout[DpadSelection.y][DpadSelection.x].Highlight(true, highlighterColor);
                currentSelectedKey = keyLayout[DpadSelection.y][DpadSelection.x];
                if (k != keyLayout[DpadSelection.y][DpadSelection.x])
                    SelectSound();

            } catch(Exception e)
            {
                Debug.LogError("Exemption in moving through OSK layout DpadSel=" + DpadSelection.ToString() +" x="+(int)DpadSelection.x+ " y="+ (int)DpadSelection.y + "\ne=" + e.Message);
            }
        }

        /// <summary>
        /// handles how the movement of the highlighted key is done when using a GamePad or Joystick. The movements on a stick are approximated to those of a Directional Pad (D-Pad)
        /// so that the key highlight is simple and elegant
        /// </summary>
        /// <param name="dir">Vector2D direction of movement on the screen</param>
        /// Returns the selection based on keyLayout x and y
        public virtual OSK_Key SelectedKeyMove(Vector2 dir, Vector2Int currentLoc, bool makeSoundIfMove = true)
        {
            Vector2Int newLoc = currentLoc;

            OSK_Key k = keyLayout[currentLoc.y][currentLoc.x];

            dir.x = Mathf.RoundToInt(dir.x);
            dir.y = Mathf.RoundToInt(dir.y);
            // movement along y-axis
            if (dir.y != 0f)
            {

                newLoc.y = newLoc.y + (int)dir.y;
                if (newLoc.y < 0)
                {
                    if (gamepadKeyboardWrap == KEYBOARD_WRAP.Y_WRAP || gamepadKeyboardWrap == KEYBOARD_WRAP.XY_WRAP)
                    {
                        newLoc.y = keyLayout.Count - 1;
                    }
                    else
                        newLoc.y = 0;

                }
                else if (newLoc.y >= keyLayout.Count)
                {
                    if (gamepadKeyboardWrap == KEYBOARD_WRAP.Y_WRAP || gamepadKeyboardWrap == KEYBOARD_WRAP.XY_WRAP)
                    {
                        newLoc.y = 0;
                    }
                    else
                        newLoc.y = keyLayout.Count - 1;

                }

            }


            // movement along x-axis
            if (dir.x != 0f)
            {
                int x = newLoc.x;

                do
                {
                    x += (int)dir.x;
                    if (x < 0)
                    {
                        if (gamepadKeyboardWrap == KEYBOARD_WRAP.X_WRAP || gamepadKeyboardWrap == KEYBOARD_WRAP.XY_WRAP)
                        {
                            x = keyLayout[newLoc.y].Count - 1;
                        }
                        else if (gamepadKeyboardWrap == KEYBOARD_WRAP.X_CASCADE)
                        {
                            newLoc.y = Mathf.Clamp(newLoc.y - 1, 0, keyLayout.Count - 1);
                            x = keyLayout[newLoc.y].Count - 1;

                        }
                        else
                            x = 0;
                        break;
                    }
                    else if (x >= keyLayout[newLoc.y].Count)
                    {

                        if (gamepadKeyboardWrap == KEYBOARD_WRAP.X_WRAP || gamepadKeyboardWrap == KEYBOARD_WRAP.XY_WRAP)
                        {
                            x = 0;
                        }
                        else if (gamepadKeyboardWrap == KEYBOARD_WRAP.X_CASCADE)
                        {
                            newLoc.y = Mathf.Clamp(newLoc.y + 1, 0, keyLayout.Count - 1);
                            x = 0;

                        }
                        else
                            x = keyLayout[newLoc.y].Count - 1;

                        break;
                    }
                } while (keyLayout[newLoc.y][x].key == k.key);

                newLoc.x = x;
            }

            try
            {

                newLoc.Clamp(new Vector2Int(0, 0), new Vector2Int(keyLayout[newLoc.y].Count - 1, keyLayout.Count - 1));
                if(newLoc != currentLoc && makeSoundIfMove)
                    SelectSound();
                return keyLayout[newLoc.y][newLoc.x];
            }
            catch (Exception e)
            {
                Debug.LogError("Exemption in moving through OSK layout DpadSel=" + newLoc.ToString() + " x=" + newLoc.x + " y=" + newLoc.y + "\ne=" + e.Message);
            }

            return null;
        }

        /// <summary>
        /// callback to capture physical key strokes, this is particular to new Input System but can also be adapted to your use
        /// </summary>
        /// <param name="c">char typed on keyboard</param>
        protected virtual void OnPhysicalKeyStroke(char c)
        {

            OSK_KeyCode k;
            if(osk_Keymap.chartoKeycode.TryGetValue(c.ToString(), out k))
            {
                I_OSK_Key key;
                if(keyDict.TryGetValue(k, out key))
                {
                    key.Click("keyboard");
                    
                }
            }

        }

        /// <summary>
        /// Method uses raycasting instead of OnMouseDown/Up callbacks
        /// It can be further refined by allocating a layer to all keys anc only raycasting against that layer
        /// it was not done here so it does not interfere with existing user layers
        /// </summary>
        protected void InputFromPointerDevice()
        {
            Vector2 pointPos = viperInput.GetPointerPos();

            if (viperInput.PointerUp())
            {
                Ray ray = Camera.main.ScreenPointToRay(pointPos);

                RaycastHit hit;


                if (Physics.Raycast(ray, out hit))
                {

                    I_OSK_Key key = hit.transform.gameObject.GetComponent<I_OSK_Key>();
                    if (key != null)
                    {
                        key.OnKeyDepress("pointer");
                    }

                }
            }

            if (viperInput.PointerDown())
            {

                Ray ray = Camera.main.ScreenPointToRay(pointPos);

                RaycastHit hit;


                if (Physics.Raycast(ray, out hit))
                {

                    I_OSK_Key key = hit.transform.gameObject.GetComponent<I_OSK_Key>();
                    if (key != null)
                    {
                        key.OnKeyPress("pointer");
                    }

                }
            }


        }

        /// <summary>
        /// this function is for developers using alternate input assets
        /// you can use this function as Action in your asset
        /// </summary>
        /// <param name="f">value of -1.0f to 1.0f on horizontal axis</param>
        public virtual void GamepadInput_Horizontal(float f)
        {
            DpadMove(Vector2.right * f);
        }

        /// <summary>
        /// this function is for developers using alternate input assets
        /// you can use this function as Action in your asset
        /// </summary>
        /// <param name="f">value of -1.0f to 1.0f on vertical axis</param>
        public virtual void GamepadInput_Vertical(float f)
        {
            DpadMove(Vector2.down * f);
        }

        /// <summary>
        /// this function is for developers using alternate input assets
        /// you can use this function as Action in your asset
        /// this function is for users selecting the highlighted key
        /// </summary>
        public virtual void GamepadInput_Submit()
        {
            ButtonA();
        }

        /// <summary>
        /// this function is for developers using alternate input assets
        /// you can use this function as Action in your asset
        /// this function is for users pressing the desired button to cancel the keyboard
        /// you can design it to be a default Backspace (as per example below) or to stow away the keyboard
        /// </summary>
        public virtual void GamepadInput_Cancel()
        {
            // change this to the desired action
            KeyBackspace(null);
        }

        // Start is called before the first frame update
        void Awake()
        {
            if(this.GetComponent<OSK_Settings>() == null)
            {
                this.gameObject.AddComponent<OSK_Settings>();
            }

            Prep();

        }

        private void Start()
        {

            if (output != null)
                output.OnFocus();
        }


        void OnGUI()
        {

            if (!hasFocus || !Application.isPlaying)
                return;

            if (acceptPhysicalKeyboard)
            {
                
                if (Event.current.isKey && keyDict.ContainsKey((OSK_KeyCode) Event.current.keyCode))
                {
                    //KeyCall(Event.current.keyCode, OSK_KEY_TYPES.LETTER);
                    keyDict[(OSK_KeyCode)Event.current.keyCode].Click("keyboard", output);
                    
                    
                }
            }

        }

        private void Update()
        {

            if (!hasFocus  || bypassDefaultInput || !Application.isPlaying)
                return;

#if ENABLE_INPUT_SYSTEM

            InputFromPointerDevice();

#endif

            // Collects gamepad or joystick input and highlights the appropriate key by calling DpadMove
            if (acceptGamePadInput && keyLayout.Count > 0)
            {
                inputTimer += Time.unscaledDeltaTime;


                if (viperInput.AButtonDown())
                {

                   currentSelectedKey.JoystickPressDown();
                }

                if(viperInput.AButtonUp())
                {

                    currentSelectedKey.JoystickPressUp();
                }

                // inputTimer dampens the input from the joystick or gamepad so that the user highlight doesn't move too quickly.
                // the "inputTimerThreshold" can be changed in the Inspector to make it slower or faster, this can also be affected by the Input Manager/System sensitivity to these input devices
                if (inputTimer > inputTimerThreshold)
                {
                    inputTimer = 0f;

                    // up/down
                    if (viperInput.GetAxis(AXIS_INPUT.DPAD_Y) != 0f)
                    {
                        DpadMove(Vector2.down * viperInput.GetAxis(AXIS_INPUT.DPAD_Y));

                        viperInput.ResetAllAxis();
                    }

                    else if (viperInput.GetAxis(AXIS_INPUT.LEFTSTICK_Y) != 0f)
                    {
                        DpadMove(Vector2.down * viperInput.GetAxis(AXIS_INPUT.LEFTSTICK_Y));

                        viperInput.ResetAllAxis();
                    }

                    // left/right
                    else if (viperInput.GetAxis(AXIS_INPUT.DPAD_X) != 0f)
                    {
                        DpadMove(Vector2.right * viperInput.GetAxis(AXIS_INPUT.DPAD_X));

                        viperInput.ResetAllAxis();
                    }

                    else if (viperInput.GetAxis(AXIS_INPUT.LEFTSTICK_X) != 0f)
                    {
                        DpadMove(Vector2.right * viperInput.GetAxis(AXIS_INPUT.LEFTSTICK_X));

                        viperInput.ResetAllAxis();
                    }
                }

            }

            

        }


    }



}