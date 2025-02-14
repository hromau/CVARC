﻿using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

using HoMM;
using NUnit.Framework;
using CVARC.V2;

namespace HexModelTesting
{
    [TestFixture]
    public class PlayerTests
    {
        private static WorldClocks mockClocks = new WorldClocks();

        [Test]
        public void PlayerInitOK()
        {
            var p = new Player("Sieur de Metz", null, mockClocks);
            Assert.That(p != null);
        }

        [Test]
        public void CheckResGainLoss()
        {
            var p = new Player("a", null, mockClocks);
            Assert.AreEqual(p.CheckResourceAmount(Resource.Gold), 0);
            p.GainResources(Resource.Gold, 100);
            Assert.AreEqual(p.CheckResourceAmount(Resource.Gold), 100);
            p.PayResources(Resource.Gold, 50);
            Assert.AreEqual(p.CheckResourceAmount(Resource.Gold), 50);
        }

        [Test]
        public void PayingMoreResThanYouHaveFails()
        {
            var p = new Player("a", null, mockClocks);
            p.GainResources(Resource.Gold, 100);
            Assert.Throws<ArgumentException>(() => p.PayResources(Resource.Gold, 120));
            Assert.AreEqual(p.CheckResourceAmount(Resource.Gold), 100);
            p.GainResources(Resource.Glass, 1);
            Assert.Throws<ArgumentException>(() => p.PayResources(Resource.Iron, 1));
        }
    }
}
