// Copyright 2020 - 2021 Vignette Project
// Licensed under MIT. See LICENSE for details.

using System.IO;

namespace Vignette.Application.Camera
{
    /// <summary>
    /// A physical camera device
    /// </summary>
    public interface ICameraDevice
    {
        float Saturation { get; set; }

        float Contrast { get; set; }

        float Exposure { get; set; }

        float Gain { get; set; }

        float Hue { get; set; }

        int Focus { get; set; }

        bool AutoExposure { get; set; }

        bool AutoFocus { get; set; }

        void Record(string path);

        void Save();
    }
}
