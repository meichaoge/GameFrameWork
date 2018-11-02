using UnityEngine;
using System.Collections.Generic;

namespace UnityEngine.UI
{
    public static class SGDefaultControls
    {
        #region code from DefaultControls.cs
        public struct DefaultResources
        {
            public Sprite standard;
            public Sprite background;
            public Sprite inputField;
            public Sprite knob;
            public Sprite checkmark;
            public Sprite dropdown;
            public Sprite mask;
        }

        private const float kWidth = 160f;
        private const float kThickHeight = 30f;
        private const float kThinHeight = 20f;
        //private static Vector2 s_ThickElementSize = new Vector2(kWidth, kThickHeight);
        //private static Vector2 s_ThinElementSize = new Vector2(kWidth, kThinHeight);
        //private static Vector2 s_ImageElementSize = new Vector2(100f, 100f);
        //private static Color s_DefaultSelectableColor = new Color(1f, 1f, 1f, 1f);
        //private static Color s_PanelColor = new Color(1f, 1f, 1f, 0.392f);
        private static Color s_TextColor = new Color(50f / 255f, 50f / 255f, 50f / 255f, 1f);

        // Helper methods at top

        private static GameObject CreateUIElementRoot(string name, Vector2 size)
        {
            GameObject child = new GameObject(name);
            RectTransform rectTransform = child.AddComponent<RectTransform>();
            rectTransform.sizeDelta = size;
            return child;
        }

        static GameObject CreateUIObject(string name, GameObject parent)
        {
            GameObject go = new GameObject(name);
            go.AddComponent<RectTransform>();
            SetParentAndAlign(go, parent);
            return go;
        }

        private static void SetDefaultTextValues(Text lbl)
        {
            // Set text values we want across UI elements in default controls.
            // Don't set values which are the same as the default values for the Text component,
            // since there's no point in that, and it's good to keep them as consistent as possible.
            lbl.color = s_TextColor;
        }

        private static void SetDefaultColorTransitionValues(Selectable slider)
        {
            ColorBlock colors = slider.colors;
            colors.highlightedColor = new Color(0.882f, 0.882f, 0.882f);
            colors.pressedColor = new Color(0.698f, 0.698f, 0.698f);
            colors.disabledColor = new Color(0.521f, 0.521f, 0.521f);
        }

        private static void SetParentAndAlign(GameObject child, GameObject parent)
        {
            if (parent == null)
                return;

            child.transform.SetParent(parent.transform, false);
            SetLayerRecursively(child, parent.layer);
        }

        private static void SetLayerRecursively(GameObject go, int layer)
        {
            go.layer = layer;
            Transform t = go.transform;
            for (int i = 0; i < t.childCount; i++)
                SetLayerRecursively(t.GetChild(i).gameObject, layer);
        }
        #endregion
        
