// Copyright 2020 - 2021 Vignette Project
// Licensed under MIT. See LICENSE for details.

using System;
using System.Collections.Generic;
using System.Threading.Tasks;
using Windows.Devices.Enumeration;
using osu.Framework.Threading;

namespace Vignette.Application.Camera.Platform
{
    public class WindowsCameraManager : CameraManager
    {
        public WindowsCameraManager(Scheduler scheduler)
            : base(scheduler)
        {
        }

        protected override IEnumerable<CameraInfo> EnumerateAllDevices()
        {
            return getAllDevices().Result;
        }

        private async Task<IEnumerable<CameraInfo>> getAllDevices()
        {
            var collection = await DeviceInformation.FindAllAsync(DeviceClass.VideoCapture);
            var devices = new List<CameraInfo>();

            foreach (var device in collection)
                devices.Add(new CameraInfo(device.Name, device.Id));

            return devices;
        }
    }
}
