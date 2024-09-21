using System.Collections.Generic;
using UnityEngine;
using UnityEngine.EventSystems;

namespace Util.Production
{
    public class Text : UIBehaviour
    {
        [SerializeField] private UnityEngine.UI.Text ui;
        [SerializeField] private List<string> strings;

        [SerializeField] private float duration;
        [SerializeField, ReadOnly] private float currentTime;
        [SerializeField, ReadOnly] private int currentIndex;

        protected override void Start()
        {
            base.Start();
            currentIndex = 0;
            currentTime = 0;

            if(strings == null || strings.Count == 0)
            {
                //== Componet 삭제
                Destroy(this);
            }

            ui.text = strings[currentIndex];
        }

        private void Update()
        {
            currentTime += Time.deltaTime;
            if(duration <= currentTime)
            {
                currentTime %= duration;
                currentIndex = (currentIndex + 1)% strings.Count;

                ui.text = strings[currentIndex];
            }
        }
    }
}

