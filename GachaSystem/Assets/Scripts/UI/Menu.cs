using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.EventSystems;

namespace UI
{
    public class Menu : Base
    {
        [SerializeField] GameObject goodsUI;
        [SerializeField] GameObject gachaUI;

        [SerializeField] Control.UI.Tap<string> controlTap;

        public override void Initialize()
        {
            #region Active True
            controlTap.InsertSelectCallback("Goods UI", (_) =>
            {
                goodsUI.SetActive(true);
            });
            controlTap.InsertSelectCallback("Gacha UI", (_) =>
            {
                gachaUI.SetActive(true);
            });
            #endregion

            #region Active False
            controlTap.InsertUnselectCallback("Goods UI", (_) =>
            {
                goodsUI.SetActive(false);
            });
            controlTap.InsertUnselectCallback("Gacha UI", (_) =>
            {
                gachaUI.SetActive(false);
            });
            #endregion

            controlTap.Initialize();
        }
    }
}
