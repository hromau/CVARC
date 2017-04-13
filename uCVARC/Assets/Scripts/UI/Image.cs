using UnityEngine;

namespace UI
{
    public class Image : Element
    {
        public Image(Sprite sprite, Rect rect)
            : base("Image")
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
            image.type = UnityEngine.UI.Image.Type.Simple;
        }
    }
}