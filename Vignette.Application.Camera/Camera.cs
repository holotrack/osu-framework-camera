// Copyright 2020 - 2021 Vignette Project
// Licensed under MIT. See LICENSE for details.

using System;
using System.IO;
using System.Threading;
using System.Threading.Tasks;
using OpenCvSharp;
using osuTK;

namespace Vignette.Application.Camera
{
    /// <summary>
    /// A base class for camera components which handles device or file access, disposal, and update logic.
    /// </summary>
    public abstract class Camera : IDisposable, ICamera
    {
        /// <summary>
        /// The output image width.
        /// </summary>
        public float Width => (Capture.IsDisposed) ? 0 : Capture.FrameWidth;

        /// <summary>
        /// The output image height.
        /// </summary>
        public float Height => (Capture.IsDisposed) ? 0 : Capture.FrameHeight;

        /// <summary>
        /// The output image size.
        /// </summary>
        public Vector2 Size => new Vector2(Width, Height);

        /// <summary>
        /// The frequency of frames outputted by the device in seconds.
        /// </summary>
        public double FramesPerSecond => (Capture.IsDisposed) ? 0 : Capture.Fps;

        /// <summary>
        /// The data stream outputted from the capture device.
        /// </summary>
        public Stream Data { get; private set; }

        /// <summary>
        /// Fired when a new update occurs. The frequency of invocations is tied to the <see cref="FramesPerSecond"/>.
        /// </summary>
        public event Action OnTick;

        /// <summary>
        /// The paused state of this <see cref="Camera"/>.
        /// </summary>
        public bool Paused => State == DecoderState.Paused;

        /// <summary>
        /// Whether this <see cref="Camera"/> has started decoding.
        /// </summary>
        public bool Started => State == DecoderState.Started;

        /// <summary>
        /// Whether this <see cref="Camera"/> has finished or stopped decoding.
        /// </summary>
        public bool Stopped => State == DecoderState.Stopped;

        /// <summary>
        /// Whether this <see cref="Camera"/> is ready to start decoding.
        /// </summary>
        public bool Ready => State == DecoderState.Ready;

        /// <summary>
        /// Whether this <see cref="Camera"/> has been disposed.
        /// </summary>
        public bool IsDisposed { get; private set; }

        protected DecoderState State { get; private set; } = DecoderState.Ready;

        protected Mat Mat;

        private CancellationTokenSource decodingTaskCancellationToken;

        private Task decodingTask;

        internal VideoCapture Capture;

        /// <summary>
        /// Starts the decoding process for this camera.
        /// </summary>
        public void Start()
        {
            // throw if we encounter errors
            Capture.SetExceptionMode(true);

            if (!Ready)
                return;

            decodingTaskCancellationToken = new CancellationTokenSource();
            decodingTask = Task.Factory.StartNew(() => decodingLoop(decodingTaskCancellationToken), decodingTaskCancellationToken.Token, TaskCreationOptions.LongRunning, TaskScheduler.Default);

            State = DecoderState.Started;
        }

        /// <summary>
        /// Suspends the decoding process.
        /// </summary>
        public virtual void Pause()
        {
            if (Ready || Paused)
                return;

            State = DecoderState.Paused;
        }

        /// <summary>
        /// Resumes the decoding process.
        /// </summary>
        public virtual void Resume()
        {
            if (Ready || !Paused)
                return;

            State = DecoderState.Started;
        }

        /// <summary>
        /// Ends the decoding process for this camera and cleans up resources.
        /// </summary>
        /// <param name="waitForDecoder">Wait for the last tick to finish before proceeding to cleanup.</param>
        public void Stop(bool waitForDecoder)
        {
            // The capture has a reference to the device or file even if decoding hasn't started yet.
            Capture.Release();

            if (Ready)
                return;

            decodingTaskCancellationToken.Cancel();
            if (waitForDecoder)
                decodingTask.Wait();

            OnTick = null;
            decodingTask = null;
            decodingTaskCancellationToken.Dispose();
            decodingTaskCancellationToken = null;

            Capture.Dispose();

            State = DecoderState.Stopped;
        }

        private void decodingLoop(CancellationTokenSource token)
        {
            while (true)
            {
                if (token.IsCancellationRequested)
                    return;

                PreTick();

                if (Paused)
                    continue;

                // Don't do anything when there are no more frames or the device has been disconnected.
                if (!Capture.Grab())
                    continue;

                Mat = Capture.RetrieveMat();

                // Bitmap seems to be the least CPU intensive format.
                if (!Mat.Empty())
                    Data = Mat.ToMemoryStream(".png");


                OnTick?.Invoke();

                Thread.Sleep((int)Math.Round(1000 / Math.Max(FramesPerSecond, 1)));
            }
        }

        protected virtual void PreTick()
        {
        }

        protected virtual void Dispose(bool disposing)
        {
            if (IsDisposed)
                return;

            Stop(true);

            IsDisposed = true;
        }

        ~Camera()
        {
            Dispose(false);
        }

        public void Dispose()
        {
            Dispose(true);
            GC.SuppressFinalize(this);
        }

        public enum DecoderState
        {
            Ready,

            Started,

            Paused,

            Stopped,
        }
    }
}
