using System.Collections;
using System.Collections.Generic;
using System.Text;
using UnityEngine;
using UnityEngine.Pool;

using ItemType = Common.Enum.GachaRewardItemType;
using ItemGrade = Common.Enum.GachaRewardGrade;

namespace Manager
{
    public class Data : Util.Inherited.Singleton<Data>
    {
        private Dictionary<int, Item> items;                    //== 모든 아이템의 정보
        private Dictionary<ItemType, HashSet<int>> acquired;    //== 각 분류별 아이템의 소유 여부
        private Dictionary<ItemType, HashSet<int>> isLevelUp;   //== 각 분류별 아이템의 레벨업 여부

        private Dictionary<int, global::Data.GachaRandomBag> gachaRandomBags;
        private Dictionary<ItemType, global::Data.GachaGradeInfo> gachaGradeInfos;
        private Dictionary<string, global::Data.GlobalValue> globalValue;
        private List<global::Data.ItemOptionUpgrade> itemOptionUpgrades;

        private const string itemPrefsKey = "Item Data Prefs Key";
        private const string moneyPrefsKey = "Money Value Prefs Key";
        private const string refillMoneyPrefsKey = "Refill Money Value Prefs Key";
        private const char itemUnit = '|';
        private const char dataUnit = ',';

        private Control.Value value = new Control.Value();
        public Control.Value Values { get => value; private set => this.value = value; }

