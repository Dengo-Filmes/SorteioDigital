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
using viperTools;

namespace viperOSK
{
    /// <summary>
    /// this component should be attached to the object that will be receiving the OSK keyboard input
    /// it will automatically detect whether the object is a TextMeshPro or a UI InputField
    /// 
    /// NB: the OSK_UI_InputReceiver does not use the inherited "text" field, it rather relies on the built-in input field text properties
    /// </summary>
    public class OSK_UI_InputReceiver : OSK_Receiver, ISubmitHandler, IPointerClickHandler, IDragHandler
    {
        public enum OSK_RECEIVER
        {
            NONE,
            INPUTFIELD,
            TMPRO_INPUTFIELD,

        }

        InputField inputReceiver;
        TMP_InputField inputTMPReceiver;

        OSK_RECEIVER receiver = OSK_RECEIVER.NONE;

        // events to be called when the Receiver is highlighted and then Clicked on or the gamepad Submit event is triggered
        public UnityEvent onSelectClick;

        private void Awake()
        {
 
        }

        // Start is called before the first frame update
        void Start()
        {

            if (TryGetComponent<InputField>(out inputReceiver))
            {

                // suppress the activation of the native keyboard on the device so input is from viperOSK
                inputReceiver.keyboardType = (TouchScreenKeyboardType)(-1);
                
                receiver = OSK_RECEIVER.INPUTFIELD;
            }
            else if (TryGetComponent<TMP_InputField>(out inputTMPReceiver))
            {

                // suppress the activation of the native keyboard on the device so input is from viperOSK
                inputTMPReceiver.keyboardType = (TouchScreenKeyboardType)(-1);
                //workaround fix to address issues with physical keyboard
                // inputTMPReceiver.interactable = false;
                // Invoke("TMPInputFieldReActivate", .5f);

                textReceiver = inputTMPReceiver.textComponent;

                receiver = OSK_RECEIVER.TMPRO_INPUTFIELD;
            }
            else
            {
                Debug.LogError("viperOSK does not have a valid text receiver. Ensure you have a valid receiver (or create one) and attach this component to the GameObject");
            }

            if (charMask.Length > 0)
                useCharMask = true;

            // (v3.5) reinforce potential use of OSK Cursor with Unity's UI
            if(cursor == null)
                cursor = this.transform.GetComponentInChildren<I_OSK_Cursor>();

        }

        void TMPInputFieldReActivate()
        {
            inputTMPReceiver.interactable = true;
        }


        public int SelectionEnd()
        {
            int start, end, selection;
            switch (receiver)
            {

                case OSK_RECEIVER.INPUTFIELD:
                    start = Mathf.Min(inputReceiver.caretPosition, inputReceiver.selectionAnchorPosition);
                    end = Mathf.Max(inputReceiver.caretPosition, inputReceiver.selectionAnchorPosition);
                    selection = Mathf.Abs(end - start);

                    if (selection > 0)
                        return end;
                    else
                        return -1;

                case OSK_RECEIVER.TMPRO_INPUTFIELD:
                    start = Mathf.Min(inputTMPReceiver.caretPosition, inputTMPReceiver.selectionAnchorPosition);
                    end = Mathf.Max(inputTMPReceiver.caretPosition, inputTMPReceiver.selectionAnchorPosition);
                    selection = Mathf.Abs(end - start);

                    if (selection > 0)
                        return end;
                    else
                        return -1;

                default:
                    return -1;

            }


        }

        /// <summary>
        /// submits the text to the callback
        /// for Unity's UI Input Field, it's best to use the input field's own text output rather than our OSK_Receiver implementation
        /// </summary>
        public override void Submit()
        {
            OnSubmit.Invoke(Text());
        }

        /// <summary>
        /// handles changes to text
        /// </summary>
        /// <param name="text">new text</param>
        /// <param name="cursor">cursor when it is blinking</param>
        public override void AddText(string newchar)
        {
            if (newchar == "\x0008")
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
                ValueChanged();
                return;
            }

            string displaychar = charMask.Length > 0 && useCharMask ? charMask.Substring(0, 1) : newchar;

