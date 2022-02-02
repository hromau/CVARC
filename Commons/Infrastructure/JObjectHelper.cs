﻿using System.Linq;
using Newtonsoft.Json.Linq;

namespace Infrastructure
{
    public static class JObjectHelper
    {
        // public static JObject CreateSimple<T>(T obj) => new JObject(new {value = obj});
        public static T ParseSimple<T>(JObject obj) => obj.Values().Single().Value<T>();
    }
}