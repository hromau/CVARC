using System;
using System.Collections.Generic;
using System.Linq;
using AIRLab.Mathematics;
using CVARC.V2;
using Pudge.Player;
using Pudge.World;
using UnityCommons;
using UnityEngine;
using PrefabLoader = UnityCommons.PrefabLoader;

namespace Pudge.RunningBinding.FromUnity
{
    public class PudgeWorldEngine : IPudgeWorldEngine
    {
        private readonly ICommonEngine _commonEngine;

        private readonly GameObject _treePrefab;
        private readonly GameObject _wardPrefab;
        private readonly GameObject _hookPrefab;
        private readonly GameObject _daggerExplosionPrefab;
        private readonly GameObject _pudgePrefab;
        private readonly GameObject _slardarPrefab;
        private readonly Texture _grassTexture;

        public PudgeWorldEngine(ICommonEngine commonEngine)
        {
            _commonEngine = commonEngine;

            _treePrefab = PrefabLoader.GetPrefab<GameObject>("pudge", "Tree");
            _wardPrefab = PrefabLoader.GetPrefab<GameObject>("pudge", "Ward");
            _hookPrefab = PrefabLoader.GetPrefab<GameObject>("pudge", "Hook");
            _daggerExplosionPrefab = PrefabLoader.GetPrefab<GameObject>("pudge", "DaggerExplosion");
            _pudgePrefab = PrefabLoader.GetPrefab<GameObject>("pudge", "PudgePrefab");
            _slardarPrefab = PrefabLoader.GetPrefab<GameObject>("pudge", "Slardar");
            _grassTexture = PrefabLoader.GetPrefab<Texture>("pudge", "GrassTexture");
        }

