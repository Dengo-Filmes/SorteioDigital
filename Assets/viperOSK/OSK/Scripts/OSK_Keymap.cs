///////////////////////////////////////////////////
/// OSK_Keymap handles the mapping between string to OSK_KeyCode to support various uses in OSK_Keyboard in interpreting the layout string
/// Includes also the autocorrective routines to solve most common errors users may have when keying in a layout
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
using System.Linq;
using UnityEngine;
using System.Text.RegularExpressions;
using System.Globalization;
using System.Text;

namespace viperOSK
{
    /// <summary>
    /// helper class keymap to ensure typeable keys are identified and recorded quickly, funnels in multiple paths in keyboards
    /// for example, OSK_KeyCode.Alpha0 vs OSK_KeyCode.Keypad0 so that the controls are always Alpha
    /// </summary>
    public class OSK_Keymap
    {
        public Dictionary<string, OSK_KeyCode> chartoKeycode = new Dictionary<string, OSK_KeyCode>()
        { 
            // letters
            {"A", OSK_KeyCode.A},
            {"B", OSK_KeyCode.B},
            {"C", OSK_KeyCode.C},
            {"D", OSK_KeyCode.D},
            {"E", OSK_KeyCode.E},
            {"F", OSK_KeyCode.F},
            {"G", OSK_KeyCode.G},
            {"H", OSK_KeyCode.H},
            {"I", OSK_KeyCode.I},
            {"J", OSK_KeyCode.J},
            {"K", OSK_KeyCode.K},
            {"L", OSK_KeyCode.L},
            {"M", OSK_KeyCode.M},
            {"N", OSK_KeyCode.N},
            {"O", OSK_KeyCode.O},
            {"P", OSK_KeyCode.P},
            {"Q", OSK_KeyCode.Q},
            {"R", OSK_KeyCode.R},
            {"S", OSK_KeyCode.S},
            {"T", OSK_KeyCode.T},
            {"U", OSK_KeyCode.U},
            {"V", OSK_KeyCode.V},
            {"W", OSK_KeyCode.W},
            {"X", OSK_KeyCode.X},
            {"Y", OSK_KeyCode.Y},
            {"Z", OSK_KeyCode.Z},

            {"a", OSK_KeyCode.A},
            {"b", OSK_KeyCode.B},
            {"c", OSK_KeyCode.C},
            {"d", OSK_KeyCode.D},
            {"e", OSK_KeyCode.E},
            {"f", OSK_KeyCode.F},
            {"g", OSK_KeyCode.G},
            {"h", OSK_KeyCode.H},
            {"i", OSK_KeyCode.I},
            {"j", OSK_KeyCode.J},
            {"k", OSK_KeyCode.K},
            {"l", OSK_KeyCode.L},
            {"m", OSK_KeyCode.M},
            {"n", OSK_KeyCode.N},
            {"o", OSK_KeyCode.O},
            {"p", OSK_KeyCode.P},
            {"q", OSK_KeyCode.Q},
            {"r", OSK_KeyCode.R},
            {"s", OSK_KeyCode.S},
            {"t", OSK_KeyCode.T},
            {"u", OSK_KeyCode.U},
            {"v", OSK_KeyCode.V},
            {"w", OSK_KeyCode.W},
            {"x", OSK_KeyCode.X},
            {"y", OSK_KeyCode.Y},
            {"z", OSK_KeyCode.Z},
 
            //digits
            {"1", OSK_KeyCode.Alpha1},
            {"2", OSK_KeyCode.Alpha2},
            {"3", OSK_KeyCode.Alpha3},
            {"4", OSK_KeyCode.Alpha4},
            {"5", OSK_KeyCode.Alpha5},
            {"6", OSK_KeyCode.Alpha6},
            {"7", OSK_KeyCode.Alpha7},
            {"8", OSK_KeyCode.Alpha8},
            {"9", OSK_KeyCode.Alpha9},
            {"0", OSK_KeyCode.Alpha0},

            //common punctuations
            {"!", OSK_KeyCode.Exclaim},
            {"\"", OSK_KeyCode.DoubleQuote},
            {"#", OSK_KeyCode.Hash},
            {"$", OSK_KeyCode.Dollar},
            {"&", OSK_KeyCode.Ampersand},
            {"\'", OSK_KeyCode.Quote},
            {"(", OSK_KeyCode.LeftParen},
            {")", OSK_KeyCode.RightParen},
            {"*", OSK_KeyCode.Asterisk},
            {"+", OSK_KeyCode.Plus},
            {",", OSK_KeyCode.Comma},
            {"-", OSK_KeyCode.Minus},
            {".", OSK_KeyCode.Period},
            {"/", OSK_KeyCode.Slash},
            {":", OSK_KeyCode.Colon},
            {";", OSK_KeyCode.Semicolon},
            {"<", OSK_KeyCode.Less},
            {"=", OSK_KeyCode.Equals},
            {">", OSK_KeyCode.Greater},
            {"?", OSK_KeyCode.Question},
            {"@", OSK_KeyCode.At},
            {"[", OSK_KeyCode.LeftBracket},
            {"\\", OSK_KeyCode.Backslash},
            {"]", OSK_KeyCode.RightBracket},
            {"^", OSK_KeyCode.Caret},
            {"_", OSK_KeyCode.Underscore},
            {"`", OSK_KeyCode.BackQuote},
            {"~", OSK_KeyCode.Tilde},
            {"{", OSK_KeyCode.LeftCurlyBracket},
            {"}", OSK_KeyCode.RightCurlyBracket},
            {"|", OSK_KeyCode.Pipe},
            {"%", OSK_KeyCode.Percent},

            // ensure your font resource is capable of showing these characters
            { "≤", OSK_KeyCode.SmallerOrEqual },
            { "≥", OSK_KeyCode.GreaterOrEqual },
            { "≠", OSK_KeyCode.NotEqual },

            {"€", OSK_KeyCode.Euro },
            {"£", OSK_KeyCode.Sterling },
            {"¥", OSK_KeyCode.Yen },

            {"¢", OSK_KeyCode.Cent },

            {"\x0020", OSK_KeyCode.Space },

             // CONTROLS
            {"\x007F", OSK_KeyCode.Delete },
            {"\x0008", OSK_KeyCode.Backspace },
            {"\x000D", OSK_KeyCode.Return },

        };

