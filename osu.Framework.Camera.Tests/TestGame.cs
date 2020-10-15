// Copyright 2020 - 2021 Vignette Project
// Licensed under MIT. See LICENSE for details.

using osu.Framework.Allocation;
using osu.Framework.Input;

namespace osu.Framework.Camera.Tests
{
    public class TestGame : Game
    {
        private DependencyContainer dependencies;
        protected override IReadOnlyDependencyContainer CreateChildDependencies(IReadOnlyDependencyContainer parent) =>
            dependencies = new DependencyContainer(base.CreateChildDependencies(parent));

        [BackgroundDependencyLoader]
        private void load()
        {
            dependencies.Cache(new CameraManager(Host.UpdateThread) { EventScheduler = Scheduler });
        }
    }
}
