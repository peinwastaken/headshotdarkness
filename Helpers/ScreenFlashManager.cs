using System.Collections;
using UnityEngine;
using UnityEngine.UI;

namespace HeadshotDarkness.Helpers
{
    public class ScreenFlashManager : MonoBehaviour
    {
        public static ScreenFlashManager Instance;

        private Canvas _canvas;
        private Image _image;
        private CanvasScaler _canvasScaler;

        public static ScreenFlashManager Create()
        {
            PluginDebug.LogInfo("Creating ScreenFlashManager");
            GameObject gameObject = new GameObject("ScreenFlashManager");
            ScreenFlashManager flashManager = gameObject.AddComponent<ScreenFlashManager>();
            return flashManager;
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

            _image = gameObject.AddComponent<Image>();
            _image.color = new Color(1, 1, 1, 0);
        }

        private IEnumerator Flash(float time)
        {
            float elapsedTime = 0f;
            _image.color = new Color(1, 1, 1, 1);

            while (elapsedTime < time)
            {
                elapsedTime += Time.deltaTime;
                yield return null;
            }

            _image.color = new Color(1, 1, 1, 0);
        }

        public void DoScreenFlash(float time)
        {
            StartCoroutine(Flash(time));
        }
    }
}
