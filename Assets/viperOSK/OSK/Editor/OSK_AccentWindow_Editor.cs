
/////////////////////////////////////////////////////////////
/// viperOSK
/// Accented characters editor available through Unity Editor. Accessible via Tools > viperOSK Accented Characters Editor
/// 
/// 
/// © vipercode corp
/// 2022
/// all rights reserved
/// Please use this asset according to the attached license
/// Attributions, mentions and reviews are always welcomed
///
///////////////////////////////////////////////////////////////////////////////////////



using UnityEngine;
using UnityEditor;
using System.Collections.Generic;
using System.Text;
using System.Globalization;
using System.Linq;
using System;
//using Newtonsoft.Json;    //uncomment to use Json serialization/deserialization for accent packages saved in json file. Must have Newtonsoft package installed
using System.IO;

namespace viperOSK {

    public class OSK_AccentWindow_Editor : EditorWindow
    {
        private Dictionary<string, List<string>> accentDictionary = new Dictionary<string, List<string>>(StringComparer.OrdinalIgnoreCase);
        private string newBaseCharacter = "";

        private Dictionary<string, string> newAccentedCharacters = new Dictionary<string, string>();

        private Vector2 scrollPosition;

        int accentsPerRow = 5;
        int accentCount = 0;

        private OSK_AccentAssetObj accentMapAsset;

        // Store foldout states
        private Dictionary<string, bool> foldoutStates = new Dictionary<string, bool>();

        // Common diacritical marks (combining characters)
        private static readonly string[] diacriticalMarks = new string[]
        {
            "\u0300", // Grave `
            "\u0301", // Acute ´
            "\u0302", // Circumflex ^
            "\u0303", // Tilde ~
            "\u0304", // Macron ¯
            "\u0306", // Breve ˘
            "\u0307", // Dot above ˙
            "\u0308", // Diaeresis ¨
            "\u0309", // Hook above ̉
            "\u030A", // Ring above ˚
            "\u030B", // Double acute ˝
            "\u030C", // Caron ˇ
            "\u0327", // Cedilla ¸
            "\u0328", // Ogonek ˛
            "\u0331", // Macron below ˍ
                      // Add more diacritical marks as needed
        };

        [MenuItem("Tools/viperOSK Accented Characters Editor")]
        public static void ShowWindow()
        {
            GetWindow<OSK_AccentWindow_Editor>("viperOSK Accented Characters Editor");
        }

        // uncomment to user Json loader (requires Newtonsoft.Json package)
/*
        private void LoadConfigurationJson()
        {
            string path = EditorUtility.OpenFilePanel("Load Accent Configuration", Application.dataPath, "json");

            if (!string.IsNullOrEmpty(path))
            {
                string json = File.ReadAllText(path);
                accentDictionary = JsonConvert.DeserializeObject<Dictionary<string, List<string>>>(json);
                foldoutStates.Clear();
                newAccentedCharacters.Clear();

                // Initialize foldout states
                foreach (var baseChar in accentDictionary.Keys)
                {
                    foldoutStates[baseChar] = false;
                    if (!newAccentedCharacters.ContainsKey(baseChar))
                    {
                        newAccentedCharacters[baseChar] = "";
                    }
                }

                EditorUtility.DisplayDialog("Success", "Configuration loaded successfully.", "OK");
            }
        }
*/

        private void OnGUI()
        {
            GUILayout.Label("viperOSK Accented Characters Editor", EditorStyles.boldLabel);

            // Add new base character
            GUILayout.BeginHorizontal();
            GUILayout.Label("New Base Character:", GUILayout.Width(150));
            newBaseCharacter = GUILayout.TextField(newBaseCharacter, GUILayout.Width(50));

            if (GUILayout.Button("Add Base Character", GUILayout.Width(150)))
            {
                AddBaseCharacter();
            }
            GUILayout.EndHorizontal();

            GUILayout.Space(10);

            // Display the dictionary
            DisplayAccentDictionary();

            GUILayout.Space(10);

            // Save and Load Buttons
            GUILayout.BeginHorizontal();
            if (GUILayout.Button("Save Configuration"))
            {
                SaveConfiguration();
            }

            if (GUILayout.Button("Load Configuration"))
            {
                LoadConfiguration();
            }

            // uncomment to use Json loader (must have Json package installed)
            /*
            if (GUILayout.Button("Load json"))
            {
                LoadConfigurationJson();
            }
            */

            GUILayout.EndHorizontal();
        }

