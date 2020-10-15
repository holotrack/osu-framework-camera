// Copyright (c) Nitrous <n20gaming2000@gmail.com>. Licensed under the MIT Licence.
// See the LICENCE file in the repository root for full licence text.

using System.IO;
using System.Threading;
using System.Threading.Tasks;
using OpenCvSharp;
using osu.Framework.Allocation;
using osu.Framework.Graphics.Sprites;
using osu.Framework.Graphics.Textures;

namespace osu.Framework.Graphics.Camera
{
    public class CameraSprite : Sprite
    {
        private Task cameraLoopTask;
        private CancellationTokenSource cameraLoopCanellationSource;
        private VideoCapture capture { get; set; }
        private readonly Mat image;

        private int cameraID = -1;
        public int CameraID
        {
            get => cameraID;
            set
            {
                if (IsLoaded)
                    stopRecording();

                cameraID = value;
                capture?.Dispose();
                capture = new VideoCapture(cameraID);

                if (IsLoaded)
                    startRecording();
            }
        }

        /// <summary>
        /// The camera's image data as a byte array.
        /// </summary>
        public byte[] CaptureData { get; private set; }

        public VideoCapture Capture => capture;
        public float FrameWidth => (float)(capture?.FrameWidth);
        public float FrameHeight => (float)(capture?.FrameHeight);

        public CameraSprite(int cameraID = 0)
        {
            CameraID = cameraID;
            image = new Mat();
        }

        [BackgroundDependencyLoader]
        private void load()
        {
            startRecording();
        }

        private void cameraLoop(CancellationToken cancellationToken)
        {
            while (true)
            {
                if (cancellationToken.IsCancellationRequested)
                {
                    clearTexture();
                    return;
                }

                if (CameraID == -1)
                {
                    clearTexture();
                    continue;
                }

                capture?.Read(image);

                if (image?.Empty() ?? true)
                {
                    clearTexture();
                    continue;
                }

                CaptureData = image.ToBytes();
                Texture = Texture.FromStream(new MemoryStream(CaptureData));
                Colour = Colour4.White;
            }
        }

        private void clearTexture()
        {
            CaptureData = null;
            Texture = Texture.WhitePixel;
            Colour = Colour4.Black;
        }

        private void startRecording()
        {
            cameraLoopCanellationSource = new CancellationTokenSource();
            cameraLoopTask = Task.Factory.StartNew(() => cameraLoop(cameraLoopCanellationSource.Token), cameraLoopCanellationSource.Token, TaskCreationOptions.LongRunning, TaskScheduler.Default);
        }

        private void stopRecording()
        {
            cameraLoopCanellationSource.Cancel();
            cameraLoopTask.Wait();
            cameraLoopTask.Dispose();
            cameraLoopCanellationSource.Dispose();

            cameraLoopTask = null;
            cameraLoopCanellationSource = null;
        }

        protected override void Dispose(bool isDisposing)
        {
            base.Dispose(isDisposing);

            stopRecording();

            capture?.Dispose();
            image?.Dispose();
        }
    }
}
