// Copyright (c) ppy Pty Ltd <contact@ppy.sh>. Licensed under the MIT Licence.
// See the LICENCE file in the repository root for full licence text.

#nullable disable

using osu.Framework.Allocation;
using osu.Framework.Graphics;
using osu.Game.Graphics;
using osu.Game.Graphics.Containers;

namespace osu.Game.Screens.OnlinePlay.Lounge.Components
{
    public abstract partial class OnlinePlayPill : OnlinePlayComposite
    {
        protected PillContainer Pill;
        protected OsuTextFlowContainer TextFlow;

        protected OnlinePlayPill()
        {
            AutoSizeAxes = Axes.Both;
        }

        [BackgroundDependencyLoader]
        private void load()
        {
            InternalChild = Pill = new PillContainer
            {
                Child = TextFlow = new OsuTextFlowContainer(s => s.Font = OsuFont.GetFont(size: 12))
                {
                    AutoSizeAxes = Axes.Both,
                    Anchor = Anchor.CentreLeft,
                    Origin = Anchor.CentreLeft
                }
            };
        }
    }
}
