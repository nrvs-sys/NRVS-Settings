using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace NRVS.Settings
{
    /// <summary>
    /// A ConditionBehavior that evaluates a SettingsBehavior's value against specified criteria.
    /// </summary>
    [CreateAssetMenu(fileName = "Condition_ Settings Behavior Value_ New", menuName = "Behaviors/Conditions/Application/Settings Behavior Value")]
    public class SettingsBehaviorValueConditionBehavior : ConditionBehavior<SettingsBehavior>
    {
        [Header("Settings Behavior Value Settings")]

        public int intValue;
        public MathComparisonType intComparisonType;

        [Space(10)]

        public float floatValue;
        public MathComparisonType floatComparisonType;

        [Space(10)]

        public string stringValue;

        protected override bool Evaluate(SettingsBehavior value)
        {
            switch (value.settingType)
            {
                case SettingsBehavior.SettingType.IntSetting:
                    return EvaluateInt(value.GetInt());
                case SettingsBehavior.SettingType.FloatSetting:
                    return EvaluateFloat(value.GetFloat());
                case SettingsBehavior.SettingType.StringSetting:
                    return EvaluateString(value.GetString());
                default:
                    return false;
            }
        }

        bool EvaluateInt(int value) => MathComparison.Compare(intComparisonType, value, intValue);
        bool EvaluateFloat(float value) => MathComparison.Compare(floatComparisonType, value, floatValue);
        bool EvaluateString(string value) => value == stringValue;
    }
}
