using System.Collections;
using System.Collections.Generic;
using UnityEngine;

using Viewer = UI.ValueViewer;

namespace Control
{
    
    public class ValueViewer : MonoBehaviour
    {
#if UNITY_EDITOR
        [SerializeField] List<Viewer> viewers;
        [SerializeField] int targetGroupID;

        //== Setting Reference
        [Header("Setting")]
        [SerializeField] bool setFont;
        [SerializeField,ShowIf("setFont")] Font font;

        [Header("Style")]
        [SerializeField] bool setBold;
        [SerializeField] bool setItalic;
        [SerializeField, ShowIf("setBold")]   bool isBold;
        [SerializeField, ShowIf("setItalic")] bool isItalic;

        public void Load()
        {
            viewers = new List<Viewer>();

            var finds = FindObjectsOfType<Viewer>();

            foreach (Viewer viewer in finds)
            {
                if(viewer.editorGroupID == targetGroupID)
                {
                    viewers.Add(viewer);
                }
            }
        }

        public void Setting()
        {
            if (viewers.Count > 0)
            {
                foreach(Viewer viewer in viewers)
                {
                    if (setFont && font != null)
                    {
                        viewer.EditorOnlyGetView.font = font;
                    }
                    if(setBold)
                    {
                        viewer.EditorOnlySetBold = isBold;
                    }
                    if(setItalic)
                    {
                        viewer.EditorOnlySetItalic = isItalic;
                    }
                }
            }
        }
#endif
    }
}