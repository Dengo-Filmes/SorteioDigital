///////////////////////////////////////////////////////////////////////////////////////
/// © vipercode corp
/// 2022
/// Please use this asset according to the attached license
/// Attributions, mentions and reviews are always welcomed
///
///////////////////////////////////////////////////////////////////////////////////////

using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using System;

namespace viperOSK
{
    public enum OSK_KEY_TYPES
    {
        DIGIT,
        LETTER,
        PUNCTUATION,
        CONTROLS,    // CTRL, SHIFT, SPACE, ENTER..ETC
    }


    /// <summary>
    /// OSK_KeyTypeMeta provides a general approach to controlling the look of keys based on their KeyType (ie, whether letter, digit, control..etc)
    /// </summary>
    [Serializable]
    public class OSK_KeyTypeMeta
    {
        public OSK_KEY_TYPES keyType;
        public Color col;
        public int keySoundCode;

        /// <summary>
        /// Establishes whether the keycode is for a digit, letter, punctuation or control
        /// Note that symbols (like mathematical symbols..etc) are categorized as punctuations for ease of use
        /// </summary>
        /// <param name="key">KeyCode of the key</param>
        /// <returns></returns>
        public static OSK_KEY_TYPES KeyType(OSK_KeyCode key)
        {
            if (key < OSK_KeyCode.__CUSTOM__)
            {


                if (char.IsDigit((char)key))
                {
                    return OSK_KEY_TYPES.DIGIT;
                }
                else if (char.IsLetter((char)key) && key != OSK_KeyCode.LeftShift && key != OSK_KeyCode.RightShift && key != OSK_KeyCode.CapsLock)
                {
                    return OSK_KEY_TYPES.LETTER;


                }
                else if (char.IsPunctuation((char)key) || char.IsSymbol((char)key))
                {
                    return OSK_KEY_TYPES.PUNCTUATION;
                }
                else
                {
                    return OSK_KEY_TYPES.CONTROLS;
                }
            } else
            {
                if (key >= OSK_KeyCode.__ACCENTS__ && key < OSK_KeyCode._END_ACCENTS__)
                {
                    return OSK_KEY_TYPES.LETTER;
                } else if(key >= OSK_KeyCode.__SYMBOLS__ && key < OSK_KeyCode._END_SYMBOLS__)
                {
                    return OSK_KEY_TYPES.PUNCTUATION;
                }

                return OSK_KEY_TYPES.CONTROLS;
            }
        }

        public OSK_KeyTypeMeta()
        {

        }

        public OSK_KeyTypeMeta(OSK_KEY_TYPES kt, Color c)
        {
            keyType = kt;
            col = c;
        }

    }

}
