// Copyright 2020 - 2021 Vignette Project
// Licensed under MIT. See LICENSE for details.

using System.Linq;
using System.Threading;
using NUnit.Framework;
using osu.Framework.Threading;

namespace osu.Framework.Camera.Tests
{
    [TestFixture]
    public class CameraManagerTest
    {
        private Scheduler scheduler;
        private CameraManager manager;

        [SetUp]
        public void SetUp()
        {
            manager = new CameraManager(scheduler = new Scheduler());
        }

        [Test]
        public void RetrieveDevicesTest()
        {
            // Ensure that the thread has at least done one update call before we proceed.
            scheduler.Update();
            Thread.Sleep(2000);

            Assert.IsTrue(manager.CameraDeviceNames.Any());
        }
    }
}
