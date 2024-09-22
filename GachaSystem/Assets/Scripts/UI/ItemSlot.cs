using UnityEngine.UI;
using UnityEngine;

namespace UI
{
    public class ItemSlot : Util.Inherited.SingleFactoryObject<ItemSlot>
    {
        [SerializeField] private Image icon;
        [SerializeField] private Text level;
        [SerializeField] private Text hasCount;
        [SerializeField] private Image hasCountGauge;

        [SerializeField, ReadOnly] private object itemID;
        public delegate void OnClickCallback(object itemID);
        public OnClickCallback onClickCallback;

        public void Init(Sprite icon, string level, string hasCount, float gague,Color gaugeColor,object itemID, OnClickCallback onClickCallback)
        {
            this.icon.sprite = icon;
            this.level.text = level;
            this.hasCount.text = hasCount;
            this.hasCountGauge.fillAmount = gague;
            this.hasCountGauge.color = gaugeColor;
            this.itemID = itemID;
            this.onClickCallback = onClickCallback;
        }

        public void OnClickEvent()
        {
            onClickCallback?.Invoke(itemID);
        }

        public override void Return()
        {
            returnFactory?.Invoke(this);
        }
    }

}
