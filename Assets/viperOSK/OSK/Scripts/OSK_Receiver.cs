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
using viperTools;

namespace viperOSK
{

    /// <summary>
    ///  OSK_Receiver should be attached to an object that is supposed to be receiving text
    /// </summary>
    public class OSK_Receiver : MonoBehaviour
    {

        // text being captured from the OSK_Keyboard - this allows the TMP_Text to display custom text (for example, '*'s for a password input field)
        protected string text = "";

        // limit of accepted characters
        public int textLimit = 20;

        // pointer to TextMeshPro text object that will be receiving text
        public TMP_Text textReceiver;

        [HideInInspector]
        // index of cursor in the string
        public int cursorIndex;

        // the start and end of a text selection (x= start [left-most char] and y= end [right-most char])
        protected Vector2Int cursorSel = new Vector2Int(-1,-1);

        // pointer to graphical cursor OSK_Cursor object
        public I_OSK_Cursor cursor;
       
        // whether the field is open for interaction
        public bool interactable;

        // whether the text can be selected by dragging touch/pointer over it
        public bool allowTextSelection;

        public Color32 normalColor;
        public Color32 highlightColor;

        // masks characters if not empty. This just the display character, the real text is stored in 'text'
        public string charMask = "";

        // toggles whether the charMask is used or not. this allows you to show or hide the pwd mask
        public bool useCharMask;

#if UNITY_2019

        // Ensure you reference this event to a "public void mySubmit(string)" style function in the inspector.
        // Ensure you choose the version of your function under "Dynamic String" in the inspector
        [SerializeField]
        public UE2019_Rec OnSubmit;// = new Action<string>();

        // (v3.4 backwards compatibility to Unity 2019.x) Allows developers to add events when the value of the input has changed
        [SerializeField]
        public UE2019_Rec OnValueChanged;// = new UnityEvent<string>();

        // (v3.4 backwards compatibility to Unity 2019.x)  Allows developers to setup events when the Receiver is on Focus
        [SerializeField]
        public UE2019_Rec onFocus;// = new UnityEvent<string>();

        // (v3.4 backwards compatibility to Unity 2019.x) Allows developers to setup events when the Receiver when Focus is lost
        [SerializeField]
        public UE2019_Rec onLostFocus;// = new UnityEvent<string>();
#else


        // Ensure you reference this event to a "public void mySubmit(string)" style function in the inspector.
        // Ensure you choose the version of your function under "Dynamic String" in the inspector
        [SerializeField]
        public UnityEvent<string> OnSubmit = new UnityEvent<string>();

        // (v3.0) Allows developers to add events when the value of the input has changed
        [SerializeField]
        public UnityEvent<string> OnValueChanged = new UnityEvent<string>();

        // (v3.0) Allows developers to setup events when the Receiver is on Focus
        [SerializeField]
        public UnityEvent<string> onFocus = new UnityEvent<string>();

        // (v3.0) Allows developers to setup events when the Receiver when Focus is lost
        [SerializeField]
        public UnityEvent<string> onLostFocus = new UnityEvent<string>();
#endif

        // whether this receiver has focus
        // (v3.5) made into a protected property
        protected bool hasFocus;

        // Start is called before the first frame update
        void Awake()
        {
            text = "";

            if (this.gameObject.TryGetComponent<TMP_Text>(out textReceiver))
            {
                textReceiver.color = normalColor;
                cursor = this.transform.GetComponentInChildren<I_OSK_Cursor>();
                if (cursor == null)
                    Debug.LogWarning("This TMPro text object does not have a cursor. If you want full edit functionality add a OSK_Cursor prefab as a child");

                OnFocusLost();

            }
            else
            {
                Debug.LogWarning("OSK_Receiver must be in the same gameobject as the TextMeshPro text object receiving text input");
            }


        }

        /// <summary>
        /// sets the use of the char mask (password) to whether the charmask field is filled with something
        /// </summary>
        private void Start()
        {
            useCharMask = charMask.Length > 0;
        }

