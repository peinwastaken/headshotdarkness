using EFT;
using EFT.HealthSystem;
using HeadshotDarkness.Enums;
using HeadshotDarkness.Patches;
using System;
using System.Reflection;
using UnityEngine;

namespace HeadshotDarkness.Helpers
{
    public class DarknessManager : MonoBehaviour
    {
        public static DarknessManager Instance;

        private Player _localPlayer;
        private ActiveHealthController _activeHealthController;
        private Camera _camera;
        private DeathFade _deathFade;
        private FieldInfo _animationCurveField = typeof(DeathFade).GetField("animationCurve_0", BindingFlags.NonPublic | BindingFlags.Instance);

        public static DarknessManager Create()
        {
            PluginDebug.LogInfo("Creating DarknessManager");
            GameObject gameObject = new GameObject("DarknessManager");
            DarknessManager darknessManager = gameObject.AddComponent<DarknessManager>();
            return darknessManager;
        }

        public void OnGameStarted()
        {
            _localPlayer = Util.GetLocalPlayer();
            _activeHealthController = _localPlayer.ActiveHealthController;
            _camera = CameraClass.Instance.Camera;
            _deathFade = _camera.GetComponent<DeathFade>();

            _activeHealthController.DiedEvent += DoHeadshotDarkness;
        }

        private void Awake() {
            if (Instance != null)
            {
                Destroy(gameObject);
                return;
            }

            Instance = this;
            DontDestroyOnLoad(gameObject);
        }

        private void DoHeadshotDarkness(EDamageType damageType) // ...
        {
            _activeHealthController.DiedEvent -= DoHeadshotDarkness;

            Type deathType = typeof(DeathFade);
            Player player = Util.GetLocalPlayer();
            EBodyPart lastBodyPart = player.LastDamagedBodyPart;
            EDamageType lastDamageType = player.LastDamageType;
            EDarknessType darknessType = Util.GetDeathFadeType(lastBodyPart, lastDamageType);

            if (darknessType != EDarknessType.Vanilla)
            {
                _animationCurveField?.SetValue(_deathFade, Curves.EnableCurve);

                if (darknessType == EDarknessType.ScreenFlash)
                {
                    ScreenFlashManager.Instance?.DoScreenFlash(0.05f);
                }

                if (Plugin.DeathTextEnabled.Value == true)
                {
                    string deathText = Util.GetDeathString(lastBodyPart, lastDamageType);
                    DeathTextManager.Instance?.DoDeathText(
                        deathText,
                        Plugin.DeathTextFontSize.Value,
                        Plugin.DeathTextLifeTime.Value,
                        Plugin.DeathTextFadeInTime.Value,
                        Plugin.DeathTextFadeOutTime.Value,
                        Plugin.DeathTextFadeDelayTime.Value
                    );
                }

                PlayUISoundPatch.SkipSound = true;
                VolumeAdjuster.Instance?.FadeVolume(0f, Plugin.AudioFadeTime.Value);
            }
        }
    }
}
