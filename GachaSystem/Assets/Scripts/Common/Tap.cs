using UnityEngine;
using UnityEngine.UI;

namespace Common.UI
{
    [System.Serializable]
    public class Tap<T>
    {
        public T id;
        public Image background;
        public Text title;
        public Button trigger;

        public delegate void InteractionEvent(T id);
        public InteractionEvent onSelect;
        public InteractionEvent disSelect;

        public void OnSelectEvent()
        {
            onSelect?.Invoke(id);
        }

        public void DisSelectEvent()
        {
            disSelect?.Invoke(id);
        }
    }
}