        public void Start()
        {
            Init();
        }
        public void OnApplicationQuit()
        {
            Save();
        }
        private void Init()
        {
            #region 데이터 기본 초기화

            acquired = new Dictionary<ItemType, HashSet<int>>();
            isLevelUp = new Dictionary<ItemType, HashSet<int>>();

            var values = System.Enum.GetValues(typeof(ItemType));
            foreach (var value in values)
            {
                ItemType type = (ItemType)value;

                acquired.Add(type, new HashSet<int>());
                isLevelUp.Add(type, new HashSet<int>());
            }
            #endregion

            #region 데이터를 받아와 세팅 [ NOTE : 현재 클라이언트 내부 처리중 ]
            var itemDic = new Dictionary<int, global::Data.ItemList>();
            var itemTypeDic = new Dictionary<int, ItemType>();
            items = new Dictionary<int, Item>();
            gachaRandomBags = new Dictionary<int, global::Data.GachaRandomBag>();
            gachaGradeInfos = new Dictionary<ItemType, global::Data.GachaGradeInfo>();
            itemOptionUpgrades = new List<global::Data.ItemOptionUpgrade>();

            string pathFront = "Data/";

            //== PathFront + Class Name
            //== typeof를 사용하지 않는 이유 : namespace가 포함되어 나와 Data.ClassName으로 반환되기 때문
            //== namespace는 데이터 파일명과 클래스가 곂치는 경우가 개발 도중 종종 발생하기 때문에 namespace 사용
            string itemDataPath = pathFront + "ItemList";
            string gachaDataPath = pathFront + "GachaRandomBag";
            string globalDataPath = pathFront + "GlobalValue";
            string gachaGradeDataPath = pathFront + "GachaGradeInfo";
            string itemOptionUpgradePath = pathFront + "ItemOptionUpgrade";

            var findItemData = Resources.LoadAll<global::Data.ItemList>(itemDataPath);
            for (int i = 0; i < findItemData.Length; i++)
            {
                if (itemDic.ContainsKey(findItemData[i].itemID))
                {
                    Debug.LogError($"[ {findItemData[i].itemID} ] ID 값을 가진 아이템 데이터가 이미 존재합니다.\n" +
                        $"ItemList CSV를 확인해주세요.");
                    continue;
                }
                itemDic.Add(findItemData[i].itemID, findItemData[i]);
            }

            var findGachaData = Resources.LoadAll<global::Data.GachaRandomBag>(gachaDataPath);
            for (int i = 0; i < findGachaData.Length; i++)
            {
                if (gachaRandomBags.ContainsKey(findGachaData[i].dropID))
                {
                    Debug.LogError($"[ {findGachaData[i].dropID} ] ID 값을 가진 아이템 데이터가 이미 존재합니다.\n" +
                        $"GachaRandomBag CSV를 확인해주세요.");
                    continue;
                }
                gachaRandomBags.Add(findGachaData[i].dropID, findGachaData[i]);

                int length = findGachaData[i].gachaRewardID.Count;
                for (int j = 0; j < length; j++)
                {
                    if (itemTypeDic.ContainsKey(findGachaData[i].gachaRewardID[j])) continue;
                    itemTypeDic.Add(findGachaData[i].gachaRewardID[j], findGachaData[i].gachaRewardItemType[j]);
                }
            }

            var findGachaGradInfos = Resources.LoadAll<global::Data.GachaGradeInfo>(gachaGradeDataPath);
            for (int i = 0; i < findGachaGradInfos.Length; i++)
            {
                int bagID = findGachaGradInfos[i].gachaRandombagID;
                if (!gachaRandomBags.ContainsKey(bagID))
                {
                    Debug.LogError($"[ {bagID} ] Bag 값을 가진 아이템 데이터가 이미 존재합니다.\n" +
                        $"현재 [ 24.09.23 ] GachaGradeInfo와 GachaRandomBag는 1:1로 연동되어있습니다." +
                        $"GachaGradeInfo CSV를 확인해주세요.");
                }

                //== [ 24.09.23 ] 현재 GachaRandomBag은 동일한 종류의 아이템을 생성하고 있기 때문에 해당 방식을 채택합니다.
                //== 종합 아이템 뽑기등의 기능이 추가된다면 GachaRewardItemType을 추가하고, 0번 인덱스에 우선적으로 설정해주시기 바랍니다.
                ItemType keyType = gachaRandomBags[bagID].gachaRewardItemType[0];

                if (gachaGradeInfos.ContainsKey(keyType))
                {
                    Debug.LogError($"[ {keyType} ] 을 생성하는 데이터가 이미 존재합니다.\n" +
                        $"GachaGradeInfo, GachaRandomBag CSV를 확인해주세요.");
                    continue;
                }
                gachaGradeInfos.Add(keyType, findGachaGradInfos[i]);
            }

            var itemUpgradeOption = Resources.LoadAll<global::Data.ItemOptionUpgrade>(itemOptionUpgradePath);
            itemOptionUpgrades.AddRange(itemUpgradeOption);
            //== 낮은 순서대로 정렬
            itemOptionUpgrades.Sort((lvalue, rvalue) =>
            {
                if (lvalue.upgradeBelowLimit < rvalue.upgradeBelowLimit) return -1;
                else if (lvalue.upgradeBelowLimit > rvalue.upgradeBelowLimit) return 1;
                else return 0;
            });

            #region 전역으로 사용될 데이터
            globalValue = new Dictionary<string, global::Data.GlobalValue>();
            var findGlobalValue = Resources.LoadAll<global::Data.GlobalValue>(globalDataPath);
            for (int i = 0; i < findGlobalValue.Length; i++)
            {
                if (globalValue.ContainsKey(findGlobalValue[i].globalValueID))
                {
                    Debug.LogError($"[ {findGlobalValue[i].globalValueID} ] ID 값을 가진 아이템 데이터가 이미 존재합니다.\n" +
                        $"GlobalValue CSV를 확인해주세요.");
                    continue;
                }
                globalValue.Add(findGlobalValue[i].globalValueID, findGlobalValue[i]);
            }
            #endregion

            #region 저장된 데이터 호출
            string saveItemData = PlayerPrefs.GetString(itemPrefsKey, string.Empty);
            var saveItemDic = new Dictionary<int, (int hasCount, int level)>();
            if (saveItemData != string.Empty)
            {
                string[] packets = saveItemData.Split(itemUnit);

                if (packets.Length > 1)
                {
                    for (int i = 0; i < packets.Length; i++)
                    {
                        //== 0 : id
                        //== 1 : has count
                        //== 2 : level
                        string[] packet = packets[i].Split(dataUnit);

                        int id = int.Parse(packet[0]);
                        int hasCount = int.Parse(packet[1]);
                        int level = int.Parse(packet[2]);

                        if (!saveItemDic.ContainsKey(id))
                        {
                            saveItemDic.Add(id, (hasCount, level));
                        }
                        if (itemTypeDic.ContainsKey(id))
                        {
                            acquired[itemTypeDic[id]].Add(id);
                        }
                    }
                }
            }
            #endregion

            #region 머니 재화 획득 및 설정
            value.Add("Money", long.Parse(PlayerPrefs.GetString(moneyPrefsKey, ((long)GlobalValue("n_DefaultMoneyCount").value).ToString())));
            value.Add("Save Money", long.Parse(PlayerPrefs.GetString(refillMoneyPrefsKey, "0")));

            value.InsertCallback("Money", RealTimeSaveMoney);
            value.InsertCallback("Save Money", RealTimeSaveRefillMoney);
            #endregion

            #region Item 데이터 세팅
            foreach (var id in itemDic.Keys)
            {
                if (!items.ContainsKey(id))
                {
                    items.Add(id, new Item(
                        itemDic[id],
                        (saveItemDic.ContainsKey(id)) ? saveItemDic[id].hasCount : 0,
                        (saveItemDic.ContainsKey(id)) ? saveItemDic[id].level : 0,
                        itemTypeDic[id]
                        ));
                }
            }
            #endregion
            #endregion

            UpdateHasItemStat();
        }
        private void Save()
        {
            StringBuilder builder = new StringBuilder();

            foreach (var id in items.Keys)
            {
                Item item = items[id];

                if (acquired[item.Type].Contains(id))
                {
                    //== 0 : id
                    //== 1 : has count
                    //== 2 : level

                    builder.Append(id);
                    builder.Append(dataUnit);
                    builder.Append(item.HasCount);
                    builder.Append(dataUnit);
                    builder.Append(item.Level);
                    builder.Append(itemUnit);
                }
            }

            if (builder.Length > 0)
            {
                //== 마지막 itemUnit 삭제
                builder = builder.Remove(builder.Length - 1, 1);
            }

            PlayerPrefs.SetString(itemPrefsKey, builder.ToString());
        }
        private void RealTimeSaveMoney(long money)
        {
            PlayerPrefs.SetString(moneyPrefsKey, money.ToString());
        }
        private void RealTimeSaveRefillMoney(long money)
        {
            PlayerPrefs.SetString(refillMoneyPrefsKey, money.ToString());
        }
        #region Stat

