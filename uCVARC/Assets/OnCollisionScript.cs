using UnityEngine;

namespace Assets
{
    class OnCollisionScript : MonoBehaviour
    {
        //так же может иметь место метод OnCollisionExit с такой же сигнатурой
        void OnCollisionEnter(Collision collision)
        {
            if (RoundScript.CollisionInfo == null)
                return;
            if (RoundScript.CollisionInfo.Item3 == 0)
            {
                RoundScript.CollisionInfo.Item3 = 1;
                RoundScript.CollisionInfo.Item1 = collision.gameObject.name;
            }
            else
            {
                RoundScript.CollisionInfo.Item2 = collision.gameObject.name;
                RoundScript.CollisionInfo.Item3 = 2;
            }
        }
    }
}