        [ToLog]
        public void CreateEmptyMap()
        {
            this.Log("CreateEmptyMap");

            var floor = GameObject.CreatePrimitive(PrimitiveType.Plane);
            floor.transform.position = Vector3.zero;
            floor.transform.rotation = Quaternion.Euler(0, 180, 0);
            floor.transform.localScale = new Vector3(17 * Metrics.Pudge, 10, 17 * Metrics.Pudge) / 10;
            var material = new Material(Shader.Find("Diffuse")) {mainTexture = _grassTexture};
            floor.GetComponent<Renderer>().material = material;
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

        /*private void CreateForestRect(int top, int bottom, int left, int right, System.Random random)
        {
            for (int x = top; x < bottom * Metrics.Pudge; x += (int)Metrics.Pudge)
                for (int z = left; z < right * Metrics.Pudge; z += (int)Metrics.Pudge)
                    CreateTree(x, z, (float)random.NextDouble() * 360);
        }*/

        public void CreateTree(int x, int z, float angle)
        {
            this.Log("CreateTree", x, z, angle);

            var tree = GameObject.Instantiate<GameObject>(_treePrefab);
            tree.transform.rotation = Quaternion.Euler(new Vector3(0, angle, 0));
            tree.transform.position = new Vector3(x, 0, z);
        }

        [ToLog]
        public void PlayAnimation(string actorId, Pudge.Animation animation)
        {
            this.Log("PlayAnimation", actorId, animation);

            var actor = GameObject.Find(actorId);
            if (actor != null) actor.GetComponent<Animator>().Play(animation.ToString());
        }

        [ToLog]
        public void SetTransparent(string actorId, bool isTransparent)
        {
            this.Log("SetTransparent", actorId, isTransparent);

            var actor = GameObject.Find(actorId);

            if (actor == null) return;

            var pudgeBodyParts = actor
                .GetComponentsInChildren<Renderer>()
                .Where(x => x.name != "Marker");

            foreach (var pudgePart in pudgeBodyParts)
                pudgePart.material.color = new Color(1, 1, 1, isTransparent ? 0.4f : 1);
        }

        [ToLog]
        public void TurnIntoCorpse(string actorId, string corpseId)
        {
            this.Log("TurnIntoCorpse", actorId, corpseId);

            var actorBody = ObjectsCache.FindGameObject(actorId);

            GameObject.Destroy(actorBody.GetComponent<Collider>());
            actorBody.GetComponent<Animator>().Play("Death");
            actorBody.name = corpseId;

            foreach (Transform child in actorBody.transform)
            {
                if (child.gameObject.name == "Marker")
                    GameObject.Destroy(child.gameObject);
            }

            _commonEngine.SetAbsoluteSpeed(corpseId, Frame3D.Identity);
        }

        

        [ToLog]
        public void SpawnHook(double x, double y, double yawGrad, string id)
        {
            this.Log("SpawnHook", x, y, yawGrad, id);

            var location = new Frame3D(x, y, 13, Angle.Zero, Angle.FromGrad(yawGrad), Angle.Zero).ToUnityBasis();

            var hook = GameObject.Instantiate<GameObject>(_hookPrefab);
            hook.transform.position = new Vector3((float)location.X, (float)location.Y, (float)location.Z);
            hook.transform.rotation = Quaternion.Euler(new Vector3((float)location.Pitch.Grad + 90,
                (float)location.Yaw.Grad, (float)location.Roll.Grad));
            hook.name = id;
        }

        private Dictionary<RuneType, GameObject> runesPrefabs = new Dictionary<RuneType, GameObject>();

        private void EnsureRunesAreReady()
        {
            if (runesPrefabs.Keys.Count == 0)
                foreach (var runeType in Enum.GetValues(typeof(RuneType)).Cast<RuneType>())
                    runesPrefabs[runeType] = PrefabLoader.GetPrefab<GameObject>("pudge", runeType + "Rune");  //Resources.Load<GameObject>("Prefabs/" + runeType + "Rune");
        }

        [ToLog]
        public void SpawnRune(RuneType type, RuneSize size, double x, double y, double z, string id)
        {
            this.Log("SpawnRune", type, size, x, y, z, id);

            EnsureRunesAreReady();

            var rune = GameObject.Instantiate<GameObject>(runesPrefabs[type]);
            rune.transform.position = new Vector3((float)x, (float)y, (float)z).ToUnityBasis();
            rune.transform.localScale = Vector3.one;
            rune.transform.localScale *= size == RuneSize.Normal ? 0.02f : 0.025f;
            rune.name = id;
        }

        

        [ToLog]
        public void MakeDaggerExplosion(Frame3D location) {
            this.Log("MakeDaggerExplosion", location);

            var unityExpLoc = location.ToUnityBasis();

            var daggerExplosion = GameObject.Instantiate<GameObject>(_daggerExplosionPrefab);
            daggerExplosion.name = "DaggerExplosion";
            daggerExplosion.transform.position = new Vector3((float)unityExpLoc.X, (float)unityExpLoc.Y, (float)unityExpLoc.Z);
        }

        #region PudgeActorManager
        [ToLog]
        public void CreateActorBody(IActor actor, bool debug)
        {
            if (actor is PudgeRobot)
                CreatePudgeBody(actor.ObjectId, actor.ControllerId);
            if (actor is Slardar)
                CreateSlardarBody(actor.ObjectId, actor.ControllerId, debug);
        }

        [ToLog]
        public void CreatePudgeBody(string actorId, string controllerId)
        {
            this.Log("CreatePudgeBody", actorId, controllerId);

            ActorInitializer.SetUpActor(BuildPudgeBody(controllerId), actorId);
            _commonEngine.SetAbsoluteSpeed(actorId, Frame3D.Identity);
        }

        [ToLog]
        public void CreateWard(string objId, Frame3D location)
        {
           // Dispatcher.CurrentLog.Log("CreateWard", objId, location);

            var unityWardLoc = location.ToUnityBasis();

            var ward = GameObject.Instantiate<GameObject>(_wardPrefab);
            ward.name = objId;
            ward.transform.rotation = Quaternion.Euler(new Vector3(0, 0, 0));
            ward.transform.position = new Vector3((float)unityWardLoc.X, (float)unityWardLoc.Y, (float)unityWardLoc.Z);
        }


        private float pudgeScale = 0.01f;

        private GameObject BuildPudgeBody(string controllerId)
        {
            Debugger.Log(Metrics.Pudge);
            switch (controllerId)
            {
                case TwoPlayersId.Left:
                    return CreatePudge(new Vector3(6.5f * Metrics.Pudge, 0, -6.5f * Metrics.Pudge),
                        Quaternion.Euler(0, -45, 0), Color.green);
                case TwoPlayersId.Right:
                    return CreatePudge(new Vector3(-6.5f * Metrics.Pudge, 0, 6.5f * Metrics.Pudge),
                        Quaternion.Euler(0, -45 + 180, 0), Color.red);
                default:
                    throw new ArgumentException("Invalid controllerId");
            }
        }

        private GameObject CreatePudge(Vector3 position, Quaternion rotation, Color markerColor)
        {
            var actorBody = GameObject.Instantiate<GameObject>(_pudgePrefab);

            actorBody.transform.position = position;
            actorBody.transform.rotation = rotation;
            actorBody.transform.localScale = Vector3.one * pudgeScale;

            Indicator.Create(actorBody.transform, Indicator.Circle, markerColor,
                PudgeRules.Current.VisibilityRadius * Metrics.Centimeter / pudgeScale);

            return actorBody;
        }

        #endregion

        #region SlardarActorManager

        [ToLog]
        public void CreateSlardarBody(string actorId, string controllerId, bool debug)
        {
            this.Log("CreateSlardarBody", actorId, controllerId, debug);

            ActorInitializer.SetUpActor(BuildSlardarBody(controllerId, debug), actorId);
            _commonEngine.SetAbsoluteSpeed(actorId, Frame3D.Identity);
        }

        private float slardarScale = 0.01f;

        public LogWriter LogWriter
        {
            get;set;
        }

        private GameObject BuildSlardarBody(string controllerId, bool debug)
        {
            return debug
                ? BuildSlardarDebugBody(controllerId)
                : BuildSlardarNormalBody(controllerId);
        }

        private GameObject BuildSlardarDebugBody(string controllerId)
        {
            switch (controllerId)
            {
                case SlardarId.LeftTop:
                    return CreateSlardar(new Vector3(-7, 0, -7), Quaternion.Euler(0, -90, 0));
                case SlardarId.RightBottom:
                    return CreateSlardar(new Vector3(5, 0, 10), Quaternion.Euler(0, -90, 0));
                default:
                    throw new ArgumentException("Invalid controllerId");
            }
        }

        private GameObject BuildSlardarNormalBody(string controllerId)
        {
            switch (controllerId)
            {
                case SlardarId.Center:
                    return CreateSlardar(new Vector3(0, 0, 7), Quaternion.Euler(0, 0, 0));
                case SlardarId.RightBottom:
                    return CreateSlardar(new Vector3(6, 0, 14), Quaternion.Euler(0, 0, 0));
                case SlardarId.LeftTop:
                    return CreateSlardar(new Vector3(-6, 0, -14), Quaternion.Euler(0, 180, 0));
                default:
                    throw new ArgumentException("Invalid controllerId");
            }
        }

        private GameObject CreateSlardar(Vector3 position, Quaternion rotation)
        {
            var actorBody = GameObject.Instantiate<GameObject>(_slardarPrefab);
            actorBody.transform.position = position;
            actorBody.transform.rotation = rotation;
            actorBody.transform.localScale = Vector3.one * slardarScale;

            Indicator.Create(actorBody.transform, Indicator.Circle, new Color(1, 0.65f, 0),
                SlardarRules.Current.SideVisibilityRadius * Metrics.Centimeter / slardarScale);

            Indicator.Create(actorBody.transform, Indicator.Angle, new Color(1, 0.65f, 0),
                SlardarRules.Current.ForwardVisibilityRadius * Metrics.Centimeter / slardarScale);

            return actorBody;
        }

        #endregion
    }
}
