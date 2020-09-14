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
        private VideoCapture capture;
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

        public Mat CaptureBuffer => image;
        public VideoCapture Capture => capture;
        public float FrameWidth => (float)(capture?.FrameWidth);
        public float FrameHeight => (float)(capture?.FrameHeight);

        int _flipMode = 1;
        public int FlipMode => _flipMode;

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

                Cv2.Flip(image, image, (FlipMode)_flipMode);
                CaptureData = image.ToBytes(ext:".bmp");
                using (var stream = new MemoryStream(CaptureData))
                {
                    if (Texture != null)
                    {
                        //TODO (AINL): Copy RAW memory buffer.
                        var uploader = new TextureUpload(stream);
                        Texture.SetData(uploader);
                    }
                    else
                    {
                        Texture = Texture.FromStream(stream);
                    }
                }
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