using System.Linq;
using UnityEngine;
using UnityEngine.UI;

namespace UI
{
    public class Canvas : Element
    {
        private readonly UnityEngine.Canvas canvas;

        public Canvas(Vector2 scale)
            : base("Canvas")
        {

            canvas = UiObject.AddComponent<UnityEngine.Canvas>();

            canvas.pixelPerfect = true;
            var canvasScal = UiObject.AddComponent<CanvasScaler>();
            var camera = Camera
                .allCameras
                .FirstOrDefault();
            if (camera != null)
                SetCamera(camera);
            else
                canvas.renderMode = RenderMode.ScreenSpaceOverlay;
        
            canvasScal.uiScaleMode = CanvasScaler.ScaleMode.ConstantPixelSize;
            canvasScal.referenceResolution = scale;

            UiObject.AddComponent<GraphicRaycaster>();
        }

        public void SetCamera(Camera camera)
        {
            canvas.renderMode = RenderMode.ScreenSpaceCamera;
            canvas.worldCamera = camera;
        }
    }
}
