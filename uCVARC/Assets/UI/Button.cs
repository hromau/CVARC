using UnityEngine;

namespace UI
{
    public class Button : Element
    {
        public Button(UnityEngine.Events.UnityAction eventListner, Sprite sprite, Rect rect, string text = "Button")
            : base("Button")
        {
            var trans = UiObject.AddComponent<RectTransform>();
            SetSize(trans, rect.size);
            SetSize(trans, new Vector2(120, 32));
            trans.anchoredPosition3D = new Vector3(0, 0, 0);
            trans.anchoredPosition = new Vector2(0, 0);
            trans.localScale = new Vector3(1.0f, 1.0f, 1.0f);
            trans.localPosition = rect.position;

            UiObject.AddComponent<CanvasRenderer>();

            var image = UiObject.AddComponent<UnityEngine.UI.Image>();
            image.sprite = sprite;
            image.type = UnityEngine.UI.Image.Type.Sliced;

            var title = new Text(text);
            var titleRect = title.UiObject.GetComponent<RectTransform>();
            title.SetAnchor(new Vector2(0, 0), new Vector2(1, 1));
            titleRect.sizeDelta.Set(0, 0);
            titleRect.offsetMin = new Vector2(0, 0);
            titleRect.offsetMax = new Vector2(0, 0);
            titleRect.localScale = new Vector3(1, 1, 1);
            AddElement(title);

            var button = UiObject.AddComponent<UnityEngine.UI.Button>();
            button.interactable = true;
            button.onClick.AddListener(eventListner);

        }
    }
}
