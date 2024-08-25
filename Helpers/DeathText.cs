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
    public class DeathTextHelper
    {
        public static GameObject deathTextObject { get; private set; }
        private static Font arial = Resources.GetBuiltinResource<Font>("Arial.ttf");

        public static void CreateDeathText(string textString, int textSize, float textDuration, float fadeIn, float fadeOut, float fadeDelay)
        {
            if (deathTextObject != null)
            {
                UnityEngine.Object.Destroy(deathTextObject);
            }

            deathTextObject = new GameObject("DeathText", typeof(Canvas), typeof(CanvasScaler));

            // do canvas
            Canvas canvas = deathTextObject.GetOrAddComponent<Canvas>();
            canvas.renderMode = RenderMode.ScreenSpaceOverlay;
            canvas.sortingOrder = 999;
            canvas.transform.SetAsFirstSibling();

            // do scaler....?
            CanvasScaler scaler = deathTextObject.GetOrAddComponent<CanvasScaler>();

            // do text
            Text text = deathTextObject.GetOrAddComponent<Text>();
            text.text = textString;
            text.font = arial;
            text.fontSize = textSize;
            text.alignment = TextAnchor.MiddleCenter;

            // do manager
            DeathTextManager deathText = deathTextObject.GetOrAddComponent<DeathTextManager>();
            deathText.lifeTime = textDuration;
            deathText.fadeInTime = fadeIn;
            deathText.fadeOutTime = fadeOut;
            deathText.fadeDelay = fadeDelay;

            deathTextObject.transform.parent = Camera.main.transform;
        }
    }

    public class DeathTextManager : MonoBehaviour
    {
        public float lifeTime;
        public float fadeInTime;
        public float fadeOutTime;
        public float fadeDelay;

        private float timeAlive;
        private Text deathText;
        private Color colorWhite;
        private Color colorTransparent;

        public void Start()
        {
            deathText = gameObject.GetComponent<Text>();
            colorWhite = new Color(1, 1, 1, 1);
            colorTransparent = new Color(1, 1, 1, 0);
            deathText.color = colorTransparent;
            timeAlive = 0f;

            StartCoroutine(DoDeathTextFade());
        }

        private IEnumerator FadeText(float targetAlpha, float duration)
        {
            Color curColor = deathText.color;
            float startAlpha = curColor.a;
            float elapsedTime = 0f;

            while (elapsedTime < duration)
            {
                elapsedTime += Time.deltaTime;
                float alpha = startAlpha + (targetAlpha - startAlpha) * (elapsedTime / duration);
                deathText.color = new Color(1, 1, 1, alpha);
                yield return null;
            }

            deathText.color = new Color(1, 1, 1, targetAlpha);
        }

        private IEnumerator DoDeathTextFade()
        {
            PluginDebug.LogInfo("FadeDelay");
            yield return new WaitForSeconds(fadeDelay); // fade delay

            PluginDebug.LogInfo("FadeIn");
            yield return StartCoroutine(FadeText(1f, fadeInTime)); // fade in

            PluginDebug.LogInfo("FadeHold");
            yield return new WaitForSeconds(lifeTime - (fadeInTime + fadeOutTime)); // hold fade

            PluginDebug.LogInfo("FadeOut");
            yield return StartCoroutine(FadeText(0f, fadeOutTime)); // fade out
        }
        public void Update()
        {
            timeAlive += Time.deltaTime;

            if (timeAlive > lifeTime + fadeDelay)
            {
                Destroy(gameObject);
            }
        }
    }
}
