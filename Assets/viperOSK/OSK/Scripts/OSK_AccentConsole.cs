///////////////////////////////////////////
///
/// viperOSK AccentConsole allows developers to build in accented characters in viperOSK and control which characters show
/// 
/// © vipercode corp
/// 2022
/// Please use this asset according to the attached license
/// Attributions, mentions and reviews are always welcomed
///
///////////////////////////////////////////////////////////////////////////////////////





using System;
using System.IO;
using System.Collections;
using System.Collections.Generic;
using System.Linq;
using UnityEngine;
using viperTools;
using UnityEditor;
using UnityEngine.Events;
using UnityEngine.EventSystems;
using UnityEngine.UI;

namespace viperOSK
{

    public class OSK_AccentConsole : MonoBehaviour
    {

        /// <summary>
        /// this dictionary is preset with viperOSK common language accents, but you can set the accentMapAsset to the asset you have created with your custom map
        /// </summary>
        public Dictionary<string, List<string>> accentMap = new Dictionary<string, List<string>> { 
    
        // Letter "a" and its accents
        { "a", new List<string> { "á", "à", "â", "ä", "ã", "å", "ā", "ă", "ą", "ǎ", "æ" } },

        // Letter "c" and its accents
        { "c", new List<string> { "ç", "ć", "č", "ĉ", "ċ" } },

        // Letter "e" and its accents
        { "e", new List<string> { "é", "è", "ê", "ë", "ē", "ĕ", "ė", "ę", "ě" } },

        // Letter "g" and its accents
        { "g", new List<string> { "ğ", "ģ", "ĝ", "ġ", "ǧ" } },

        // Letter "i" and its accents
        { "i", new List<string> { "í", "ì", "î", "ï", "ĩ", "ī", "ĭ", "į", "ı", "ǐ" } },

        // Letter "n" and its accents
        { "n", new List<string> { "ñ", "ń", "ň", "ņ", "ŋ", "ṅ" } },

        // Letter "o" and its accents
        { "o", new List<string> { "ó", "ò", "ô", "ö", "õ", "ō", "ŏ", "ő", "œ", "ø", "ǒ" } },

        // Letter "s" and its accents
        { "s", new List<string> { "ś", "š", "ş", "ŝ", "ș", "ß" } },

        // Letter "u" and its accents
        { "u", new List<string> { "ú", "ù", "û", "ü", "ũ", "ū", "ŭ", "ů", "ű", "ų", "ǔ" } },

        // Letter "y" and its accents
        { "y", new List<string> { "ý", "ÿ", "ŷ" } },

        // Letter "z" and its accents
        { "z", new List<string> { "ź", "ž", "ż", "ẑ" } },

        // Letter "d" and its accents
        { "d", new List<string> { "ď", "đ", "ð" } },

        // Letter "l" and its accents
        { "l", new List<string> { "ĺ", "ľ", "ļ", "ł" } },

        // Letter "r" and its accents
        { "r", new List<string> { "ŕ", "ř", "ŗ" } },

        // Letter "t" and its accents
        { "t", new List<string> { "ť", "ţ", "ț", "ŧ" } },

        // Letter "h" and its accents
        { "h", new List<string> { "ĥ", "ħ" } },

        // Letter "j" and its accents
        { "j", new List<string> { "ĵ" } },

        // Letter "k" and its accents
        { "k", new List<string> { "ķ" } },

        // Letter "b" and its accents
        { "b", new List<string> { "ƀ", "ɓ" } },

        // Letter "m" and its accents
        { "m", new List<string> { "ṁ" } },

        // Letter "p" and its accents
        { "p", new List<string> { "ṕ" } },

        // Letter "f" and its accents
        { "f", new List<string> { "ƒ" } },

        // Letter "w" and its accents
        { "w", new List<string> { "ŵ" } },

        // Letter "q" and its accents
        { "q", new List<string> { "ɋ" } },

        // Letter "x" and its accents
        { "x", new List<string> { "ẋ", "ẍ" } },

        // Letter "v" and its accents
        { "v", new List<string> { "ṽ", "ʋ" } }
        };

