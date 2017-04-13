using UnityEngine;

namespace UI
{
    public class InputField : Element
    {

        public InputField(string name, Sprite sprite, Rect rect)
            : base(name)
        {
            UiObject = new Panel(sprite, rect).UiObject;
            var inputScript = UiObject.AddComponent<UnityEngine.UI.InputField>();
            var text = new Text("");
            var placeholder = new Text("Enter Text...");
            text.SetAnchor(0, 0, 1, 1);
            placeholder.SetAnchor(0, 0, 1, 1);
            text.UiObject.GetComponent<UnityEngine.UI.Text>().alignment = TextAnchor.MiddleLeft;
            placeholder.UiObject.GetComponent<UnityEngine.UI.Text>().alignment = TextAnchor.MiddleLeft;
            placeholder.UiObject.GetComponent<UnityEngine.UI.Text>().fontStyle = FontStyle.Italic;
            placeholder.UiObject.GetComponent<UnityEngine.UI.Text>().color = Color.gray;

            var rectT = text.UiObject.GetComponent<RectTransform>();
            rectT.offsetMin = new Vector2(10, 5);
            rectT.offsetMax = new Vector2(-10, -5);

            rectT = placeholder.UiObject.GetComponent<RectTransform>();
            rectT.offsetMin = new Vector2(10, 5);
            rectT.offsetMax = new Vector2(-10, -5);

            AddElement(text);
            AddElement(placeholder);
            inputScript.placeholder = placeholder.UiObject.GetComponent<UnityEngine.UI.Text>();
            inputScript.textComponent = text.UiObject.GetComponent<UnityEngine.UI.Text>();
        }
    }
}
