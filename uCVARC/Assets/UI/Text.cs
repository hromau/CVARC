using UnityEngine;

namespace UI
{
    public class Text : Element
    {
        private readonly UnityEngine.UI.Text text;

        public Text(string message, int fontSize = 12)
            : base("Text")
        {
            var trans = UiObject.AddComponent<RectTransform>();
            trans.sizeDelta.Set(0, 0);
            trans.anchoredPosition3D = new Vector3(0, 0, 0);
            trans.anchoredPosition = new Vector2(0, 0);
            trans.localScale = new Vector3(1.0f, 1.0f, 1.0f);
            trans.localPosition.Set(0, 0, 0);

            UiObject.AddComponent<CanvasRenderer>();

            text = UiObject.AddComponent<UnityEngine.UI.Text>();
            text.supportRichText = true;
            text.text = message;
            text.fontSize = fontSize;
            text.font = Resources.GetBuiltinResource(typeof(Font), "Arial.ttf") as Font;
            text.alignment = TextAnchor.MiddleCenter;
            text.horizontalOverflow = HorizontalWrapMode.Overflow;
            text.color = new Color(0, 0, 0);
        }

        public void SetText(string message)
        {
            text.text = message;
        }
    }
}
