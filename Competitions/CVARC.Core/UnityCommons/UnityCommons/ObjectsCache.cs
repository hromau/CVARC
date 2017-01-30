using System.Collections.Generic;
using UnityEngine;

namespace UnityCommons
{
    public static class ObjectsCache
    {
        static readonly Dictionary<string, GameObject> objectsCache = new Dictionary<string, GameObject>();

        public static GameObject FindGameObject(string id)
        {
            // тут вроде все как надо.
            // для оптимизации можно вызывать этот метод, но не обязательно.
            if (objectsCache.ContainsKey(id))
                if (objectsCache[id] == null || objectsCache[id].name != id) // у GameObject перегружен оператор == так, что он возвращает true в сравнении с null, если объект уничтожен GameObject.Destroy
                    objectsCache.Remove(id);
                else
                    return objectsCache[id];
            var obj = GameObject.Find(id);
            if (obj != null)
                objectsCache.Add(id, obj);
            return obj;
        }
    }
}
