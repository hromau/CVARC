using UnityEngine;

namespace UI
{
    public class Panel : Element
    {
        public Panel(Sprite sprite, Rect rect)
            : base("Panel")
        {
            var trans = UiObject.AddComponent<RectTransform>();
            SetSize(trans, rect.size);
            SetAnchor(new Vector2(0.5f, 0.5f), new Vector2(0.5f, 0.5f));
            trans.anchoredPosition3D = new Vector3(0, 0, 0);
            trans.anchoredPosition = new Vector2(0, 0);
            trans.localPosition = rect.position;

            UiObject.AddComponent<CanvasRenderer>();

            var image = UiObject.AddComponent<UnityEngine.UI.Image>();
            image.sprite = sprite;
            image.type = UnityEngine.UI.Image.Type.Sliced;

            image.color = new Color32(240, 240, 240, 150);
        }
    }
}