        #endregion
        public global::Data.GlobalValue GlobalValue(string id)
        {
            if (globalValue.ContainsKey(id))
            {
                return globalValue[id];
            }
            else
            {
                Debug.LogError($"Can't find {id} in global value database\n");
                return null;
            }
        }

        public global::Data.GachaGradeInfo GetGachaGradeInfo(ItemType type)
        {
            if (gachaGradeInfos.ContainsKey(type))
            {
                return gachaGradeInfos[type];
            }
            else
            {
                return null;
            }
        }

        public global::Data.GachaRandomBag GetGachaRandomBag(int id)
        {
            if (gachaRandomBags.ContainsKey(id))
            {
                return gachaRandomBags[id];
            }
            else
            {
                return null;
            }
        }

        public Dictionary<int /* ID */, int /* Count */> CreateGachaItems(int count, ItemType type)
        {
            //== 아이템 개수만큼 랜덤백 뽑아오기
            var info = GetGachaGradeInfo(type);
            var gachaID = info.gachaRandombagID;

            var itemPool = DictionaryPool<ItemGrade, List<int>>.Get();
            var grades = System.Enum.GetValues(typeof(ItemGrade));

            foreach (var grade in grades)
            {
                itemPool.Add((ItemGrade)grade, ListPool<int>.Get());
            }

            #region Data Making
            var gacha = GetGachaRandomBag(gachaID);
            for (int i = 0; i < gacha.gachaRewardGrade.Count; i++)
            {
                itemPool[gacha.gachaRewardGrade[i]].Add(gacha.gachaRewardID[i]);
            }
            #endregion

            //== 뽑기 결과 생성
            Dictionary<int, int> result = new Dictionary<int, int>();
            for (int i = 0; i < count; i++)
            {
                ItemGrade grade = GetRandomGachaGrade(info);

                List<int> items = itemPool[grade];
                int select = Random.Range(0, items.Count);

                if (result.ContainsKey(items[select]))
                {
                    result[items[select]]++;
                }
                else
                {
                    result.Add(items[select], 1);
                }
            }

            foreach (var key in itemPool.Keys)
            {
                ListPool<int>.Release(itemPool[key]);
            }
            DictionaryPool<ItemGrade, List<int>>.Release(itemPool);

            return result;
        }

        /// <summary>
        /// 획득한적이 있는 아이템을 반환하는 함수
        /// </summary>
        /// <param name="type"> 탐색할 아이템 타입 </param>
        /// <returns>Pool 객체임으로, 사용 이후 Release 처리해 주시는 게 좋습니다.</returns>
        public List<Item> GetAcquiredItems(ItemType type)
        {
            List<Item> list = ListPool<Item>.Get();

            foreach (var item in items.Values)
            {
                if (item.Type == type)
                {
                    if (acquired[item.Type].Contains(item.Data.itemID))
                    {
                        list.Add(item);
                    }
                }
            }

            return list;
        }
        public bool IsAcquired(ItemType type, int id)
        {
            if(acquired.ContainsKey(type))
            {
                if (acquired[type].Contains(id))
                {
                    return true;
                }
                else
                {
                    return false;
                }
            }

            return false;
        }
        public Item GetItem(int id)
        {
            if (items.ContainsKey(id))
            {
                return items[id];
            }
            else
            {
                return null;
            }
        }
        public void InsertItem(int id, int count)
        {
            if(items.ContainsKey(id))
            {
                items[id].HasCount += count;
                acquired[items[id].Type].Add(id);
                CheckItemLevelUp(items[id]);
            }
            else
            {
#if UNITY_EDITOR
                Debug.LogError($"ID {id}값을 가지고있는 아이템이 존재하지 않습니다.\n" +
                    $"뽑기 테이블과 아이템 테이블이 일치 하지 않습니다.");
#endif
            }
        }

