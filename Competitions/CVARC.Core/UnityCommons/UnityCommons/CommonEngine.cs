using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using AIRLab;
using AIRLab.Mathematics;
using CVARC.V2;
using UnityEngine;
using Infrastructure;

namespace UnityCommons
{
    public class CommonEngine : ICommonEngine
    {
        Dictionary<string, Frame3D> requestedAbsoluteSpeed = new Dictionary<string, Frame3D>();
        Dictionary<string, Frame3D> requestedRelativeSpeed = new Dictionary<string, Frame3D>();

        System.Random rand = new System.Random(10);

        public void Initialize(CVARC.V2.IWorld world)
        {

        }

        [ToLog]
        public void Stop()
        {
        //    Dispatcher.CurrentLog.Log("Stop");

            foreach (var id in requestedAbsoluteSpeed.Keys.ToArray())
            {
                requestedAbsoluteSpeed[id] = Frame3D.Identity;
                requestedRelativeSpeed[id] = Frame3D.Identity;
            }
        }

        [ToLog]
        public void SetRelativeSpeed(string id, Frame3D speed)
        {
        //    Dispatcher.CurrentLog.Log("SetRelativeSpeed", id, speed);
            requestedRelativeSpeed[id] = speed;

            if (requestedAbsoluteSpeed.ContainsKey(id))
                requestedAbsoluteSpeed.Remove(id);
        }

        [ToLog]
        public void SetAbsoluteSpeed(string id, Frame3D speed)
        {
         //   Dispatcher.CurrentLog.Log("SetAbsoluteSpeed", id, speed);
            requestedAbsoluteSpeed[id] = speed.ToUnityBasis();

            if (requestedRelativeSpeed.ContainsKey(id))
                requestedRelativeSpeed.Remove(id);
        }

        public Frame3D GetSpeed(string id)
        {
            var movingObject = GameObject.Find(id);
            var vel = movingObject.GetComponent<Rigidbody>().velocity;
            var angVel = movingObject.GetComponent<Rigidbody>().angularVelocity;

            return new Frame3D(vel.x, vel.y, vel.z,
                Angle.FromRad(angVel.x), Angle.FromRad(angVel.y), Angle.FromRad(angVel.z)).ToCvarcBasis();
        }

        public void UpdateSpeeds()
        {
            UpdateAbsoluteSpeeds();
            UpdateRelativeSpeeds();
        }

        private void UpdateRelativeSpeeds()
        {
            foreach (var requestedId in requestedRelativeSpeed.Keys)
            {
                if (!ContainBody(requestedId)) continue;

                var actorLocation = GetAbsoluteLocation(requestedId);

                var requestedSpeed = requestedRelativeSpeed[requestedId];

                // FIXME: rewrite transformations for 3d space
                var speed = new Frame3D(
                    requestedSpeed.X * Math.Cos(actorLocation.Yaw.Radian),
                    requestedSpeed.X * Math.Sin(actorLocation.Yaw.Radian),
                    0,
                    requestedSpeed.Pitch,
                    requestedSpeed.Yaw,
                    requestedSpeed.Roll
                ).ToUnityBasis();

                UpdateSpeed(requestedId, speed);
            }
        }

        private void UpdateAbsoluteSpeeds()
        {
            foreach (var requestedId in requestedAbsoluteSpeed.Keys)
                UpdateSpeed(requestedId, requestedAbsoluteSpeed[requestedId]);
        }

        private void UpdateSpeed(string requestedId, Frame3D speed)
        {
            var target = ObjectsCache.FindGameObject(requestedId);
            if (target == null) return;

            var gravityVelocity = target.GetComponent<Rigidbody>().velocity.y;

            target.GetComponent<Rigidbody>().velocity =
                new Vector3((float)speed.X, (float)speed.Y + gravityVelocity, (float)speed.Z);

            target.GetComponent<Rigidbody>().angularVelocity =
                new Vector3((float)speed.Pitch.Radian, (float)speed.Yaw.Radian, (float)speed.Roll.Radian);
        }

        public Frame3D GetAbsoluteLocation(string id)
        {
            var obj = ObjectsCache.FindGameObject(id);
            var pos = obj.transform.position;
            var rot = obj.transform.rotation.eulerAngles;

            return new Frame3D(pos.x, pos.y, pos.z,
                Angle.FromGrad(rot.x),
                Angle.FromGrad(rot.y),
                Angle.FromGrad(rot.z)).ToCvarcBasis();
        }

