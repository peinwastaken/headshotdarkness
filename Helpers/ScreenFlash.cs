using System;
using System.Collections;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using UnityEngine;
using UnityEngine.UI;

namespace HeadshotDarkness.Helpers
{
    public class ScreenFlash
    {
        public static GameObject screenFlashObject;

        // right.. what i should do instead is have an instance with the component and whatnot and run a method and set values but whatever lol
        public static void StartFlash()
        {
            if (screenFlashObject != null)
            {
                UnityEngine.Object.Destroy(screenFlashObject);
            }

            screenFlashObject = new GameObject("ScreenFlash");
            screenFlashObject.transform.parent = Camera.main.transform;

            Canvas canvas = screenFlashObject.AddComponent<Canvas>();
            canvas.renderMode = RenderMode.ScreenSpaceOverlay;

            Image image = screenFlashObject.AddComponent<Image>();

            CanvasGroup canvasGroup = screenFlashObject.AddComponent<CanvasGroup>();

            ScreenFlashManager manager = screenFlashObject.AddComponent<ScreenFlashManager>();
        }
    }

    public class ScreenFlashManager : MonoBehaviour
    {
        public void Start()
        {
            // Uhhh..
        }

        public void Update()
        {
            Destroy(gameObject);
        }
    }
}
