using EFT;
using EFT.CameraControl;
using EFT.HealthSystem;
using HeadshotDarkness.Enums;
using HeadshotDarkness.Helpers;
using HeadshotDarkness.Patches;
using System;
using System.Reflection;
using UnityEngine;

namespace HeadshotDarkness.Components
{
    public class DarknessManager : MonoBehaviour
    {
        public static DarknessManager Instance;

        private Player _localPlayer;
        private ActiveHealthController _activeHealthController;
        private Camera _camera;
        private DeathFade _deathFade;

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
            _camera = CameraManager.Instance.Camera;
            _deathFade = _camera.GetComponent<DeathFade>();

            _activeHealthController.DiedEvent += OnPlayerDied;
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

        private void OnPlayerDied(EDamageType damageType) // ...
        {
            if (!Plugin.Enabled.Value) return;
            
            _activeHealthController.DiedEvent -= OnPlayerDied;
            
            Player player = Util.GetLocalPlayer();
            EBodyPart lastBodyPart = player.LastDamagedBodyPart;
            EDamageType lastDamageType = player.LastDamageType;
            EDarknessType darknessType = Util.GetDeathFadeType(lastBodyPart, lastDamageType);
            
            DoHeadshotDarkness(lastBodyPart, lastDamageType, darknessType);
        }

        private void DoHeadshotDarkness(EBodyPart lastBodyPart, EDamageType lastDamageType, EDarknessType darknessType)
        {
            if (darknessType != EDarknessType.Vanilla)
            {
                _deathFade._enableCurve = Curves.EnableCurve;

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
