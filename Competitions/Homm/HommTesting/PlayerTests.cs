using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

using HoMM;
using NUnit.Framework;

namespace HexModelTesting
{
    [TestFixture]
    public class PlayerTests
    {
        [Test]
        public void PlayerInitOK()
        {
            var p = new Player("Sieur de Metz", null);
            Assert.That(p != null);
        }

        [Test]
        public void CheckResGainLoss()
        {
            var p = new Player("a", null);
            Assert.AreEqual(p.CheckResourceAmount(Resource.Gold), 0);
            p.GainResources(Resource.Gold, 100);
            Assert.AreEqual(p.CheckResourceAmount(Resource.Gold), 100);
            p.PayResources(Resource.Gold, 50);
            Assert.AreEqual(p.CheckResourceAmount(Resource.Gold), 50);
        }

        [Test]
        public void PayingMoreResThanYouHaveFails()
        {
            var p = new Player("a", null);
            p.GainResources(Resource.Gold, 100);
            Assert.Throws<ArgumentException>(() => p.PayResources(Resource.Gold, 120));
            Assert.AreEqual(p.CheckResourceAmount(Resource.Gold), 100);
            p.GainResources(Resource.Wood, 1);
            Assert.Throws<ArgumentException>(() => p.PayResources(Resource.Ore, 1));
        }
    }
}