        [ToLog]
        public void SetAbsoluteLocation(string id, Frame3D location)
        {
        //    Dispatcher.CurrentLog.Log("SetAbsoluteLocation", id, location);

            var obj = ObjectsCache.FindGameObject(id);
            var unityLocation = location.ToUnityBasis();
            var newPosition = new Vector3(
                (float)unityLocation.X,
                (float)unityLocation.Y,
                (float)unityLocation.Z);
            var newRotation = new Vector3(
                (float)unityLocation.Pitch.Simplify360().Grad,
                (float)unityLocation.Yaw.Simplify360().Grad,
                (float)unityLocation.Roll.Simplify360().Grad);
            obj.transform.position = newPosition;
            obj.transform.rotation = Quaternion.Euler(newRotation);
        }

        public event Action<string, string> Collision;

        public void CollisionSender(string firstId, string secondId)
        {
            if (Collision != null)
                Collision(firstId, secondId);
        }

        public bool ContainBody(string id)
        {
            return !(ObjectsCache.FindGameObject(id) == null);
        }


        Dictionary<GameObject, Tuple<float, float>> attachedParams = new Dictionary<GameObject, Tuple<float, float>>();

        [ToLog]
        public void Attach(string objectToAttach, string host, Frame3D relativePosition)
        {
        //    Dispatcher.CurrentLog.Log("Attach", objectToAttach, host, relativePosition);

            var parent = GameObject.Find(host);
            var attachment = GameObject.Find(objectToAttach);

            attachment.transform.position = parent.transform.position;
            attachment.transform.rotation = parent.transform.rotation;

            var rp = relativePosition.ToUnityBasis();
            attachment.transform.position += Quaternion.Euler(parent.transform.eulerAngles) *
                new Vector3((float)rp.X, (float)rp.Y, (float)rp.Z);
            attachment.transform.rotation *= Quaternion.Euler((float)rp.Pitch.Grad, (float)rp.Yaw.Grad, (float)rp.Roll.Grad);

            var joint = attachment.AddComponent<FixedJoint>();
            joint.connectedBody = parent.GetComponent<Rigidbody>();
            joint.enableCollision = false;
            joint.breakForce = Single.PositiveInfinity;
            joint.enablePreprocessing = false;

            attachedParams.Add(attachment, new Tuple<float, float>(attachment.GetComponent<Rigidbody>().drag, attachment.GetComponent<Rigidbody>().angularDrag));
            attachment.GetComponent<Rigidbody>().drag = attachment.GetComponent<Rigidbody>().angularDrag = 0;
        }

        [ToLog]
        public void Detach(string objectToDetach, Frame3D absolutePosition)
        {
         //   Dispatcher.CurrentLog.Log("Detach", objectToDetach, absolutePosition);

            var attachment = GameObject.Find(objectToDetach);

            var joints = attachment.GetComponents<FixedJoint>();
            foreach (var joint in joints)
                GameObject.Destroy(joint);

            if (attachedParams.ContainsKey(attachment))
            {
                var attachmentParams = attachedParams[attachment];
                attachment.GetComponent<Rigidbody>().drag = attachmentParams.Item1;
                attachment.GetComponent<Rigidbody>().angularDrag = attachmentParams.Item2;
                attachedParams.Remove(attachment);
            }
        }

        [ToLog]
        public void DeleteObject(string objectId)
        {
        //    Dispatcher.CurrentLog.Log("DeleteObject", objectId);

            GameObject.Destroy(GameObject.Find(objectId));
            if (requestedAbsoluteSpeed.ContainsKey(objectId))
                requestedAbsoluteSpeed.Remove(objectId);
        }

        public string FindParent(string objectId)
        {
            var obj = GameObject.Find(objectId);
            if (obj == null) return null;

            var parent = FindParentByJoints(obj);
            if (parent == null)
                parent = FindParentByHierarchy(obj);

            return parent;
        }

        string FindParentByHierarchy(GameObject obj)
        {
            if (obj.transform == null) return null;
            if (obj.transform.parent == null) return null;
            return obj.transform.parent.name;
        }

        string FindParentByJoints(GameObject obj)
        {
            var joint = obj.GetComponents<FixedJoint>().FirstOrDefault();
            if (joint == null) return null;
            if (joint.connectedBody == null) return null;
            return joint.connectedBody.name;
        }


    }
}
