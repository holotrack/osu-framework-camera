// Copyright 2020 - 2021 Vignette Project
// Licensed under MIT. See LICENSE for details.

namespace osu.Framework.Camera.Graphics
{
    /// <summary>
    /// A <see cref="CameraDevice"/> that can be added to the scene hierarchy.
    /// </summary>
    public class DrawableCameraDevice : DrawableCameraWrapper, ICameraDevice
    {
        /// <inheritdoc cref="CameraDevice.Saturation"/>
        public float Saturation
        {
            get => ((CameraDevice)Camera).Saturation;
            set => ((CameraDevice)Camera).Saturation = value;
        }

        /// <inheritdoc cref="CameraDevice.Contrast"/>
        public float Contrast
        {
            get => ((CameraDevice)Camera).Contrast;
            set => ((CameraDevice)Camera).Contrast = value;
        }

        /// <inheritdoc cref="CameraDevice.Exposure"/>
        public float Exposure
        {
            get => ((CameraDevice)Camera).Exposure;
            set => ((CameraDevice)Camera).Exposure = value;
        }

        /// <inheritdoc cref="CameraDevice.Gain"/>
        public float Gain
        {
            get => ((CameraDevice)Camera).Gain;
            set => ((CameraDevice)Camera).Gain = value;
        }

        /// <inheritdoc cref="CameraDevice.Hue"/>
        public float Hue
        {
            get => ((CameraDevice)Camera).Hue;
            set => ((CameraDevice)Camera).Hue = value;
        }

        /// <inheritdoc cref="CameraDevice.Focus"/>
        public int Focus
        {
            get => ((CameraDevice)Camera).Focus;
            set => ((CameraDevice)Camera).Focus = value;
        }

        /// <inheritdoc cref="CameraDevice.AutoExposure"/>
        public bool AutoExposure
        {
            get => ((CameraDevice)Camera).AutoExposure;
            set => ((CameraDevice)Camera).AutoExposure = value;
        }

        /// <inheritdoc cref="CameraDevice.AutoFocus"/>
        public bool AutoFocus
        {
            get => ((CameraDevice)Camera).AutoFocus;
            set => ((CameraDevice)Camera).AutoFocus = value;
        }


        public DrawableCameraDevice(CameraDevice camera, bool disposeUnderlyingCameraOnDispose = true)
            : base(camera, disposeUnderlyingCameraOnDispose)
        {
        }

        /// <inheritdoc cref="CameraDevice.Record"/>
        public void Record(string path) => ((CameraDevice)Camera).Record(path);

        /// <inheritdoc cref="CameraDevice.Save"/>
        public void Save() => ((CameraDevice)Camera).Save();
    }
}
