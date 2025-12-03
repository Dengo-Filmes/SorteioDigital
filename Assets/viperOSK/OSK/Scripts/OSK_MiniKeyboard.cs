////////////////////////////////////////////////////////
/// OSK_MiniKeyboard is a module to allow for simple and small keyboards. 
/// In particular the pop-up mini-keyboard that shows during long-press (if enabled) to show accented or special characters
/// This is a simplified approach where the callback sends directly the character to print
/// All keys are created with the same uniform look (does not have the OSK_Keyboard capability of specialkeys)
/// All keys have only 1 callback and they do not perform special tasks (unless you program that in)
/// separatiors and layout in OSK_Keyboard is not available here as this is for specific and limited use
/// 
/// This class can be used on its own as well as a highly simplified OSK_Keyboard
/// 
/// 
/// © vipercode corp
/// 2024
/// Please use this asset according to the attached license
/// Attributions, mentions and reviews are always welcomed
///
///////////////////////////////////////////////////////////////////////////////////////

using System;
using System.Collections;
using System.Collections.Generic;
using System.Linq;
using UnityEngine;
using UnityEngine.EventSystems;
using UnityEngine.UI;
using viperTools;

namespace viperOSK
{
    public class OSK_MiniKeyboard : MonoBehaviour
    {

        /// <summary>
        /// dimensions of the accent popup in rows and columns of keys
        /// default 2x6, 2 rows of 6 keys each
        /// </summary>
        public Vector2Int dimensions = new Vector2Int(2, 6);

        /// <summary>
        /// base key to use as prefab when generating mini-keyboard
        /// </summary>
        public I_OSK_Key baseKey;

        GameObject baseKeyGO;

        public GameObject keyPrefab;
        public Vector3 keySize = new Vector3(1f, 1f, 1f);

        /// <summary>
        /// ordered layout of keys
        /// </summary>
        List<List<I_OSK_Key>> keyLayout = new List<List<I_OSK_Key>>();

        public Sprite backgroundImg;
        public GameObject backgroundObj;

        /// <summary>
        /// baricenter of keys
        /// </summary>
        Vector3 center;


        /// <summary>
        /// world size of keyboard
        /// </summary>
        Vector3 size;

        /// <summary>
        /// selected key in when navigating
        /// </summary>
        I_OSK_Key selectedKey;

        /// <summary>
        /// selected key location in keyLayout (row/col)
        /// </summary>
        Vector2Int selectedKeyLoc;

        int numKeys;

        /// <summary>
        /// boolean to identify whether the mini-keyboard is for a UI or 3D keyboard
        /// </summary>
        public bool isUI;
        public bool isJoystickSelection;



        public bool acceptGamePadInput;
        public Color highlighterColor;
        float inputTimerThreshold = 0.15f;
        float inputTimer;
        bool AbtnDown;


        bool isActive;

        // Start is called before the first frame update
        void Start()
        {

        }

        public Vector3 GetSize()
        {
            return size;
        }

        public void Reset()
        {
            OSK_Key[] list = this.transform.GetComponentsInChildren<OSK_Key>();
            if (list.Length > 0)
            {
                foreach (OSK_Key p in list)
                {
                    Destroy(p.gameObject);
                }
            }

            OSK_UI_Key[] list2 = this.transform.GetComponentsInChildren<OSK_UI_Key>();
            if (list2.Length > 0)
            {
                foreach (OSK_UI_Key p in list2)
                {
                    Destroy(p.gameObject);
                }
            }

            numKeys = 0;

            keyLayout.Clear();

            if(backgroundObj != null)
            {
                backgroundObj.SetActive(false);
            }

            isActive = false;
        }

        public void SetBaseKey(GameObject base_key)
        {
            // first find the type of the base key
            OSK_Key key = base_key.GetComponent<OSK_Key>();
            if (key != null)
            {
                isUI = false;
                baseKey = key;
                baseKeyGO = base_key;
                return;
            }

            OSK_UI_Key uiKey = base_key.GetComponent<OSK_UI_Key>();
            if (uiKey != null)
            {
                isUI = true;
                baseKey = uiKey;
                baseKeyGO = base_key;

                if(this.gameObject.GetComponent<CanvasGroup>() == null)
                    this.gameObject.AddComponent<CanvasGroup>();
            }
        }


