using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Events;
using viperTools;

namespace viperOSK_Examples
{

    /// <summary>
    /// Button: is a simple Button class to for very basic button behaviour
    /// it is not meant to replace any UI approaches
    /// it is solely provided for viperOSK's examples for those that may not have UI controls configured in their Unity
    /// </summary>
    public class viperOSK_SimpleButton : MonoBehaviour
    {

        public UnityEvent action;

        public Color color1;
        public Color color2;

        private void Start()
        {
            color1 = this.GetComponent<SpriteRenderer>().color;
        }

        private void OnMouseUp()
        {
            action.Invoke();

            // show press visually
            this.transform.Translate(new Vector3(.05f, -.05f, 0f));
            Invoke("ReturnToPos", .1f);
        }

        /// <summary>
        /// function to return btn to position
        /// </summary>
        public void ReturnToPos()
        {
            this.transform.Translate(new Vector3(-.05f, .05f, 0f));
        }

        public void Enable(bool enabled)
        {

            this.gameObject.SetActive(enabled);

        }

        /// <summary>
        /// visually switch between 2 colors that are set in the inspector (or through script)
        /// </summary>
        public void SwitchColor()
        {
            SpriteRenderer renderer = this.GetComponent<SpriteRenderer>();
            if(renderer.color == color1)
            {
                renderer.color = color2;
            } else
            {
                renderer.color = color1;
            }
        }

        /// <summary>
        /// Method uses raycasting instead of OnMouseDown/Up callbacks
        /// It can be further refined by allocating a layer to all keys anc only raycasting against that layer
        /// it was not done here so it does not interfere with existing user layers
        /// </summary>
        void InputFromPointerDevice()
        {
            Vector2 pointPos = viperInput.GetPointerPos();

            if (viperInput.PointerUp())
            {

                Ray ray = Camera.main.ScreenPointToRay(pointPos);

                RaycastHit hit;


                if (Physics.Raycast(ray, out hit))
                {


                    if (hit.collider.gameObject == this.gameObject)
                    {
                        OnMouseUp();
                    }

                }
            }

        }

        private void Update()
        {
#if !ENABLE_LEGACY_INPUT_MANAGER
        InputFromPointerDevice();
#endif
        }
    }

}
