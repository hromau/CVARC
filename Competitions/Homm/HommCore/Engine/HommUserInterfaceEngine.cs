using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using CVARC.V2;
using UnityCommons;
using UnityEngine;

namespace HoMM.Engine
{
    class HommUserInterfaceEngine : IHommUserInterfaceEngine
    {
        public LogWriter LogWriter { get; set; }

        private Dictionary<string, HeroInfo> heroInfos = new Dictionary<string, HeroInfo>();

        [ToLog]
        public void InitUi(string id, UiLocation uiLocation)
        {
            this.Log($"{nameof(InitUi)}", id, uiLocation);

            if (heroInfos.ContainsKey(id)) throw new ArgumentException("This id was already initialized");

            var location = GetUiLocationVector(uiLocation);
            heroInfos[id] = new HeroInfo(id, location);
        }

        private Vector3 GetUiLocationVector(UiLocation uiLocation)
        {
            if (uiLocation == UiLocation.Left)
                return new Vector3(0.02f, 0.8f, 0);
            return new Vector3(0.88f, 0.8f, 0);
        }

        [ToLog]
        public void UpdateArmy(string id, UnitType unit, int count)
        {
            this.Log($"{nameof(UpdateArmy)}", id, unit, count);

            heroInfos[id].Army[unit] = count;
            heroInfos[id].UpdateText();
        }

        [ToLog]
        public void UpdateResources(string id, Resource resource, int count)
        {
            this.Log($"{nameof(UpdateResources)}", id, resource, count);

            heroInfos[id].Resources[resource] = count;
            heroInfos[id].UpdateText();
        }

        private class HeroInfo
        {
            public Dictionary<UnitType, int> Army { get; } = new Dictionary<UnitType, int>();
            public Dictionary<Resource, int> Resources { get; } = new Dictionary<Resource, int>();

            private GUIText text;
            private string id;

            public HeroInfo(string id, Vector3 location)
            {
                this.id = id;

                foreach (UnitType unitType in Enum.GetValues(typeof(UnitType)))
                    Army[unitType] = 0;

                foreach (Resource resource in Enum.GetValues(typeof(Resource)))
                    Resources[resource] = 0;

                text = new GameObject($"{id}'s HeroInfo").AddComponent<GUIText>() as GUIText;

                text.pixelOffset = new Vector2(1, 1);
                text.transform.position = location;

                UpdateText();
            }

            public void UpdateText()
            {
                var builder = new StringBuilder();

                builder.Append($"{id}\n\n");

                builder.Append($"Resources:\n");

                foreach (var resource in Resources)
                    builder.Append($"{resource.Key} - {resource.Value}\n");

                builder.Append("\nArmy:\n");

                foreach (var unit in Army)
                    builder.Append($"{unit.Key} - {unit.Value}\n");

                text.text = builder.ToString();
            }
        }
    }
}
