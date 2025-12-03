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

namespace viperOSK
{

    public class OSK_Cursor : MonoBehaviour, I_OSK_Cursor
    {
        bool blink;

        /// <summary>
        /// cursor blinks delay in secs
        /// </summary>
        public float bps;

        Vector3 startingCursorPos;
        Vector3 cursorPos;



        public OSK_Receiver input;

        public TMP_Text textComponent;
        TMP_TextInfo textInfo;
        TMP_CharacterInfo charInfo;

        public SpriteRenderer cursorImg;
        Color cursorImgColor;

        /// <summary>
        /// Finds a component of type T in the parent or siblings of this GameObject.
        /// </summary>
        /// <typeparam name="T">The type of component to search for.</typeparam>
        /// <returns>The component if found; otherwise, null.</returns>
        public T FindComponentInParentOrSiblings<T>() where T : Component
        {
            // Check if the parent exists
            if (transform.parent != null)
            {
                // Try to get the component from the parent
                T componentInParent = transform.parent.GetComponent<T>();
                if (componentInParent != null)
                {
                    return componentInParent;
                }

                // Iterate over the siblings
                foreach (Transform sibling in transform.parent)
                {
                    // Skip this GameObject
                    if (sibling == transform)
                        continue;

                    // Try to get the component from the sibling
                    T componentInSibling = sibling.GetComponent<T>();
                    if (componentInSibling != null)
                    {
                        return componentInSibling;
                    }
                }
            }

            // Component not found in parent or siblings
            return null;
        }

        // Awake is called before the first frame update
        void Awake()
        {

            if (cursorImg == null)
                cursorImg = this.GetComponent<SpriteRenderer>();

            if (input == null)
            {
                input = FindComponentInParentOrSiblings<OSK_Receiver>();
            }

            if (textComponent == null)
            {
                if (input != null)
                {
                    textComponent = input.textReceiver;

                }

                if (textComponent == null)
                {
                    textComponent = FindComponentInParentOrSiblings<TMP_Text>();
                }
            }

            if (input == null)
                Debug.LogError("viperOSK OSK_Cursor object must be the child of a OSK_Receiver object to work");

            if (textComponent == null)
                Debug.LogError("viperOSK OSK_Cursor object must be the child of a TextMeshPro object to work");

            if (cursorImg == null)
                Debug.LogError("viperOSK OSK_Cursor object must have a SpriteRenderer component to work");
            else
                cursorImgColor = cursorImg.color;

        }

        void Start() 
        { 


            // establish cursor starting position
            // this hack gets around TMP's lack of a method or property to expose where the first character should appear.
            textComponent.text = "A";
            textComponent.ForceMeshUpdate();
            startingCursorPos = textComponent.textInfo.characterInfo[0].vertex_BL.position;
            startingCursorPos.y = textComponent.textInfo.characterInfo[0].baseLine + cursorImg.bounds.extents.y*.75f;
            startingCursorPos.z = this.transform.localPosition.z;
            textComponent.text = "";

            textInfo = textComponent.textInfo;

            StartCoroutine(BlinkCoroutine());   //v3.3 changed from InvokeRepeating to avoid issues for devs using TimeScale=0 in pause menus
            Cursor();
        }

        /// <summary>
        /// calculates where to place the cursor in UI
        /// </summary>
        /// <param name="indx">index of character where to place cursor</param>
        public void Cursor()
        {

            if (textComponent.text.Length == 0)
            {
                cursorPos = startingCursorPos;
            }
            else if (input.cursorIndex >= 0 && textComponent.textInfo.characterCount > 0)
            {

                    charInfo = textInfo.characterInfo[Mathf.Clamp(input.cursorIndex, 0, textInfo.characterCount - 1)];
                    // if the cursor is at the begining of not the last character, then place it at bottom left, otherwise, place at end
                    cursorPos.x = (input.cursorIndex < textInfo.characterCount ? charInfo.bottomLeft.x : charInfo.bottomRight.x);

                    cursorPos.y = charInfo.baseLine + cursorImg.bounds.extents.y*.75f;
                
            }

            this.GetComponent<RectTransform>().localPosition = new Vector3(cursorPos.x + cursorImg.bounds.extents.x, cursorPos.y, cursorPos.z);
        }

        IEnumerator BlinkCoroutine()
        {

            while(true)
            {
                yield return new WaitForSecondsRealtime(bps);

                if (cursorImg.enabled)
                    cursorImg.color = new Color(cursorImg.color.r, cursorImg.color.g, cursorImg.color.b, 0f);

                yield return new WaitForSecondsRealtime(bps);

                if (cursorImg.enabled)
                    cursorImg.color = cursorImgColor;

            }
                
        }

        public void Show(bool show)
        {
            if (cursorImg != null)
                cursorImg.enabled = show;
        }

        // (v3.4.1) address issues with coroutine not starting after enable/disable calls to the gameobject
        public void OnEnable()
        {
            StartCoroutine(BlinkCoroutine());
        }

        public void OnDisable()
        {
            StopCoroutine(BlinkCoroutine());
        }

        // Update is called once per frame
        void Update()
        {
            Cursor();
        }
    }

}
