/// //////////////////////////////////////////////////////////////////////////////////
/// OSK_Background: class and methods to automatically resize the keyboard background to fit the keyboard when Auto-Size is pressed in Editor
/// 
/// © vipercode corp
/// 2024
/// Please use this asset according to the attached license
/// Attributions, mentions and reviews are always welcomed
///
///////////////////////////////////////////////////////////////////////////////////////


using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

namespace viperOSK
{
    /// <summary>
    /// (v3.5) class and methods to automatically resize the keyboard background to fit the keyboard when Auto-Size is pressed in Editor
    /// This approach is still experimental and only works with the 3D version of viperOSK Keyboard (not the Unity UI at present).
    /// </summary>
    public class OSK_Background : MonoBehaviour
    {

        public OSK_Keyboard keyboard;

        public float topBottomMargins = 0.25f;
        public float leftRightMargins = 0.25f;

        // Start is called before the first frame update
        void Start()
        {
            if (keyboard == null)
            {
                keyboard = this.gameObject.GetComponentInParent<OSK_Keyboard>();
                if (keyboard == null)
                {
#if UNITY_2019
                    keyboard = GameObject.FindObjectOfType<OSK_Keyboard>();
#else
                    keyboard = GameObject.FindAnyObjectByType<OSK_Keyboard>();
#endif
                }
                else
                {
                    Debug.LogError("OSK_Background requires a OSK_Keyboard to be in the scene");
                }
            }


        }


        /// <summary>
        /// resize the keyboard background to fit the keyboard layout + the margins
        /// </summary>
        public void ResizeToFit()
        {
            keyboard.Traverse();

            
            Vector3 scl = new Vector3(1f/keyboard.transform.localScale.x, 1f/ keyboard.transform.localScale.y, 1f/ keyboard.transform.localScale.z);          
            Vector2 hmar = new Vector2((keyboard.SpanBottomRight().x - keyboard.SpanTopLeft().x)*scl.x, (keyboard.SpanTopLeft().y - keyboard.SpanBottomRight().y)*scl.y);


            Vector2 newSize = new Vector2(hmar.x + leftRightMargins * 2f, hmar.y + topBottomMargins * 2f);


            SpriteRenderer render = this.GetComponent<SpriteRenderer>();
            if (render != null)
            {
                render.size = newSize;
                this.transform.position = (keyboard.SpanTopLeft() + keyboard.SpanBottomRight()) * .5f + new Vector3(0f, 0f, .25f);
            }
            else // is UI
            {
                Image img = this.GetComponent<Image>();
                if (img != null)
                {
                    img.rectTransform.sizeDelta = newSize;
                    this.transform.position = (keyboard.SpanTopLeft() + keyboard.SpanBottomRight()) * .5f + new Vector3(0f, 0f, .25f);
                }
            }

        }

        // Update is called once per frame
        void Update()
        {

        }
    }

}
