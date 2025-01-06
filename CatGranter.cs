using BepInEx;
using BepInEx.Logging;
using Eremite;
using Eremite.Model;
using GetSharkCat;


namespace GetSharkCat;
public class CatGranter

{
    public static void SafeGrant()
    {
        SO.NpcsService.RemoveNpc(Cat); // To make sure no doubles are present when someone has the cycle reward.
        Plugin.myLog.LogInfo("Cat NPC removed.");
        GrantCat();

        if ((Configs.EffectGrantingEnabled && IsSafetyOn()))
        {
            GrantCatEffect();
            SO.NpcsService.RemoveNpc(Cat); // Doing this because applying the perk spawns the cat.
            Plugin.myLog.LogInfo("Cat NPC removed.");
        }
    }

    public static void UnsafeGrant()
    {
        GrantCat();
        if (Configs.EffectGrantingEnabled)
        {
            GrantCatEffect();
        }
    }

    private static void GrantCat()
    {
        SO.NpcsService.SpawnNewNpc(Cat, SO.BuildingsService.GetMainHearth().Entrance);
        Plugin.myLog.LogInfo("Cat NPC added.");
    }

    private static void GrantCatEffect()
    {
        EffectModel catEffectModel = MB.Settings.GetEffect("[WE] First Elder Bonus");
        catEffectModel.Apply();
        Plugin.myLog.LogInfo("Cat effect cycle perk added.");
    }

    private static GameNpcModel Cat
    {
        get
        {
            return SO.Settings.GetNpc("Cat");

        }
    }

    private static bool IsSafetyOn()
    {
        return !SO.PerksService.HasPerk("[WE] First Elder Bonus");
    }

}