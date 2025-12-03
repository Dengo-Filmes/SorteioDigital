///////////////////////////////////////////////////////////////////////////////////////
/// © vipercode corp
/// 2022
/// Please use this asset according to the attached license
/// Attributions, mentions and reviews are always welcomed
///
///////////////////////////////////////////////////////////////////////////////////////

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
    public class OSK_UI_CustomReceiver : OSK_Receiver, IPointerDownHandler, IPointerUpHandler, ISelectHandler, ISubmitHandler
    {

        public UnityEvent onSelect;

        // events to be called when the Receiver is highlighted and then Clicked on or the gamepad Submit event is triggered
        public UnityEvent onSelectClick;


        // Start is called before the first frame update
        void Awake()
        {
            if (this.gameObject.TryGetComponent<TMP_Text>(out textReceiver))
            {

                textReceiver.color = normalColor;
                cursor = this.transform.GetComponentInChildren<I_OSK_Cursor>();
                if (cursor == null)
                    Debug.LogWarning("This TMPro text object does not have a cursor. If you want full edit functionality add a OSK_UI_Cursor prefab as a child");


                OnFocusLost();

            } else
            {
                Debug.LogWarning("The OSK_UI_CustomReceiver must be in the same gameobject as the TextMeshPro text object receiving text input");
            }


        }

        #region UnityEngine.EventSystems Pointer Handlers

        /// <summary>
        /// PointerDown handle. Used solely for the TextMeshPro receivers
        /// </summary>
        /// <param name="eventData">Event System event data</param>
        public void OnPointerDown(PointerEventData eventData)
        {
            if (cursor != null && allowTextSelection)
            {

                Deselect();
                cursorSel.x = Selection(eventData.pointerPressRaycast.screenPosition);
                cursorSel.x = Mathf.Min(cursorSel.x, textReceiver.text.Length - 1);
            }

        }

        /// <summary>
        /// PointerUp handle. Used solely for the TextMeshPro receivers
        /// </summary>
        /// <param name="eventData">Event System event data</param>
        public void OnPointerUp(PointerEventData eventData)
        {
            if (!isFocused() && onSelectClick != null)
            {
                onSelectClick.Invoke();
                OnFocus();
                return;
            }
            

            if (cursor != null)
            {
                // check whether the pointer was down before (dragging on text)
                if (cursorSel.x >= 0 && allowTextSelection)
                {

                    cursorSel.y = Selection(eventData.pointerCurrentRaycast.screenPosition, true);
                    if (cursorSel.y < 0)
                        cursorSel.y = Selection(eventData.pointerCurrentRaycast.screenPosition);
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
                    cursorIndex = Selection(eventData.pointerCurrentRaycast.screenPosition);

                }
            }

        }
        public void OnSelect(BaseEventData eventData)
        {
            if (onSelect != null)
                onSelect.Invoke();
        }

        /// <summary>
        /// this function is called when the Receiver is highlighted 
        /// and the user presses on the "action" button on their joystick/gamepad (ex: A button or whatever button you set as the Submit action in Input)
        /// </summary>
        /// <param name="eventData"></param>
        void ISubmitHandler.OnSubmit(BaseEventData eventData)
        {
            if (onSelectClick != null)
                onSelectClick.Invoke();

            if (!isFocused())
            { 

                OnFocus();
            };
            
        }

        #endregion


        #region inherited routines

        /// <summary>
        /// Finds the cursor index in the text based on the mouse/touch hitpoint
        /// </summary>
        /// <param name="hitpoint">mouse/touch position</param>
        /// <returns>index of the text where cursor</returns>
        public override int Selection(Vector3 hitpoint, bool charhit = false)
        {
            Camera cam = Camera.main;

            if (textReceiver.canvas != null)
            {
                if (textReceiver.canvas.renderMode == RenderMode.ScreenSpaceOverlay)
                    cam = null;
                else
                    cam = textReceiver.canvas.worldCamera;
            }

            if (charhit)
                return TMP_TextUtilities.FindIntersectingCharacter(textReceiver, hitpoint, cam, false);

            return TMP_TextUtilities.GetCursorIndexFromPosition(textReceiver, hitpoint, cam);
        }

        /// <summary>
        /// Deselects text
        /// </summary>
        public override void Deselect()
        {


            SelectionHighlight(normalColor, true);
            cursorSel.x = cursorSel.y = -1;
        }


        public override void SelectionHighlight(Color32 c, bool all = false)
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



        #endregion


        // Update is called once per frame
        void Update()
        {

        }


    }
}