        #region Upgrade Reference
        private void CheckItemLevelUp(Item item)
        {
            int needCost = GetItemUpgradeNeedCount(item);

            if (needCost <= item.HasCount)
            {
                item.Level++;
                item.HasCount -= needCost;

                CheckItemLevelUp(item);
            }
        }

        public int GetItemUpgradeNeedCount(Item item)
        {
            for(int i = 0; i < itemOptionUpgrades.Count; i++)
            {
                if (item.Level <= itemOptionUpgrades[i].upgradeBelowLimit)
                {
                    return itemOptionUpgrades[i].upgradeCost;
                }
            }

            return (int)GlobalValue("n_DefaultUpgradeCost").value;
        }

        //== [ 24.09.24 ] Increase형태만 존재하기에 정수형 계산만 진행합니다.

        public int GetItemStat(Item item)
        {
            int stat = item.Data.defaultValue;
            int level = item.Level;

            System.Func<int, global::Data.ItemOptionUpgrade, int> OperationStat = (count,upgrade) =>
            {
                switch (item.Data.itemGrade)
                {
                    case Common.Enum.ItemGrade.Normal:
                        return count * upgrade.normalUpgradeValue;
                    case Common.Enum.ItemGrade.Rare:
                        return count * upgrade.rareUpgradeValue;
                    case Common.Enum.ItemGrade.Epic:
                        return count * upgrade.epicUpgradeValue;
                    default: return 0;
                }
            };

            for (int i = 0; i < itemOptionUpgrades.Count; i++)
            {
                int upgradeCount = itemOptionUpgrades[i].upgradeBelowLimit;

                //== 이전에 계산했던 회수만큼 제외.
                if (i != 0 /* Not first */)
                {
                    for (int reverse = i - 1 /* Prev */; 0 <= reverse; reverse--)
                    {
                        upgradeCount -= itemOptionUpgrades[reverse].upgradeBelowLimit;
                    }
                }
                if (level <= 0) break;

                if (upgradeCount <= level)
                {
                    stat += OperationStat(upgradeCount, itemOptionUpgrades[i]);
                }
                else
                {
                    stat += OperationStat(level, itemOptionUpgrades[i]);
                }

                //== 계산을 진행한 만큼 회수 제외
                level -= itemOptionUpgrades[i].upgradeBelowLimit;
            }

            //== 테이블 내의 데이터 제한 보다 높은 레벨 보유
            //== 마지막 증가값으로 남은 레벨 처리
            if(0 < level)
            {
                stat += OperationStat(level, itemOptionUpgrades[itemOptionUpgrades.Count - 1]);
            }

            return stat;
        }
        #endregion
        public void UpdateHasItemStat()
        {
            Dictionary<string, int> values = DictionaryPool<string, int>.Get();

            foreach(var key in acquired.Keys)
            {
                foreach(var itemID in acquired[key])
                {
                    Item item = items[itemID];
                    int stat = GetItemStat(item);
                    string optionKey = OptionKeyClassification(item.Data.itemOptionType);

                    if (!values.ContainsKey(optionKey))
                    {
                        values.Add(optionKey, 0);
                    }

                    values[optionKey] += stat;
                }
            }

            //== 능력치 총합 업데이트
            int combatPower = 0;
            foreach(var key in values.Keys)
            {
                this.value.Change(key, values[key]);
                combatPower += values[key];
            }
            value.Change("CombatPower", combatPower);
        }

        public string OptionKeyClassification(Common.Enum.ItemOptionType type)
        {
            switch (type)
            {
                case Common.Enum.ItemOptionType.AttackIncrease: return "Attack";
                case Common.Enum.ItemOptionType.DefenseIncrease: return "Defense";
                case Common.Enum.ItemOptionType.HpIncrease: return "Health";
                default: return string.Empty;
            }
        }

        private ItemGrade GetRandomGachaGrade(global::Data.GachaGradeInfo info)
        {
            int[] percent =
            {
                info.normalGachaRate,
                info.rareGachaRate,
                info.epicGachaRate
            };

            ItemGrade[] grade =
            {
                ItemGrade.Normal,
                ItemGrade.Rare,
                ItemGrade.Epic
            };

            int range = info.normalGachaRate + info.rareGachaRate + info.epicGachaRate;
            int random = Random.Range(0, range);
            for (int i = 0; i < percent.Length; i++)
            {
                random -= percent[i];
                if(random < 0)
                {
                    return grade[i];
                }
            }

            return ItemGrade.Normal;
        }
    }
}
