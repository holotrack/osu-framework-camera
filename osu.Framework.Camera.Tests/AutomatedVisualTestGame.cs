// Copyright 2020 - 2021 Vignette Project
// Licensed under MIT. See LICENSE for details.

using osu.Framework.Testing;

namespace osu.Framework.Camera.Tests
{
    internal class AutomatedVisualTestGame : TestGame
    {
        public AutomatedVisualTestGame()
        {
            Add(new TestBrowserTestRunner(new TestBrowser()));
        }
    }
}
