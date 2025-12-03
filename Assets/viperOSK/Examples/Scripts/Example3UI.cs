///////////////////////////////////////////////////////////////////////////////////////
///
/// Example 3UI - this example showcases the UI implementation of viperOSK
/// The OSK_UI_Keyboard component is adapted to Unity's UI
/// The keyboard is made of UIKeys that are adapted from UI Button
/// The text receiver (input field) are handled through OSK_UI_CustomReceiver that functions similarly to UI InputField
/// or with the use of OSK_UI_InputReceiver that can be attached to either Unity's UI InputField or InputField (TMP)
/// the TMP version is preferred
/// Example 3 shows the handling of all 3. The component OSK_UI_InputConnector allows the user to click/touch the 
/// desired input to send the keyboard input there.
///
/// © vipercode corp
/// 2022
/// Please use this asset according to the attached license
/// Attributions, mentions and reviews are always welcomed
///
///////////////////////////////////////////////////////////////////////////////////////

using System;
using TMPro;
using UnityEngine;
using UnityEngine.EventSystems;
using UnityEngine.SceneManagement;
using UnityEngine.UI;
using viperOSK;
using viperTools;

namespace viperOSK_Examples
{
    public class Example3UI : MonoBehaviour
    {

        Vector2 pointerPos;

        public GameObject cube;
        Vector3 cubeRotation;

        public OSK_UI_Keyboard keyboard;

        public OSK_Receiver field;

        public TMP_Text output1;
        public TMP_Text output2;
        public TMP_Text output3;

        public TMP_InputField infield;


        public void OnSubmit(string text)
        {
            if (text.Equals("switch", StringComparison.InvariantCultureIgnoreCase))
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

            if (keyboard.output.name.Equals("UI_Input_Text"))
            {
                output1.text = text;
            }

            if (keyboard.output.name.Equals("OSK_InputField (TMP)"))
            {
                output2.text = text;
              
            }

            if (keyboard.output.name.Equals("OSK_InputField"))
            {
                output3.text = text;
            }
            RandomizeCubeRotation();


        }

        private void Awake()
        {

        }

        // Start is called before the first frame update
        void Start()
        {

            RandomizeCubeRotation();
 
        }


        // for testing
        public void Hello()
        {
            field.SetText("HELLO123");
            Debug.Log("HELLO");
        }

        private void RandomizeCubeRotation()
        {
            Vector3 rot1 = new Vector3(UnityEngine.Random.Range(-1f, 1f), UnityEngine.Random.Range(-1f, 1f), UnityEngine.Random.Range(-1f, 1f));

            cubeRotation = rot1;

            Time.timeScale = Mathf.Abs(1f - Time.timeScale);
            Debug.Log("timescale=" + Time.timeScale);
        }



        // Update is called once per frame
        void Update()
        {
            if (cube != null)
            {
                cube.transform.Rotate(cubeRotation*15f*Time.deltaTime, Space.World);
            }

           

        }
    }
}
