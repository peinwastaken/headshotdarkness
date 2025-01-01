using UnityEngine;
using UnityEngine.UI;
using System.Collections;

namespace HeadshotDarkness.Helpers
{
    public struct DeathTextInfo
    {
        public string Text;
        public bool Contextual;
        public float Lifetime;
        public float Size;
        public float FadeInTime;
        public float FadeOutTime;
        public float FadeDelayTime;
    }

    public class DeathTextManager : MonoBehaviour
    {
        public static DeathTextManager Instance { get; private set; }

        private static Font _arial = Resources.GetBuiltinResource<Font>("Arial.ttf");
        private static Color _colorWhite = new Color(1, 1, 1, 1);
        private static Color _colorTransparent = new Color(1, 1, 1, 0);

        private Canvas _canvas;
        private CanvasScaler _canvasScaler;
        private Text _deathText;

        public static DeathTextManager Create()
        {
            GameObject gameObject = new GameObject("DeathTextManager");
            DeathTextManager deathTextManager = gameObject.AddComponent<DeathTextManager>();
            return deathTextManager;
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

            _canvas = gameObject.AddComponent<Canvas>();
            _canvas.renderMode = RenderMode.ScreenSpaceOverlay;
            _canvas.sortingOrder = 999;

            _canvasScaler = gameObject.AddComponent<CanvasScaler>();

            _deathText = gameObject.AddComponent<Text>();
            _deathText.color = new Color(1, 1, 1, 0);
            _deathText.font = _arial;
            _deathText.fontSize = Plugin.DeathTextFontSize.Value;
            _deathText.alignment = TextAnchor.MiddleCenter;
        }

        private IEnumerator FadeText(float targetAlpha, float duration)
        {
            Color curColor = _deathText.color;
            float startAlpha = curColor.a;
            float elapsedTime = 0f;

            while (elapsedTime < duration)
            {
                elapsedTime += Time.deltaTime;
                float newAlpha = startAlpha + (targetAlpha - startAlpha) * (elapsedTime / duration);
                _deathText.color = new Color(1, 1, 1, newAlpha);
                yield return null;
            }

            _deathText.color = new Color(1, 1, 1, targetAlpha);
        }

        private IEnumerator TextSequence(string text, int size, float time, float fadeInTime, float fadeOutTime, float fadeDelay)
        {
            _deathText.text = text;
            _deathText.fontSize = size;

            PluginDebug.LogInfo("text fade delay");
            yield return new WaitForSeconds(fadeDelay);

            PluginDebug.LogInfo("text fade in");
            yield return StartCoroutine(FadeText(1f, fadeInTime));

            PluginDebug.LogInfo("text fade hold");
            yield return new WaitForSeconds(time);

            PluginDebug.LogInfo("text fade out");
            yield return StartCoroutine(FadeText(0f, fadeOutTime));
        }

        public void DoDeathText(string text, int size, float time, float fadeInTime, float fadeOutTime, float fadeDelay)
        {
            StartCoroutine(TextSequence(text, size, time, fadeInTime, fadeOutTime, fadeDelay));
        }
    }
}
