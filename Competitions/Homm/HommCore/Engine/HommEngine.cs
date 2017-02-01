using CVARC.V2;
using System;
using System.Collections.Generic;
using UnityEngine;


namespace HoMM.Engine {
    public class HommEngine : IHommEngine {
        //setspeed(enum), commonengine, coords, del color, коллайдеры, перейти на раунд, heroes
        private Dictionary<string, GameObject> prefabs = new Dictionary<string, GameObject>() {
            {"hex",  Resources.Load<GameObject>("hex")}
        };

        ICommonEngine commonEngine;
        private const float hexHeight = 1; // !

        public HommEngine(ICommonEngine commonEngine) {
            this.commonEngine = commonEngine;
        }

        public GameObject CreateObject(string id, MapObject mapObject, int x = 0, int y = 0) {
            GameObject obj = null;
            switch (mapObject) {
                case MapObject.Hexagon:
                    obj = GameObject.Instantiate<GameObject>(prefabs["hex"]);
                    obj.transform.Rotate(90, 0 ,0);
                    break;
                case MapObject.Mine: obj = GameObject.CreatePrimitive(PrimitiveType.Cube); break;
                case MapObject.Flag: obj = GameObject.CreatePrimitive(PrimitiveType.Sphere); break;
                case MapObject.Dwelling: obj = GameObject.CreatePrimitive(PrimitiveType.Capsule); break;
                case MapObject.NeutralArmy: obj = GameObject.CreatePrimitive(PrimitiveType.Cylinder); break;
                case MapObject.ResourcesPile: obj = GameObject.CreatePrimitive(PrimitiveType.Sphere); break;
                case MapObject.Hero: obj = GameObject.CreatePrimitive(PrimitiveType.Capsule);  break;
                default: obj = GameObject.CreatePrimitive(PrimitiveType.Cube); break;
            }
            obj.name = id;
            SetPosition(id, x, y);
            UnityEngine.Object.Destroy(obj.GetComponent(typeof(Collider)));
            
            return obj;
        }
        
        public void DeleteObject(string id) {
            GameObject.Destroy(GameObject.Find(id));
        }

        private static Dictionary<string, Color> playersColors = new Dictionary<string, Color>();

        public void CreatePlayers(string[] players) {
            playersColors[players[0]] = Color.red;
            playersColors[players[1]] = Color.blue;
            playersColors[""] = Color.gray;

            foreach(var name in players) {
                CreateObject(name, MapObject.Hero);
                SetColor(name, playersColors[name]);
                SetSize(name, 0.5f, 0.5f, 0.5f);
            }
        }

        public void SetFlag(string id, string owner) {
            var obj = GameObject.Find(id);
            var flagPosition = new Vector3(obj.transform.position.x,
                                           2,   // высота флага
                                           obj.transform.position.z);

            var flagId = id + " flag";
            var oldFlag = GameObject.Find(flagId);
            if (oldFlag != null)
                DeleteObject(flagId);

            CreateObject(flagId, MapObject.Flag);
            SetPosition(flagId, flagPosition);
            SetColor(flagId, playersColors[owner]);
            SetSize(flagId, 0.5f, 0.5f, 0.5f);
        }

        public void CreateHexagon(TerrainType terrain, int x, int y) {
            var hexId = string.Format("Tile {0} {1}", x, y);
            CreateObject(hexId, MapObject.Hexagon, x, y);
            
            var color = GetHexagonColor(terrain);
            SetColor(hexId, color);
        }

        private static Color GetHexagonColor(TerrainType terrain) {
            switch (terrain) {
                case TerrainType.Grass: return Color.green;
                case TerrainType.Road: return Color.grey;
                case TerrainType.Arid: return new Color32(0xDA, 0xA5, 0x20, 1);
                case TerrainType.Desert: return Color.yellow;
                case TerrainType.Marsh: return new Color32(0x00, 0x64, 0x00, 1);
                case TerrainType.Snow: return Color.white;

                default: return Color.black;
            }
        }

        public void SetPosition(string id, int x, int y) {
            SetPosition(id, new Vector3(x, 0, y).ToUnityBasis(hexHeight));
        }

        private void SetPosition(string id, Vector3 position) {
            GameObject.Find(id).transform.position = position;
        }

        private void SetColor(string id, Color color) {
            var obj = GameObject.Find(id);
            foreach (var mr in obj.transform.GetComponentsInChildren<MeshRenderer>())
                mr.material.color = color;
        }

        public void SetSize(string id, float x, float y, float z) {
            GameObject.Find(id).transform.localScale = new Vector3(x, y, z);
        }
        
        public void SetCamera(float w, float h) {
            var camera = GameObject.Find("Main Camera");
            //SetPosition("Main Camera", w / 2, 10, -h / 2);
            //camera.transform.Rotate(90, 0, 0);

            SetPosition("Main Camera", new Vector3(w / 2, 6, -(h + 3)));
            camera.transform.Rotate(45, 0, 0);
        }

        public void Move(string id, Direction direction, double duration) {
            /*var obj = GameObject.Find(id);
            obj.AddComponent<Rigidbody>();
            obj.GetComponent<Rigidbody>().useGravity = false;
            obj.GetComponent<Rigidbody>().isKinematic = true;
            var curpos = obj.transform.position;
            var targetCell = new Vector3();
            switch(direction) {
                case Directions.Up: targetCell = new Vector3(curpos.x, curpos.y, curpos.z + hexHeight); break;
            }
            var time = 1.0;
            var speed = new Vector3(targetCell.x - curpos.x, targetCell.y - curpos.y, targetCell.z - curpos.z);*/
            //speed.Normalize();
            //obj.GetComponent<Rigidbody>().velocity = speed;
            //commonEngine.SetAbsoluteSpeed(id, speed);
            //(())
            //while (time > 0) time -= Time.deltaTime;

            //obj.GetComponent<Rigidbody>().velocity = new Vector3(0, 0, 0);
            
        }
    }

    public static class CoordsConverter {
        public static Vector3 ToUnityBasis(this Vector3 pos, float hexSize = 1) {
            return new Vector3((3 * hexSize * pos.x) / (2 * (float)Math.Sqrt(3)),
                               0,
                               0 - (pos.z * hexSize + (pos.x % 2 == 0 ? 0 : hexSize / 2)));
        }
    }
}
