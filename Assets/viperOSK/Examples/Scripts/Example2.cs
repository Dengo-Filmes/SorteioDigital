///////////////////////////////////////////////////////////////////////////////////////
///
/// Example 2 - this example showcases a more flexible use of the viperOSK keyboard
/// The keyboard disappears after input and will reappear in 5 seconds
///
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
using UnityEngine.SceneManagement;
using viperOSK;


namespace viperOSK_Examples
{
    public class Example2 : MonoBehaviour
    {

        public SpriteRenderer mySpeechBubble;
        public TextMeshPro mySpeech;

        public TextMeshPro NPCSpeech;

        public OSK_Keyboard keyboard;

        bool contextShift;
        Vector3 current, target;
        float t;

        // Start is called before the first frame update
        void Start()
        {
            if (Camera.main.aspect < 16f / 9f - .1f)
            {
                // reduce scale size of keyboard for 4:3 type screens
                //keyboard.transform.localScale = new Vector3(.8f, .8f, 1f);
                Camera.main.orthographicSize = 5.5f;
            }

        }

        /// <summary>
        /// This function is called by OSK_Keyboard when the user triggers a Submit
        /// </summary>
        /// <param name="text">the text from OSK_Keyboard</param>
        public void Submit(string text)
        {
            if (text.Equals("switch", StringComparison.InvariantCultureIgnoreCase))
            {
                int currentSceneIndex = SceneManager.GetActiveScene().buildIndex;
                Debug.Log("current scene = " + currentSceneIndex + " / " + SceneManager.sceneCountInBuildSettings);
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

            Debug.Log(text);
            mySpeech.text = text + "<-";
            mySpeechBubble.color = Color.cyan;

            if (text.ToLowerInvariant().Contains("hello") || text.ToLower().Contains("hi"))
            {
                NPCSpeech.text = "Hi friend";
            }
            else
            {
                NPCSpeech.text = "Aha";
            }

            keyboard.gameObject.SetActive(false);
            Invoke("Clear", 5f);
            current = Camera.main.gameObject.transform.position;
            target = current + new Vector3(0f, 3.5f, 0f);
            contextShift = true;
            t = 0f;

        }

        void Clear()
        {
            mySpeech.text = "";
            mySpeechBubble.color = Color.white;
            NPCSpeech.text = "Hey there!";
            keyboard.gameObject.SetActive(true);

            current = Camera.main.gameObject.transform.position;
            target = current + new Vector3(0f, -3.5f, 0f);
            contextShift = true;
            t = 0f;
        }


        // Update is called once per frame
        void Update()
        {
            if (contextShift)
            {
                t += Time.deltaTime * 3f;
                Camera.main.gameObject.transform.position = Vector3.Lerp(current, target, t);

                if (t >= 1f)
                {
                    Camera.main.gameObject.transform.position = target;
                    contextShift = false;
                }
            }
        }
    }
}
