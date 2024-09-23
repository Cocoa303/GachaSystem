using System.Collections.Generic;
using UnityEngine;

namespace Control.UI
{
    [System.Serializable]
    public class Tap<T> : IUIInitialize
    {
        [SerializeField] List<Common.UI.Tap<T>> uis;
        [SerializeField] T firstOpen;

        private Dictionary<T, System.Action<T>> selectCallbacks = new Dictionary<T, System.Action<T>>();
        private Dictionary<T, System.Action<T>> unselectCallbacks = new Dictionary<T, System.Action<T>>();


        private Common.UI.Tap<T> openTap;

        #region Setting
        [Space]
        [SerializeField] bool backgroundChange;
        [SerializeField] bool titleChange;

        [SerializeField, ShowIf("backgroundChange")] bool backgroundSpriteChange;
        [SerializeField, ShowIf("backgroundChange")] bool backgroundColorChange;

        [Space]
        [SerializeField, ShowIf("titleChange")] bool titleFontChange;
        [SerializeField, ShowIf("titleChange")] bool titleSizeChange;
        [SerializeField, ShowIf("titleChange")] bool titleColorChange;
        #endregion

        #region Setting Data
        //== Background Sprite
        [SerializeField, ShowIf("backgroundSpriteChange")] Sprite selectBackgroundSprite;
        [SerializeField, ShowIf("backgroundSpriteChange")] Sprite unselectBackgroundSprite;

        //== Background Color
        [SerializeField, ShowIf("backgroundColorChange")] Color selectBackgroundColor;
        [SerializeField, ShowIf("backgroundColorChange")] Color unselectBackgroundColor;

        //== Title Font
        [SerializeField, ShowIf("titleFontChange")] Font selectTitleFont;
        [SerializeField, ShowIf("titleFontChange")] Font unselectTitleFont;

        //== Title Font Size
        [SerializeField, ShowIf("titleSizeChange")] int selectTitleSize;
        [SerializeField, ShowIf("titleSizeChange")] int unselectTitleSize;

        //== Title Color
        [SerializeField, ShowIf("titleColorChange")] Color selectTitleColor;
        [SerializeField, ShowIf("titleColorChange")] Color unselectTitleColor;

        #endregion

        public Common.UI.Tap<T> CurrentOpenTap { get => openTap; private set=> openTap = value; }

        public void Initialize()
        {
            foreach (var ui in uis)
            {
                ui.onSelect += OnSelect;
                ui.trigger.onClick.AddListener(ui.OnSelectEvent);
                if (backgroundChange || titleChange)
                {
                    SetUnselectedTap(ui);
                }
            }

            if (Comparer<T>.Default.Compare(firstOpen, default) != 0 || typeof(T).IsEnum)
            {
                OnSelect(firstOpen);
            }
        }
        public void InsertSelectCallback(T id, System.Action<T> callback)
        {
            if(selectCallbacks.ContainsKey(id))
            {
                selectCallbacks[id] += callback;
            }
            else
            {
                selectCallbacks.Add(id, callback);
            }
        }
        public void InsertUnselectCallback(T id, System.Action<T> callback)
        {
            if (unselectCallbacks.ContainsKey(id))
            {
                unselectCallbacks[id] += callback;
            }
            else
            {
                unselectCallbacks.Add(id, callback);
            }
        }

        private void OnSelect(T id)
        {
            if (openTap == null)
            {
                int findIndex = uis.FindIndex((ui) => Comparer<T>.Default.Compare(ui.id, id) == 0);
                if (findIndex != -1)
                {
                    SetSelectedTap(uis[findIndex]);
                    openTap = uis[findIndex];
                }
            }
            else
            {
                int findIndex = uis.FindIndex((ui) => Comparer<T>.Default.Compare(ui.id, id) == 0);
                if (findIndex != -1)
                {
                    SetUnselectedTap(openTap);

                    SetSelectedTap(uis[findIndex]);
                    openTap = uis[findIndex];
                }
            }
        }
        private void DisSelect(T id)
        {
            int findIndex = uis.FindIndex((ui) => Comparer<T>.Default.Compare(ui.id,id) == 0);
            if (findIndex != -1)
            {
                SetUnselectedTap(uis[findIndex]);
                uis[findIndex].DisSelectEvent();
            }
        }

        private void SetSelectedTap(Common.UI.Tap<T> tap)
        {
            if (backgroundChange)
            {
                if (backgroundSpriteChange && tap.background != null)
                {
                    tap.background.sprite = selectBackgroundSprite;
                }

                if (backgroundColorChange && tap.background != null)
                {
                    tap.background.color = selectBackgroundColor;
                }
            }

            if (titleChange)
            {
                if (titleFontChange && tap.title != null)
                {
                    tap.title.font = selectTitleFont;
                }

                if (titleSizeChange && tap.title != null)
                {
                    tap.title.fontSize = selectTitleSize;
                }

                if (titleColorChange && tap.title != null)
                {
                    tap.title.color = selectTitleColor;
                }
            }

            if(selectCallbacks.ContainsKey(tap.id))
            {
                selectCallbacks[tap.id]?.Invoke(tap.id);
            }
        }
        private void SetUnselectedTap(Common.UI.Tap<T> tap)
        {
            if (backgroundChange)
            {
                if (backgroundSpriteChange && tap.background != null)
                {
                    tap.background.sprite = unselectBackgroundSprite;
                }

                if (backgroundColorChange && tap.background != null)
                {
                    tap.background.color = unselectBackgroundColor;
                }
            }

            if (titleChange)
            {
                if (titleFontChange && tap.title != null)
                {
                    tap.title.font = unselectTitleFont;
                }

                if (titleSizeChange && tap.title != null)
                {
                    tap.title.fontSize = unselectTitleSize;
                }

                if (titleColorChange && tap.title != null)
                {
                    tap.title.color = unselectTitleColor;
                }
            }

            if (unselectCallbacks.ContainsKey(tap.id))
            {
                unselectCallbacks[tap.id]?.Invoke(tap.id);
            }
        }
    }
}
