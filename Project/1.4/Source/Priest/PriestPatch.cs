﻿using HarmonyLib;
using RimWorld;
using System.Linq;
using Verse;

namespace NewRatkin
{

    [HarmonyPatch(typeof(PawnGenerator))]
    [HarmonyPatch("TryGenerateNewPawnInternal")]
    public static class PawnGeneratorPatch
    {
        [HarmonyPostfix]
        static void Postfix(Pawn __result)
        {
            if (__result == null) return;

            if (BackstoryCache.Ratkin_Sister == null)
            {
                BackstoryCache.CacheBackstorys(__result);
            }

            if (__result?.story?.Adulthood == BackstoryCache.Ratkin_Sister)
            {
                __result.abilities?.GainAbility(RatkinAbilityDefOf.RK_PrayerService);
            }
        }
    }

    // For backward compatiblity
    [HarmonyPatch(typeof(Pawn_AbilityTracker))]
    [HarmonyPatch("ExposeData")]
    public static class Pawn_AbilityTrackerPatch
    {
        [HarmonyPostfix]
        static void Postfix(Pawn_AbilityTracker __instance, Pawn ___pawn)
        {
            if (Scribe.mode == LoadSaveMode.ResolvingCrossRefs)
            {
                if (___pawn?.story?.Adulthood == BackstoryCache.Ratkin_Sister && !__instance.abilities.Any(x => x.def == RatkinAbilityDefOf.RK_PrayerService))
                {
                    __instance.GainAbility(RatkinAbilityDefOf.RK_PrayerService);
                }
            }
        }
    }

    [HarmonyPatch(typeof(NegativeInteractionUtility))]
    [HarmonyPatch("NegativeInteractionChanceFactor")]
    public static class NegativeInteractionUtilityPatch
    {
        [HarmonyPrefix]
        static bool Prefix(ref float __result, Pawn initiator, Pawn recipient)
        {
            if (initiator.story?.Adulthood == BackstoryCache.Ratkin_Sister)
            {
                __result = 0f;
                return false;
            }

            return true;
        }
    }
}
