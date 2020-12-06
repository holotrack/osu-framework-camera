// Copyright 2020 - 2021 Vignette Project
// Licensed under MIT. See LICENSE for details.

using System;
using osu.Framework.Graphics;
using osu.Framework.Graphics.Containers;
using osu.Framework.Graphics.Sprites;
using osu.Framework.Graphics.Textures;

namespace osu.Framework.Camera.Graphics
{
    public abstract class DrawableCameraWrapper : CompositeDrawable, ICamera
    {
        protected readonly Camera Camera;

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

            camera.OnTick += () => sprite.Texture = Texture.FromStream(camera.Data);
        }

        /// <inheritdoc cref="Camera.Pause"/>
        public void Pause() => Camera.Pause();

        /// <inheritdoc cref="Camera.Resume"/>
        public void Resume() => Camera.Resume();

        /// <inheritdoc cref="Camera.Start"/>
        public void Start() => Camera.Start();

        /// <inheritdoc cref="Camera.Stop"/>
        public void Stop(bool waitForDecoder) => Camera.Stop(waitForDecoder);

        protected override void Dispose(bool isDisposing)
        {
            base.Dispose(isDisposing);

            if (disposeUnderlyingCameraOnDispose)
                Camera?.Dispose();
        }
    }
}
