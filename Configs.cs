using System;
using BepInEx.Configuration;

namespace GetSharkCat;

public static class Configs
{
    public static bool EffectEnabled
    {
        get => m_EffectEnabled.Value;
        set
        {
            m_EffectEnabled.Value = value;
            Plugin.Instance.Config.Save();
        }


    }

    private static ConfigEntry<bool> m_EffectEnabled = Bind("General", "Add Cat perk effect", false, "When set to true adds the associated reward perk.");

    private static ConfigEntry<T> Bind<T>(string section, string key, T defaultValue, string description)
    {
        return Plugin.Instance.Config.Bind(section, key, defaultValue, new ConfigDescription(description, null, Array.Empty<object>()));
    }
}
