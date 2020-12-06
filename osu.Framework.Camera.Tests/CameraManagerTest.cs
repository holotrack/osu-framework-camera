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
            manager = CameraManager.CreateSuitableManager(scheduler = new Scheduler());
        }

        [Test]
        public void RetrieveDevicesTest()
        {
            // Ensure that the thread has at least done one update call before we proceed.
            scheduler.Update();
            Thread.Sleep(2000);

            // This requires a physical device to be connected to succeed.
            Assert.IsTrue(manager.CameraDeviceNames.Any());
        }
    }
}