        /// <summary>
        /// asset holding the list of accented characters for each base character
        /// </summary>
        public OSK_AccentAssetObj accentAsset;

        OSK_LongPressPacket longPressPacket;

        string baseChar;

        public OSK_Keyboard keyboard;

        //public SpriteRenderer background;

        public OSK_MiniKeyboard miniKeyboard;

        bool isVisible;

        bool BbtnDown;

        /// <summary>
        /// Records the timestamp (in seconds since startup) when the minikeyboard was launched, 
        /// use this to stow the mini-keyboard away after a delay if needed
        /// </summary>
        float chrono;

        /// <summary>
        /// detection of pointers in UI
        /// </summary>
        PointerEventData pointerEventData;

        public bool IsVisible()
        {
            return isVisible;
        }

        // Start is called before the first frame update
        void Start()
        {
            OSK_Settings.instance.hasAccentedConsole = true;

            if (keyboard == null)
            {
#if UNITY_2019
                keyboard = GameObject.FindObjectOfType<OSK_Keyboard>();
#else
                keyboard = GameObject.FindFirstObjectByType<OSK_Keyboard>();
#endif
                if (keyboard == null)
                {
                    Debug.LogWarning("No OSK_Keyboard found in the scene. You must have an OSK_Keyboard in the scene for this component to work");
                }
                else
                {
                    Debug.Log("No OSK_Keyboard was set, successfully selected the first OSK_Keyboard object in the scene");
                }
            } 


            if(miniKeyboard == null)
            {
                //GameObject obj = Instantiate();
            }

            EventSystem eventSystem;
            Canvas canvas = this.GetComponentInParent<Canvas>();
            if (canvas != null)
            {
#if UNITY_2019
                eventSystem = GameObject.FindObjectOfType<EventSystem>();
#else
                eventSystem = GameObject.FindAnyObjectByType<EventSystem>();
#endif

                // Initialize the PointerEventData with the current EventSystem
                pointerEventData = new PointerEventData(eventSystem);
            }

            LoadAccentMap(accentAsset);

            OSK_Settings.instance.SetLongPressAction(SetConsole);

        }

        public void LoadAccentMap(OSK_AccentAssetObj accents)
        {
            if (accents == null)
            {
                Debug.Log("Accent Map not set. Using default viperOSK common accented characters");
                return;
            }

            // Load the AccentMapScriptableObject into AccentMap dictionary

            // Clear the current dict data
            accentMap.Clear();

            // Populate accentMap with data from the asset
            foreach (var entry in accents.entries)
            {
                accentMap[entry.baseCharacter] = new List<string>(entry.accentedCharacters);
            }

        }


        private void OnDestroy()
        {
            OSK_Settings.instance.hasAccentedConsole = false;
        }

        public void SetConsole(OSK_LongPressPacket packet)
        {
            Set(packet);
        }

        /// <summary>
        /// use this function if you need to check via script whether a the key pressed has accented variants.
        /// </summary>
        /// <param name="packet">longpress packet of information</param>
        /// <returns>true if key has accented variant, false otherwise</returns>
        public bool Set(OSK_LongPressPacket packet)
        {

            longPressPacket = packet;

            miniKeyboard.SetBaseKey(packet.keyObj);
            miniKeyboard.isJoystickSelection = packet.keyPressType == "joystick";

            if (keyboard.osk_Keymap.chartoKeycode.Values.Contains(packet.keyCode))
            {
                baseChar = keyboard.osk_Keymap.chartoKeycode.FirstOrDefault(t => t.Value == packet.keyCode).Key.ToLower();
            } else
            {
                baseChar = string.Empty;
            }
            

            if (baseChar != null && baseChar != string.Empty && accentMap.ContainsKey(baseChar))
            {

                Generate();
                return true;
            }

            return false;
        }

        public void Reset()
        {
            miniKeyboard.Reset();

        }

        public void ShowBackground(bool show)
        {
            //background.enabled = show;
            //background.GetComponent<BoxCollider>().enabled = show;
            isVisible = show;
        }

