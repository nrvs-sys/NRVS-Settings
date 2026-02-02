using NRVS.Store;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace NRVS.Settings
{
    /// <summary>
    /// A StoreBehavior for SettingsCache objects.
    /// </summary>
    [CreateAssetMenu(fileName = "Store_ Settings Cache_ New", menuName = "Behaviors/Store/Settings Cache")]
    public class SettingsCacheStoreBehavior : StoreBehavior<SettingsCache> { }
}