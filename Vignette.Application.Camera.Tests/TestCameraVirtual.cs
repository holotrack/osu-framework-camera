// Copyright 2020 - 2021 Vignette Project
// Licensed under MIT. See LICENSE for details.

using System.IO;
using OpenCvSharp;

namespace Vignette.Application.Camera.Tests
{
    internal class TestCameraVirtual : CameraVirtual
    {
        public new string FilePath => base.FilePath;

        public new DecoderState State => base.State;

        public TestCameraVirtual(Stream stream)
            : base(stream, EncodingFormat.JPEG, new[]
            {
                new ImageEncodingParam(ImwriteFlags.JpegQuality, 20),
                new ImageEncodingParam(ImwriteFlags.JpegOptimize, 1),
            })
        {
        }
    }
}
