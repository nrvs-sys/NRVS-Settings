using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using NaughtyAttributes;

namespace NRVS.Settings
{
    [CreateAssetMenu(fileName = "Reset Settings Behavior_ New", menuName = "Behaviors/Application/Reset Settings Behavior")]
    public class ResetSettingsBehavior : ScriptableObject
    {
        [SerializeField]
        private List<SettingsBehavior> settingsToReset;

        [Button]
        public void ResetSettings()
        {
            Debug.Log("Resetting Settings Behavior Values");

            foreach (var setting in settingsToReset)
            {
                if (setting is null)
                    continue;

                Debug.Log($"Resetting Setting: {setting.settingName}");

                setting.DeleteValue();
            }
        }
    }
}
