// Copyright (c) ppy Pty Ltd <contact@ppy.sh>. Licensed under the MIT Licence.
// See the LICENCE file in the repository root for full licence text.

using System.Linq;
using osu.Framework.Allocation;
using osu.Framework.Audio;
using osu.Framework.Bindables;
using osu.Framework.Graphics;
using osu.Framework.Localisation;
using osu.Game.Graphics.Containers;
using osu.Game.Graphics.UserInterfaceV2;
using osu.Game.Localisation;

namespace osu.Game.Overlays.Settings.Sections.Audio
{
    public partial class AsioSettings : SettingsSubsection
    {
        protected override LocalisableString Header => AudioSettingsStrings.AsioHeader;

        private IBindable<string>? status;
        private OsuTextFlowContainer statusText = null!;
        private DriverDropdown driver = null!;

        [BackgroundDependencyLoader]
        private void load(AudioManager audio)
        {
            Children = new Drawable[]
            {
                new SettingsItemV2(new FormCheckBox
                {
                    Caption = AudioSettingsStrings.UseAsio,
                    HintText = AudioSettingsStrings.UseAsioHint,
                    Current = audio.UseAsio,
                }) { Keywords = new[] { "asio", "focusrite", "vocaster", "latency", "native" } },
                new SettingsItemV2(driver = new DriverDropdown
                {
                    Caption = AudioSettingsStrings.AsioDriver,
                    Current = audio.AsioDevice,
                }),
                new SettingsItemV2(new FormDropdown<int>
                {
                    Caption = AudioSettingsStrings.AsioSampleRate,
                    Items = new[] { 44100, 48000, 88200, 96000, 176400, 192000 }.Append(audio.AsioSampleRate.Value).Distinct().Order(),
                    Current = audio.AsioSampleRate,
                }),
                new SettingsItemV2(new BufferDropdown
                {
                    Caption = AudioSettingsStrings.AsioBufferSize,
                    Items = new[] { 0, 32, 64, 128, 256, 512, 1024, 2048 }.Append(audio.AsioBufferSize.Value).Distinct().Order(),
                    Current = audio.AsioBufferSize,
                }),
                new FormButton
                {
                    Caption = AudioSettingsStrings.AsioRetry,
                    ButtonText = AudioSettingsStrings.AsioRetryButton,
                    Action = () =>
                    {
                        refreshDrivers(audio);
                        audio.RetryAsio();
                    },
                },
                statusText = new OsuTextFlowContainer
                {
                    RelativeSizeAxes = Axes.X,
                    AutoSizeAxes = Axes.Y,
                    Padding = new MarginPadding { Horizontal = 10, Vertical = 5 },
                },
            };

            refreshDrivers(audio);
            status = audio.AsioStatus.GetBoundCopy();
            status.BindValueChanged(_ => Schedule(() => statusText.Text = status.Value), true);
        }

        private void refreshDrivers(AudioManager audio)
        {
            driver.Items = new[] { string.Empty }.Concat(audio.AsioDeviceNames).Append(audio.AsioDevice.Value).Distinct().ToArray();
        }

        protected override void Dispose(bool isDisposing)
        {
            status?.UnbindAll();
            base.Dispose(isDisposing);
        }

        private partial class DriverDropdown : FormDropdown<string>
        {
            protected override LocalisableString GenerateItemText(string item)
                => string.IsNullOrEmpty(item) ? AudioSettingsStrings.AsioFirstDriver : base.GenerateItemText(item);
        }

        private partial class BufferDropdown : FormDropdown<int>
        {
            protected override LocalisableString GenerateItemText(int item)
                => item == 0 ? AudioSettingsStrings.AsioDriverDefault : item.ToString();
        }
    }
}