            switch (receiver)
            {


                case OSK_RECEIVER.INPUTFIELD:
                    // this avoid writing over the placeholder text when no input is being received
                    if (newchar.Length > 0 && inputReceiver.text.Length < textLimit)
                    {
                        int caret_ = (inputReceiver.caretPosition != cursorIndex ? cursorIndex : inputReceiver.caretPosition);
                        int start = cursorSel.x;
                        int end = cursorSel.y; 
                        int sel = Mathf.Abs(end - start);


                        if (sel > 0)
                        {
                            inputReceiver.text = inputReceiver.text.Remove(start, sel);
                            inputReceiver.text = inputReceiver.text.Insert(start, displaychar);
                            inputReceiver.caretPosition = start + 1;

                            text = text.Remove(start, sel);
                            text = text.Insert(start, newchar);

                        }
                        else if (caret_ < text.Length && caret_ >= 0)
                        {

                            text = text.Insert(caret_, newchar);
                            if (useCharMask)
                                inputReceiver.text = new string(displaychar[0], text.Length);
                            else
                                inputReceiver.text = text;

                            

                            inputReceiver.caretPosition = caret_ + 1;
                        }
                        else
                        {
                            inputReceiver.text += displaychar;
                            inputReceiver.caretPosition = inputReceiver.text.Length + 1;

                            text += newchar;
                        }

                        cursorIndex = inputReceiver.caretPosition;
                        cursorSel = -1 * Vector2Int.one;


                    }
                    break;
                case OSK_RECEIVER.TMPRO_INPUTFIELD:
                    // this avoid writing over the placeholder text when no input is being received
                    if (newchar.Length > 0 && inputTMPReceiver.text.Length < textLimit && (inputTMPReceiver.lineLimit == 0 || inputTMPReceiver.textComponent.textInfo.lineCount <= inputTMPReceiver.lineLimit))
                    {
                        int start = Mathf.Min(inputTMPReceiver.caretPosition, inputTMPReceiver.selectionAnchorPosition);
                        int end = Mathf.Max(inputTMPReceiver.caretPosition, inputTMPReceiver.selectionAnchorPosition);
                        int sel = Mathf.Abs(end - start);
                        if (sel > 0)
                        {
                           // inputTMPReceiver.text = inputTMPReceiver.text.Remove(start, sel);
                           // inputTMPReceiver.text = inputTMPReceiver.text.Insert(start, displaychar);
                            //inputTMPReceiver.Select();
                            //inputTMPReceiver.caretPosition = start + 1;// inputTMPReceiver.text.Length;
                                                                       //inputTMPReceiver.ActivateInputField();

                            text = text.Remove(start, sel);
                            text = text.Insert(start, newchar);

                            if (useCharMask) 
                                inputTMPReceiver.text = new string(displaychar[0], text.Length);
                            else
                                inputTMPReceiver.text = text;
                            inputTMPReceiver.caretPosition = start + 1;

                        } else if(inputTMPReceiver.caretPosition < text.Length && inputTMPReceiver.caretPosition >= 0)
                        {
                            int caret = inputTMPReceiver.caretPosition;
                            text = text.Insert(inputTMPReceiver.caretPosition, newchar);
                            if (useCharMask)
                                inputTMPReceiver.text = new string(displaychar[0], text.Length);
                            else
                                inputTMPReceiver.text = text;

                            inputTMPReceiver.ForceLabelUpdate();

                            inputTMPReceiver.caretPosition = caret + 1;
                            


                        } else
                        {
                            //inputTMPReceiver.text += displaychar;
                            //inputTMPReceiver.caretPosition = inputTMPReceiver.text.Length + 1;
                            //inputTMPReceiver.text = inputTMPReceiver.text.Insert(inputTMPReceiver.caretPosition, newchar);
                            //inputTMPReceiver.Select();
                            //inputTMPReceiver.caretPosition += 1;// inputTMPReceiver.text.Length;
                            //inputTMPReceiver.ActivateInputField();

                            text += newchar;
                            if (useCharMask)
                                inputTMPReceiver.text = new string(displaychar[0], text.Length);
                            else
                                inputTMPReceiver.text = text;

                            inputTMPReceiver.caretPosition = text.Length;
                            //inputTMPReceiver.caretPosition = inputTMPReceiver.text.Length + 1;
                        }

                    }
                    cursorIndex = inputTMPReceiver.caretPosition;
                    break;
            }



            ValueChanged();
        }
     
        public override string Text ()
        {
            return text == null ? "" : text;
        }

