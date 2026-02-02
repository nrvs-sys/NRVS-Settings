using NaughtyAttributes;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Events;

namespace NRVS.Settings
{
	/// <summary>
	/// Monobehavior exposing settings behavior events, used for updating script values on settings changes
	/// </summary>
	public class SettingsUtility : MonoBehaviour
	{
		[SerializeField, Expandable]
		private SettingsBehavior setting;

		[SerializeField]
		private bool invokeOnStart = true;

		[ShowIf(nameof(isIntSetting))]
		public UnityEvent<int> onIntChanged;
		[ShowIf(nameof(isFloatSetting))]
		public UnityEvent<float> onFloatChanged;
		[ShowIf(nameof(isStringSetting))]
		public UnityEvent<string> onStringChanged;


		bool isIntSetting => setting != null ? setting.isInt : false;
		bool isFloatSetting => setting != null ? setting.isFloat : false;
		bool isStringSetting => setting != null ? setting.isString : false;


		private void Start()
		{
			if (setting == null)
				return;

			if (invokeOnStart)
				Invoke();

			setting.onIntChanged.AddListener(Setting_onIntChanged);
			setting.onFloatChanged.AddListener(Setting_onFloatChanged);
			setting.onStringChanged.AddListener(Setting_onStringChanged);
		}

		private void OnDestroy()
		{
			if (setting == null)
				return;


			setting.onIntChanged.RemoveListener(Setting_onIntChanged);
			setting.onFloatChanged.RemoveListener(Setting_onFloatChanged);
			setting.onStringChanged.RemoveListener(Setting_onStringChanged);
		}


		private void Invoke()
		{
			switch (setting.settingType)
			{
				case SettingsBehavior.SettingType.IntSetting:
					onIntChanged?.Invoke(setting.GetInt());
					break;
				case SettingsBehavior.SettingType.FloatSetting:
					onFloatChanged?.Invoke(setting.GetFloat());
					break;
				case SettingsBehavior.SettingType.StringSetting:
					onStringChanged?.Invoke(setting.GetString());
					break;
			}
		}


		private void Setting_onIntChanged(int value) => onIntChanged?.Invoke(value);
		private void Setting_onFloatChanged(float value) => onFloatChanged?.Invoke(value);
		private void Setting_onStringChanged(string value) => onStringChanged?.Invoke(value);
	}
}
