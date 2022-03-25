using System;
using System.Collections.Generic;
using HarmonyLib;
using RimWorld;
using ScavengingExpansion.Comps;
using UnityEngine;
using Verse;

namespace ScavengingExpansion.Harmony.Patches
{
    [HarmonyPatch(typeof(Projectile))]
    [HarmonyPatch("Launch")]
    [HarmonyPatch(new Type[] {typeof(Thing), typeof(Vector3), typeof(LocalTargetInfo), typeof(LocalTargetInfo), typeof(ProjectileHitFlags), typeof(bool), typeof(Thing), typeof(ThingDef)})]
    public class PatchBulletLaunch
    {
        [HarmonyPrefix]
        public static void Prefix(Thing equipment, ref Projectile __instance)
        {
            if (__instance is Bullet && equipment?.TryGetComp<CompOverclockable>() is CompOverclockable comp && comp.Overclocked)
            {
                if (__instance.def.projectile.extraDamages == null)
                {
                    __instance.def.projectile.extraDamages = new List<ExtraDamage>();
                }

                ExtraDamage toAdd = new ExtraDamage();
                toAdd.def = DamageDefOf.EMP;
                toAdd.amount = __instance.def.projectile.GetDamageAmount(1f) * comp.Props.AdditionalEMPDamagePercent;
                __instance.def.projectile.extraDamages.Add(toAdd);
            }
        }
    }
}