using System.Collections.Generic;
using UnityEngine;

namespace Control.UI
{
    [System.Serializable]
    public class Tap<T> : IUIInitialize where T : System.IComparable<T>
    {
        [SerializeField] List<Common.UI.Tap<T, GameObject>> uis;
        [SerializeField] T firstOpen;

        private Common.UI.Tap<T, GameObject> openTap;

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

        public Common.UI.Tap<T, GameObject> CurrentOpenTap { get => openTap; private set=> openTap = value; }

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

            if (firstOpen.CompareTo(default) != 0)
            {
                OnSelect(firstOpen);
            }
        }

        private void OnSelect(T id)
        {
            if (openTap == null)
            {
                int findIndex = uis.FindIndex((ui) => ui.id.CompareTo(id) == 0);
                if (findIndex != -1)
                {
                    SetSelectedTap(uis[findIndex]);
                    openTap = uis[findIndex];
                }
            }
            else
            {
                int findIndex = uis.FindIndex((ui) => ui.id.CompareTo(id) == 0);
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
            int findIndex = uis.FindIndex((ui) => ui.id.CompareTo(id) == 0);
            if (findIndex != -1)
            {
                SetUnselectedTap(uis[findIndex]);
                uis[findIndex].DisSelectEvent();
            }
        }

        private void SetSelectedTap(Common.UI.Tap<T, GameObject> tap)
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

            tap.target.SetActive(true);
        }
        private void SetUnselectedTap(Common.UI.Tap<T, GameObject> tap)
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

            tap.target.SetActive(false);
        }
    }
}
