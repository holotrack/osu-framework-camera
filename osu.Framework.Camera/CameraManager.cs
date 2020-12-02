// Copyright 2020 - 2021 Vignette Project
// Licensed under MIT. See LICENSE for details.

using System;
using System.Collections.Generic;
using System.Collections.Immutable;
using System.IO;
using System.Linq;
using System.Management;
using System.Threading;
using osu.Framework.Bindables;
using osu.Framework.Threading;

namespace osu.Framework.Camera
{
    public class CameraManager : IDisposable
    {
        public IEnumerable<string> CameraDeviceNames => deviceNames;
        public event Action<string> OnNewDevice;
        public event Action<string> OnLostDevice;

        public readonly Bindable<string> Current = new Bindable<string>();
        private CameraInfo current;

        private ImmutableList<CameraInfo> devices = ImmutableList<CameraInfo>.Empty;
        private ImmutableList<string> deviceNames = ImmutableList<string>.Empty;
        private readonly CameraInfoComparer cameraInfoComparer = new CameraInfoComparer();

        private Scheduler scheduler;
        private readonly CancellationTokenSource cancellationToken = new CancellationTokenSource();

        public CameraManager(Scheduler scheduler)
        {
            this.scheduler = scheduler;

            Current.ValueChanged += onDeviceChanged;

            scheduler.Add(() =>
            {
                new Thread(() =>
                {
                    while (!cancellationToken.IsCancellationRequested)
                    {
                        syncCameraDevices();
                        Thread.Sleep(1000);
                    }
                })
                {
                    IsBackground = true,
                }.Start();
            });
        }

        protected static IEnumerable<CameraInfo> EnumerateAllDevices()
        {
            yield return new CameraInfo("No Device", string.Empty);

            switch (RuntimeInfo.OS)
            {
                case RuntimeInfo.Platform.Windows:
                {
                    using var query = new ManagementObjectSearcher("SELECT * FROM Win32_PnpEntity WHERE PNPClass = \"Camera\"");
                    using var collection = query.Get();

                    foreach (var device in collection)
                        yield return new CameraInfo((string)device.GetPropertyValue("Name"), ((string[])device.GetPropertyValue("HardwareID"))[0]);

                    break;
                }

                case RuntimeInfo.Platform.Linux:
                {
                    for (int i = 0; i < Directory.EnumerateDirectories(@"/dev/").Count(); i++)
                    {
                        string path = $"/dev/video{i}";

                        string friendlyName;
                        string friendlyNamePath = $"/sys/class/video4linux/video{i}/name";

                        if (File.Exists(friendlyNamePath))
                        {
                            using var reader = new StreamReader(File.OpenRead(friendlyNamePath));
                            friendlyName = reader.ReadToEnd();
                        }
                        else
                        {
                            friendlyName = path;
                        }

                        yield return new CameraInfo(friendlyName, path);
                    }
                    break;
                }

                default:
                    throw new NotSupportedException();
            }
        }

        private void syncCameraDevices()
        {
            var updatedCameraDevices = EnumerateAllDevices().ToImmutableList();

            if (devices.SequenceEqual(updatedCameraDevices, cameraInfoComparer))
                return;

            devices = updatedCameraDevices;

            onDevicesChanged();

            var oldDeviceNames = deviceNames;
            var newDeviceNames = deviceNames = devices.Select(d => d.Name).ToImmutableList();

            var addedDevices = newDeviceNames.Except(oldDeviceNames).ToList();
            var lostDevices = oldDeviceNames.Except(newDeviceNames).ToList();

            if (addedDevices.Count > 0 || lostDevices.Count > 0)
            {
                scheduler.Add(delegate
                {
                    foreach (var d in addedDevices)
                        OnNewDevice?.Invoke(d);

                    foreach (var d in lostDevices)
                        OnLostDevice?.Invoke(d);
                });
            }
        }

        private void onDeviceChanged(ValueChangedEvent<string> args)
        {
            scheduler.Add(() =>
            {
                var device = devices.FirstOrDefault(d => d.Name == args.NewValue);
                if (!devices.Contains(device))
                    current = devices.Last();
            });
        }

        private void onDevicesChanged()
        {
            scheduler.Add(() =>
            {
                if (!devices.Contains(current, cameraInfoComparer))
                    current = devices.Last();
            });
        }

        private bool isDisposed;

        protected virtual void Dispose(bool disposing)
        {
            if (isDisposed)
                return;

            if (disposing)
            {
                cancellationToken.Cancel();

                OnNewDevice = null;
                OnLostDevice = null;
            }

            isDisposed = true;
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

        private class CameraInfoComparer : IEqualityComparer<CameraInfo>
        {
            public bool Equals(CameraInfo a, CameraInfo b) => a.Path == b.Path;
            public int GetHashCode(CameraInfo camera) => camera.Path.GetHashCode();
        }
    }
}
