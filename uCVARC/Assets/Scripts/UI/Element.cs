using System;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.EventSystems;

namespace UI
{
    public class Element
    {
        public GameObject UiObject;
        public readonly List<Element> Children;

        public Element(string name)
        {
            UiObject = new GameObject(name) {layer = LayerMask.NameToLayer("UI")};
            SetAnchor(0.5f, 0.5f, 0.5f, 0.5f);
            Children = new List<Element>();
        }


        public Element()
            : this("UiObject")
        {
        }

        public void SetName(string name)
        {
            UiObject.name = name;
        }

        public static void InitUi()
        {
            var es = new GameObject("Event System");
            es.AddComponent<EventSystem>();
            es.AddComponent<StandaloneInputModule>();
        }


        public void SetAnchor(Vector2 min, Vector2 max)
        {
            try
            {
                var rect = UiObject.GetComponent<RectTransform>();
                rect.anchorMin = min;
                rect.anchorMax = max;
            }
            catch (Exception)
            {
                // ignored
            }
        }

        public void SetAnchor(float minX, float minY, float maxX, float maxY)
        {
            SetAnchor(new Vector2(minX, minY), new Vector2(maxX, maxY));
        }

        public void SetPivot(Vector2 pos)
        {
            var rect = UiObject.GetComponent<RectTransform>();
            rect.pivot = pos;
        }

        public void SetPivot(float x, float y)
        {
            SetPivot(new Vector2(x, y));
        }

        public void AddElement(Element element)
        {
            var oldTransform = element.UiObject.GetComponent<RectTransform>();
            var size = oldTransform.sizeDelta;
            var oldPos = oldTransform.anchoredPosition;
            var offsMin = oldTransform.offsetMin;
            var offsMax = oldTransform.offsetMax;
            var scale = oldTransform.localScale;

            oldTransform.SetParent(UiObject.transform);
            oldTransform.sizeDelta = size;
            oldTransform.anchoredPosition = oldPos;
            oldTransform.offsetMin = offsMin;
            oldTransform.offsetMax = offsMax;
            oldTransform.localScale = scale;

            Children.Add(element);
        }

        public static void SetSize(RectTransform trans, Vector2 size)
        {
            var currSize = trans.rect.size;
            var sizeDiff = size - currSize;
            trans.offsetMin = trans.offsetMin -
                              new Vector2(sizeDiff.x * trans.pivot.x,
                                  sizeDiff.y * trans.pivot.y);
            trans.offsetMax = trans.offsetMax +
                              new Vector2(sizeDiff.x * (1.0f - trans.pivot.x),
                                  sizeDiff.y * (1.0f - trans.pivot.y));
        }
    }
}