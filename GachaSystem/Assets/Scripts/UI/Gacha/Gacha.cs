﻿using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.EventSystems;
using UnityEngine.Pool;

using ItemType = Common.Enum.GachaRewardItemType;
using ItemGrade = Common.Enum.ItemGrade;


namespace UI
{
    public class Gacha : Base
    {
        #region Inventory Reference
        [SerializeField] Util.SingleFactory<ItemSlot> itemSlotF;
        [SerializeField] Transform itemSlotParent;
        [SerializeField, ReadOnly] List<ItemSlot> itemSlots;

        [SerializeField, ReadOnly] Util.Inspector.UniDictionary<ItemGrade, Color> gradeColor;

        [SerializeField] ItemPopup itemPopup;
        #endregion

        #region Trigger Reference
        [SerializeField] List<GachaTrigger> triggers;
        #endregion

        #region Reward Reference
        [SerializeField] Util.SingleFactory<GachaReward> rewardSlotF;
        [SerializeField] Transform rewardParent;
        [SerializeField, ReadOnly] List<GachaReward> rewardSlots;
        [SerializeField] GameObject rewardPanel;

        private WaitForSeconds wait = new WaitForSeconds(0.1f);
        private WaitForSeconds overDelay = new WaitForSeconds(0.2f);
        #endregion

        [SerializeField] Control.UI.Tap<ItemType> tapControl;

        Coroutine productionRoutine;

        private int requireGachaPrice = 0;

        public override void Initialize()
        {
            #region Tap Select 
            tapControl.InsertSelectCallback(ItemType.Weapon, (type) => { Setting(type); });
            tapControl.InsertSelectCallback(ItemType.Shield, (type) => { Setting(type); });
            tapControl.InsertSelectCallback(ItemType.Armor, (type) => { Setting(type); });
            #endregion

            requireGachaPrice = (int)Manager.Data.Instance.GlobalValue("n_RequireGachaPrice").value;

            for (int i = 0; i < triggers.Count; i++)
            {
                int count = triggers[i].Count;

                triggers[i].Set($"x {count}회\n{(count * requireGachaPrice):N0}", PressGachaTrigger);
            }

            Manager.Data.Instance.Values.InsertCallback("Money", UpdateGachaTrigger);
            UpdateGachaTrigger(Manager.Data.Instance.Values.Get("Money"));

            itemSlots = new List<ItemSlot>();
            rewardSlots = new List<GachaReward>();

            gradeColor = new Util.Inspector.UniDictionary<ItemGrade, Color>();
            gradeColor.Add(ItemGrade.Normal, Util.Convert.ArgbToColor(0xFF9DA8B6));
            gradeColor.Add(ItemGrade.Rare, Util.Convert.ArgbToColor(0xFF30AF52));
            gradeColor.Add(ItemGrade.Epic, Util.Convert.ArgbToColor(0xFF41AEEE));


            tapControl.Initialize();
        }

        private void Setting(ItemType type)
        {
            //== Serch
            List<Item> items = Manager.Data.Instance.GetAcquiredItems(type);
            if (itemSlots.Count < items.Count)
            {
                int difference = items.Count - itemSlots.Count;

                for (int i = 0; i < difference; i++)
                {
                    itemSlots.Insert(0, itemSlotF.Create(itemSlotParent));
                }
            }
            else if (items.Count < itemSlots.Count)
            {
                int difference = itemSlots.Count - items.Count;

                for (int i = 1; i <= difference; i++)
                {
                    itemSlots[itemSlots.Count - i].Return();
                }

                itemSlots.RemoveRange(itemSlots.Count - difference, difference);
            }

            for (int i = 0; i < itemSlots.Count; i++)
            {
                int levelUpNeedCount = Manager.Data.Instance.GetItemUpgradeNeedCount(items[i]);

                itemSlots[i].Init(
                    Manager.Resource.Instance.GetSprite(items[i].Data.atlasName, items[i].Data.iconName),
                    Manager.Resource.Instance.GetSprite("UI", $"ItemSlot_{items[i].Data.itemGrade.ToString()}"),
                    $"Lv.{items[i].Level}",
                    $"{items[i].HasCount} / {levelUpNeedCount}",
                    (float)items[i].HasCount / levelUpNeedCount,
                    gradeColor[items[i].Data.itemGrade],
                    items[i].Data.itemID,
                    OpenItemPopup
                    );
            }

            ListPool<Item>.Release(items);
        }

        private void OpenItemPopup(object id)
        {
            int itemID = (int)id;
            var item = Manager.Data.Instance.GetItem(itemID);
            int levelUpNeedCount = Manager.Data.Instance.GetItemUpgradeNeedCount(item);
            string key = Manager.Data.Instance.OptionKeyClassification(item.Data.itemOptionType);
            int stat = Manager.Data.Instance.GetItemStat(item);

            itemPopup.Open(
                    stat,
                    key,
                    Manager.Resource.Instance.GetSprite(item.Data.atlasName, item.Data.iconName),
                    Manager.Resource.Instance.GetSprite("UI", $"ItemSlot_{item.Data.itemGrade.ToString()}"),
                    $"Lv.{item.Level}",
                    $"{item.HasCount} / {levelUpNeedCount}",
                    (float)item.HasCount / levelUpNeedCount,
                    gradeColor[item.Data.itemGrade]
                );
        }

        private void PressGachaTrigger(int count)
        {
            if (productionRoutine != null) return;

            productionRoutine = StartCoroutine(GachaProduction(count, tapControl.CurrentOpenTap.id));
        }

        private IEnumerator GachaProduction(int count, ItemType type)
        {
            Dictionary<int, int> items = Manager.Data.Instance.CreateGachaItems(count, type);

            //== 보유중인 가챠 리워드 슬롯 모두 회수
            foreach (var slot in rewardSlots)
            {
                slot.Return();
            }
            rewardSlots.Clear();

            //== 아이템 삽입
            foreach (var itemID in items.Keys)
            {
                bool isAcquired = Manager.Data.Instance.IsAcquired(type, itemID);
                Item item = Manager.Data.Instance.GetItem(itemID);

                var slot = rewardSlotF.Create(rewardParent, 1, false, (slot) =>
                {
                    slot.Set($"<b>x {items[itemID]}</b>",
                        Manager.Resource.Instance.GetSprite(item.Data.atlasName, item.Data.iconName),
                        Manager.Resource.Instance.GetSprite("UI", $"ItemSlot_{item.Data.itemGrade.ToString()}"),
                        !isAcquired
                        );
                });


                rewardSlots.Add(slot);

                Manager.Data.Instance.InsertItem(item.Data.itemID, items[itemID]);
                slot.gameObject.SetActive(false);
            }

            //== 재화 감소
            Manager.Data.Instance.Values.Use("Money", count * requireGachaPrice);

            //== 아이템 정보 갱신
            Manager.Data.Instance.UpdateHasItemStat();

            rewardPanel.SetActive(true);

            //== 연출
            yield return overDelay;
            foreach (var slot in rewardSlots)
            {
                slot.gameObject.SetActive(true);
                slot.transform.SetAsLastSibling();

                Manager.Sound.Instance.PlaySound("GachaSlotOpen", -1, false);
                yield return wait;
            }

            yield return null;
            Setting(tapControl.CurrentOpenTap.id);
            productionRoutine = null;
        }

        public void UpdateGachaTrigger(long money)
        {
            foreach (var trigger in triggers)
            {
                int price = trigger.Count * requireGachaPrice;
                if (money < price)
                {
                    trigger.SetEnable(false);
                }
                else
                {
                    trigger.SetEnable(true);
                }
            }
        }
    }
}
