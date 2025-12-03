/// ///////////////////////////////////////////////////////////////////////////////////
/// © vipercode corp
/// 2022
/// Please use this asset according to the attached license
/// Attributions, mentions and reviews are always welcomed
///
///////////////////////////////////////////////////////////////////////////////////////


#if UNITY_EDITOR

namespace viperOSK
{

using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEditor;
    using System;
    using UnityEngine.Events;

    [CustomEditor(typeof(OSK_Keyboard))]
    [CanEditMultipleObjects]
    public class OSK_Keyboard_Editor : UnityEditor.Editor
    {

        public override void OnInspectorGUI()
        {

            base.OnInspectorGUI();
            var osk = target as OSK_Keyboard;


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
