using System.Collections.Generic;
using Unity.VisualScripting;
using UnityEngine;
using UnityEngine.EventSystems;
using UnityEngine.UI;

namespace UI
{
    public class ItemPopup : UIBehaviour
    {
        [SerializeField] List<Util.Inspector.UniPair<string, Sprite>> icons;
        [SerializeField] Image icon;
        [SerializeField] Text value;
        [SerializeField] ItemSlot infoView;

        public void Open(long value, string key, Sprite itemicon, Sprite cover, string level, string hasCount, float gague, Color gradeColor)
        {
            int findIndex = icons.FindIndex((data) => data.key.CompareTo(key) == 0);
            if (findIndex != -1)
            {
                icon.sprite = icons[findIndex].value;
            }

            this.value.text = Util.Convert.NumberToUnitString(value);

            infoView.Init(itemicon, cover, level, hasCount, gague, gradeColor, null, null);
            this.gameObject.SetActive(true);
        }

        public void Close()
        {
            this.gameObject.SetActive(false);
        }
    }

}
