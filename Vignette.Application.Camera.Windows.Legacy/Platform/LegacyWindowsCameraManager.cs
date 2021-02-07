// Copyright 2020 - 2021 Vignette Project
// Licensed under MIT. See LICENSE for details.

using System.Collections.Generic;
using DirectShowLib;
using osu.Framework.Threading;

namespace Vignette.Application.Camera.Platform
{
    public class LegacyWindowsCameraManager : CameraManager
    {
        public LegacyWindowsCameraManager(Scheduler scheduler)
            : base(scheduler)
        {
        }

        protected override IEnumerable<CameraInfo> EnumerateAllDevices()
        {
            foreach (var device in DsDevice.GetDevicesOfCat(FilterCategory.VideoInputDevice))
                yield return new CameraInfo(device.Name, device.DevicePath);
        }
    }
}