        /// <summary>
        /// provides text without Rich-Text format identifiers
        /// </summary>
        /// <returns>text without RTF formatting</returns>
        public override string ParsedText()
        {
            switch (receiver)
            {

                case OSK_RECEIVER.INPUTFIELD:
                    return inputReceiver.text;

                case OSK_RECEIVER.TMPRO_INPUTFIELD:
                    return inputTMPReceiver.text;

                default:
                    return "";

            }
        }

        /// <summary>
        /// toggles whether a password field's content is shown or hidden
        /// </summary>
        public override void ToggleCharMask(bool on_off_charmask)
        {
            useCharMask = on_off_charmask;
            switch (receiver)
            {

                case OSK_RECEIVER.INPUTFIELD:
                    if (useCharMask)
                    {
                        inputReceiver.text = new string(charMask[0], text.Length); ;
                    } else
                    {
                        inputReceiver.text = text;
                    }
                    break;

                case OSK_RECEIVER.TMPRO_INPUTFIELD:
                    if (useCharMask)
                    {
                        inputTMPReceiver.text = new string(charMask[0], text.Length); ;
                    }
                    else
                    {
                        inputTMPReceiver.text = text;
                    }
                    break;


            }
        }

        public override void OnFocus()
        {
            // (v3.5) implemented custom routines that run on gained focus
            hasFocus = true;

            if (onFocus != null) onFocus.Invoke(Text());

            switch (receiver)
            {
                case OSK_RECEIVER.INPUTFIELD:
                    // sets the cursor to the end of the line
                    //inputReceiver.caretPosition = inputReceiver.text.Length;
                    
                    break;
                case OSK_RECEIVER.TMPRO_INPUTFIELD:
                    // sets the cursor to the end of the line
                    //inputTMPReceiver.caretPosition = inputTMPReceiver.text.Length;
                    break;
            }

            if (cursor != null)
                cursor.Show(true);
        }

        public override void OnFocusLost()
        {
            // (v3.5) implemented custom routines that run on lost focus
            hasFocus = false;

            if (onLostFocus != null)
                onLostFocus.Invoke(Text());

            if (cursor != null)
                cursor.Show(false);
        }

        /// <summary>
        /// adds a new line (\n) to the text, only useful when using multi-line text box
        /// </summary>
        public override void NewLine()
        {

            switch (receiver)
            {


                case OSK_RECEIVER.INPUTFIELD:
                    // this avoid writing over the placeholder text when no input is being received
                    if (inputReceiver.text.Length < textLimit && inputTMPReceiver.textComponent.textInfo.lineCount < inputTMPReceiver.lineLimit)
                    {
                        int caret_ = (inputReceiver.caretPosition != cursorIndex ? cursorIndex : inputReceiver.caretPosition);
                        int start = cursorSel.x;
                        int end = cursorSel.y;
                        int sel = Mathf.Abs(end - start);

                        if (sel > 0)
                        {
                            inputReceiver.text = inputReceiver.text.Remove(start, sel);
                            inputReceiver.text = inputReceiver.text.Insert(start, "\n");
                            inputReceiver.caretPosition = start + 1;

                            text = text.Remove(start, sel);
                            text = text.Insert(start, "\n");

                        }
                        else if (caret_ < text.Length && caret_ >= 0)
                        {

                            text = text.Insert(caret_, "\n");

                            inputReceiver.caretPosition = caret_ + 1;

                        }
                        else
                        {
                            inputReceiver.text += "\n";
                            inputReceiver.caretPosition = inputReceiver.text.Length + 1;

                            text += "\n";
                        }


                        cursorIndex = inputReceiver.caretPosition;
                        cursorSel = -1 * Vector2Int.one;

                    }
                    break;
                case OSK_RECEIVER.TMPRO_INPUTFIELD:
                    // this avoid writing over the placeholder text when no input is being received
                    if (inputTMPReceiver.text.Length < textLimit && inputTMPReceiver.textComponent.textInfo.lineCount < inputTMPReceiver.lineLimit)
                    {
                        int start = Mathf.Min(inputTMPReceiver.caretPosition, inputTMPReceiver.selectionAnchorPosition);
                        int end = Mathf.Max(inputTMPReceiver.caretPosition, inputTMPReceiver.selectionAnchorPosition);
                        int sel = Mathf.Abs(end - start);
                        if (sel > 0)
                        {
                            // inputTMPReceiver.text = inputTMPReceiver.text.Remove(start, sel);
                            // inputTMPReceiver.text = inputTMPReceiver.text.Insert(start, displaychar);
                            //inputTMPReceiver.Select();
                            //inputTMPReceiver.caretPosition = start + 1;// inputTMPReceiver.text.Length;
                            //inputTMPReceiver.ActivateInputField();

                            text = text.Remove(start, sel);
                            text = text.Insert(start, "\n");

                            inputTMPReceiver.caretPosition = start + 1;

                        }
                        else if (inputTMPReceiver.caretPosition < text.Length && inputTMPReceiver.caretPosition >= 0)
                        {
                            int caret = inputTMPReceiver.caretPosition;
                            text = text.Insert(inputTMPReceiver.caretPosition, "\n");

                            inputTMPReceiver.ForceLabelUpdate();

                            inputTMPReceiver.caretPosition = caret + 1;

                        }
                        else
                        {

                            text += "\n";
                            inputTMPReceiver.text = inputTMPReceiver.text + "\n"; //\u00A0";

                            inputTMPReceiver.caretPosition = text.Length;

                        }

                    }
                    cursorIndex = inputTMPReceiver.caretPosition;
                    break;
            }

            ValueChanged();
        }

