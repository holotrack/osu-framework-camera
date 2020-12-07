﻿// Copyright 2020 - 2021 Vignette Project
// Licensed under MIT. See LICENSE for details.

using System.IO;
using NUnit.Framework;

namespace Vignette.Application.Camera.Tests
{
    [TestFixture]
    public class CameraVirtualTest
    {
        [Test]
        public void TestFileCaching()
        {
            var camera = new TestCameraVirtual(new MemoryStream());
            Assert.IsTrue(File.Exists(camera.FilePath));

            camera.Dispose();
            Assert.IsFalse(File.Exists(camera.FilePath));
        }
    }
}
