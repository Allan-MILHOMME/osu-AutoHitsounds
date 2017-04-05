﻿// Copyright (c) 2007-2017 ppy Pty Ltd <contact@ppy.sh>.
// Licensed under the MIT Licence - https://raw.githubusercontent.com/ppy/osu/master/LICENCE

using OpenTK;
using osu.Game.Beatmaps.Samples;
using osu.Game.Modes.Objects.Types;
using System;
using System.Collections.Generic;
using System.Globalization;
using osu.Game.Modes.Objects.Legacy;

namespace osu.Game.Modes.Objects
{
    internal class LegacyHitObjectParser : HitObjectParser
    {
        public override HitObject Parse(string text)
        {
            string[] split = text.Split(',');
            var type = (LegacyHitObjectType)int.Parse(split[3]) & ~LegacyHitObjectType.ColourHax;
            bool combo = type.HasFlag(LegacyHitObjectType.NewCombo);
            type &= ~LegacyHitObjectType.NewCombo;

            HitObject result;

            if ((type & LegacyHitObjectType.Circle) > 0)
            {
                result = new LegacyHit
                {
                    Position = new Vector2(int.Parse(split[0]), int.Parse(split[1])),
                    NewCombo = combo
                };
            }
            else if ((type & LegacyHitObjectType.Slider) > 0)
            {
                CurveType curveType = CurveType.Catmull;
                double length = 0;
                List<Vector2> points = new List<Vector2> { new Vector2(int.Parse(split[0]), int.Parse(split[1])) };

                string[] pointsplit = split[5].Split('|');
                foreach (string t in pointsplit)
                {
                    if (t.Length == 1)
                    {
                        switch (t)
                        {
                            case @"C":
                                curveType = CurveType.Catmull;
                                break;
                            case @"B":
                                curveType = CurveType.Bezier;
                                break;
                            case @"L":
                                curveType = CurveType.Linear;
                                break;
                            case @"P":
                                curveType = CurveType.PerfectCurve;
                                break;
                        }
                        continue;
                    }

                    string[] temp = t.Split(':');
                    Vector2 v = new Vector2(
                        (int)Convert.ToDouble(temp[0], CultureInfo.InvariantCulture),
                        (int)Convert.ToDouble(temp[1], CultureInfo.InvariantCulture)
                    );
                    points.Add(v);
                }

                int repeatCount = Convert.ToInt32(split[6], CultureInfo.InvariantCulture);

                if (repeatCount > 9000)
                    throw new ArgumentOutOfRangeException(nameof(repeatCount), @"Repeat count is way too high");

                if (split.Length > 7)
                    length = Convert.ToDouble(split[7], CultureInfo.InvariantCulture);

                result = new LegacySlider
                {
                    ControlPoints = points,
                    Distance = length,
                    CurveType = curveType,
                    RepeatCount = repeatCount,
                    Position = new Vector2(int.Parse(split[0]), int.Parse(split[1])),
                    NewCombo = combo
                };
            }
            else if ((type & LegacyHitObjectType.Spinner) > 0)
            {
                result = new LegacySpinner
                {
                    EndTime = Convert.ToDouble(split[5], CultureInfo.InvariantCulture)
                };
            }
            else if ((type & LegacyHitObjectType.Hold) > 0)
            {
                // Note: Hold is generated by BMS converts
                result = new LegacyHold
                {
                    Position = new Vector2(int.Parse(split[0]), int.Parse(split[1])),
                    NewCombo = combo
                };
            }
            else
                throw new InvalidOperationException($@"Unknown hit object type {type}");

            result.StartTime = Convert.ToDouble(split[2], CultureInfo.InvariantCulture);

            // TODO: "addition" field

            return result;
        }
    }
}