        private void LateUpdate()
        {
            //this.gameObject.SetActive(false);
            //textReceiver.SetText(text, true);
            //this.gameObject.SetActive(true);

        }

#region UnityEngine.EventSystems Pointer Handlers

        /// <summary>
        /// PointerDown handle. Used solely for the TextMeshPro receivers
        /// </summary>
        public void OnMouseDown()
        {
            if (!interactable)
                return;
            
            if (cursor != null && allowTextSelection)
            {

                Deselect();
                cursorSel.x = Selection(viperInput.GetPointerPos());
                cursorSel.x = Mathf.Min(cursorSel.x, textReceiver.text.Length - 1);
            }

        }

        /// <summary>
        /// PointerUp handle. Used solely for the TextMeshPro receivers
        /// </summary>
        public void OnMouseUp()
        {
            
            if (!hasFocus)
            {

                OSK_Keyboard keyboard = FindObjectOfType<OSK_Keyboard>();
                if (keyboard != null && keyboard.output != this)
                {
                    
                    keyboard.SetOutput(this);
                }

                OnFocus();
            }

            if (!interactable)
                return;

            if (cursor != null)
            {
                // check whether the pointer was down before (dragging on text)
                if (cursorSel.x >= 0 && allowTextSelection)
                {
                    cursorSel.y = Selection(viperInput.GetPointerPos(), true);
                    if(cursorSel.y < 0)
                        cursorSel.y = Selection(viperInput.GetPointerPos());
                    cursorIndex = cursorSel.y + (cursorSel.x < cursorSel.y ? 1 : 0);
                    cursorSel.y = Mathf.Min(cursorSel.y, textReceiver.text.Length - 1);



                    // counts as a click as the pointer did not drag over to other characters
                    if (cursorSel.x == cursorSel.y)
                    {
                        cursorSel.x = cursorSel.y = -1;
                    }
                    else
                    {
                        // mark the selection so that cursorSel.x is the leftmost char and cursorSel.y is the rightmost char
                        int temp = cursorSel.x;
                        cursorSel.x = Mathf.Min(cursorSel.x, cursorSel.y);
                        cursorSel.y = Mathf.Max(temp, cursorSel.y);

                        SelectionHighlight(highlightColor);
                    }

                }
                else
                {
                    // handle as if it was a pointer click
                    Deselect();
                    cursorIndex = Selection(viperInput.GetPointerPos());

                }
            }

        }

#endregion

        public virtual int selection { get { return (cursorSel.x >= 0 && cursorSel.y >= 0 ? cursorSel.y - cursorSel.x : 0); } }


        public virtual int Selection(Vector3 hitpoint, bool charhit = false)
        {
            Camera cam = Camera.main;

            if (charhit)
                return TMP_TextUtilities.FindIntersectingCharacter(textReceiver, hitpoint, cam, false);

            return TMP_TextUtilities.GetCursorIndexFromPosition(textReceiver, hitpoint, cam);
        }

        public virtual void Deselect()
        {

            textReceiver.text = charMask.Length > 0 && useCharMask ? new string(charMask[0], text.Length) : text;
            SelectionHighlight(normalColor, true);
            cursorSel.x = cursorSel.y = -1;
        }

        public virtual void SelectionHighlight(Color32 c, bool all = false)
        {
            if (selection == 0)
                return;

            TMP_TextInfo textInfo = textReceiver.textInfo;
            int start = cursorSel.x;
            int end = cursorSel.y;
            if (all)
            {
                start = 0;
                end = textReceiver.text.Length - 1;
            }
         
            
            for (int i = start; i <= end; i++)
            {

                // Get the index of the material / sub text object used by this character.
                int meshIndex = textReceiver.textInfo.characterInfo[i].materialReferenceIndex;

                // Get the index of the first vertex of this character.
                int vertexIndex = textReceiver.textInfo.characterInfo[i].vertexIndex;

                // Get a reference to the vertex color
                Color32[] vertexColors = textReceiver.textInfo.meshInfo[meshIndex].colors32;

                
                vertexColors[vertexIndex + 0] = c;
                vertexColors[vertexIndex + 1] = c;
                vertexColors[vertexIndex + 2] = c;
                vertexColors[vertexIndex + 3] = c;

            }
      


        textReceiver.UpdateVertexData(TMP_VertexDataUpdateFlags.All);

        }

#region Receiver virtual routines