        public override void Backspace()
        {
            switch (receiver)
            {

                case OSK_RECEIVER.INPUTFIELD:                   
                    if (inputReceiver.text.Length > 0)
                    {
                        int caret_ = (inputReceiver.caretPosition != cursorIndex ? cursorIndex : inputReceiver.caretPosition);
                        int start = cursorSel.x;
                        int end = cursorSel.y;
                        int sel = Mathf.Abs(end - start);

                        if (sel > 0)
                        {
                            inputReceiver.text = inputReceiver.text.Remove(start, sel);
                            text = text.Remove(start, sel);
                            inputReceiver.caretPosition = start;
                        }
                        else
                        {
                            start = Mathf.Max(0, caret_ - 1);

                            inputReceiver.text = inputReceiver.text.Remove(start, 1);

                            

                            text = text.Remove(start, 1);
                            inputReceiver.caretPosition = start;

                        }

                        cursorIndex = start;
                        cursorSel = -1*Vector2Int.one;
                    }
                    break;
                case OSK_RECEIVER.TMPRO_INPUTFIELD:
                    if (inputTMPReceiver.text.Length > 0)
                    {

                        int start = Mathf.Min(inputTMPReceiver.caretPosition, inputTMPReceiver.selectionAnchorPosition);
                        int end = Mathf.Max(inputTMPReceiver.caretPosition, inputTMPReceiver.selectionAnchorPosition);
                        int sel = Mathf.Abs(end - start);
                        if (sel > 0)
                        {

                            //inputTMPReceiver.text = inputTMPReceiver.text.Remove(start, sel);
                            text = text.Remove(start, sel);

                            if (useCharMask)
                                inputTMPReceiver.text = new string(charMask[0], text.Length);
                            else
                                inputTMPReceiver.text = text;

                            inputTMPReceiver.caretPosition = start;

                        }
                        else
                        {

                            start = Mathf.Max(0, inputTMPReceiver.caretPosition - 1);


                            text = text.Remove(start, 1);

                            if (useCharMask)
                                inputTMPReceiver.text = new string(charMask[0], text.Length);
                            else
                                inputTMPReceiver.text = text;

                            inputTMPReceiver.caretPosition = start;
                        }

                        // only uncomment the following lines if you do not plan to use a gamepad controller
                        //inputTMPReceiver.Select();

                        //inputTMPReceiver.ActivateInputField();
                        cursorIndex = inputTMPReceiver.caretPosition;
                    }
                    break;
            }
        }

