using System;
using System.Collections;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using UnityEngine;
using UnityEngine.UI;

namespace HeadshotDarkness
{
    public class ScreenFlash
    {
        public static GameObject screenFlashObject;

        public static void StartFlash(float duration, bool smoothDecay)
        {
            if (screenFlashObject != null)
            {
                GameObject.Destroy(screenFlashObject);
            }

            screenFlashObject = new GameObject("ScreenFlash");
            screenFlashObject.transform.parent = Camera.main.transform;

            Canvas canvas = screenFlashObject.AddComponent<Canvas>();
            canvas.renderMode = RenderMode.ScreenSpaceOverlay;

            Image image = screenFlashObject.AddComponent<Image>();

            CanvasGroup canvasGroup = screenFlashObject.AddComponent<CanvasGroup>();

            ScreenFlashManager manager = screenFlashObject.AddComponent<ScreenFlashManager>();
            manager.lifeTime = duration;
            manager.smoothDecay = false;
        }
    }

    public class ScreenFlashManager : MonoBehaviour
    {
        public float lifeTime;
        public bool smoothDecay;

        private CanvasGroup canvasGroup;

        private float timeAlive = 0f;
        private float decayTime = 0.4f;

        public void Start()
        {
            canvasGroup = GetComponent<CanvasGroup>();
        }

        public void Update()
        {
            float time = 0f;

            if (time < lifeTime)
            {
                time += Time.deltaTime;
                if (smoothDecay)
                {
                    canvasGroup.alpha = 1f - (time / lifeTime);
                }
            }

            Destroy(gameObject);
        }
    }
}
