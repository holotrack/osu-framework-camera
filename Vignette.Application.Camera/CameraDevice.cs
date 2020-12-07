// Copyright 2020 - 2021 Vignette Project
// Licensed under MIT. See LICENSE for details.

using System;
using OpenCvSharp;

namespace Vignette.Application.Camera
{
    /// <inheritdoc cref="ICameraDevice"/>
    public class CameraDevice : Camera, ICameraDevice
    {
        /// <summary>
        /// Gets or sets the device's saturation setting. Changes are only visible if your device supports it.
        /// </summary>
        public float Saturation
        {
            get => (float)Capture.Saturation;
            set => Capture.Saturation = value;
        }

        /// <summary>
        /// Gets or sets the device's contrast setting. Changes are only visible if your device supports it.
        /// </summary>
        public float Contrast
        {
            get => (float)Capture.Contrast;
            set => Capture.Contrast = value;
        }

        /// <summary>
        /// Gets or sets the device's exposure setting. Changes are only visible if your device supports it.
        /// </summary>
        public float Exposure
        {
            get => (float)Capture.Exposure;
            set => Capture.Exposure = value;
        }

        /// <summary>
        /// Gets or sets the device's gain setting. Changes are only visible if your device supports it.
        /// </summary>
        public float Gain
        {
            get => (float)Capture.Gain;
            set => Capture.Gain = value;
        }

        /// <summary>
        /// Gets or sets the device's hue setting. Changes are only visible if your device supports it.
        /// </summary>
        public float Hue
        {
            get => (float)Capture.Hue;
            set => Capture.Hue = value;
        }

        private int focus;

        /// <summary>
        /// Gets or sets the device's focus setting in percentage (0% - 100%). Changes are only visible if your device supports it.
        /// </summary>
        public int Focus
        {
            get => focus;
            set
            {
                if (value == focus)
                    return;

                if (focus > 100 || focus < 0)
                    throw new ArgumentException($"{nameof(value)} must be between 0 to 100.");

                focus = value;

                const int max = 51;
                const int inc = 5;

                // See: https://stackoverflow.com/a/42819965
                Capture.Focus = Math.Floor((double)(max * (value / 100))) * inc;
            }
        }

        private bool autoExposure;

        /// <summary>
        /// Gets or sets the device's auto exposure setting. Changes are only visible if your device supports it.
        /// </summary>
        public bool AutoExposure
        {
            get => autoExposure;
            set
            {
                if (value == autoExposure)
                    return;

                autoExposure = value;

                // See: https://github.com/opencv/opencv/issues/9738#issuecomment-447388754
                Capture.AutoExposure = value ? 0.25 : 0.75;
            }
        }

        /// <summary>
        /// Gets or sets the device's auto focus setting. Changes are only visible if your device supports it.
        /// </summary>
        public bool AutoFocus
        {
            get => Capture.AutoFocus;
            set => Capture.AutoFocus = value;
        }

        private VideoWriter writer;

        /// <summary>
        /// Create a new camera from a physical device.
        /// </summary>
        /// <param name="cameraId">The camera's numeric identifier.</param>
        public CameraDevice(int cameraId)
        {
            Capture = new VideoCapture(cameraId);
        }

        /// <summary>
        /// Initializes a recording session.
        /// </summary>
        /// <param name="path">The destination path of the recording.</param>
        public void Record(string path)
        {
            writer = new VideoWriter(path, -1, FramesPerSecond, new Size(Size.X, Size.Y));
        }

        /// <summary>
        /// Releases and disposes resources and ends the current recording.
        /// </summary>
        public void Save()
        {
            writer.Release();
            writer.Dispose();
            writer = null;
        }

        protected override void PreTick()
        {
            if (writer != null && Mat != null)
                writer.Write(Mat);
        }
    }
}
