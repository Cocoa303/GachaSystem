using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

namespace UI
{
    public class GachaReward : Util.Inherited.SingleFactoryObject<GachaReward>
    {
        [SerializeField] Text count;
        [SerializeField] Image icon;
        [SerializeField] Image cover;
        [SerializeField] GameObject newNotice;

        public void Set(string count, Sprite icon, Sprite cover, bool isNew)
        {
            this.count.text = count;
            this.icon.sprite = icon;
            this.cover.sprite = cover;

            if (isNew) 
            {
                newNotice.SetActive(true);
            }
            else
            {
                newNotice.SetActive(false);
            }
        }

        public override void Return()
        {
            returnFactory?.Invoke(this);
        }
    }
}

