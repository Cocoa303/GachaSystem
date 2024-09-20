using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.EventSystems;
using UnityEngine.UI;

namespace Manager
{
    public class UI : Util.Inherited.Singleton<UI>
    {
        [SerializeField] private Canvas canvas;
        [SerializeField] private RectTransform uiParent;

        [SerializeField] private List<IUIInitialize> initializes;

        private void OnValidate()
        {
            #region Create
            //== Create the Main Canvas of the UI [initially once]
            if (canvas == null)
            {
                //== Canvas Create
                GameObject canvasObject = new GameObject("UI Main canvas");
                canvas = canvasObject.AddComponent<Canvas>();
                canvas.renderMode = RenderMode.ScreenSpaceOverlay;
                canvas.pixelPerfect = false;
                canvas.sortingOrder = 0;
                canvas.targetDisplay = 0;
                canvas.additionalShaderChannels = AdditionalCanvasShaderChannels.None;

                //== Canvas scaler set
                CanvasScaler scaler = canvasObject.AddComponent<CanvasScaler>();
                scaler.uiScaleMode = CanvasScaler.ScaleMode.ScaleWithScreenSize;
                scaler.referenceResolution = new Vector2(1280, 720);
                scaler.screenMatchMode = CanvasScaler.ScreenMatchMode.Expand;
                scaler.referencePixelsPerUnit = 100;

                //== Graphic raycaster set
                GraphicRaycaster raycaster = canvasObject.AddComponent<GraphicRaycaster>();
                raycaster.ignoreReversedGraphics = true;
                raycaster.blockingObjects = GraphicRaycaster.BlockingObjects.None;

                //== Transform set
                canvasObject.transform.SetParent(this.transform);
            }
            if (uiParent == null)
            {
                GameObject parent = new GameObject("UI Items", typeof(RectTransform));
                parent.transform.SetParent(canvas.transform);
                uiParent = parent.transform as RectTransform;

                uiParent.localScale = Vector3.one;
                uiParent.anchorMin = Vector3.zero;
                uiParent.anchorMax = Vector3.one;
                uiParent.offsetMin = Vector3.zero;
                uiParent.offsetMax = Vector3.zero;
            }

            EventSystem findEventSystem = GameObject.FindObjectOfType<EventSystem>(true);
            if (findEventSystem == null)
            {
                GameObject eventSystem = new GameObject("Event System");
                eventSystem.transform.SetParent(null);
                eventSystem.transform.localPosition = Vector3.zero;
                eventSystem.transform.localScale = Vector3.one;
                EventSystem system = eventSystem.AddComponent<EventSystem>();
                eventSystem.AddComponent<StandaloneInputModule>();
            }
            #endregion
        }

        private void Start()
        {
            for (int i = 0; i < initializes.Count; i++)
            {
                initializes[i].Initialize();
            }
        }
    }
}
