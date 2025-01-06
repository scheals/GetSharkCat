using System;
using BepInEx.Configuration;

namespace GetSharkCat;

public static class Configs
{
    public static bool EffectGrantingEnabled
    {
        get => m_EffectGrantingEnabled.Value;
        set
        {
            m_EffectGrantingEnabled.Value = value;
            Plugin.Instance.Config.Save();
        }
    }

    public static bool CheckForDuplicates
    {
        get => m_CheckForDuplicates.Value;
        set
        {
            m_CheckForDuplicates.Value = value;
            Plugin.Instance.Config.Save();
        }
    }

    private static ConfigEntry<bool> m_EffectGrantingEnabled = Bind("General", "Add Cat perk effect", false, "When set to true adds the associated reward perk.");
    private static ConfigEntry<bool> m_CheckForDuplicates = Bind("General", "Check for duplicate cats and effects", true,
        "When set to true it checks for presence of other cats or effects being active already.");


    private static ConfigEntry<T> Bind<T>(string section, string key, T defaultValue, string description)
    {
        return Plugin.Instance.Config.Bind(section, key, defaultValue, new ConfigDescription(description, null, Array.Empty<object>()));
    }
}