        /// <summary>
        /// Generates the mini-keyboard for the list of characters based
        /// Keyboards are generated so that the pivot of the OSK_MiniKeyboard transform is at the center of the mini-keyboard
        /// 
        /// </summary>
        /// <param name="chars">list of characters to populate the mini-keyboard</param>
        /// <param name="bottomLeftOrder">true= keyboard ordered from bottom-right to top-left, false for vice versa</param>
        public void Generate(List<string> chars, bool shiftup, Action<string, OSK_Receiver> callbackAction, bool bottomLeftOrder = true)
        {
            if (baseKey == null)
            {
                if (keyPrefab != null)
                {
                    SetBaseKey(keyPrefab);
                } else
                {
                    Debug.LogError("OSK_MiniKeyboard requires you set either a baseKey or a keyPrefab to Generate a mini-keyboard");
                    return;
                }
            }


            Reset();

            CreateBackground();

            keyLayout.Add(new List<I_OSK_Key>());

            // if the number of accented characters is less than ideal items per row, then all keys on one row
            // otherwise, split the numbr of accented characters so that it fits best the ideal dimensions of rows and column
            // with a preference to splilling on columns but not rows (given that keyboards are usually horizontal)
            int keys_per_row, num_of_rows;
            if (chars.Count == dimensions.x * dimensions.y)
            {
                keys_per_row = dimensions.y;
                num_of_rows = dimensions.x;

            } else
            {
                keys_per_row = chars.Count <= dimensions.y ? chars.Count : Mathf.CeilToInt((float)chars.Count / (float)dimensions.x);
                num_of_rows = Mathf.CeilToInt((float)chars.Count / (float)keys_per_row);
            }



            int j = 0;
            int l = 0;



            // Initialize min and max vectors to find center of the keyboard
            Vector3 minPoint = new Vector3(float.MaxValue, float.MaxValue, float.MaxValue);
            Vector3 maxPoint = new Vector3(float.MinValue, float.MinValue, float.MinValue);

            GameObject replica = keyPrefab == null ? baseKeyGO : keyPrefab;

            Vector3 keyOffset = keySize;
            if(isUI)
            {
                RectTransform k_transform = replica.GetComponent<RectTransform>();
                keyOffset = new Vector3(keySize.x * k_transform.rect.width, keySize.y * k_transform.rect.height, keySize.z);
            }
            float orderModifier = -1f;
            if(!bottomLeftOrder)
            {
                orderModifier = 1f;
            }
            Vector3 v = new Vector3(- orderModifier * (keyOffset.x * (0.5f * keys_per_row) - keyOffset.x * .5f), orderModifier * (num_of_rows - 1) * keyOffset.y / 2f, -.5f);

            numKeys = chars.Count;
            for (int i = 0; i < chars.Count; i++)
            {

                // if keyPrefab is set, then we'll use keyPrefab otherwise it will be based on the pressed key
                // CAUTION: you should not mix a UI prefab with a 3d keyboard, it will not yield the intended results
                GameObject k = Instantiate(replica, this.transform) as GameObject;
                
                k.name = chars[i];

                //string basekeychar = OSK_Keymap.BaseCharacter(chars[i]);
                //OSK_KeyCode code = (OSK_KeyCode)1000 + ((int)(basekeychar[0] - 'a') + 1) * 100 + ;

                if (isUI)
                { 
                    OSK_UI_Key thisKey = k.GetComponent<OSK_UI_Key>();
                    
                    thisKey.x_size = 1f;
                    thisKey.BackScale(keyOffset);

                    thisKey.Assign(OSK_KeyCode._MINIKEYBOARD_, OSK_KEY_TYPES.CONTROLS, shiftup ? chars[i].ToUpper() : chars[i]);                   

                    thisKey.AssignSpecialAction(callbackAction);
                    thisKey.isPressed = false;  // reset since we could be cloning a live key
                    thisKey.isLongPress = false;

                    keyLayout[l].Add(thisKey);

                }
                else
                {
                    OSK_Key thisKey = k.GetComponent<OSK_Key>();


                    thisKey.BackScale(keySize);

                    thisKey.Assign(OSK_KeyCode._MINIKEYBOARD_, OSK_KEY_TYPES.CONTROLS, shiftup ? chars[i].ToUpper() : chars[i]);

                    

                    thisKey.AssignSpecialAction(callbackAction);
                    thisKey.isPressed = false;  // reset since we could be cloning a live key
                    thisKey.isLongPress = false;

                    keyLayout[l].Add(thisKey);
                }
                

                k.transform.localPosition = v;


                try
                {
                    BoxCollider collider = k.GetComponent<BoxCollider>();
                    minPoint = Vector3.Min(minPoint, collider.bounds.min);
                    maxPoint = Vector3.Max(maxPoint, collider.bounds.max);
                }
                catch (Exception e)
                {
                    Debug.LogError("Key " + k.name + " does not have a collider and may not function properly\n" + e.Message);

                }

                v.x = v.x + orderModifier * keyOffset.x;
                // manage the setting of position by row.
                j++;
                if (j >= keys_per_row && i < chars.Count - 1)
                {
                    v.y = v.y + -orderModifier * keyOffset.y;
                    v.x = -orderModifier * (keyOffset.x * (0.5f * keys_per_row) - keyOffset.x * .5f);
                    j = 0;
                    l++;
                    //col++;
                    keyLayout.Add(new List<I_OSK_Key>());
                }
            }

            center = new Vector3((maxPoint.x - minPoint.x) / 2f, -(maxPoint.y - minPoint.y) / 2f, 0f);
            size = new Vector3(keyOffset.x * keyLayout.Max(t => t.Count), keyOffset.y * keyLayout.Count, keyOffset.z);

            ResizeBackground();

            selectedKey = keyLayout[0][0];  // set the selected key to first key
            selectedKeyLoc = Vector2Int.zero;

            if(isUI && isJoystickSelection)
            {
                EventSystem.current.SetSelectedGameObject(selectedKey.GetGameObject());
            }

            isActive = true;
            // have delay before the new input is taken into account
            inputTimer = -0.5f;
        }

