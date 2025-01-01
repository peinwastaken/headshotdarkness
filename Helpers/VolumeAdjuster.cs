using System.Collections;
using UnityEngine;

namespace HeadshotDarkness.Helpers
{
    public class VolumeAdjuster : MonoBehaviour
    {
        public static VolumeAdjuster Instance;

        public static VolumeAdjuster Create()
        {
            PluginDebug.LogInfo("Creating VolumeAdjuster");
            GameObject gameObject = new GameObject("VolumeAdjuster");
            VolumeAdjuster volumeAdjuster = gameObject.AddComponent<VolumeAdjuster>();
            return volumeAdjuster;
        }

        private IEnumerator DoVolumeFade(float target, float duration)
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

        private void Awake()
        {
            if (Instance != null)
            {
                Destroy(gameObject);
                return;
            }

            Instance = this;
            DontDestroyOnLoad(gameObject);
        }

        public void FadeVolume(float target, float duration)
        {
            StartCoroutine(DoVolumeFade(target, duration));
        }
    }
}