        public static GameObject CreateLoopHorizontalScrollRect(DefaultControls.Resources resources)
        {
            GameObject root = CreateUIElementRoot("LoopHorizontalScrollRect", new Vector2(200, 200));
            root.GetAddComponent<Image>().color = Color.gray;
            GameObject viewPort = CreateViewPort(root.transform);
            GameObject content = CreateUIObject("Content", viewPort);

            RectTransform contentRT = content.GetComponent<RectTransform>();
            contentRT.anchorMin = new Vector2(0, 1f);
            contentRT.anchorMax = new Vector2(1, 1f);
            contentRT.sizeDelta = new Vector2(0, 200);
            contentRT.pivot = new Vector2(0, 1f);

            // Setup UI components.

            LoopHorizontalScrollRect scrollRect = root.AddComponent<LoopHorizontalScrollRect>();
            scrollRect.content = contentRT;
            scrollRect.viewport = null;
            scrollRect.horizontalScrollbar = null;
            scrollRect.verticalScrollbar = null;
            scrollRect.horizontal = true;
            scrollRect.vertical = false;
            scrollRect.horizontalScrollbarVisibility = LoopScrollRect.ScrollbarVisibility.Permanent;
            scrollRect.verticalScrollbarVisibility = LoopScrollRect.ScrollbarVisibility.Permanent;
            scrollRect.horizontalScrollbarSpacing = 0;
            scrollRect.verticalScrollbarSpacing = 0;

            CreateHorizontalDefaultScrollBar(root.transform, scrollRect);
            CreateVerticalDefaultScrollBar(root.transform, scrollRect);


            Image img = viewPort.AddComponent<Image>();
            img.raycastTarget = true;
            Mask mask= viewPort.AddComponent<Mask>();
            mask.showMaskGraphic = false;


            HorizontalLayoutGroup layoutGroup = content.AddComponent<HorizontalLayoutGroup>();
            layoutGroup.childAlignment = TextAnchor.MiddleLeft;
            layoutGroup.childForceExpandWidth = false;
            layoutGroup.childForceExpandHeight = true;

            ContentSizeFitter sizeFitter = content.AddComponent<ContentSizeFitter>();
            sizeFitter.horizontalFit = ContentSizeFitter.FitMode.PreferredSize;
            sizeFitter.verticalFit = ContentSizeFitter.FitMode.Unconstrained;

            return root;
        }


        public static GameObject CreateLoopVerticalScrollRect(DefaultControls.Resources resources)
        {
            GameObject root = CreateUIElementRoot("LoopVerticalScrollRect", new Vector2(200, 200));
            root.GetAddComponent<Image>().color = Color.gray;
            GameObject viewPort = CreateViewPort(root.transform);
            GameObject content = CreateUIObject("Content", viewPort);
            
            RectTransform contentRT = content.GetComponent<RectTransform>();
            contentRT.anchorMin = new Vector2(0, 1f);
            contentRT.anchorMax = new Vector2(1, 1f);
            contentRT.sizeDelta = new Vector2(0, 200);
            contentRT.pivot = new Vector2(0, 1f);

            // Setup UI components.

            LoopVerticalScrollRect scrollRect = root.AddComponent<LoopVerticalScrollRect>();
            scrollRect.content = contentRT;
            scrollRect.viewport = null;
            scrollRect.horizontalScrollbar = null;
            scrollRect.verticalScrollbar = null;
            scrollRect.horizontal = false;
            scrollRect.vertical = true;
            scrollRect.horizontalScrollbarVisibility = LoopScrollRect.ScrollbarVisibility.Permanent;
            scrollRect.verticalScrollbarVisibility = LoopScrollRect.ScrollbarVisibility.Permanent;
            scrollRect.horizontalScrollbarSpacing = 0;
            scrollRect.verticalScrollbarSpacing = 0;

            CreateHorizontalDefaultScrollBar(root.transform, scrollRect);
            CreateVerticalDefaultScrollBar(root.transform, scrollRect);

            Image img= viewPort.AddComponent<Image>();
            img.raycastTarget = true;
            Mask mask = viewPort.AddComponent<Mask>();
            mask.showMaskGraphic = false;

            VerticalLayoutGroup layoutGroup = content.AddComponent<VerticalLayoutGroup>();
            layoutGroup.childAlignment = TextAnchor.UpperCenter;
            layoutGroup.childForceExpandWidth = true;
            layoutGroup.childForceExpandHeight = false;

            ContentSizeFitter sizeFitter = content.AddComponent<ContentSizeFitter>();
            sizeFitter.horizontalFit = ContentSizeFitter.FitMode.Unconstrained;
            sizeFitter.verticalFit = ContentSizeFitter.FitMode.PreferredSize;

            return root;
        }

