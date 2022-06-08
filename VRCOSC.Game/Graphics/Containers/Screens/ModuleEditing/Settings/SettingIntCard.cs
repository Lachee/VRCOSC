// Copyright (c) VolcanicArts. Licensed under the GPL-3.0 License.
// See the LICENSE file in the repository root for full license text.

using osu.Framework.Allocation;
using osu.Framework.Graphics;
using osu.Framework.Graphics.Containers;
using osuTK;
using VRCOSC.Game.Graphics.Containers.UI.TextBox;

namespace VRCOSC.Game.Graphics.Containers.Screens.ModuleEditing.Settings;

public class SettingIntCard : SettingBaseCard
{
    [BackgroundDependencyLoader]
    private void load()
    {
        VRCOSCTextBox textBox;

        Add(new Container
        {
            Anchor = Anchor.CentreRight,
            Origin = Anchor.CentreRight,
            RelativeSizeAxes = Axes.Both,
            Size = new Vector2(0.5f, 1.0f),
            Padding = new MarginPadding(15),
            Child = textBox = new VRCOSCTextBox
            {
                Anchor = Anchor.Centre,
                Origin = Anchor.Centre,
                RelativeSizeAxes = Axes.Both,
                BorderThickness = 3,
                Text = SourceModule.DataManager.GetSettingAs<int>(Key).ToString(),
            }
        });

        textBox.OnCommit += (_, _) =>
        {
            if (int.TryParse(textBox.Text, out var newValue))
                updateSetting(newValue);
        };

        ResetToDefault.Action += () =>
        {
            var defaultValue = SourceModule.GetDefaultSetting<int>(Key);
            updateSetting(defaultValue);
            textBox.Text = defaultValue.ToString();
        };

        updateSetting(int.Parse(textBox.Text));
    }

    private void updateSetting(int newValue)
    {
        SourceModule.DataManager.UpdateIntSetting(Key, newValue);

        if (!SourceModule.GetDefaultSetting<int>(Key).Equals(SourceModule.DataManager.GetSettingAs<int>(Key)))
        {
            ResetToDefault.Show();
        }
        else
        {
            ResetToDefault.Hide();
        }
    }
}