        /// <summary>
        /// removes the accent console and returns the focus to the keyboard
        /// </summary>
        public void RemoveConsole()
        {
            Reset();
            //ShowBackground(false);
            keyboard.HasFocus(true);
            keyboard.SetInteractable(true);
            if(miniKeyboard.isUI) EventSystem.current.SetSelectedGameObject(miniKeyboard.baseKey.GetGameObject());
            isVisible = false;
            chrono = 0f;
        }

        /// <summary>
        /// main action to insert a character in the OSK_Receiver (or default output receiver of the keyboard)
        /// </summary>
        /// <param name="accentedChar">character to print</param>
        /// <param name="receiver">target receiver, null sends it to the keyboard's receiver</param>
        public void AccentCharClick(string accentedChar, OSK_Receiver receiver)
        {
            keyboard.InsertText(accentedChar, receiver);
            RemoveConsole();
        }

        void Generate()
        {
            Reset();
            //ShowBackground(true);

            StartCoroutine(GenerateCoroutine());

            keyboard.HasFocus(false);
            keyboard.SetInteractable(false);

        }

        IEnumerator GenerateCoroutine()
        {


            miniKeyboard.Generate(accentMap[baseChar], keyboard.shift || keyboard.caps, AccentCharClick);

            miniKeyboard.acceptGamePadInput = keyboard.acceptGamePadInput;


            this.transform.position = miniKeyboard.baseKey.GetKeyTransform().position + new Vector3(0f, miniKeyboard.GetSize().y * .5f + miniKeyboard.baseKey.getYSize() * .5f, -.5f);

            chrono = Time.realtimeSinceStartup;

            yield return new WaitForSecondsRealtime(0.1f);

            if (miniKeyboard.acceptGamePadInput)
            {
                if(longPressPacket.keyPressType == "joystick")
                {
                    miniKeyboard.SelectedFirstKey();

                }
               
            }

            isVisible = true;
        }

        // Update is called once per frame
        void Update()
        {
            // manage user clicking outside the mini-keyboard. If so, the mini-keyboard should be stowed away
            if (isVisible)
            {
                // allow keyboard to stay alive for 1sec before it can be dismissed
                if (viperInput.PointerUp() && Time.realtimeSinceStartup > chrono + 1f)
                {
                    bool outsideConsole = true;

                    if (miniKeyboard.isUI)
                    {
                        pointerEventData.position = viperInput.GetPointerPos();

                        // Create a list to store all hit results
                        List<RaycastResult> hits = new List<RaycastResult>();
                        GraphicRaycaster uiRaycaster;

#if UNITY_2019
                        uiRaycaster = GameObject.FindObjectOfType<GraphicRaycaster>();
#else
                        uiRaycaster = GameObject.FindAnyObjectByType<GraphicRaycaster>();
#endif                    

                        // Raycast using the GraphicRaycaster and store the hits in the results list
                        uiRaycaster.Raycast(pointerEventData, hits);

                        // Process results if any UI elements were hit
                        foreach (RaycastResult h in hits)
                        {

                            if (h.gameObject.GetComponentInParent<OSK_AccentConsole>() != null)
                            {
                                outsideConsole = false;
                            }
                        }
                    }
                    else
                    {

                        Vector2 pointPos = viperInput.GetPointerPos();

                        Ray ray = Camera.main.ScreenPointToRay(pointPos);

                        RaycastHit[] hits = Physics.RaycastAll(ray, 1000f); // if the keyboard is further away ensure you change the distance value             

                        foreach (RaycastHit h in hits)
                        {
                            if (h.collider.GetComponentInParent<OSK_MiniKeyboard>() != null)
                            {
                                outsideConsole = false;
                            }
                        }
                    }
                    

                    if (outsideConsole)
                    {
                        RemoveConsole();
                    }


                }


                // When using a gamepad/joystick, the default for button B is to cancel the mini-keyboard.
                // Change this if  you need a different key to cancel the mini-keyboard
                if (viperInput.BButtonUp() && BbtnDown)
                {
                    RemoveConsole();
                    BbtnDown = false;
                }

                if (viperInput.BButtonDown())
                {
                    BbtnDown = true;

                } else
                {
                    BbtnDown = false;
                }


            }
        }
    }

}


