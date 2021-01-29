// Copyright 2020 - 2021 Vignette Project
// Licensed under MIT. See LICENSE for details.

using System;
using System.IO;
using OpenCvSharp;

namespace Vignette.Application.Camera
{
    /// <inheritdoc cref="ICameraVirtual"/>
    public class CameraVirtual : Camera, ICameraVirtual
    {
        /// <summary>
        /// Whether the playback should loop or not.
        /// </summary>
        public bool Loop { get; set; }

        /// <summary>
        /// The number of frames the playback has.
        /// </summary>
        public int FrameCount => (Capture.IsDisposed) ? 0 : Capture.FrameCount;

        /// <summary>
        /// The current frame of the playback.
        /// </summary>
        public int Position => (Capture.IsDisposed) ? 0 : Capture.PosFrames;

        /// <summary>
        /// Skip frames when this camera resumes from suspension emulating a physical device.
        /// </summary>
        public bool EmulateDevice { get; set; }

        private int droppedFrames;

        private bool hasTempFile;

        protected string FilePath;

        /// <summary>
        /// Create a new virtual camera device from a file.
        /// </summary>
        /// <param name="filePath">File relative to the executing application.</param>
        /// <param name="format">Image format used for encoding.</param>
        /// <param name="encodingParams">An array of parameters used for encoding.</param>
        public CameraVirtual(string filePath, EncodingFormat format = EncodingFormat.PNG, ImageEncodingParam[] encodingParams = null)
            : base(format, encodingParams)
        {
            if (!File.Exists(filePath))
                throw new FileNotFoundException($"\"{filePath}\" is not found.");

            FilePath = filePath;
            Capture = new VideoCapture(FilePath);
        }

        /// <summary>
        /// Create a new virtual camera device from a <see cref="Stream"/>.
        /// </summary>
        /// <param name="stream">A video stream.</param>
        /// <param name="format">Image format used for encoding.</param>
        /// <param name="encodingParams">An array of parameters used for encoding.</param>
        public CameraVirtual(Stream stream, EncodingFormat format = EncodingFormat.PNG, ImageEncodingParam[] encodingParams = null)
            : base(format, encodingParams)
        {
            if (stream == null)
                throw new NullReferenceException($"{nameof(stream)} cannot be null.");

            // since osu!framework works a lot with System.IO.Stream we'll just have
            // to save this temporarily on the file system and cleanup
            FilePath = Path.Combine(Path.GetTempPath(), Guid.NewGuid().ToString());

            using (var file = File.Create(FilePath))
                stream.CopyTo(file);

            Capture = new VideoCapture(FilePath);
            hasTempFile = true;
        }

        /// <summary>
        /// Seeks the video playback to the (n)th frame.
        /// </summary>
        /// <param name="frame">The position to seek to.</param>
        public void Seek(int frame)
        {
            if (frame < 0 || frame > Capture.FrameCount - 2)
                throw new IndexOutOfRangeException($"{nameof(frame)} should not be greater than the number of frames or less than zero.");

            if (frame == Position)
                return;

            bool wasPaused = Paused;

            Pause();

            lock (Capture)
                Capture.PosFrames = frame;

            if (!wasPaused)
                Resume();
        }

        public override void Pause()
        {
            if (Ready || Paused)
                return;

            droppedFrames = 0;

            base.Pause();
        }

        public override void Resume()
        {
            if (Ready || !Paused)
                return;

            if (EmulateDevice)
            {
                if (!Loop)
                {
                    Seek(Math.Min(Position + droppedFrames, FrameCount));
                }
                else
                {
                    Seek((Position + droppedFrames) % FrameCount);
                }
            }

            base.Resume();
        }

        protected override void PreTick()
        {
            if (Paused)
                droppedFrames++;

            if (Loop)
            {
                if (Position >= FrameCount - 2)
                {
                    Seek(0);
                    Resume();
                }
            }
            else
            {
                if (Position >= FrameCount - 2)
                    Pause();
            }
        }

        protected override void Dispose(bool disposing)
        {
            if (IsDisposed)
                return;

            base.Dispose(disposing);

            if (hasTempFile)
                File.Delete(FilePath);
        }
    }
}