        void CreateBackground()
        {
            if(backgroundImg != null && backgroundObj == null)
            {
                // Instantiate an empty GameObject
                backgroundObj = new GameObject("BG");
                backgroundObj.transform.SetParent(this.transform);

                if (isUI)
                {
                    RectTransform rectTransform = backgroundObj.AddComponent<RectTransform>();


                    rectTransform.anchoredPosition = Vector2.zero;
                    rectTransform.localPosition = new Vector3(0f, 0f, .5f);

                    // Add the Image component
                    Image image = backgroundObj.AddComponent<Image>();

                    image.sprite = backgroundImg;
                    backgroundObj.transform.localPosition = new Vector3(0f, 0f, .5f);
                }
                else
                {
                    SpriteRenderer spriteRenderer = backgroundObj.AddComponent<SpriteRenderer>();
                    spriteRenderer.drawMode = SpriteDrawMode.Sliced;
                    spriteRenderer.sprite = backgroundImg;

                    backgroundObj.transform.localPosition = new Vector3(0f,0f,.5f);
                    
                }

                backgroundObj.AddComponent<BoxCollider>();
            }
        }

        void ResizeBackground()
        {
            if (backgroundObj == null)
            {
                CreateBackground();
            }

            if (backgroundObj != null)
            { 
                if (isUI)
                {
                    backgroundObj.GetComponent<Image>().rectTransform.sizeDelta = new Vector2(size.x * 1.05f, size.y * 1.05f);
                    backgroundObj.transform.localPosition = new Vector3(0f, 0f, .5f);
                    backgroundObj.GetComponent<BoxCollider>().size = new Vector3(size.x, size.y, 0.1f);
                } else
                {
                    // adds 5% margin on ends
                    backgroundObj.GetComponent<SpriteRenderer>().size = new Vector2(size.x*1.05f, size.y*1.05f);
                    backgroundObj.transform.localPosition = new Vector3(0f, 0f, .5f);
                    backgroundObj.GetComponent<BoxCollider>().size = new Vector3(size.x, size.y, 0.1f);

                }

                backgroundObj.SetActive(true);
            }
        }

        public void SelectedFirstKey()
        {
            if (keyLayout.Count == 0)
                return;

            selectedKeyLoc = Vector2Int.zero;
            selectedKey = keyLayout[selectedKeyLoc.y][selectedKeyLoc.x];
            if (!isUI)
            {
                selectedKey.Highlight(true, highlighterColor);
            } else
            {
                EventSystem.current.SetSelectedGameObject(selectedKey.GetGameObject());
            }
        }

