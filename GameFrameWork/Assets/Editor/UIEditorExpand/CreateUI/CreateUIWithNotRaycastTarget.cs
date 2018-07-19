using System;
using System.Collections;
using System.Collections.Generic;
using UnityEditor;
using UnityEngine;
using UnityEngine.UI;
using TMPro;

namespace GameFrameWork.EditorExpand
{
    /// <summary>
    /// 创建取消勾选 RaycastTarget的UI
    /// </summary>
    public class CreateUIWithNotRaycastTarget : Editor
    {
        [MenuItem("GameObject/UI/Image")]
     private   static void CreatImage()
        {
            Canvas parentCanvas = GetCanvasParent(Selection.activeTransform);
            GameObject go;
            if (Selection.activeTransform == null)
                go = CreateGameWithTypeAndName("Image", typeof(Image), parentCanvas.transform);
            else
                go = CreateGameWithTypeAndName("Image", typeof(Image), Selection.activeTransform);

            go.GetComponent<Image>().raycastTarget = false;
            Selection.activeGameObject = go;
        }

        [MenuItem("GameObject/UI/Raw Image")]
        private static void CreatRawImage()
        {
            Canvas parentCanvas = GetCanvasParent(Selection.activeTransform);
            GameObject go;
            if (Selection.activeTransform == null)
                go = CreateGameWithTypeAndName("Raw Image", typeof(RawImage), parentCanvas.transform);
            else
                go = CreateGameWithTypeAndName("Raw Image", typeof(RawImage), Selection.activeTransform);

            go.GetComponent<RawImage>().raycastTarget = false;
            Selection.activeGameObject = go;
        }


        [MenuItem("GameObject/UI/Button")]
        private static void CreatButton()
        {
            Canvas parentCanvas = GetCanvasParent(Selection.activeTransform);
            GameObject go;
            if (Selection.activeTransform == null)
                go = CreateGameWithTypeAndName("Button", typeof(Image), parentCanvas.transform);
            else
                go = CreateGameWithTypeAndName("Button", typeof(Image), Selection.activeTransform);
            go.tag = "UI.Button";

            go.GetComponent<Image>().raycastTarget = true;
            Button btn= go.AddComponent<Button>();
            btn.transition = Button.Transition.None;

            RectTransform rect = go.transform as RectTransform;
            rect.sizeDelta = new Vector2(150, 50);

            //***创建Text
            GameObject textObj = CreateGameWithTypeAndName("Text", typeof(TextMeshProUGUI), go.transform);
            TextMeshProUGUI text = textObj.GetComponent<TextMeshProUGUI>();
            text.text = "text";
            text.alignment = TextAlignmentOptions.Center;
            RectTransform textRect = text.transform as RectTransform;
            textRect.anchoredPosition = new Vector2(0, 0);
            textRect.anchorMax = Vector2.one;
            textRect.anchorMin = Vector2.zero;
            textRect.sizeDelta = Vector2.zero;
            textRect.anchoredPosition3D = Vector3.zero;
            text.raycastTarget = false;
            text.fontSize = 24;
            Selection.activeGameObject = go;
        }

        [MenuItem("GameObject/UI/Text")]
        private static void CreatText()
        {
            Canvas parentCanvas = GetCanvasParent(Selection.activeTransform);
            GameObject go;
            if (Selection.activeTransform == null)
                go = CreateGameWithTypeAndName("Text", typeof(Text), parentCanvas.transform);
            else
                go = CreateGameWithTypeAndName("Text", typeof(Text), Selection.activeTransform);
            go.tag = "UI.Text";

            Text text = go.GetComponent<Text>();
            text.text = "text";
            text.alignment = TextAnchor.MiddleCenter;
            RectTransform textRect = text.transform as RectTransform;
            textRect.anchoredPosition = Vector2.one / 2f;
            textRect.anchorMax = Vector2.one / 2f;
            textRect.anchorMin = Vector2.one / 2f;
            textRect.sizeDelta = new Vector2(150, 50);
            textRect.anchoredPosition3D = Vector3.zero;
            text.raycastTarget = false;
            text.fontSize = 24;
            Selection.activeGameObject = go;
        }

        [MenuItem("GameObject/UI/TextMeshPro - Text")]
        private static void CreatTextMeshProUGUI()
        {
            Canvas parentCanvas = GetCanvasParent(Selection.activeTransform);
            GameObject go;
            if (Selection.activeTransform == null)
                go = CreateGameWithTypeAndName("TextMeshPro Text", typeof(TextMeshProUGUI), parentCanvas.transform);
            else
                go = CreateGameWithTypeAndName("TextMeshPro Text", typeof(TextMeshProUGUI), Selection.activeTransform);
            go.tag = "TMPro.TextMeshProUGUI";

            TextMeshProUGUI text = go.GetComponent<TextMeshProUGUI>();
            text.text = "text";
            text.alignment = TextAlignmentOptions.Center;
            RectTransform textRect = text.transform as RectTransform;
            textRect.anchoredPosition = Vector2.one / 2f;
            textRect.anchorMax = Vector2.one / 2f;
            textRect.anchorMin = Vector2.one / 2f;
            textRect.sizeDelta = new Vector2(150, 50);
            textRect.anchoredPosition3D = Vector3.zero;
            text.raycastTarget = false;
            text.fontSize = 24;
            Selection.activeGameObject = go;
        }


        private static Canvas GetCanvasParent(Transform selectTrans)
        {
            Canvas parentCanvas = null;
            if (selectTrans == null)
            {
                GameObject goCanvas = new GameObject("Canvas", typeof(Canvas));
                goCanvas.AddComponent<GraphicRaycaster>();
                goCanvas.AddComponent<CanvasScaler>();
                parentCanvas = goCanvas.GetComponent<Canvas>();
                parentCanvas.renderMode = RenderMode.ScreenSpaceOverlay;
            }
            else
            {
                parentCanvas = selectTrans.GetComponentInParent<Canvas>();
            }
            return parentCanvas;
        }

        private static GameObject CreateGameWithTypeAndName(string name, Type type, Transform parent)
        {
            GameObject go = new GameObject(name, type);
            go.transform.SetParent(parent);
            go.transform.localScale = Vector3.one;
            go.transform.localPosition = Vector3.zero;
            go.transform.localRotation = Quaternion.identity;

            return go;
        }





    }
}