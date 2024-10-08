﻿using System.Collections;
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
            Manager.Data.Instance.Values.InsertCallback("Save Money", OnSaveMoneyCallback);

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
            OnSaveMoneyCallback(Manager.Data.Instance.Values.Get("Save Money"));

            saveMoneyGetTrigger.onClick.AddListener(OnClickEvent);
        }

        private void OnClickEvent()
        {
            long money = Manager.Data.Instance.Values.Get("Save Money");
            Manager.Data.Instance.Values.Use("Save Money", money);
            Manager.Data.Instance.Values.Add("Money", money);
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
                if (saveMoneyGetTrigger.enabled == false)
                {
                    //== 비활성화 상태에서 활성화 되었을때 사운드 출력
                    Manager.Sound.Instance.PlaySound("RefillMoneyEnable", -1, false);
                }
                notice.SetActive(true);
                saveMoneyGetTrigger.enabled = true;
            }
        }
    }
}

