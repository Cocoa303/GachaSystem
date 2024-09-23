using UnityEngine;
using UnityEngine.EventSystems;
using UnityEngine.UI;

namespace UI
{
    public class GachaTrigger : UIBehaviour
    {
        [SerializeField] private int count;
        [SerializeField] private Text notice;
        [SerializeField] private Image icon;
        [SerializeField] private GameObject eventLock;

        [SerializeField, ReadOnly] private Button trigger;

        public delegate void OnClickCallback(int count);
        public OnClickCallback onClickCallback;

        public int Count { get => count; private set => count = value; }

#if UNITY_EDITOR
        protected override void OnValidate()
        {
            trigger = GetComponent<Button>();
        }
#endif

        protected override void Start()
        {
            base.Start();

            trigger.onClick.AddListener(OnClickEvent);
        }

        public void Set(string notice, OnClickCallback onClickCallback)
        {
            this.notice.text = notice;
            this.onClickCallback = onClickCallback;
        }

        public void SetEnable(bool enable)
        {
            if (enable)
            {
                this.icon.color = Color.white;
                eventLock.SetActive(false);
            }
            else
            {
                this.icon.color = new Color(1, 1, 1, 0.7f);
                eventLock.SetActive(true);
            }
        }

        private void OnClickEvent()
        {
            onClickCallback?.Invoke(count);
        }
    }
}