        void SelectedKeyMove(Vector2 dir)
        {
            Vector2Int from = selectedKeyLoc;
            I_OSK_Key k = keyLayout[selectedKeyLoc.y][selectedKeyLoc.x];
            k.Highlight(false, Color.white);
            dir.x = Mathf.RoundToInt(dir.x);
            dir.y = Mathf.RoundToInt(dir.y);
            // movement along y-axis
            if (dir.y != 0f)
            {

                selectedKeyLoc.y = selectedKeyLoc.y + (int)dir.y;
                if (selectedKeyLoc.y < 0)
                {
                    selectedKeyLoc.y = 0;

                }
                else if (selectedKeyLoc.y >= keyLayout.Count)
                {
                    selectedKeyLoc.y = keyLayout.Count - 1;

                }

            }

            // movement along x-axis
            if (dir.x != 0f)
            {
                int x = selectedKeyLoc.x;


                x += (int)dir.x;
                if (x < 0)
                {
                    x = 0;
                    
                }
                else if (x >= keyLayout[selectedKeyLoc.y].Count)
                {

                    x = keyLayout[selectedKeyLoc.y].Count - 1;

                    
                }


                selectedKeyLoc.x = x;
            }

            try
            {

                selectedKeyLoc.Clamp(new Vector2Int(0, 0), new Vector2Int(keyLayout[selectedKeyLoc.y].Count - 1, keyLayout.Count - 1));
                keyLayout[selectedKeyLoc.y][selectedKeyLoc.x].Highlight(true, highlighterColor);
                selectedKey = keyLayout[selectedKeyLoc.y][selectedKeyLoc.x];
                /*if (k != keyLayout[selectedKeyLoc.y][selectedKeyLoc.x])
                    SelectSound();*/

            }
            catch (Exception e)
            {
                Debug.LogError("Exemption in moving through OSK layout DpadSel=" + selectedKeyLoc.ToString() + " x=" + (int)selectedKeyLoc.x + " y=" + (int)selectedKeyLoc.y + "\ne=" + e.Message);
            }
        }

        /// <summary>
        /// Method uses raycasting instead of OnMouseDown/Up callbacks -> needed for Unity's new inputsystem if you don't bind
        /// It can be further refined by allocating a layer to all keys anc only raycasting against that layer
        /// it was not done here so it does not interfere with existing user layers
        /// </summary>
        protected void InputFromPointerDevice()
        {
            Vector2 pointPos = viperInput.GetPointerPos();

            if (viperInput.PointerDown())
            {

                Ray ray = Camera.main.ScreenPointToRay(pointPos);

                RaycastHit hit;


                if (Physics.Raycast(ray, out hit))
                {

                    I_OSK_Key key = hit.transform.gameObject.GetComponent<I_OSK_Key>();
                    if (key != null && key.GetKeyCode() == OSK_KeyCode._MINIKEYBOARD_ )
                    {
                        key.Click("pointer");
                    }

                }
            }


        }

        // Update is called once per frame
        void Update()
        {
            if(isActive && keyLayout.Count > 0)
            {

#if ENABLE_INPUT_SYSTEM
                InputFromPointerDevice();
#endif

                // Collects gamepad or joystick input and highlights the appropriate key by calling DpadMove
                if (acceptGamePadInput)
                {
                    inputTimer += Time.unscaledDeltaTime;


                    if(isUI && isJoystickSelection)
                    {
                        OSK_UI_Key ui_key;
                        if (EventSystem.current.currentSelectedGameObject != null && EventSystem.current.currentSelectedGameObject.TryGetComponent<OSK_UI_Key>(out ui_key))
                        {
                            selectedKey = ui_key;

                        }
                        
                    }
                    
                    if (viperInput.AButtonUp() && AbtnDown)
                    {
                        selectedKey.Click("joystick");
                        AbtnDown = false;
                    }

                    // inputTimer dampens the input from the joystick or gamepad so that the user highlight doesn't move too quickly.
                    // the "inputTimerThreshold" can be changed in the Inspector to make it slower or faster, this can also be affected by the Input Manager/System sensitivity to these input devices
                    if (inputTimer > inputTimerThreshold)
                    {
                        inputTimer = 0f;


                        if (viperInput.AButtonDown() && !AbtnDown)
                        {
                            AbtnDown = true;
                        }
                        else
                        {
                            AbtnDown = false;
                        }

                        if (!isUI)
                        {
                            // up/down
                            if (viperInput.GetAxis(AXIS_INPUT.DPAD_Y) != 0f)
                            {
                                SelectedKeyMove(Vector2.up * viperInput.GetAxis(AXIS_INPUT.DPAD_Y));

                                viperInput.ResetAllAxis();
                            }

                            else if (viperInput.GetAxis(AXIS_INPUT.LEFTSTICK_Y) != 0f)
                            {
                                SelectedKeyMove(Vector2.up * viperInput.GetAxis(AXIS_INPUT.LEFTSTICK_Y));

                                viperInput.ResetAllAxis();
                            }

                            // left/right
                            else if (viperInput.GetAxis(AXIS_INPUT.DPAD_X) != 0f)
                            {
                                SelectedKeyMove(Vector2.left * viperInput.GetAxis(AXIS_INPUT.DPAD_X));

                                viperInput.ResetAllAxis();
                            }

                            else if (viperInput.GetAxis(AXIS_INPUT.LEFTSTICK_X) != 0f)
                            {
                                SelectedKeyMove(Vector2.left * viperInput.GetAxis(AXIS_INPUT.LEFTSTICK_X));

                                viperInput.ResetAllAxis();
                            }
                        }
                    }

                }

            }
        }
    }
}
