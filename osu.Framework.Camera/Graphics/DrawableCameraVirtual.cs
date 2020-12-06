// Copyright 2020 - 2021 Vignette Project
// Licensed under MIT. See LICENSE for details.

namespace osu.Framework.Camera.Graphics
{
    /// <summary>
    /// A <see cref="CameraVirtual"/> that can be added to the scene hierarchy.
    /// </summary>
    /// <remarks>The playback rate is unaffected by <see cref="Framework.Graphics.Drawable.Clock"/>.</remarks>
    public class DrawableCameraVirtual : DrawableCameraWrapper, ICameraVirtual
    {
        /// <<inheritdoc cref="CameraVirtual.Loop"/>
        public bool Loop
        {
            get => ((CameraVirtual)Camera).Loop;
            set => ((CameraVirtual)Camera).Loop = value;
        }

        /// <<inheritdoc cref="CameraVirtual.FrameCount"/>
        public int FrameCount => ((CameraVirtual)Camera).FrameCount;

        public DrawableCameraVirtual(CameraVirtual camera, bool disposeUnderlyingCameraOnDispose = true)
            : base(camera, disposeUnderlyingCameraOnDispose)
        {
        }

        /// <<inheritdoc cref="CameraVirtual.Position"/>
        int ICameraVirtual.Position => ((CameraVirtual)Camera).Position;

        /// <<inheritdoc cref="CameraVirtual.Seek"/>
        public void Seek(int frame) => ((CameraVirtual)Camera).Seek(frame);
    }
}
