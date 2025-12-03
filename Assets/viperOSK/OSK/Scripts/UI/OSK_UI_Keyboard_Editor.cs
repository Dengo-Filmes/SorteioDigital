#if UNITY_EDITOR

namespace viperOSK
{

    using System.Collections;
    using System.Collections.Generic;
    using UnityEngine;
    using UnityEditor;




    [CustomEditor(typeof(OSK_UI_Keyboard))]
    [CanEditMultipleObjects]
    public class OSK_UI_Keyboard_Editor : UnityEditor.Editor
    {

        public override void OnInspectorGUI()
        {

            base.OnInspectorGUI();
            var osk = target as OSK_UI_Keyboard;

            osk.PrepAssetGroup();

            GUILayout.Space(20);
            EditorGUILayout.BeginHorizontal();
            EditorGUILayout.Space();

            //EditorGUI.BeginDisabledGroup(!EditorApplication.isPlaying);
            if (GUILayout.Button("Auto-Correct Layout", GUILayout.ExpandWidth(true), GUILayout.ExpandHeight(true), GUILayout.Height(30)))
            {
                GUIUtility.keyboardControl = 0;

                osk.AutoCorrectLayout();
                osk.Generate();

                EditorUtility.SetDirty(osk);


            }
            EditorGUILayout.Space();
            EditorGUILayout.EndHorizontal();

            EditorGUILayout.BeginHorizontal();
            {
                EditorGUILayout.Space();
                //EditorGUI.BeginDisabledGroup(!EditorApplication.isPlaying);
                if (GUILayout.Button("Generate", GUILayout.ExpandWidth(true), GUILayout.ExpandHeight(true), GUILayout.Height(30)))
                {
                    osk.Generate();
                }
                //EditorGUI.EndDisabledGroup();
                EditorGUILayout.Space();
            }
            EditorGUILayout.EndHorizontal();

            serializedObject.ApplyModifiedProperties();
            serializedObject.Update();
        }
    }



}

#endif
