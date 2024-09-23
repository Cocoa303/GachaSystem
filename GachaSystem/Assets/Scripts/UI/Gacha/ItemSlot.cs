using UnityEngine.UI;
using UnityEngine;

namespace UI
{
    public class ItemSlot : Util.Inherited.SingleFactoryObject<ItemSlot>
    {
        [SerializeField] private Image icon;
        [SerializeField] private Image cover;
        [SerializeField] private Text level;
        [SerializeField] private Image grade;
        [SerializeField] private Text hasCount;
        [SerializeField] private Image hasCountGauge;

        [SerializeField, ReadOnly] private object itemID;
        public delegate void OnClickCallback(object itemID);
        public OnClickCallback onClickCallback;

        public void Init(Sprite icon, Sprite cover, string level, string hasCount, float gague, Color gradeColor, object itemID, OnClickCallback onClickCallback)
        {
            this.icon.sprite = icon;
            this.cover.sprite = cover;
            this.level.text = level;
            this.hasCount.text = hasCount;
            this.hasCountGauge.fillAmount = gague;
            this.grade.color = gradeColor;
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
