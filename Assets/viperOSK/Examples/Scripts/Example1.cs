///////////////////////////////////////////////////////////////////////////////////////
///
/// Example 1 - this example shows the basic use of the viperOSK keyboard out-of-the-box
/// graphics can always be adjusted as needed and they won't interfere with the functioning of the asset (as long as no broken references)
/// different samples to show the range of options are provided in the examples shown in Ex1 scene
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
using TMPro;
using viperOSK;
using System;
using UnityEngine.SceneManagement;

namespace viperOSK_Examples
{
    public class Example1 : MonoBehaviour
    {
        // text output attached to camera
        public TextMeshPro outputtext;

        // list of sample keyboards
        public List<OSK_Keyboard> keyboards = new List<OSK_Keyboard>();

        // navigation buttons to browse the list of keyboard samples
        public viperOSK_SimpleButton upBtn, downBtn;

        // current keyboard being sampled
        int currentKeyboard;

        // t, start and end parameters for Slerp camera move
        float t;
        Vector3 start;
        Vector3 end;

        // Start is called before the first frame update
        void Start()
        {
            t = -1f;
            start = Camera.main.transform.position;
            if (upBtn != null)
                upBtn.Enable(false);
            else
                Debug.Log("Please set the Up Button in inspector");

            if (downBtn == null)
                Debug.Log("Please set the Down Button in inspector");

            // set all keyboards except first to lose focus, their focus will be managed when we slide to them
            for(int i=1; i <keyboards.Count; i++)
            {
                keyboards[i].HasFocus(false);
            }

        }

        public void MoveUp()
        {
            // this provides one approach of managing keyboard focus
            keyboards[currentKeyboard].HasFocus(false);
            currentKeyboard--;
            currentKeyboard = (int)Mathf.Clamp(currentKeyboard, 0, keyboards.Count - 1);
            keyboards[currentKeyboard].HasFocus(true);

            SetMoveCoord();
        }

        public void MoveDown()
        {
            // this provides one approach of managing keyboard focus
            keyboards[currentKeyboard].HasFocus(false);
            currentKeyboard++;
            currentKeyboard = (int)Mathf.Clamp(currentKeyboard, 0, keyboards.Count - 1);
            keyboards[currentKeyboard].HasFocus(true);

            SetMoveCoord();
        }

        public void SetMoveCoord()
        {
            if (currentKeyboard == 0)
            {
                upBtn.Enable(false);
            }
            else
            {
                upBtn.Enable(true);
            }

            if (currentKeyboard == keyboards.Count - 1)
            {
                downBtn.Enable(false);
            }
            else
            {
                downBtn.Enable(true);
            }

            t = 0f;
            end = keyboards[currentKeyboard].transform.position;
            end.y = end.y + 2f;
            end.z = start.z;
        }

        /// <summary>
        /// loads a keyboard layout using script example
        /// </summary>
        public void LoadEmailLayout()
        {
            string layout = "1 2 3 4 5 6 7 8 9 0 \n Q W E R T Y U I O P \n A S D F G H J K L \" Skip.2 Backspace \n CapsLock Z X C V B N M & \n LeftShift # \' - _ @ . Skip.2 Return";
            if (keyboards.Count >= 2)
                keyboards[1].LoadLayout(layout);
        }

        /// <summary>
        /// callback from sample keyboards
        /// </summary>
        /// <param name="s">text sent by OSK_Keyboard</param>
        public void SubmitText(string s)
        {
            if (s.Equals("switch", StringComparison.InvariantCultureIgnoreCase))
            {
                int currentSceneIndex = SceneManager.GetActiveScene().buildIndex;
                if (currentSceneIndex < SceneManager.sceneCountInBuildSettings - 1)
                {
                    SceneManager.LoadScene(currentSceneIndex + 1);
                }
                else
                {
                    SceneManager.LoadScene(0);
                }
                return;
            }

            Debug.Log("TEXT SUBMITTED: " + s);
            outputtext.text = s;
        }

        // Update is called once per frame
        void Update()
        {
            if (t >= 0f && keyboards.Count > 0)
            {
                t += Time.deltaTime;

                Camera.main.transform.position = Vector3.Slerp(start, end, t);

                if (t >= 1f)
                {
                    t = -1f;
                    start = Camera.main.transform.position;
                }
            }
        }
    }
}