        private static GameObject CreateViewPort(Transform target)
        {
            DrivenRectTransformTracker m_Tracker;
            GameObject go = new GameObject("Viewport");
            go.AddComponent<RectTransform>();
            go.transform.SetParent(target);
            RectTransform rectrans = go.transform as RectTransform;
            rectrans.anchorMin = Vector2.zero;
            rectrans.anchorMax = new Vector2(1, 1);
            rectrans.pivot = new Vector2(0, 1);
            rectrans.anchoredPosition = Vector2.zero;
            rectrans.sizeDelta = new Vector2(-17, -17);
            m_Tracker.Add(go, rectrans, DrivenTransformProperties.AnchoredPosition | DrivenTransformProperties.AnchorMin | DrivenTransformProperties.AnchorMax );

            return go;
        }

        /// <summary>
        /// 创建水平ScroolBar
        /// </summary>
        /// <param name="target"></param>
        /// <param name="loopScrollRect"></param>
        /// <returns></returns>
        private static GameObject CreateHorizontalDefaultScrollBar(Transform target, LoopScrollRect loopScrollRect)
        {
            DrivenRectTransformTracker m_Tracker;
            GameObject prefab = Resources.Load<GameObject>("LoopScrollRect/Scrollbar Horizontal");
            GameObject go = GameObject.Instantiate(prefab);
            go.name = prefab.name;
            go.transform.SetParent(target, false);
            go.transform.rotation = Quaternion.identity;
            go.transform.localScale = Vector3.one;
        

            RectTransform rectrans = go.transform as RectTransform;
            rectrans.anchorMin = Vector2.zero;
            rectrans.anchorMax = new Vector2(1, 0);
            rectrans.pivot = Vector2.zero;
            rectrans.anchoredPosition = Vector2.zero;
            rectrans.sizeDelta = new Vector2(-17,20);
            m_Tracker.Add(go, rectrans, DrivenTransformProperties.Anchors | DrivenTransformProperties.SizeDelta | DrivenTransformProperties.AnchoredPosition);


            Scrollbar horizongtialScrollBar = go.GetComponent<Scrollbar>();
            horizongtialScrollBar.GetComponent<Image>().color = new Color(127, 127, 127, 255) / 255f;
            loopScrollRect.horizontalScrollbar = horizongtialScrollBar;
            horizongtialScrollBar.size = 0.6f;
       //     loopScrollRect.horizontalScrollbarVisibility = LoopScrollRect.ScrollbarVisibility.Permanent;
            return go;
        }
        /// <summary>
        ///  创建垂直ScroolBar
        /// </summary>
        /// <param name="target"></param>
        /// <param name="loopScrollRect"></param>
        /// <returns></returns>
        private static GameObject CreateVerticalDefaultScrollBar(Transform target, LoopScrollRect loopScrollRect)
        {
            DrivenRectTransformTracker m_Tracker;
            GameObject prefab = Resources.Load<GameObject>("LoopScrollRect/Scrollbar Vertical");
            GameObject go =GameObject.Instantiate(prefab);
            go.name = prefab.name;
            go.transform.SetParent(target, false);
            go.transform.rotation = Quaternion.identity;
            go.transform.localScale = Vector3.one;

            RectTransform rectrans = go.transform as RectTransform;
            rectrans.anchorMin = new Vector2(1, 0);
            rectrans.anchorMax = new Vector2(1, 1);
            rectrans.pivot = Vector2.one;
            rectrans.anchoredPosition = Vector2.zero;
            rectrans.sizeDelta = new Vector2(20, -17);

            m_Tracker.Add(go, rectrans, DrivenTransformProperties.Anchors | DrivenTransformProperties.SizeDelta | DrivenTransformProperties.AnchoredPosition);


            Scrollbar verticalScrollBar = go.GetComponent<Scrollbar>();
            verticalScrollBar.GetComponent<Image>().color = new Color(127, 127, 127, 255) / 255f;
            loopScrollRect.verticalScrollbar = verticalScrollBar;
            //     loopScrollRect.verticalScrollbarVisibility = LoopScrollRect.ScrollbarVisibility.Permanent;
            verticalScrollBar.size = 0.6f;
            return go;
        }
    }
}
