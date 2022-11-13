﻿// Copyright (c) VolcanicArts. Licensed under the GPL-3.0 License.
// See the LICENSE file in the repository root for full license text.

using osu.Framework.Allocation;
using osu.Framework.Bindables;
using osu.Framework.Extensions.EnumExtensions;
using osu.Framework.Extensions.IEnumerableExtensions;
using osu.Framework.Graphics;
using osu.Framework.Graphics.Containers;
using osu.Framework.Graphics.Shapes;
using osu.Framework.Graphics.Sprites;
using osuTK;
using VRCOSC.Game.Modules;
using VRCOSC.Game.Modules.Util;

namespace VRCOSC.Game.Graphics.ModuleListing;

public class ModuleInfoPopover : PopoverScreen
{
    private readonly SpriteText title;
    private readonly TextFlowContainer description;
    private readonly FillFlowContainer<ParameterData> parameters;

    [Resolved(name: "InfoModule")]
    private Bindable<Module?> infoModule { get; set; } = null!;

    public ModuleInfoPopover()
    {
        Children = new Drawable[]
        {
            new Box
            {
                RelativeSizeAxes = Axes.Both,
                Colour = VRCOSCColour.Gray4
            },
            new Container
            {
                RelativeSizeAxes = Axes.Both,
                Padding = new MarginPadding
                {
                    Vertical = 2.5f
                },
                Child = new BasicScrollContainer
                {
                    RelativeSizeAxes = Axes.Both,
                    ScrollbarVisible = false,
                    ClampExtension = 20,
                    Padding = new MarginPadding
                    {
                        Horizontal = 26
                    },
                    Child = new FillFlowContainer
                    {
                        Anchor = Anchor.TopCentre,
                        Origin = Anchor.TopCentre,
                        RelativeSizeAxes = Axes.X,
                        AutoSizeAxes = Axes.Y,
                        Direction = FillDirection.Vertical,
                        Spacing = new Vector2(0, 5),
                        Padding = new MarginPadding(5),
                        Children = new Drawable[]
                        {
                            title = new SpriteText
                            {
                                Anchor = Anchor.TopCentre,
                                Origin = Anchor.TopCentre,
                                Font = FrameworkFont.Regular.With(size: 75)
                            },
                            description = new TextFlowContainer(t => t.Font = FrameworkFont.Regular.With(size: 30))
                            {
                                Anchor = Anchor.TopCentre,
                                Origin = Anchor.TopCentre,
                                RelativeSizeAxes = Axes.X,
                                AutoSizeAxes = Axes.Y,
                                Padding = new MarginPadding(5),
                                TextAnchor = Anchor.Centre
                            },
                            new LineSeparator
                            {
                                Anchor = Anchor.TopCentre,
                                Origin = Anchor.TopCentre
                            },
                            new FillFlowContainer
                            {
                                Anchor = Anchor.TopCentre,
                                Origin = Anchor.TopCentre,
                                RelativeSizeAxes = Axes.X,
                                AutoSizeAxes = Axes.Y,
                                Spacing = new Vector2(0, 5),
                                Children = new Drawable[]
                                {
                                    new SpriteText
                                    {
                                        Anchor = Anchor.TopCentre,
                                        Origin = Anchor.TopCentre,
                                        Font = FrameworkFont.Regular.With(size: 35),
                                        Text = "Parameters"
                                    },
                                    parameters = new FillFlowContainer<ParameterData>
                                    {
                                        Anchor = Anchor.TopCentre,
                                        Origin = Anchor.TopCentre,
                                        RelativeSizeAxes = Axes.X,
                                        AutoSizeAxes = Axes.Y,
                                        Spacing = new Vector2(0, 5)
                                    }
                                }
                            }
                        }
                    }
                }
            }
        };
    }

    private sealed class ParameterData : Container
    {
        public string ParameterName { get; init; } = null!;
        public string Description { get; init; } = null!;
        public string Type { get; init; } = null!;
        public bool Outgoing { get; init; }
        public bool Incoming { get; init; }

        public ParameterData()
        {
            RelativeSizeAxes = Axes.X;
            AutoSizeAxes = Axes.Y;
            Masking = true;
            CornerRadius = 5;
            BorderThickness = 2;
            BorderColour = VRCOSCColour.Gray0;
        }

        [BackgroundDependencyLoader]
        private void load()
        {
            var name = $"Name: {ParameterName}";
            var type = $"Type: {Type}";
            var outgoing = $"Writes To VRC?: {Outgoing}";
            var incoming = $"Reads From VRC?: {Incoming}";
            var description = $"Description: {Description}";

            Children = new Drawable[]
            {
                new Box
                {
                    Colour = VRCOSCColour.Gray2,
                    RelativeSizeAxes = Axes.Both
                },
                new FillFlowContainer
                {
                    RelativeSizeAxes = Axes.X,
                    AutoSizeAxes = Axes.Y,
                    Direction = FillDirection.Vertical,
                    Padding = new MarginPadding(5),
                    Children = new Drawable[]
                    {
                        new TextFlowContainer
                        {
                            RelativeSizeAxes = Axes.X,
                            AutoSizeAxes = Axes.Y,
                            Text = $"{name}\n{type}\n{outgoing}\n{incoming}\n{description}"
                        }
                    }
                }
            };
        }
    }

    protected override void LoadComplete()
    {
        base.LoadComplete();

        infoModule.ValueChanged += e =>
        {
            if (e.NewValue is null)
                Hide();
            else
                Show();
        };
    }

    public override void Show()
    {
        title.Text = $"{infoModule.Value!.Title} Info";
        description.Text = $"{infoModule.Value.Description}\nCreated by {infoModule.Value.Author}";

        parameters.Clear();

        infoModule.Value.Parameters.Values.ForEach(parameter =>
        {
            parameters.Add(new ParameterData
            {
                ParameterName = parameter.Name,
                Description = parameter.Description,
                Type = parameter.ExpectedType.ToReadableName(),
                Outgoing = parameter.Mode.HasFlagFast(ParameterMode.Write),
                Incoming = parameter.Mode.HasFlagFast(ParameterMode.Read)
            });
        });

        base.Show();
    }

    public override void Hide()
    {
        base.Hide();
        infoModule.Value = null;
    }
}
