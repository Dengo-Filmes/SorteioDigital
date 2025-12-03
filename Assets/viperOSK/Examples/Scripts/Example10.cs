/// 
/// © vipercode corp
/// 2024
/// Please use this asset according to the attached license
/// Attributions, mentions and reviews are always welcomed
///
///////////////////////////////////////////////////////////////////////////////////////

using System.Collections;
using System.Collections.Generic;
using System.Linq;
using UnityEngine;
using viperOSK;

namespace viperOSK_Examples
{
    public class Example10 : MonoBehaviour
    {
        public OSK_MiniKeyboard osk;

        public OSK_MiniKeyboard osk_numbers;

        public OSK_Receiver receiverText;


        public string layout = "q w e r t y u i o p a s d f g h j k l z x c v b n m _ .";


        public OSK_Receiver receiverNum;

        public string layoutNum = "= . 0 3 2 1 6 5 4 9 8 7";

        // Start is called before the first frame update
        void Start()
        {
            List<string> keylist = layout.Split(' ').ToList();

            // the list is reversed as Mini-Keyboard is mainly designed for accent console
            // the keys are organized from bottom-right to top-left.
            // reversing the List re-organizes them to a top-left to bottom-right layout.
            // You can do so by using the following code
            //keylist.Reverse();
            //osk.Generate(keylist, false, CallBack, true);

            // otherwise, you can set the BottomRightOrder parameter to false in the Generate() routine
            osk.Generate(keylist, false, CallBack, false);
            receiverText.OnFocus();

            keylist = layoutNum.Split(' ').ToList();
            osk_numbers.Generate(keylist, false, CallBackNum);
            receiverNum.OnFocus();
        }

        public void CallBack(string input, OSK_Receiver receiver)
        {
            receiverText.AddText(input);
        }


        public void CallBackNum(string input, OSK_Receiver receiver)
        {
            receiverNum.AddText(input);
        }

        // Update is called once per frame
        void Update()
        {

        }
    }
}
