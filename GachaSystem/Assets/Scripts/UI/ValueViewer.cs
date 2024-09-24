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

