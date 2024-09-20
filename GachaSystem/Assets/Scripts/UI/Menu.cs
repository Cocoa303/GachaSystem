using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.EventSystems;

namespace UI
{
    public class Menu : Base
    {
        [SerializeField] Control.UI.Tap<string> controlTap;

        public override void Initialize()
        {
            controlTap.Initialize();
        }
    }
}