        public Dictionary<string, int> altAlphabeticalAssignment = new Dictionary<string, int>();



        /// <summary>
        /// uses unicode to return the base character of an accented char
        /// </summary>
        /// <param name="accentedChar"></param>
        /// <returns></returns>
        public static string BaseCharacter(string accentedChar)
        {
            if (string.IsNullOrEmpty(accentedChar))
                return accentedChar;

            string normalizedString = accentedChar.Normalize(NormalizationForm.FormD);
            StringBuilder stringBuilder = new StringBuilder();

            foreach (char c in normalizedString)
            {
                UnicodeCategory unicodeCategory = CharUnicodeInfo.GetUnicodeCategory(c);
                if (unicodeCategory != UnicodeCategory.NonSpacingMark)
                {
                    stringBuilder.Append(c);
                }
            }

            return stringBuilder.ToString().Normalize(NormalizationForm.FormC);
        }

        public static bool IsAccentedCharacter(char c)
        {
            // Normalize the character to Form D (Canonical Decomposition)
            string decomposed = c.ToString().Normalize(NormalizationForm.FormD);

            // Check each character in the decomposed string
            foreach (char ch in decomposed)
            {
                UnicodeCategory category = CharUnicodeInfo.GetUnicodeCategory(ch);

                // If the character is a non-spacing mark, it's a combining diacritical mark
                if (category == UnicodeCategory.NonSpacingMark)
                {
                    // The character is accented
                    return true;
                }
            }

            // No combining marks found; the character is not accented
            return false;
        }


        Dictionary<string, OSK_KeyCode> keyCodeDict = GenKeyMapDict();

