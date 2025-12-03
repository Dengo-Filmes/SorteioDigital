using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using viperOSK;
using System;
using TMPro;

namespace viperOSK_Examples
{



    public class Example5UI : MonoBehaviour
    {


        public OSK_UI_Keyboard keyboard;

        public TextMeshProUGUI label_viperOSKReceiver;
        public TextMeshProUGUI label_UnityInputField;

        public GameObject arrowRight;

        public GameObject arrowLeft;

        float t;


        // Start is called before the first frame update
        void Start()
        {
            if(keyboard == null)
            {
                keyboard = GameObject.FindObjectOfType<OSK_UI_Keyboard>();
                if (keyboard == null)
                    Debug.LogError("keyboard needs to be assigned an OSK_Keyboard, you can do this in the inspector");
            }

            Reset();
        }

        // Update is called once per frame
        void Update()
        {
        }

        public void print_viperOSK(string s)
        {
            label_viperOSKReceiver.text = s;
        }

        public void print_UnityInputField(string s)
        {
            label_UnityInputField.text = s;
        }

        public void select_viperOSKInputField()
        {
            arrowLeft.SetActive(true);
            arrowRight.SetActive(false);
        }

        public void select_UnityInputField()
        {
            arrowLeft.SetActive(false);
            arrowRight.SetActive(true);
        }

        public void ShowHideKeyboard(bool show)
        {

            keyboard.ShowHideKeyboard(show);
        }

        public void Reset()
        {
            ShowHideKeyboard(false);
            arrowLeft.SetActive(false);
            arrowRight.SetActive(false);
        }


    }
}
