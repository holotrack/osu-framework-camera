// Copyright 2020 - 2021 Vignette Project
// Licensed under MIT. See LICENSE for details.

using System.IO;

namespace osu.Framework.Camera.Tests
{
    internal class TestCameraVirtual : CameraVirtual
    {
        public new string FilePath => base.FilePath;

        public new DecoderState State => base.State;

        public TestCameraVirtual(Stream stream)
            : base(stream)
        {
        }
    }
}
