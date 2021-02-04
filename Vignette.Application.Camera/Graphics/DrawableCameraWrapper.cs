// Copyright 2020 - 2021 Vignette Project
// Licensed under MIT. See LICENSE for details.

using System;
using System.IO;
using osu.Framework.Graphics;
using osu.Framework.Graphics.Containers;
using osu.Framework.Graphics.Sprites;
using osu.Framework.Graphics.Textures;
using osu.Framework.Threading;

namespace Vignette.Application.Camera.Graphics
{
    public abstract class DrawableCameraWrapper : CompositeDrawable, ICamera
    {
        public double FramesPerSecond => Camera.FramesPerSecond;

        public byte[] Data => Camera.Data;

        public bool Paused => Camera.Paused;

        public bool Started => Camera.Started;

        public bool Stopped => Camera.Stopped;

        public bool Ready => Camera.Ready;

        protected readonly Camera Camera;

        private ScheduledDelegate scheduled;

        private readonly Sprite sprite;
        
        private readonly bool disposeUnderlyingCameraOnDispose;

        protected DrawableCameraWrapper(Drawable content)
        {
            AddInternal(content);
        }

        protected DrawableCameraWrapper(Camera camera, bool disposeUnderlyingCameraOnDispose = true)
        {
            Camera = camera ?? throw new ArgumentNullException(nameof(camera));
            this.disposeUnderlyingCameraOnDispose = disposeUnderlyingCameraOnDispose;

            AddInternal(sprite = new Sprite
            {
                RelativeSizeAxes = Axes.Both,
                FillMode = FillMode.Fit,
                Anchor = Anchor.Centre,
                Origin = Anchor.Centre,
            });

            camera.OnTick += () =>
            {
                scheduled?.Cancel();
                scheduled = Schedule(() =>
                {
                    using var memory = new MemoryStream(camera.Data);
                    sprite.Texture = Texture.FromStream(memory);
                });
            };
        }

        public void Pause() => Camera.Pause();

        public void Resume() => Camera.Resume();

        public void Start() => Camera.Start();

        public void Stop(bool waitForDecoder) => Camera.Stop(waitForDecoder);

        protected override void Dispose(bool isDisposing)
        {
            base.Dispose(isDisposing);

            if (disposeUnderlyingCameraOnDispose)
                Camera?.Dispose();
        }
    }
}
