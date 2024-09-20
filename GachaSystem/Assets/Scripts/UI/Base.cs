using UnityEngine.EventSystems;

namespace UI
{
    public abstract class Base : UIBehaviour, IUIInitialize
    {
        public abstract void Initialize();
    }

}
