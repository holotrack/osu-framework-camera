// Copyright (c) Nitrous <n20gaming2000@gmail.com>. Licensed under the MIT Licence.
// See the LICENCE file in the repository root for full licence text.

using System;
using System.Collections.Generic;
using System.Collections.Immutable;
using System.Diagnostics.CodeAnalysis;
using System.IO;
using System.Linq;
using System.Text.RegularExpressions;
using System.Threading;
using DirectShowLib;
using osu.Framework.Bindables;
using osu.Framework.Threading;

namespace osu.Framework.Input
{
    public class CameraManager : IDisposable
    {
        private ImmutableList<CameraDevice> cameraDevices = ImmutableList<CameraDevice>.Empty;
        private ImmutableList<string> cameraDeviceNames = ImmutableList<string>.Empty;
        private readonly CameraDeviceUpdateComparer updateComparer = new CameraDeviceUpdateComparer();
        private readonly GameThread thread;
        private Scheduler scheduler => thread.Scheduler;
        private Scheduler eventScheduler => EventScheduler ?? scheduler;
        public Scheduler EventScheduler;
        private readonly CancellationTokenSource cancelSource = new CancellationTokenSource();
        public IEnumerable<string> CameraDeviceNames => cameraDeviceNames;
        public event Action<string> OnNewDevice;
        public event Action<string> OnLostDevice;
        public readonly Bindable<string> CameraDevice = new Bindable<string>();

        public CameraManager(GameThread updateThread)
        {
            thread = updateThread;

            scheduler.Add(() =>
            {
                new Thread(() =>
                {
                    while (!cancelSource.Token.IsCancellationRequested)
                    {
                        syncCameraDevices();
                        Thread.Sleep(1000);
                    }
                })
                {
                    IsBackground = true
                }.Start();
            });
        }
        protected virtual IEnumerable<CameraDevice> EnumerateAllDevices()
        {
            // There aren't any good cross-platform friendly examples of camera device enumeration for .NET.
            // Most of which found are really old examples which is very unfortunate for us.

            var devices = new List<CameraDevice>();
            switch (RuntimeInfo.OS)
            {
                case RuntimeInfo.Platform.Windows:
                    var cameras = DsDevice.GetDevicesOfCat(FilterCategory.VideoInputDevice);

                    foreach (var camera in cameras)
                        devices.Add(new CameraDevice
                        {
                            Name = camera.Name,
                            Path = camera.DevicePath,
                        });

                    break;

                case RuntimeInfo.Platform.Linux:
                    var devDir = Directory.EnumerateDirectories(@"/dev/").ToArray();

                    for (int i = 0; i < devDir.Length; i++)
                    {
                        string path = $"/dev/video{i}";
                        string name = null;

                        try
                        {
                            using (var reader = new StreamReader(File.OpenRead($"/sys/class/video4linux/video{i}/name")))
                                name = reader.ReadToEnd();
                        }
                        catch
                        {
                        }

                        devices.Add(new CameraDevice
                        {
                            Name = !string.IsNullOrEmpty(name) ? name : path,
                            Path = path
                        });

                    }
                    break;

                default:
                    throw new PlatformNotSupportedException($"{nameof(RuntimeInfo.OS)} is not supported.");
            }

            // checks for duplicates in the devices
            // Since the dropdown component in o!f doesn't handle duplicate entries well.
            for (int i = 0; i < devices.Count; i++)
            {
                var qualifiedNames = new List<string>();

                while (qualifiedNames.Contains(devices[i].Name))
                {
                    var oldDeviceName = devices[i].Name;
                    var deviceLocation = devices[i].Path;

                    devices[i] = new CameraDevice 
                    {
                        Name = $"{oldDeviceName} - ({i++})",
                        Path = deviceLocation 
                    };

                }

                qualifiedNames.Add(devices[i].Name);

            }

            return devices;
        }


        private void syncCameraDevices()
        {
            var updatedCameraDevices = EnumerateAllDevices().ToImmutableList();
            if (cameraDevices.SequenceEqual(updatedCameraDevices, updateComparer))
                return;

            cameraDevices = updatedCameraDevices;

            var oldDeviceNames = cameraDeviceNames;
            var newDeviceNames = cameraDeviceNames = cameraDevices.Select(d => d.Name).ToImmutableList();

            var newDevices = newDeviceNames.Except(oldDeviceNames).ToList();
            var lostDevices = oldDeviceNames.Except(newDeviceNames).ToList();

            if (newDevices.Count > 0 || lostDevices.Count > 0)
            {
                eventScheduler.Add(delegate
                {
                    foreach (var d in newDevices)
                        OnNewDevice?.Invoke(d);
                    foreach (var d in lostDevices)
                        OnLostDevice?.Invoke(d);
                });
            }
        }

        protected virtual void Dispose(bool disposing)
        {
            cancelSource.Cancel();

            OnNewDevice = null;
            OnLostDevice = null;
        }

        ~CameraManager()
        {
            Dispose(false);
        }

        public void Dispose()
        {
            Dispose(true);
            GC.SuppressFinalize(this);
        }

        private class CameraDeviceUpdateComparer : IEqualityComparer<CameraDevice>
        {
            public bool Equals([AllowNull] CameraDevice x, [AllowNull] CameraDevice y) => x.Path == y.Path;

            public int GetHashCode(CameraDevice obj) => obj.Name.GetHashCode();
        }
    }
}
