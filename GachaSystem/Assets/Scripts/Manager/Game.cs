using System.Collections;
using System.Collections.Generic;
using UnityEngine;


namespace Manager
{
    public class Game : Util.Inherited.Singleton<Game>
    {
        [SerializeField, ReadOnly] float refillMoneyCurrentTime;
        [SerializeField, ReadOnly] float remainedMoney;
        [SerializeField, ReadOnly] bool isRefiilAble = true;

        public delegate void OnChangeValue<T>(T value);
        public OnChangeValue<float> onChangeRefillMoneyInterval;

        private float refillMoneyInterval;
        private float refillMoneyCount;
        private float maxMoneyLimit;

        private void Start()
        {
            refillMoneyInterval = Data.Instance.GlobalValue("n_RefillMoneyInterval").value;
            refillMoneyCount = Data.Instance.GlobalValue("n_RefillMoneyCount").value;
            maxMoneyLimit = Data.Instance.GlobalValue("n_MaxMoneyLimit").value;
            remainedMoney = 0.0f;

            Data.Instance.Values.InsertCallback("Save Money", OnSaveMoneyCallback);

            OnSaveMoneyCallback(Data.Instance.Values.Get("Save Money"));

            //== In Game Setting
            Sound.Instance.PlaySound("BGM", -1, true);
        }

        private void OnSaveMoneyCallback(long saveMoney)
        {
            if (saveMoney < maxMoneyLimit)
            {
                isRefiilAble = true;
            }
            else
            {
                isRefiilAble = false;
            }
        }

        private void Update()
        {
            if (!isRefiilAble) return;

            refillMoneyCurrentTime -= Time.deltaTime;
            if(refillMoneyCurrentTime <= 0)
            {
                refillMoneyCurrentTime = refillMoneyInterval;

                remainedMoney += refillMoneyCount;

                if (1 <= remainedMoney)
                {
                    int saveCount = (int)remainedMoney;
                    remainedMoney -= saveCount;

                    Data.Instance.Values.Add("Save Money", saveCount);
                }
            }

            onChangeRefillMoneyInterval?.Invoke(refillMoneyCurrentTime);
        }
    }
}

