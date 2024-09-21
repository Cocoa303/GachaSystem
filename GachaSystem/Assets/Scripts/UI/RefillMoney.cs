using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.EventSystems;
using UnityEngine.UI;

namespace UI
{
    public class RefillMoney : UIBehaviour
    {
        [SerializeField] private Text remainSec;
        [SerializeField] private Image gague;
        [SerializeField] private Text saveMoney;
        [SerializeField] private Button saveMoneyGetTrigger;
        [SerializeField] private GameObject notice;

        protected override void Start()
        {
            Manager.Data.Instance.Goods.InsertCallback("Save Money", OnSaveMoneyCallback);

            Manager.Game.Instance.onChangeRefillMoneyInterval += (remain) =>
            {
                //== 2024.09.21 LHC
                //== NOTE : 소숫점 뒷자리중 0을 제거할 경우, 텍스트가 너무 어지럽게 연출되어 기능 삭제
                remainSec.text = remain.ToString("0.#") + "s";

                if (remain == 0) gague.fillAmount = 0;
                else
                {
                    float refillMoneyInterval = Manager.Data.Instance.GlobalValue("n_RefillMoneyInterval").value;
                    float result = 1 - (remain / refillMoneyInterval);
                    gague.fillAmount = result;
                }
            };

            //== 초기 세팅을 위함.
            OnSaveMoneyCallback(Manager.Data.Instance.Goods.Get("Save Money"));
        }

        private void OnSaveMoneyCallback(long saveMoney)
        {
            this.saveMoney.text = Util.Convert.NumberToUnitString(saveMoney);

            if (saveMoney < Manager.Data.Instance.GlobalValue("n_MinMoneyToGet").value)
            {
                //== unget
                notice.SetActive(false);
                saveMoneyGetTrigger.enabled = false;
            }
            else
            {
                //== getable
                notice.SetActive(true);
                saveMoneyGetTrigger.enabled = true;
            }
        }
    }
}

