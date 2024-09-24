using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

namespace UI
{
    public class GoodsView : Base
    {
        [SerializeField] Text view;
        [SerializeField] string key;
        [SerializeField] string format;

        public override void Initialize()
        {
            Manager.Data.Instance.Goods.InsertCallback(key, UpdateView);
            UpdateView(Manager.Data.Instance.Goods.Get(key));
        }

        private void UpdateView(long value)
        {
            view.text = string.Format(format, value);
        }
    }
}

