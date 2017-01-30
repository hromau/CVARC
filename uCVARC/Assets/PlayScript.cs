using UnityCommons;
using UnityEngine;

namespace Assets
{
    public abstract class PlayScript : MonoBehaviour
    {
        protected GUIText scoresTextLeft;
        protected GUIText scoresTextRight;
        protected GameObject myCamera;

        protected abstract void Initialization();

        void Start()
        {
            CreateLight();
            CreateCamera();
            CreateScoresFields();
            Initialization();
        }

        void CreateCamera()
        {
            myCamera = new GameObject("Camera");
            myCamera.AddComponent<Camera>();
            myCamera.AddComponent<GUILayer>();
            myCamera.AddComponent<AudioListener>();
            myCamera.transform.position = new Vector3(2 * Metrics.Meter, 2 * Metrics.Meter, 0);
            myCamera.transform.rotation = Quaternion.Euler(45, 270, 0);
        }

        void CreateLight()
        {
            var oldLight = GameObject.Find("Point light");
            if (oldLight != null) GameObject.Destroy(oldLight);

            var light = new GameObject("Main Light");
            light.AddComponent<Light>();
            light.GetComponent<Light>().type = LightType.Directional;
            light.GetComponent<Light>().shadows = LightShadows.Hard;
            light.GetComponent<Light>().shadowStrength = 0.7f;
            light.transform.position = new Vector3(0, 2 * Metrics.Meter, 0);
            light.transform.rotation = Quaternion.Euler(45, 160, 0);
        }

        void CreateScoresFields()
        {
            scoresTextLeft = new GameObject("LeftScoreText").AddComponent<GUIText>() as GUIText;
            scoresTextLeft.pixelOffset = new Vector2(1, 1);
            scoresTextLeft.text = "Left Scores: 0";
            scoresTextLeft.transform.position = new Vector3(0, 1, 0);
            scoresTextRight = new GameObject("RightScoreText").AddComponent<GUIText>() as GUIText;
            scoresTextRight.pixelOffset = new Vector2(2, 2);
            scoresTextRight.text = "Right Scores: 0";
            scoresTextRight.transform.position = new Vector3(0.88f, 1, 0);
        }
    }
}
