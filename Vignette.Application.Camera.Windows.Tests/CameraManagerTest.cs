// Copyright 2020 - 2021 Vignette Project
// Licensed under MIT. See LICENSE for details.

using osu.Framework.Threading;
using Vignette.Application.Camera.Platform;
using Vignette.Application.Camera.Tests;

namespace Vignette.Application.Camera.Windows.Tests
{
    public class WindowsCameraManagerTest : CameraManagerTest
    {
        protected override CameraManager CreateSuitableManager(Scheduler scheduler)
        {
            return new WindowsCameraManager(scheduler);
        }
    }
}
