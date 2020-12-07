// Copyright 2020 - 2021 Vignette Project
// Licensed under MIT. See LICENSE for details.

using osu.Framework.Graphics;
using osu.Framework.Testing;
using osuTK;
using Vignette.Application.Camera.Graphics;

namespace Vignette.Application.Camera.Tests.Visual
{
    public class TestSceneDrawableCameraDevice : TestScene
    {
        private CameraDevice camera;

        public TestSceneDrawableCameraDevice()
        {
            // THIS REQUIRES A PHYSICAL CAMERA CONNECTED!
            camera = new CameraDevice(0);
            camera.Start();

            Add(new DrawableCameraDevice(camera)
            {
                Anchor = Anchor.Centre,
                Origin = Anchor.Centre,
                Size = new Vector2(256),
            });
        }
    }
}