        private void AddBaseCharacter()
        {
            if (string.IsNullOrEmpty(newBaseCharacter))
            {
                EditorUtility.DisplayDialog("Error", "Base character cannot be empty.", "OK");
                return;
            }

            newBaseCharacter = newBaseCharacter.Trim();

            if (newBaseCharacter.Length != 1)
            {
                EditorUtility.DisplayDialog("Error", "Base character must be a single character.", "OK");
                return;
            }

            // Convert to lowercase for consistency
            newBaseCharacter = newBaseCharacter.ToLower();
            char baseChar = newBaseCharacter[0];

            if (!char.IsLetter(baseChar))
            {
                EditorUtility.DisplayDialog("Error", "Base character must be a letter (A-Z, a-z).", "OK");
                return;
            }

            if (OSK_Keymap.IsAccentedCharacter(baseChar))
            {
                EditorUtility.DisplayDialog("Error", "Base character cannot be an accented character.", "OK");
                return;
            }

            if (!accentDictionary.ContainsKey(newBaseCharacter))
            {
                accentDictionary.Add(newBaseCharacter, new List<string>());
                foldoutStates[newBaseCharacter] = true; // Initialize foldout state
                newAccentedCharacters.Add(newBaseCharacter, "");
                newBaseCharacter = "";
            }
            else
            {
                EditorUtility.DisplayDialog("Error", "Base character already exists.", "OK");
            }
        }

        private void DisplayAccentDictionary()
        {
            scrollPosition = EditorGUILayout.BeginScrollView(scrollPosition);

            List<string> keysToRemove = new List<string>();

            foreach (var baseChar in accentDictionary.Keys.OrderBy(key => key, StringComparer.OrdinalIgnoreCase))
            {
                GUILayout.BeginVertical("box");

                // Initialize foldout state if not present
                if (!foldoutStates.ContainsKey(baseChar))
                {
                    foldoutStates[baseChar] = false;
                }

                foldoutStates[baseChar] = EditorGUILayout.Foldout(foldoutStates[baseChar], "Base Character: " + baseChar );
    
            if (foldoutStates[baseChar])
                {
                    GUILayout.BeginHorizontal();
                    GUILayout.Space(20);
                    if (GUILayout.Button("Remove Base Character", GUILayout.Width(150)))
                    {
                        keysToRemove.Add(baseChar);
                    }
                    GUILayout.EndHorizontal();

                    GUILayout.Space(5);

                    // Display accented characters
                    GUILayout.BeginHorizontal();
                    GUILayout.Space(20);
                    GUILayout.BeginVertical();

                    List<string> accentedChars = accentDictionary[baseChar];
                    List<string> accentsToRemove = new List<string>();
                    accentCount = 0;
                    GUILayout.BeginHorizontal();
                    foreach (var accentedChar in accentedChars)
                    {
                        GUILayout.BeginVertical("box", GUILayout.Width(60));
                        GUILayout.Label(accentedChar, GUILayout.Width(50));

                        if (GUILayout.Button(new GUIContent("-", "Removes "+accentedChar+" from list"), GUILayout.Width(50)))
                        {
                            accentsToRemove.Add(accentedChar);
                        }
                        GUILayout.EndVertical();

                        accentCount++;

                        if (accentCount % accentsPerRow == 0)
                        {
                            GUILayout.EndHorizontal(); // End current row
                            GUILayout.BeginHorizontal(); // Start new row
                        }
                    }
                    GUILayout.EndHorizontal();

                    // Remove selected accented characters
                    foreach (var accentedChar in accentsToRemove)
                    {
                        accentedChars.Remove(accentedChar);
                    }

                    GUILayout.Space(5);

                    // Ensure there's an entry in the newAccentedCharacters dictionary
                    if (!newAccentedCharacters.ContainsKey(baseChar))
                    {
                        newAccentedCharacters[baseChar] = "";
                    }

                    // Add new accented character
                    GUILayout.BeginHorizontal();
                    GUILayout.Label("New Accented Character:", GUILayout.Width(150));
                    newAccentedCharacters[baseChar] = GUILayout.TextField(newAccentedCharacters[baseChar], GUILayout.Width(50));
                    if (GUILayout.Button(new GUIContent("+", "Adds a new accented character"), GUILayout.Width(50)))
                    {
                        AddAccentedCharacter(baseChar);
                    }
                    if (GUILayout.Button(new GUIContent("[+]", "Adds all accented characters"), GUILayout.Width(50)))
                    {
                        AddAllAccents(baseChar);
                    }
                    GUILayout.EndHorizontal();

                    GUILayout.EndVertical();
                    GUILayout.EndHorizontal();
                }

                GUILayout.EndVertical();
            }

            // Remove base characters marked for removal
            foreach (var key in keysToRemove)
            {
                accentDictionary.Remove(key);
                foldoutStates.Remove(key);
                newAccentedCharacters.Remove(key);
            }

            EditorGUILayout.EndScrollView();
        }

