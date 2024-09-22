using System.Collections;
using System.Collections.Generic;
using System.Text;
using UnityEngine;
using UnityEngine.Pool;
using ItemType = global::Data.GachaRandomBag.GachaRewardItemType;

namespace Manager
{
    public class Data : Util.Inherited.Singleton<Data>
    {
        private Dictionary<int, Item> items;                    //== 모든 아이템의 정보
        private Dictionary<ItemType, HashSet<int>> acquired;    //== 각 분류별 아이템의 소유 여부
        private Dictionary<ItemType, HashSet<int>> isLevelUp;   //== 각 분류별 아이템의 레벨업 여부

        private Dictionary<int, global::Data.GachaRandomBag> gachaRandomBags;
        private Dictionary<string, global::Data.GlobalValue> globalValue;

        private const string itemPrefsKey = "Item Data Prefs Key";
        private const char itemUnit = '|';
        private const char dataUnit = ',';

        private Control.Goods goods = new Control.Goods();
        public Control.Goods Goods { get => goods; private set => goods = value; }

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

            string pathFront = "Data/";
            string itemDataPath = pathFront + "ItemList";
            string gachaDataPath = pathFront + "GachaRandomBag";
            string globalDataPath = pathFront + "GlobalValue";

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

        /// <summary>
        /// 획득한적이 있는 아이템을 반환하는 함수
        /// </summary>
        /// <param name="type"> 탐색할 아이템 타입 </param>
        /// <returns>Pool 객체임으로, 사용 이후 Release 처리해 주시는 게 좋습니다.</returns>
        public List<Item> GetAcquiredItems(ItemType type)
        {
            List<Item> list = ListPool<Item>.Get();

            foreach(var item in items.Values)
            {
                if(item.Type == type)
                {
                    if (acquired[item.Type].Contains(item.Data.itemID))
                    {
                        list.Add(item);
                    }
                }
            }

            return list;
        }
    }
}
