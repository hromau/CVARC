using UnityEngine;

namespace Pudge.RunningBinding.FromUnity
{
    public static class ActorInitializer
    {
        public static void SetUpActor(GameObject actor, string actorObjectId)
        {
            // Закоментил, чтобы хоть как-то работало
            //actor.AddComponent<OnCollisionScript>();
            actor.name = actorObjectId;

            actor.GetComponent<Rigidbody>().drag = 0;
            actor.GetComponent<Rigidbody>().angularDrag = 0;
            actor.GetComponent<Rigidbody>().useGravity = true;
            actor.GetComponent<Rigidbody>().mass = 2.7f;
            actor.GetComponent<Rigidbody>().constraints = RigidbodyConstraints.FreezeRotationX |
                                                          RigidbodyConstraints.FreezeRotationZ |
                                                          RigidbodyConstraints.FreezePositionY;
            var floor = GameObject.Find("floor");
            foreach (var robotCollider in actor.GetComponents<Collider>())
                Physics.IgnoreCollision(floor.GetComponent<MeshCollider>(), robotCollider);
        }
    }
}
