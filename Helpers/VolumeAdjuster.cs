using System.Collections;
using UnityEngine;

namespace HeadshotDarkness.Helpers
{
    public class VolumeAdjuster
    {
        public static IEnumerator FadeVolume(float target, float duration)
        {
            float _startVolume = AudioListener.volume;
            float _time = 0f;

            while (_time < duration)
            {
                _time += Time.deltaTime;
                AudioListener.volume = Mathf.Lerp(_startVolume, target, _time / duration);
                yield return null;
            }

            AudioListener.volume = target;
        }
    }
}
