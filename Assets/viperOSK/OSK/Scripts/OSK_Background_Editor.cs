#if UNITY_EDITOR

namespace viperOSK
{

    using System.Collections;
    using System.Collections.Generic;
    using UnityEngine;
    using UnityEditor;




    [CustomEditor(typeof(OSK_Background))]
    [CanEditMultipleObjects]
    public class OSK_Background_Editor : UnityEditor.Editor
    {

        public override void OnInspectorGUI()
        {

            base.OnInspectorGUI();
            var bg = target as OSK_Background;



            GUILayout.Space(20);

            EditorGUILayout.BeginHorizontal();
            {
                EditorGUILayout.Space();


                if (GUILayout.Button("Auto-Size", GUILayout.ExpandWidth(true), GUILayout.ExpandHeight(true), GUILayout.Height(30)))
                {
                    bg.ResizeToFit();
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