using System.Collections.Generic;
using AIRLab.Mathematics;
using CVARC.V2;
using UnityEngine;
//using Demo;
using UnityCommons;

namespace Assets {
    class DemoWorldEngine //: IDemoWorldEngine
    {
        private Dictionary<string, GameObject> Forms = new Dictionary<string, GameObject>();
        private int counter = 0;
        private GameObject conePrefab = Resources.Load<GameObject>("Prefabs/Cone");
        private GameObject teaportPrefab = Resources.Load<GameObject>("Prefabs/Teaport");
        private ICommonEngine commonEngine;

        //public event Action<string, string> Collision;

        public DemoWorldEngine(ICommonEngine commonEngine)
        {
            this.commonEngine = commonEngine;
        }

        public string CreateForm(string ObjectId, Form3D form, Frame3D position, Frame3D size)
        {
            GameObject obj = null;
            switch (form)
            {
                case Form3D.Sphere: obj = GameObject.CreatePrimitive(PrimitiveType.Sphere); break;
                case Form3D.Cube: obj = GameObject.CreatePrimitive(PrimitiveType.Cube); break;
                case Form3D.Cylindre: obj = GameObject.CreatePrimitive(PrimitiveType.Cylinder); break;
                case Form3D.Cone: obj = GameObject.Instantiate(conePrefab); break;
                case Form3D.Teaport: obj = GameObject.Instantiate(teaportPrefab); break;
            }
            obj.name = ObjectId;
            Forms[ObjectId] = obj;
            SetPosition(ObjectId, position);
            SetSize(ObjectId, size);
            obj.AddComponent<Rigidbody>();

            return ObjectId;
        }

        public void Initialize() { }

        public void MoveOnVector(string id, Frame3D vector)
        {
            Forms[id].transform.Translate((float)vector.X, (float)vector.Y, (float)vector.Z);
        }


        public void SetPosition(string id, Frame3D position)
        {
            Forms[id].transform.position = new Vector3((float)position.X,
                                                       (float)(position.Y) + (Forms[id].transform.localScale.y / 2),
                                                       (float)position.Z);
        }

        public void SetRotation(string id, Angle xAngle, Angle yAngle, Angle zAngle)
        {
            Forms[id].transform.rotation = Quaternion.Euler((float)zAngle.Grad, (float)yAngle.Grad, (float)zAngle.Grad);
        }

        public void SetColor(string id, float red, float green, float blue)
        {
            Forms[id].GetComponent<Renderer>().material.color = new Color(red, green, blue);
        }
        public void SetSize(string id, Frame3D size)
        {
            var pos = Forms[id].transform.position;
            Forms[id].transform.localScale = new Vector3((float)size.X, (float)size.Y, (float)size.Z);
            SetPosition(id, new Frame3D(pos.x, pos.y, pos.z));
        }

        public void CreateEmptyMap()
        {
            Dispatcher.CurrentLog.Log("CreateEmptyMap");

            var floor = GameObject.CreatePrimitive(PrimitiveType.Plane);
            floor.transform.position = Vector3.zero;
            floor.transform.rotation = Quaternion.Euler(0, 180, 0);
            floor.transform.localScale = new Vector3(17 * Metrics.Pudge, 10, 17 * Metrics.Pudge) / 10;
            floor.GetComponent<Renderer>().material.mainTexture = Resources.Load<Texture>("GrassTexture");
            floor.GetComponent<Renderer>().material.mainTexture.wrapMode = TextureWrapMode.Repeat;
            floor.GetComponent<Renderer>().material.SetFloat("_Glossiness", 0);
            floor.name = "floor";

            var mainLight = GameObject.Find("Main Light");
            mainLight.transform.rotation = Quaternion.Euler(65, 10, 0);
            mainLight.GetComponent<Light>().intensity = 0.9f;
            mainLight.GetComponent<Light>().shadowStrength = 1;

            var camera = GameObject.Find("Camera");
            camera.transform.rotation = Quaternion.Euler(70, 270, 0);
            camera.transform.position = new Vector3(13, 29, 0);
        }
    }
}
