using NaughtyAttributes;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Events;
using UnityAtoms.BaseAtoms;

namespace NRVS.Settings
{
	[CreateAssetMenu(fileName = "Settings Behavior_ New", menuName = "Behaviors/Application/Settings Behavior")]
	public class SettingsBehavior : ScriptableObject
	{
		public enum SettingsBackend
		{
			PlayerPrefs,
			PlayerData
		}

		[Header("Settings")]
		public string settingName;
		public SettingType settingType;
		public SettingsBackend settingsBackend = SettingsBackend.PlayerPrefs;

		[Space(10)]

		[SerializeField]
		bool logValueChanges = true;

		public enum SettingType
		{
			IntSetting,
			FloatSetting,
			StringSetting,
		}

		[Button]
		void LogValueButton() => Debug.Log($"Setting: {settingName}, Value: {GetString()}");

		[Button]
		void DeleteValueButton()
		{
#if UNITY_EDITOR

			if (!UnityEditor.EditorUtility.DisplayDialog(
				$"Delete Setting at Key: {settingName}?",
				$"Are you sure you want to delete the Setting at Key: {settingName}?",
				"Confirm", "Cancel"))
				return;

#endif

			DeleteValue();
		}

		[Header("Int Settings")]

		[ShowIf(nameof(isInt))]
		public int defaultInt = 0;

		[ShowIf(nameof(isInt))]
		public UnityEvent<int> onIntChanged;

		[Header("Float Settings")]

		[ShowIf(nameof(isFloat))]
		public float defaultFloat = 0;

		[ShowIf(nameof(isFloat))]
		public UnityEvent<float> onFloatChanged;

		[Header("String Settings")]

		[ShowIf(nameof(isString))]
		public string defaultString = string.Empty;

		[ShowIf(nameof(isString))]
		public UnityEvent<string> onStringChanged;

		public bool isInt => settingType == SettingType.IntSetting;
		public bool isFloat => settingType == SettingType.FloatSetting;
		public bool isString => settingType == SettingType.StringSetting;


		public int GetInt()
		{
			switch (settingsBackend)
			{
				case SettingsBackend.PlayerData:
					return PlayerData.GetInt(settingName, defaultInt);
				case SettingsBackend.PlayerPrefs:
				default:
					return PlayerPrefs.GetInt(settingName, defaultInt);
			}
		}

		public bool GetBoolFromInt() => GetInt() == 1 ? true : false;

		public float GetFloat()
		{
			switch (settingsBackend)
			{
				case SettingsBackend.PlayerData:
					return PlayerData.GetFloat(settingName, defaultFloat);
				case SettingsBackend.PlayerPrefs:
				default:
					return PlayerPrefs.GetFloat(settingName, defaultFloat);
			}
		}

		public string GetString()
		{
			switch (settingType)
			{
				case SettingType.IntSetting:
					return GetInt().ToString();
				case SettingType.FloatSetting:
					return GetFloat().ToString();
				case SettingType.StringSetting:
				default:
					switch (settingsBackend)
					{
						case SettingsBackend.PlayerData:
							return PlayerData.GetString(settingName, defaultString);
						case SettingsBackend.PlayerPrefs:
						default:
							return PlayerPrefs.GetString(settingName, defaultString);
					}
			}
		}

		public void SetValue(int value)
		{
			// exit early if the value is already set to avoid unnecessary writes
			if (GetInt() == value)
				return;

			switch (settingsBackend)
			{
				case SettingsBackend.PlayerData:
					PlayerData.SetInt(settingName, value);
					break;
				case SettingsBackend.PlayerPrefs:
				default:
					PlayerPrefs.SetInt(settingName, value);
					break;
			}

			if (logValueChanges)
				Debug.Log($"{settingName} is being saved to {value}");

			onIntChanged?.Invoke(value);
		}

		public void SetValue(float value)
		{
			// exit early if the value is already set to avoid unnecessary writes
			if (GetFloat() == value)
				return;

			switch (settingsBackend)
			{
				case SettingsBackend.PlayerData:
					PlayerData.SetFloat(settingName, value);
					break;
				case SettingsBackend.PlayerPrefs:
				default:
					PlayerPrefs.SetFloat(settingName, value);
					break;
			}

			if (logValueChanges)
				Debug.Log($"{settingName} is being saved to {value}");

			onFloatChanged?.Invoke(value);
		}

		public void SetValue(string value)
		{
			// exit early if the value is already set to avoid unnecessary writes
			if (GetString() == value)
				return;

			switch (settingsBackend)
			{
				case SettingsBackend.PlayerData:
					PlayerData.SetString(settingName, value);
					break;
				case SettingsBackend.PlayerPrefs:
				default:
					PlayerPrefs.SetString(settingName, value);
					break;
			}

			if (logValueChanges)
				Debug.Log($"{settingName} is being saved to {value}");

			onStringChanged?.Invoke(value);
		}

		public void SetValue(SettingsBehavior value)
		{
			switch (settingType)
			{
				case SettingType.IntSetting:
					SetValue(value.GetInt());
					break;
				case SettingType.FloatSetting:
					SetValue(value.GetFloat());
					break;
				case SettingType.StringSetting:
					SetValue(value.GetString());
					break;
			}
		}

		public void SetValue(IntReference value) => SetValue(value.Value);
		public void SetValue(FloatReference value) => SetValue(value.Value);
		public void SetValue(StringReference value) => SetValue(value.Value);

		public void SetValue(IntVariable value) => SetValue(value.Value);
		public void SetValue(FloatVariable value) => SetValue(value.Value);
		public void SetValue(StringVariable value) => SetValue(value.Value);

		public void SetIntFromBool(bool value) => SetValue(value ? 1 : 0);
		public void SetIntFromBool(BoolReference value) => SetIntFromBool(value.Value);
		public void SetIntFromBool(BoolVariable value) => SetIntFromBool(value.Value);

		public void CopyToVariable(IntVariable variable) => variable.Value = GetInt();
		public void CopyToVariable(FloatVariable variable) => variable.Value = GetFloat();
		public void CopyToVariable(StringVariable variable) => variable.Value = GetString();
		public void CopyToVariable(BoolVariable variable) => variable.Value = GetBoolFromInt();

		public void DeleteValue()
		{
			switch (settingsBackend)
			{
				case SettingsBackend.PlayerData:
					PlayerData.DeleteKey(settingName);
					break;
				case SettingsBackend.PlayerPrefs:
				default:
					PlayerPrefs.DeleteKey(settingName);
					break;
			}
		}

		/// <summary>
		/// Calls the Save method on the selected settings backend.
		/// </summary>
		public void Save()
		{
			switch (settingsBackend)
			{
				case SettingsBackend.PlayerData:
					PlayerData.Save();
					break;
				case SettingsBackend.PlayerPrefs:
				default:
					PlayerPrefs.Save();
					break;
			}
		}
	}
}
