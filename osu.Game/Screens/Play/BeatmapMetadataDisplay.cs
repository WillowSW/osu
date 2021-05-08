// Copyright (c) ppy Pty Ltd <contact@ppy.sh>. Licensed under the MIT Licence.
// See the LICENCE file in the repository root for full licence text.

using System.Collections.Generic;
using osu.Framework.Allocation;
using osu.Framework.Bindables;
using osu.Framework.Graphics;
using osu.Framework.Graphics.Containers;
using osu.Framework.Graphics.Sprites;
using osu.Framework.Localisation;
using osu.Game.Beatmaps;
using osu.Game.Graphics;
using osu.Game.Graphics.Sprites;
using osu.Game.Graphics.UserInterface;
using osu.Game.Rulesets;
using osu.Game.Rulesets.Mods;
using osu.Game.Screens.Play.HUD;
using osu.Game.Screens.Ranking.Expanded;
using osuTK;

namespace osu.Game.Screens.Play
{
    /// <summary>
    /// Displays beatmap metadata inside <see cref="PlayerLoader"/>
    /// </summary>
    public class BeatmapMetadataDisplay : Container
    {
        private readonly WorkingBeatmap beatmap;
        private readonly Bindable<IReadOnlyList<Mod>> mods;
        private readonly Drawable facade;
        private LoadingSpinner loading;

        public IBindable<IReadOnlyList<Mod>> Mods => mods;

        [Resolved]
        private IBindable<RulesetInfo> ruleset { get; set; }

        public bool Loading
        {
            set
            {
                if (value)
                    loading.Show();
                else
                    loading.Hide();
            }
        }

        public BeatmapMetadataDisplay(WorkingBeatmap beatmap, Bindable<IReadOnlyList<Mod>> mods, Drawable facade)
        {
            this.beatmap = beatmap;
            this.facade = facade;

            this.mods = new Bindable<IReadOnlyList<Mod>>();
            this.mods.BindTo(mods);
        }

        [BackgroundDependencyLoader]
        private void load(BeatmapDifficultyCache difficultyCache)
        {
            var metadata = beatmap.BeatmapInfo?.Metadata ?? new BeatmapMetadata();

            var starDifficulty = difficultyCache.GetDifficultyAsync(beatmap.BeatmapInfo, ruleset.Value, mods.Value).Result;

            AutoSizeAxes = Axes.Both;
            Children = new Drawable[]
            {
                new FillFlowContainer
                {
                    AutoSizeAxes = Axes.Both,
                    Origin = Anchor.TopCentre,
                    Anchor = Anchor.TopCentre,
                    Direction = FillDirection.Vertical,
                    Children = new[]
                    {
                        facade.With(d =>
                        {
                            d.Anchor = Anchor.TopCentre;
                            d.Origin = Anchor.TopCentre;
                        }),
                        new OsuSpriteText
                        {
                            Text = new RomanisableString(metadata.TitleUnicode, metadata.Title),
                            Font = OsuFont.GetFont(size: 36, italics: true),
                            Origin = Anchor.TopCentre,
                            Anchor = Anchor.TopCentre,
                            Margin = new MarginPadding { Top = 15 },
                        },
                        new OsuSpriteText
                        {
                            Text = new RomanisableString(metadata.ArtistUnicode, metadata.Artist),
                            Font = OsuFont.GetFont(size: 26, italics: true),
                            Origin = Anchor.TopCentre,
                            Anchor = Anchor.TopCentre,
                        },
                        new Container
                        {
                            Size = new Vector2(300, 60),
                            Margin = new MarginPadding(10),
                            Origin = Anchor.TopCentre,
                            Anchor = Anchor.TopCentre,
                            CornerRadius = 10,
                            Masking = true,
                            Children = new Drawable[]
                            {
                                new Sprite
                                {
                                    RelativeSizeAxes = Axes.Both,
                                    Texture = beatmap?.Background,
                                    Origin = Anchor.Centre,
                                    Anchor = Anchor.Centre,
                                    FillMode = FillMode.Fill,
                                },
                                loading = new LoadingLayer(true)
                            }
                        },
                        new FillFlowContainer
                        {
                            AutoSizeAxes = Axes.Both,
                            Anchor = Anchor.TopCentre,
                            Origin = Anchor.TopCentre,
                            Direction = FillDirection.Vertical,
                            Spacing = new Vector2(5f),
                            Margin = new MarginPadding { Bottom = 40 },
                            Children = new Drawable[]
                            {
                                new OsuSpriteText
                                {
                                    Text = beatmap?.BeatmapInfo?.Version,
                                    Font = OsuFont.GetFont(size: 26, italics: true),
                                    Anchor = Anchor.TopCentre,
                                    Origin = Anchor.TopCentre,
                                },
                                new StarRatingDisplay(starDifficulty)
                                {
                                    Anchor = Anchor.TopCentre,
                                    Origin = Anchor.TopCentre,
                                }
                            }
                        },
                        new GridContainer
                        {
                            Anchor = Anchor.TopCentre,
                            Origin = Anchor.TopCentre,
                            AutoSizeAxes = Axes.Both,
                            RowDimensions = new[]
                            {
                                new Dimension(GridSizeMode.AutoSize),
                                new Dimension(GridSizeMode.AutoSize),
                            },
                            ColumnDimensions = new[]
                            {
                                new Dimension(GridSizeMode.AutoSize),
                                new Dimension(GridSizeMode.AutoSize),
                            },
                            Content = new[]
                            {
                                new Drawable[]
                                {
                                    new MetadataLineLabel("Source"),
                                    new MetadataLineInfo(metadata.Source)
                                },
                                new Drawable[]
                                {
                                    new MetadataLineLabel("Mapper"),
                                    new MetadataLineInfo(metadata.AuthorString)
                                }
                            }
                        },
                        new ModDisplay
                        {
                            Anchor = Anchor.TopCentre,
                            Origin = Anchor.TopCentre,
                            AutoSizeAxes = Axes.Both,
                            Margin = new MarginPadding { Top = 20 },
                            Current = mods
                        },
                    },
                }
            };

            Loading = true;
        }

        private class MetadataLineLabel : OsuSpriteText
        {
            public MetadataLineLabel(string text)
            {
                Anchor = Anchor.TopRight;
                Origin = Anchor.TopRight;
                Margin = new MarginPadding { Right = 5 };
                Colour = OsuColour.Gray(0.8f);
                Text = text;
            }
        }

        private class MetadataLineInfo : OsuSpriteText
        {
            public MetadataLineInfo(string text)
            {
                Margin = new MarginPadding { Left = 5 };
                Text = string.IsNullOrEmpty(text) ? @"-" : text;
            }
        }
    }
}
