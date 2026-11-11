// Copyright (c) ppy Pty Ltd <contact@ppy.sh>. Licensed under the MIT Licence.
// See the LICENCE file in the repository root for full licence text.

using System.Linq;
using NUnit.Framework;
using osu.Framework.Allocation;
using osu.Framework.Audio;
using osu.Framework.Graphics;
using osu.Framework.Graphics.Containers;
using osu.Framework.Testing;
using osu.Game.Graphics.UserInterfaceV2;
using osu.Game.Overlays;
using osu.Game.Overlays.Settings.Sections.Audio;

namespace osu.Game.Tests.Visual.Settings
{
    public partial class TestSceneAsioSettings : OsuTestScene
    {
        [Cached]
        private readonly OverlayColourProvider colourProvider = new OverlayColourProvider(OverlayColourScheme.Purple);

        [Resolved]
        private AudioManager audio { get; set; } = null!;

        private AsioSettings settings = null!;

        [Test]
        public void TestRetainsUnavailableDriverAndCustomBuffer()
        {
            string previousDriver = string.Empty;
            int previousBuffer = 0;

            AddStep("set saved driver settings", () =>
            {
                previousDriver = audio.AsioDevice.Value;
                previousBuffer = audio.AsioBufferSize.Value;
                audio.AsioDevice.Value = "Disconnected test ASIO interface";
                audio.AsioBufferSize.Value = 192;
            });
            AddStep("create settings", () => Child = new Container
            {
                Width = 600,
                AutoSizeAxes = Axes.Y,
                Child = settings = new AsioSettings(),
            });
            AddUntilStep("settings loaded", () => settings.IsLoaded);
            AddAssert("saved driver retained", () => settings.ChildrenOfType<FormDropdown<string>>().Single().Current.Value,
                () => Is.EqualTo("Disconnected test ASIO interface"));
            AddAssert("custom buffer retained", () => settings.ChildrenOfType<FormDropdown<int>>().Any(dropdown => dropdown.Current.Value == 192));
            AddStep("restore audio settings", () =>
            {
                Clear();
                audio.AsioDevice.Value = previousDriver;
                audio.AsioBufferSize.Value = previousBuffer;
            });
        }
    }
}
