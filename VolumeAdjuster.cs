using System;
using System.Collections;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using UnityEngine;

namespace HeadshotDarkness
{
    public class VolumeAdjuster : MonoBehaviour
    {
        public static VolumeAdjuster Instance { get; private set; }

        private void Awake()
        {
            if (Instance == null)
            {
                Instance = this;
                DontDestroyOnLoad(gameObject);
            }
            else
            {
                Destroy(gameObject);
            }
        }

        public void StartVolumeAdjust(float target, float duration)
        {
            StartCoroutine(AdjustVolume(target, duration));
        }

        private IEnumerator AdjustVolume(float target, float duration)
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