        /// <summary>
        /// submits the text to the callback
        /// </summary>
        public virtual void Submit()
        {
            if (OnSubmit != null) OnSubmit.Invoke(text);
        }

        /// <summary>
        /// sends the text (string) to the UnityEvent function that's stored
        /// </summary>
        public virtual void ValueChanged()
        {

            if (OnValueChanged != null) OnValueChanged.Invoke(text);
        }

        /// <summary>
        /// repopulates the text and sets the cursor to the end
        /// </summary>
        /// <param name="newText">new text</param>
        public virtual void SetText(string newText)
        {
            // constraint the text to the receiver's limit
            newText = newText.Substring(0, Mathf.Min(textLimit, newText.Length));

            text = newText;
            if(charMask.Length > 0 && useCharMask)
            {
                textReceiver.text = new string(charMask[0], text.Length);
            } else
            {
                textReceiver.text = newText;
            }

            cursorIndex = text.Length;

            ValueChanged();
        }

        /// <summary>
        /// handles changes to text
        /// </summary>
        /// <param name="text">new text</param>
        /// <param name="cursor">cursor when it is blinking</param>
        public virtual void AddText(string newchar)
        {
            // this fix is required as TMPro has an issue with a string ending in new line. The new line does not show unless there's a hiden character.
            // this line removes the hidden character
            NewLineFix();

            if(newchar == "\x0008")
            {
                Backspace();
                ValueChanged();
                return;
            }

            if (newchar == "\x007F")
            {
                Del();
                ValueChanged();
                return;
            }

            if (newchar == "\x000A")
            {
                NewLine();
                return;
            }

            newchar = newchar.Substring(0, 1);
            string displayChar = charMask.Length > 0 && useCharMask? charMask.Substring(0,1) : newchar;

            if (selection > 0)
            {
                textReceiver.text = textReceiver.text.Remove(cursorSel.x, selection+1);
                textReceiver.text = textReceiver.text.Insert(cursorSel.x, displayChar);

                text = text.Remove(cursorSel.x, selection);
                text = text.Insert(cursorSel.x, newchar);

                cursorIndex = cursorSel.x + 1;
                cursorSel.x = cursorSel.y = -1;

            }
            else if ((text.Length == 0 || cursorIndex >= text.Length) && textReceiver.text.Length < textLimit)
            {
                textReceiver.text += displayChar;

                text += newchar;

                cursorIndex++;

            }
            else if (textReceiver.text.Length < textLimit)
            {

                textReceiver.text = textReceiver.text.Insert(cursorIndex, displayChar);
                text = text.Insert(cursorIndex, newchar);
                
                cursorIndex++;
            }

            ValueChanged();
        }

        /// <summary>
        /// adds a new line (\n) to the text, only useful when using multi-line text box
        /// </summary>
        public virtual void NewLine()
        {
            
            if (selection > 0)
            {
                textReceiver.text = textReceiver.text.Remove(cursorSel.x, selection + 1);
                textReceiver.text = textReceiver.text.Insert(cursorSel.x, "\n");

                text = text.Remove(cursorSel.x, selection);
                text = text.Insert(cursorSel.x, "\n");

                cursorIndex = cursorSel.x + 1;
                cursorSel.x = cursorSel.y = -1;

            }
            else if ((text.Length == 0 || cursorIndex >= text.Length) && textReceiver.text.Length < textLimit)
            {
                // this fix is to trick TMPro to update the position of the text after a carriage return has been entered
                textReceiver.text += "\n\u00A0";
                textReceiver.ForceMeshUpdate();

                text += '\n';

                

                cursorIndex++;

            }
            else if (textReceiver.text.Length < textLimit)
            {

                textReceiver.text = textReceiver.text.Insert(cursorIndex, "\n");
                text = text.Insert(cursorIndex, "\n");

                cursorIndex++;
            }

            //textReceiver.ForceMeshUpdate(true);
            

            cursor.Cursor();
            ValueChanged();
        }

