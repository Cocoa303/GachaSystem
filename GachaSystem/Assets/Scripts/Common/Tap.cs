using UnityEngine;
using UnityEngine.UI;

namespace Common.UI
{
    [System.Serializable]
    public class Tap<TID,TTarget> where TID : System.IComparable<TID>
    {
        public TID id;
        public TTarget target;
        public Image background;
        public Text title;
        public Button trigger;

        public delegate void InteractionEvent(TID id);
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
