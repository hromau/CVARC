using UnityEngine;
using UnityEngine.UI;

namespace UI
{
    public class List : Element
    {
        public Panel MainElement;

        public List(Sprite sprite, Rect rect)
            : base("List")
        {
            UiObject = new Panel(sprite, rect).UiObject;
            UiObject.AddComponent<Mask>();
            var scrollRect = UiObject.AddComponent<ScrollRect>();
            var panelIn = new Panel(sprite, new Rect(0, 0, 0, 200));
            var panelInRect = panelIn.UiObject.GetComponent<RectTransform>();
            panelInRect.pivot = new Vector2(0.5f, 1);
            panelIn.SetAnchor(new Vector2(0, 0), new Vector2(1, 1));
            panelIn.UiObject.AddComponent<VerticalLayoutGroup>();

            AddElement(panelIn);
            MainElement = panelIn;

            scrollRect.content = MainElement.UiObject.GetComponent<RectTransform>();

        }
    }
}