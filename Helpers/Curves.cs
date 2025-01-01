using System;
using UnityEngine;

namespace HeadshotDarkness.Helpers
{
    public static class Curves
    {
        public static AnimationCurve EnableCurve = new AnimationCurve(
            new Keyframe(0f, 0f),
            new Keyframe(0.01f, 3f)
        );
    }
}