        public static Dictionary<string, OSK_KeyCode> GenKeyMapDict()
        {
            Dictionary<string, OSK_KeyCode> keyDict = new Dictionary<string, OSK_KeyCode>(StringComparer.InvariantCultureIgnoreCase);

            foreach (OSK_KeyCode keyCode in Enum.GetValues(typeof(OSK_KeyCode)))
            {
                if (!keyDict.ContainsKey(keyCode.ToString()))
                    keyDict.Add(keyCode.ToString(), keyCode);

            }

            return keyDict;
        }

        public static string GenKeyMapStr()
        {
            Dictionary<string, OSK_KeyCode> keyDict = new Dictionary<string, OSK_KeyCode>();
            string s = "{";

            foreach (OSK_KeyCode keyCode in Enum.GetValues(typeof(OSK_KeyCode)))
            {
                if (!keyDict.ContainsKey(keyCode.ToString()))
                    keyDict.Add(keyCode.ToString(), keyCode);
            }

            // Example usage: print all keycodes and their string representation
            foreach (var kvp in keyDict)
            {
                s += "{ \"" + kvp.Key + "\" , OSK_KeyCode." + kvp.Value + " },\n";
            }

            s += "}";
            return s;
        }

        public string AutoCorrectLayout(string layout)
        {
            // Split the layout into lines to handle row by row
            string[] rows = layout.Split('\n');
            List<string> correctedRows = new List<string>();

            foreach (var row in rows)
            {
                string correctedRow = AutoCorrectRow(row);
                correctedRows.Add(correctedRow);
            }

            return string.Join("\n", correctedRows).TrimEnd();
        }

        private string AutoCorrectRow(string row)
        {
            List<string> result = new List<string>();
            AutoCorrectRecursive(row, result);
            return string.Join(" ", result.Where(s => !string.IsNullOrEmpty(s)));
        }

        private void AutoCorrectRecursive(string input, List<string> result)
        {
            // Trim the input to remove leading and trailing spaces
            input = input.Trim();

            // Base case: if input is empty, return
            if (string.IsNullOrWhiteSpace(input))
            {
                return;
            }

            // Check if the input starts with "Skip" followed by a float (case-insensitive)
            Match skipMatch = Regex.Match(input, @"^skip\d*\.?\d*", RegexOptions.IgnoreCase);
            if (skipMatch.Success)
            {
                string skipKey = CapitalizeCorrectly(skipMatch.Value, "Skip");
                result.Add(skipKey);
                AutoCorrectRecursive(input.Substring(skipKey.Length).Trim(), result);
                return;
            }

            // Try to match the longest possible valid key (case-insensitive)
            for (int i = input.Length; i > 0; i--)
            {
                string currentPart = input.Substring(0, i).Trim();

                string correctedKey = GetCorrectedKey(currentPart);
                if (correctedKey != null)
                {
                    result.Add(correctedKey);
                    AutoCorrectRecursive(input.Substring(i).Trim(), result);
                    return;
                }
            }

            // If no valid key is found, consider the first character as a separate element
            result.Add(input[0].ToString());
            AutoCorrectRecursive(input.Substring(1).Trim(), result);
        }

        private string GetCorrectedKey(string key)
        {
            // Check in dictionaries if the key is valid, correcting capitalization if necessary
            string correctedKey = keyCodeDict.Keys.FirstOrDefault(k => string.Equals(k, key, StringComparison.OrdinalIgnoreCase));
            if (correctedKey != null)
            {
                return correctedKey;
            }

            if (key.Length == 1 && IsAccentedCharacter(key[0]))
            {
                return key;
            }

            correctedKey = chartoKeycode.Keys.FirstOrDefault(k => string.Equals(k, key, StringComparison.OrdinalIgnoreCase));
            return correctedKey;
        }

        private string CapitalizeCorrectly(string input, string correctForm)
        {
            // Replace the miscapitalized part with the correctly capitalized form
            return Regex.Replace(input, Regex.Escape(correctForm), correctForm, RegexOptions.IgnoreCase);
        }
    }
}