        void NewLineFix()
        {
            if (textReceiver.text.EndsWith("\u00A0"))
            {
                textReceiver.text = textReceiver.text.Remove(textReceiver.text.Length - 1);
                //textReceiver.ForceMeshUpdate();
            }
        }

        /// <summary>
        /// text based on what's stored in the 'text' variable in OSK_Receiver
        /// </summary>
        /// <returns></returns>
        public virtual string Text()
        {

            return text == null ? "" : text;
        }

        /// <summary>
        /// provides text without Rich-Text format identifiers of the TMP_Text component
        /// </summary>
        /// <returns>text without RTF formatting</returns>
        public virtual string ParsedText()
        {
            return textReceiver.GetParsedText();
        }

        /// <summary>
        /// called when the input receiver is focused
        /// </summary>
        public virtual void OnFocus()
        {
            hasFocus = true;

            if(onFocus != null) onFocus.Invoke(Text());

            if(textReceiver != null && text !=null)
            {
                if (text.Length == 0 && textReceiver.text.Length > 0)
                {
                    text = textReceiver.text;
                    cursorIndex = text.Length;
                }
            }

            // prepopulate if the user had something in the TMP text field


            if (cursor != null)
                cursor.Show(true);
        }

        /// <summary>
        /// toggles whether a password field's content is shown or hidden
        /// </summary>
        public virtual void ToggleCharMask()
        {
            ToggleCharMask(!useCharMask);
        }

        /// <summary>
        /// toggles whether a password field's content is shown or hidden
        /// </summary>
        public virtual void ToggleCharMask(bool on_off_charmask)
        {
            useCharMask = on_off_charmask;
            if(useCharMask)
            {
                textReceiver.text = new string(charMask[0], text.Length);
            } else
            {
                textReceiver.text = text;
            }
        }

        /// <summary>
        /// 
        /// </summary>
        /// <returns>returns whether the receiver is in focus</returns>
        public virtual bool isFocused()
        {
            return hasFocus;
        }

        /// <summary>
        /// called when the input receiver loses focus
        /// </summary>
        public virtual void OnFocusLost()
        {
            hasFocus = false;

             if (onLostFocus != null)
                onLostFocus.Invoke(Text());

            Deselect();

            if (cursor != null)
                cursor.Show(false);
        }

        

        /// <summary>
        /// does a backspace based on where the cursor is
        /// </summary>
        public virtual void Backspace()
        {


            if (selection > 0)
            {
                textReceiver.text = textReceiver.text.Remove(cursorSel.x, selection + 1);
                text = text.Remove(cursorSel.x, selection + 1);
                cursorIndex = cursorSel.x;
                cursorSel.x = cursorSel.y = -1;
            }
            else if (cursorIndex > 0)
            {
                textReceiver.text = textReceiver.text.Remove(Mathf.Min(cursorIndex - 1, textReceiver.text.Length - 1), 1);
                text = text.Remove(Mathf.Min(cursorIndex - 1, text.Length - 1), 1);
                cursorIndex--;
            }


        }

        /// <summary>
        /// does a delete based on where the cursor is in the text
        /// </summary>
        public virtual void Del()
        {

            if (selection > 0)
            {
                textReceiver.text = textReceiver.text.Remove(cursorSel.x, selection + 1);
                text = text.Remove(cursorSel.x, selection + 1);
                cursorIndex = cursorSel.x;
                cursorSel.x = cursorSel.y = -1;
            }
            else if (cursorIndex <= textReceiver.text.Length - 1)
            {
                textReceiver.text = textReceiver.text.Remove(cursorIndex, 1);
                text = text.Remove(cursorIndex, 1);
            }

        }

        /// <summary>
        /// clears the input field
        /// </summary>
        public virtual void ClearText()
        {

            textReceiver.text = "";
            text = "";

            ValueChanged();
        }

#endregion
    }
}