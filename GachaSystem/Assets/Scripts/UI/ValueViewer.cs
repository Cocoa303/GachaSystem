using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

namespace UI
{
    public class ValueViewer : Base
    {
        [SerializeField] Text view;
        [SerializeField] string key;

        //== Setting
        [SerializeField] bool isBold;
        [SerializeField] bool isItalic;

#if UNITY_EDITOR
        //== [ 24.09.30 ] NOTE
        //== Editor에서 사용되는 변수 / 접근자 입니다.
        //== 개발에 있어서 사용에 조심하여주시기 바랍니다.
        public int editorGroupID;
        public Text EditorOnlyGetView { get => view; }
        public bool EditorOnlySetBold { set => isBold = value; }
        public bool EditorOnlySetItalic { set => isItalic = value; }
#endif

        public override void Initialize()
        {
            Manager.Data.Instance.Values.InsertCallback(key, UpdateView);
            UpdateView(Manager.Data.Instance.Values.Get(key));
        }

        private void UpdateView(long value)
        {
            view.text =
                ((isBold) ? "<b>" : "") +
                ((isItalic) ? "<i>" : "") +
                Util.Convert.NumberToUnitString(value) +
                ((isItalic) ? "</i>" : "") +
                ((isBold) ? "</b>" : "");
        }
    }
}

