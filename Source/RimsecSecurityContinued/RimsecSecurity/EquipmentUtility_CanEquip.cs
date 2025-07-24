using System;
using HarmonyLib;
using RimWorld;
using Verse;

namespace RimsecSecurity;

[HarmonyPatch(typeof(EquipmentUtility), "CanEquip", new Type[]
{
	typeof(Thing),
	typeof(Pawn),
	typeof(string),
	typeof(bool)
}, new ArgumentType[]
{
	ArgumentType.Normal,
	ArgumentType.Normal,
	ArgumentType.Out,
	ArgumentType.Normal
})]
public class EquipmentUtility_CanEquip
{
	public static void Postfix(ref bool __result, Thing thing, Pawn pawn, ref string cantReason, bool checkBonded = true)
	{
		if (!__result)
		{
			return;
		}
		RSPeacekeeperWeaponModExt modExtension = thing.def.GetModExtension<RSPeacekeeperWeaponModExt>();
		if (modExtension != null && !(modExtension.weightType != "heavy"))
		{
			RSPeacekeeperModExt modExtension2 = pawn.def.GetModExtension<RSPeacekeeperModExt>();
			if (modExtension2 == null || modExtension2.gunDef != thing.def)
			{
				cantReason = "RSGunTooHeavy".Translate();
				__result = false;
			}
		}
	}
}
