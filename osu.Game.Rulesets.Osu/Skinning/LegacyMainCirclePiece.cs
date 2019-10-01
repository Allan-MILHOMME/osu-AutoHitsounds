// Copyright (c) ppy Pty Ltd <contact@ppy.sh>. Licensed under the MIT Licence.
// See the LICENCE file in the repository root for full licence text.

using osu.Framework.Allocation;
using osu.Framework.Bindables;
using osu.Framework.Graphics;
using osu.Framework.Graphics.Containers;
using osu.Framework.Graphics.Sprites;
using osu.Game.Graphics;
using osu.Game.Graphics.Sprites;
using osu.Game.Rulesets.Objects.Drawables;
using osu.Game.Rulesets.Osu.Objects;
using osu.Game.Rulesets.Osu.Objects.Drawables;
using osu.Game.Skinning;
using osuTK;
using osuTK.Graphics;
using System.Collections.Generic;
using System.Linq;

namespace osu.Game.Rulesets.Osu.Skinning
{
    public class LegacyMainCirclePiece : CompositeDrawable
    {
        public LegacyMainCirclePiece()
        {
            Size = new Vector2(OsuHitObject.OBJECT_RADIUS * 2);
        }

        private Sprite hitCircleSprite;
        private SkinnableSpriteText hitCircleText;

        private List<Drawable> scalables;

        private readonly IBindable<ArmedState> state = new Bindable<ArmedState>();
        private readonly Bindable<Color4> accentColour = new Bindable<Color4>();
        private readonly IBindable<int> indexInCurrentCombo = new Bindable<int>();
        private readonly IBindable<bool> expandNumberPiece = new BindableBool();

        [Resolved]
        private DrawableHitObject drawableObject { get; set; }

        [BackgroundDependencyLoader]
        private void load(ISkinSource skin)
        {
            OsuHitObject osuObject = (OsuHitObject)drawableObject.HitObject;
            DrawableHitCircle drawableCircle = (DrawableHitCircle)drawableObject;

            InternalChildren = new Drawable[]
            {
                hitCircleSprite = new Sprite
                {
                    Texture = skin.GetTexture("hitcircle"),
                    Colour = drawableObject.AccentColour.Value,
                    Anchor = Anchor.Centre,
                    Origin = Anchor.Centre,
                },
                hitCircleText = new SkinnableSpriteText(new OsuSkinComponent(OsuSkinComponents.HitCircleText), _ => new OsuSpriteText
                {
                    Font = OsuFont.Numeric.With(size: 40),
                    UseFullGlyphHeight = false,
                }, confineMode: ConfineMode.NoScaling),
                new Sprite
                {
                    Texture = skin.GetTexture("hitcircleoverlay"),
                    Anchor = Anchor.Centre,
                    Origin = Anchor.Centre,
                }
            };

            state.BindTo(drawableObject.State);
            accentColour.BindTo(drawableObject.AccentColour);
            indexInCurrentCombo.BindTo(osuObject.IndexInCurrentComboBindable);
            expandNumberPiece.BindTo(drawableCircle.ExpandNumberPiece);
        }

        protected override void LoadComplete()
        {
            base.LoadComplete();

            state.BindValueChanged(updateState, true);
            accentColour.BindValueChanged(colour => hitCircleSprite.Colour = colour.NewValue, true);
            indexInCurrentCombo.BindValueChanged(index => hitCircleText.Text = (index.NewValue + 1).ToString(), true);
            expandNumberPiece.BindValueChanged(expand =>
            {
                scalables = InternalChildren.ToList();

                if (!expand.NewValue)
                    scalables.Remove(hitCircleText);
            }, true);
        }

        private void updateState(ValueChangedEvent<ArmedState> state)
        {
            const double legacy_fade_duration = 240;

            switch (state.NewValue)
            {
                case ArmedState.Hit:
                    this.FadeOut(legacy_fade_duration, Easing.Out);
                    scalables.ForEach(d => d.ScaleTo(1.4f, legacy_fade_duration, Easing.Out));
                    break;
            }
        }
    }
}
