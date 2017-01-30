using UnityEngine;
using UnityEngine.UI;

namespace UI
{
    public class ScrollBar : Element
    {
        public ScrollBar(Sprite sprite)
            : base("ScrollBar")
        {
            var trans = UiObject.AddComponent<RectTransform>();
            trans.sizeDelta = new Vector2(0, 0);

            SetSize(trans, new Vector2(20, 0));
            SetAnchor(new Vector2(1, 0), new Vector2(1, 1));
            trans.anchoredPosition3D = new Vector3(0, 0, 0);
            trans.anchoredPosition = new Vector2(0, 0);
            trans.localPosition = new Vector3(-10, 0, 0);
            trans.localScale = new Vector3(1f, 1f, 1.0f);

            UiObject.AddComponent<CanvasRenderer>();

            var slidingArea = new Element("Sliding Area");
            var rectSlidingArea = slidingArea.UiObject.AddComponent<RectTransform>();
            slidingArea.SetAnchor(new Vector2(0, 0), new Vector2(1, 1));
            rectSlidingArea.sizeDelta = new Vector2(0, 0);
            rectSlidingArea.localScale = new Vector3(1, 1, 1);

            var Handle = new Element("Handle");
            var rectHandle = Handle.UiObject.AddComponent<RectTransform>();
            SetSize(rectHandle, new Vector2(0, 0));
            rectHandle.anchorMin = new Vector2(0, 0.5f);
            rectHandle.anchorMax = new Vector2(1, 1);
            rectHandle.localScale = new Vector3(1f, 1f, 1f);

            Handle.UiObject.AddComponent<CanvasRenderer>();
            var imageHandle = Handle.UiObject.AddComponent<UnityEngine.UI.Image>();
            imageHandle.sprite = sprite;
            imageHandle.type = UnityEngine.UI.Image.Type.Sliced;
            slidingArea.AddElement(Handle);


            AddElement(slidingArea);

            var image = UiObject.AddComponent<UnityEngine.UI.Image>();
            image.sprite = sprite;
            image.type = UnityEngine.UI.Image.Type.Sliced;

            image.color = new Color32(120, 120, 120, 180);

            var scrollBarScript = UiObject.AddComponent<Scrollbar>();
            scrollBarScript.interactable = true;
            scrollBarScript.handleRect = rectHandle;
            scrollBarScript.direction = Scrollbar.Direction.BottomToTop;


        }
    }
}