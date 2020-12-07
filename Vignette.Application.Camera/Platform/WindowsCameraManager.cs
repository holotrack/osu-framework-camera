// Copyright 2020 - 2021 Vignette Project
// Licensed under MIT. See LICENSE for details.

using System.Collections.Generic;
using System.Management;
using osu.Framework.Threading;

namespace Vignette.Application.Camera.Platform
{
    internal class WindowsCameraManager : CameraManager
    {
        public WindowsCameraManager(Scheduler scheduler)
            : base(scheduler)
        {
        }

        protected override IEnumerable<CameraInfo> EnumerateAllDevices()
        {
            using var query = new ManagementObjectSearcher("SELECT * FROM Win32_PnpEntity WHERE PNPClass = \"Camera\"");
            using var collection = query.Get();

            foreach (var device in collection)
                yield return new CameraInfo((string)device.GetPropertyValue("Name"), ((string[])device.GetPropertyValue("HardwareID"))[0]);
        }
    }
}