        private void AddAccentedCharacter(string baseChar)
        {
            string newAccentedCharacter = newAccentedCharacters[baseChar];

            if (string.IsNullOrEmpty(newAccentedCharacter))
            {
                EditorUtility.DisplayDialog("Error", "Accented character cannot be empty.", "OK");
                return;
            }


            newAccentedCharacter = newAccentedCharacter.Trim();

            if (newAccentedCharacter.Length != 1)
            {
                EditorUtility.DisplayDialog("Error", "Accented character must be a single character.", "OK");
                return;
            }


            List<string> accentedChars = accentDictionary[baseChar];

            if (!accentedChars.Contains(newAccentedCharacter))
            {
                accentedChars.Add(newAccentedCharacter);
                newAccentedCharacters[baseChar] = ""; // Clear the input field after adding
            }
            else
            {
                EditorUtility.DisplayDialog("Error", "Accented character already exists for this base character.", "OK");
            }
        }

        private void AddAllAccents(string baseChar)
        {
            List<string> generatedAccents = GenerateAccentedCharacters(baseChar);
            int cnt = 0;
            if (generatedAccents.Count > 0)
            {
                List<string> accentedChars = accentDictionary[baseChar];

                foreach (string accent in generatedAccents)
                {
                    // Avoid duplicates
                    if (!accentedChars.Contains(accent))
                    {
                        accentedChars.Add(accent);
                        cnt++;
                    }
                }

                if(cnt > 0)
                    EditorUtility.DisplayDialog("Success", $"Added {cnt} accented characters for '{baseChar}'.", "OK");
            }
            else
            {
                EditorUtility.DisplayDialog("Info", $"No accented characters could be generated for '{baseChar}'.", "OK");
            }
        }

        private List<string> GenerateAccentedCharacters(string baseChar)
        {
            List<string> accentedChars = new List<string>();

            foreach (string diacritic in diacriticalMarks)
            {
                string combined = baseChar + diacritic;
                string normalized = combined.Normalize(NormalizationForm.FormC);

                // Ensure the normalized character is different from the base character
                if (!normalized.Equals(baseChar, StringComparison.OrdinalIgnoreCase))
                {
                    // Avoid duplicates
                    if (!accentedChars.Contains(normalized))
                    {
                        accentedChars.Add(normalized);
                    }
                }
            }

            return accentedChars;
        }

        private void SaveConfiguration()
        {
            string path = EditorUtility.SaveFilePanelInProject("Save Accent Map", "AccentMap", "asset", "Please enter a file name to save the accent map to");

            if (!string.IsNullOrEmpty(path))
            {
                // Create a new AccentMapScriptableObject
                OSK_AccentAssetObj asset = ScriptableObject.CreateInstance<OSK_AccentAssetObj>();

                // Populate the asset with data from accentDictionary
                foreach (var baseChar in accentDictionary.Keys.OrderBy(key => key))
                {
                    AccentEntry entry = new AccentEntry();
                    entry.baseCharacter = baseChar;
                    entry.accentedCharacters = new List<string>(accentDictionary[baseChar]);
                    asset.entries.Add(entry);
                }

                // Save the asset
                AssetDatabase.CreateAsset(asset, path);
                AssetDatabase.SaveAssets();
                AssetDatabase.Refresh();

                EditorUtility.DisplayDialog("Success", "Accent map saved successfully.", "OK");
            }
        }

        private void LoadConfiguration()
        {
            string path = EditorUtility.OpenFilePanel("Load Accent Map", Application.dataPath, "asset");

            if (!string.IsNullOrEmpty(path))
            {
                // Convert the absolute path to a relative path
                string relativePath = "Assets" + path.Substring(Application.dataPath.Length);

                // Load the AccentMapScriptableObject
                accentMapAsset = AssetDatabase.LoadAssetAtPath<OSK_AccentAssetObj>(relativePath);

                if (accentMapAsset != null)
                {
                    // Clear the current data
                    accentDictionary.Clear();
                    foldoutStates.Clear();
                    newAccentedCharacters.Clear();

                    // Populate accentDictionary with data from the asset
                    foreach (var entry in accentMapAsset.entries)
                    {
                        accentDictionary[entry.baseCharacter] = new List<string>(entry.accentedCharacters);
                        foldoutStates[entry.baseCharacter] = false;
                        newAccentedCharacters[entry.baseCharacter] = "";
                    }

                    EditorUtility.DisplayDialog("Success", "Accent map loaded successfully.", "OK");
                }
                else
                {
                    EditorUtility.DisplayDialog("Error", "Failed to load accent map asset.", "OK");
                }
            }
        }



    }

}