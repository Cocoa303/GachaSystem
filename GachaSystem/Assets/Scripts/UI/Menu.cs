using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.EventSystems;

namespace UI
{
    public class Menu : UIBehaviour, IUIInitialize
    {
        [SerializeField] Control.UI.Tap<string> controlTap;

        public void Initialize()
        {
            controlTap.Initialize();
        }
    }
}