        public override void Del()
        {

            switch (receiver)
            {
                case OSK_RECEIVER.INPUTFIELD:
                    // this avoid writing over the placeholder text when no input is being received

                    if (inputReceiver.text.Length > 0)
                    {
                        int caret_ = (inputReceiver.caretPosition != cursorIndex ? cursorIndex : inputReceiver.caretPosition);
                        int start = cursorSel.x;
                        int end = cursorSel.y;
                        int sel = Mathf.Abs(end - start);

                        if (sel > 0)
                        {

                            inputReceiver.text = inputReceiver.text.Remove(start, sel);
                            text = text.Remove(start, sel);

                        }
                        else
                        {
                            
                            start = Mathf.Min(inputReceiver.text.Length-1, caret_);

                            inputReceiver.text = inputReceiver.text.Remove(start, 1);
                            text = text.Remove(start, 1);

                            inputReceiver.caretPosition = start;
                        }


                        cursorIndex = start;
                        cursorSel = -1 * Vector2Int.one;

                    }
                    break;
                case OSK_RECEIVER.TMPRO_INPUTFIELD:
                    // this avoid writing over the placeholder text when no input is being received
                    if (inputTMPReceiver.text.Length > 0)
                    {

                        int start = Mathf.Min(inputTMPReceiver.caretPosition, inputTMPReceiver.selectionAnchorPosition);
                        int end = Mathf.Max(inputTMPReceiver.caretPosition, inputTMPReceiver.selectionAnchorPosition);
                        int sel = Mathf.Abs(end - start);
                        if (sel > 0)
                        {
                            //inputTMPReceiver.text = inputTMPReceiver.text.Remove(start, sel);
                           // inputTMPReceiver.Select();
                           // inputTMPReceiver.caretPosition = start;
                           // inputTMPReceiver.ActivateInputField();
                           text = text.Remove(start, sel);
                            if (useCharMask)
                                inputTMPReceiver.text = new string(charMask[0], text.Length);
                            else
                                inputTMPReceiver.text = text;

                            inputTMPReceiver.caretPosition = start;

                        }
                        else if(inputTMPReceiver.caretPosition < inputTMPReceiver.text.Length-1)
                        {
                            
                            start = Mathf.Min(inputTMPReceiver.text.Length - 1, inputTMPReceiver.caretPosition);

                            //inputTMPReceiver.text = inputTMPReceiver.text.Remove(start, 1);
                          //  inputTMPReceiver.Select();
                          //  inputTMPReceiver.caretPosition = start;
                          //  inputTMPReceiver.ActivateInputField();
                          text = text.Remove(start, 1);
                            if (useCharMask)
                                inputTMPReceiver.text = new string(charMask[0], text.Length);
                            else
                                inputTMPReceiver.text = text;

                            inputTMPReceiver.caretPosition = start;
                        }
                        cursorIndex = inputTMPReceiver.caretPosition;
                    }
                    break;
            }


        }


        public override void ClearText()
        {
            
            switch (receiver)
            {
                case OSK_RECEIVER.INPUTFIELD:
                    inputReceiver.text = "";
                    text = "";
                    break;
                case OSK_RECEIVER.TMPRO_INPUTFIELD:
                    inputTMPReceiver.text = "";
                    text = "";
                    break;
            }
        }

        void IPointerClickHandler.OnPointerClick(PointerEventData eventData)
        {

            if (onSelectClick != null)
            {
                onSelectClick.Invoke();
            }

            switch (receiver)
            {
                case OSK_RECEIVER.INPUTFIELD:

                    cursorIndex = inputReceiver.caretPosition;
                    cursorSel.x = Mathf.Min(inputReceiver.caretPosition, inputReceiver.selectionAnchorPosition);
                    cursorSel.y = Mathf.Max(inputReceiver.caretPosition, inputReceiver.selectionAnchorPosition);
                    break;
                case OSK_RECEIVER.TMPRO_INPUTFIELD:

                    cursorIndex = inputTMPReceiver.caretPosition;
                    break;
            }

            if (!isFocused())
            {
                OnFocus();
            };


        }

        public void OnDrag(PointerEventData eventData)
        {
            
            switch (receiver)
            {
                case OSK_RECEIVER.INPUTFIELD:

                    cursorIndex = inputReceiver.caretPosition;
                    cursorSel.x = Mathf.Min(inputReceiver.caretPosition, inputReceiver.selectionAnchorPosition);
                    cursorSel.y = Mathf.Max(inputReceiver.caretPosition, inputReceiver.selectionAnchorPosition);
                    break;
                case OSK_RECEIVER.TMPRO_INPUTFIELD:

                    cursorIndex = inputTMPReceiver.caretPosition;
                    break;
            }

            if (!isFocused())
            {
                OnFocus();
            };
        }


        /// <summary>
        /// this function is called when the Receiver is highlighted 
        /// and the user presses on the "action" button on their joystick/gamepad (ex: A button or whatever button you set as the Submit action in Input)
        /// </summary>
        /// <param name="eventData"></param>
        void ISubmitHandler.OnSubmit(BaseEventData eventData)
        {

            if (onSelectClick != null)
            {
                onSelectClick.Invoke();
            }


            if (!isFocused())
            {
                OnFocus();
            };

        }


    }



}
