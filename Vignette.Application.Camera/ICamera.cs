// Copyright 2020 - 2021 Vignette Project
// Licensed under MIT. See LICENSE for details.

namespace Vignette.Application.Camera
{
    /// <summary>
    /// An interface containing base logic for cameras.
    /// </summary>
    public interface ICamera
    {
        void Start();

        void Pause();

        void Resume();

        void Stop(bool waitForDecoder);
    }
}
