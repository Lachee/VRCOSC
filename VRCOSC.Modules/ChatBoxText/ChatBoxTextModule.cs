﻿// Copyright (c) VolcanicArts. Licensed under the GPL-3.0 License.
// See the LICENSE file in the repository root for full license text.

using VRCOSC.Game.Modules.ChatBox;

namespace VRCOSC.Modules.ChatBoxText;

public class ChatBoxTextModule : ChatBoxModule
{
    public override string Title => "ChatBox Text";
    public override string Description => "Display custom text and animate it for the ChatBox";
    public override string Author => "VolcanicArts";
    public override ModuleType Type => ModuleType.General;
    protected override TimeSpan DeltaUpdate => TimeSpan.FromSeconds(1.5f);
    protected override bool ShouldUpdateImmediately => false;

    private readonly Dictionary<string, int> indexes = new();

    protected override void CreateAttributes()
    {
        CreateSetting(ChatBoxTextSetting.TextList, new ChatBoxTextInstanceListAttribute
        {
            Default = new List<ChatBoxTextInstance>
            {
                new()
                {
                    Key = { Value = "Example" },
                    Text = { Value = "ExampleText" },
                    Direction = { Value = ChatBoxTextDirection.Right },
                    ScrollSpeed = { Value = 1 },
                    MaxLength = { Value = 8 }
                }
            },
            Name = "Texts",
            Description = "Each text instance to register for the ChatBox\nText instances can be accessed with the '_Key' suffix"
        });

        CreateVariable(ChatBoxTextVariable.Text, "Text", "text");

        CreateState(ChatBoxTextState.Default, "Default", $"{GetVariableFormat(ChatBoxTextVariable.Text, "Example")}");
    }

    protected override void OnModuleStart()
    {
        indexes.Clear();

        GetSettingList<ChatBoxTextInstance>(ChatBoxTextSetting.TextList).ForEach(instance =>
        {
            indexes.Add(instance.Key.Value, 0);
            SetVariableValue(ChatBoxTextVariable.Text, instance.Text.Value, instance.Key.Value);
        });

        ChangeStateTo(ChatBoxTextState.Default);
    }

    protected override void OnModuleUpdate()
    {
        GetSettingList<ChatBoxTextInstance>(ChatBoxTextSetting.TextList).ForEach(instance =>
        {
            var text = instance.Text.Value;

            switch (instance.Direction.Value)
            {
                case ChatBoxTextDirection.Right:
                    indexes[instance.Key.Value] += instance.ScrollSpeed.Value;
                    if (indexes[instance.Key.Value] > text.Length - 1) indexes[instance.Key.Value] = 0 + (indexes[instance.Key.Value] - text.Length);
                    break;

                case ChatBoxTextDirection.Left:
                    indexes[instance.Key.Value] -= instance.ScrollSpeed.Value;
                    if (indexes[instance.Key.Value] < 1) indexes[instance.Key.Value] = text.Length - (0 - indexes[instance.Key.Value]);
                    break;
            }

            var index = indexes[instance.Key.Value];

            if (instance.ScrollSpeed.Value > 0)
            {
                var tickerText = $"{text}{text}";
                var maxLength = Math.Min(instance.MaxLength.Value, text.Length);

                text = tickerText[index..(maxLength + index)];
            }

            SetVariableValue(ChatBoxTextVariable.Text, text, instance.Key.Value);
        });
    }

    private enum ChatBoxTextSetting
    {
        TextList
    }

    private enum ChatBoxTextState
    {
        Default
    }

    private enum ChatBoxTextVariable
    {
        Text
    }
}
