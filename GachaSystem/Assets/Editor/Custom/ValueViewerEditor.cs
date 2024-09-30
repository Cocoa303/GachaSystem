using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace UnityEditor
{
    namespace Control
    {
        [CustomEditor(typeof(global::Control.ValueViewer))]
        public class ValueViewerEditor : Editor
        {
            public override void OnInspectorGUI()
            {
                var viewer = (global::Control.ValueViewer)target;

                EditorGUILayout.Space();
                GUILayout.Label("┌ Value Viewer UI 탐색 툴입니다.");
                GUILayout.Label("└ 입력하신 Target Group ID와 동일한 UI를 수정할수 있습니다.");
                EditorGUILayout.Space();

                if (GUILayout.Button("Load"))
                {
                    viewer.Load();
                }
                if (GUILayout.Button("Setting"))
                {
                    viewer.Setting();
                }
                EditorGUILayout.Space();

                base.OnInspectorGUI();
            }
        }
    }
}