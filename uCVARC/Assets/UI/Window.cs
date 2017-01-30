using UnityEngine;
using UnityEngine.UI;

namespace UI
{
    public class Window : Element
    {
        public bool IsOpen { get; private set; }
        public Element Head;
        private readonly Text title;

        public Window(Sprite sprite, string title, Rect rect)
        {
            IsOpen = true;
            Head = new Panel(sprite, rect) {UiObject = {name = string.Format("Window {0}", title)}};
            var trans = Head.UiObject.GetComponent<RectTransform>();
            Head.SetAnchor(new Vector2(0.5f, 0.5f), new Vector2(0.5f, 0.5f));

            trans.anchoredPosition3D = new Vector3(0, 0, 0);
            trans.anchoredPosition = new Vector2(0, 0);
            trans.localPosition = new Vector3(0, 0, 0);
            trans.localScale = new Vector3(1, 1, 1);
            SetSize(trans, rect.size);

            var header = new Panel(sprite, new Rect(0, 0, 0, 20));
            header.SetName("Header");
            header.UiObject.GetComponent<UnityEngine.UI.Image>().color = new Color32(230, 230, 230, 200);
            header.SetAnchor(new Vector2(0, 1), new Vector2(1, 1));
            header.SetPivot(.5f, 1);

            this.title = new Text(title);
            header.AddElement(this.title);

            var buttonClosed = new Button(() => SetActive(false), sprite, new Rect(0, 0, 64, 48), "X");
            var rectButton = buttonClosed.UiObject.GetComponent<RectTransform>();
            buttonClosed.SetAnchor(new Vector2(1, 1), new Vector2(1, 1));
            rectButton.sizeDelta = new Vector2(50, 20);
            rectButton.localPosition = new Vector3(-25, -10, 0);
            var buttonScript = buttonClosed.UiObject.GetComponent<UnityEngine.UI.Button>();
            buttonScript.transition = Selectable.Transition.ColorTint;
            var selColor = new ColorBlock();
            selColor.normalColor = buttonClosed.UiObject.GetComponent<UnityEngine.UI.Button>().colors.normalColor;
            selColor.pressedColor = new Color32(255, 80, 80, 255);
            selColor.highlightedColor = new Color32(255, 0, 0, 255);
            selColor.colorMultiplier = 1;
            selColor.fadeDuration = 0.2f;

            buttonClosed.UiObject.GetComponent<UnityEngine.UI.Button>().colors = selColor;


            UiObject = new Panel(sprite, rect).UiObject;
            var transM = UiObject.GetComponent<RectTransform>();

            SetAnchor(new Vector2(0, 0), new Vector2(1, 1));
            transM.anchoredPosition3D = new Vector3(0, 0, 0);
            transM.anchoredPosition = new Vector2(0, 0);

            transM.sizeDelta = new Vector2(0, 0);
            transM.localPosition = new Vector3(0, -20, 0);
            transM.offsetMin = new Vector2(transM.offsetMin.x, 0);

            transM.localScale = new Vector3(1, 1, 1);

            header.AddElement(buttonClosed);
            Head.AddElement(header);
            Head.AddElement(this);
        }

        public void SetActive(bool isActive)
        {
            Head.UiObject.SetActive(isActive);
            IsOpen = isActive;
        }

        public void SetTitle(string message)
        {
            title.SetText(message);
        }
    }
}