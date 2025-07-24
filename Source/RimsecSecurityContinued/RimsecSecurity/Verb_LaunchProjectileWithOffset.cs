using RimWorld;
using UnityEngine;
using Verse;

namespace RimsecSecurity;

internal class Verb_LaunchProjectileWithOffset : Verb_Shoot
{
	protected override bool TryCastShot()
	{
		if (currentTarget.HasThing && currentTarget.Thing.Map != caster.Map)
		{
			return false;
		}
		ThingDef projectile = Projectile;
		if (projectile == null)
		{
			return false;
		}
		ShootLine resultingLine;
		bool flag = TryFindShootLineFromTo(caster.Position, currentTarget, out resultingLine);
		if (verbProps.stopBurstWithoutLos && !flag)
		{
			return false;
		}
		if (base.EquipmentSource != null)
		{
			base.EquipmentSource.GetComp<CompChangeableProjectile>()?.Notify_ProjectileLaunched();
			base.EquipmentSource.GetComp<CompApparelReloadable>()?.UsedOnce();
		}
		Thing manningPawn = caster;
		Thing equipmentSource = base.EquipmentSource;
		CompMannable compMannable = caster.TryGetComp<CompMannable>();
		if (compMannable != null && compMannable.ManningPawn != null)
		{
			manningPawn = compMannable.ManningPawn;
			equipmentSource = caster;
		}
		_ = caster.DrawPos;
		if (caster is Building_TurretGun building_TurretGun)
		{
			_ = building_TurretGun.gun;
		}
		Vector3 origin = ApplyOffset(resultingLine.Source, resultingLine.Dest).ToVector3();
		Projectile projectile2 = (Projectile)GenSpawn.Spawn(projectile, caster.Position, caster.Map);
		if (verbProps.ForcedMissRadius > 0.5f)
		{
			float num = VerbUtility.CalculateAdjustedForcedMiss(verbProps.ForcedMissRadius, currentTarget.Cell - caster.Position);
			if (num > 0.5f)
			{
				int maxExclusive = GenRadial.NumCellsInRadius(num);
				int num2 = Rand.Range(0, maxExclusive);
				if (num2 > 0)
				{
					IntVec3 intVec = currentTarget.Cell + GenRadial.RadialPattern[num2];
					ThrowDebugText("ToRadius");
					ThrowDebugText("Rad\nDest", intVec);
					ProjectileHitFlags projectileHitFlags = ProjectileHitFlags.NonTargetWorld;
					if (Rand.Chance(0.5f))
					{
						projectileHitFlags = ProjectileHitFlags.All;
					}
					if (!canHitNonTargetPawnsNow)
					{
						projectileHitFlags &= ~ProjectileHitFlags.NonTargetPawns;
					}
					projectile2.Launch(manningPawn, origin, intVec, currentTarget, projectileHitFlags, preventFriendlyFire: false, equipmentSource);
					return true;
				}
			}
		}
		ShotReport shotReport = ShotReport.HitReportFor(caster, this, currentTarget);
		Thing randomCoverToMissInto = shotReport.GetRandomCoverToMissInto();
		ThingDef targetCoverDef = randomCoverToMissInto?.def;
		if (!Rand.Chance(shotReport.AimOnTargetChance_IgnoringPosture))
		{
			resultingLine.ChangeDestToMissWild(shotReport.AimOnTargetChance_StandardTarget, flyOverhead: false, caster.Map);
			ThrowDebugText("ToWild" + (canHitNonTargetPawnsNow ? "\nchntp" : ""));
			ThrowDebugText("Wild\nDest", resultingLine.Dest);
			ProjectileHitFlags projectileHitFlags2 = ProjectileHitFlags.NonTargetWorld;
			if (Rand.Chance(0.5f) && canHitNonTargetPawnsNow)
			{
				projectileHitFlags2 |= ProjectileHitFlags.NonTargetPawns;
			}
			projectile2.Launch(manningPawn, origin, resultingLine.Dest, currentTarget, projectileHitFlags2, preventFriendlyFire: false, equipmentSource, targetCoverDef);
			return true;
		}
		if (currentTarget.Thing != null && currentTarget.Thing.def.category == ThingCategory.Pawn && !Rand.Chance(shotReport.PassCoverChance))
		{
			ThrowDebugText("ToCover" + (canHitNonTargetPawnsNow ? "\nchntp" : ""));
			ThrowDebugText("Cover\nDest", randomCoverToMissInto.Position);
			ProjectileHitFlags projectileHitFlags3 = ProjectileHitFlags.NonTargetWorld;
			if (canHitNonTargetPawnsNow)
			{
				projectileHitFlags3 |= ProjectileHitFlags.NonTargetPawns;
			}
			projectile2.Launch(manningPawn, origin, randomCoverToMissInto, currentTarget, projectileHitFlags3, preventFriendlyFire: false, equipmentSource, targetCoverDef);
			return true;
		}
		ProjectileHitFlags projectileHitFlags4 = ProjectileHitFlags.IntendedTarget;
		if (canHitNonTargetPawnsNow)
		{
			projectileHitFlags4 |= ProjectileHitFlags.NonTargetPawns;
		}
		if (!currentTarget.HasThing || currentTarget.Thing.def.Fillage == FillCategory.Full)
		{
			projectileHitFlags4 |= ProjectileHitFlags.NonTargetWorld;
		}
		ThrowDebugText("ToHit" + (canHitNonTargetPawnsNow ? "\nchntp" : ""));
		if (currentTarget.Thing != null)
		{
			projectile2.Launch(manningPawn, origin, currentTarget, currentTarget, projectileHitFlags4, preventFriendlyFire: false, equipmentSource, targetCoverDef);
			ThrowDebugText("Hit\nDest", currentTarget.Cell);
		}
		else
		{
			projectile2.Launch(manningPawn, origin, resultingLine.Dest, currentTarget, projectileHitFlags4, preventFriendlyFire: false, equipmentSource, targetCoverDef);
			ThrowDebugText("Hit\nDest", resultingLine.Dest);
		}
		return true;
	}

	private IntVec3 ApplyOffset(IntVec3 source, IntVec3 dest)
	{
		Rot4 rotation = GetRotation(source, dest);
		IntVec3 intVec = ((burstShotsLeft % 2 != 0) ? RightHandCellOffset(rotation) : LeftHandCellOffset(rotation));
		return new IntVec3(source.x + intVec.x, source.y, source.z + intVec.z);
	}

	private Rot4 GetRotation(IntVec3 source, IntVec3 dest)
	{
		Quaternion.LookRotation((dest.ToVector3() - source.ToVector3()).Yto0()).ToAngleAxis(out var angle, out var _);
		if (angle >= 45f && angle < 135f)
		{
			return Rot4.East;
		}
		if (angle >= 135f && angle < 225f)
		{
			return Rot4.South;
		}
		if (angle >= 225f && angle < 315f)
		{
			return Rot4.West;
		}
		return Rot4.North;
	}

	public IntVec3 LeftHandCellOffset(Rot4 rot)
	{
		return rot.AsInt switch
		{
			0 => new IntVec3(0, 0, 1), 
			1 => new IntVec3(1, 0, 3), 
			2 => new IntVec3(3, 0, 1), 
			3 => new IntVec3(1, 0, 0), 
			_ => default(IntVec3), 
		};
	}

	public IntVec3 RightHandCellOffset(Rot4 rot)
	{
		return rot.AsInt switch
		{
			0 => new IntVec3(3, 0, 1), 
			1 => new IntVec3(1, 0, 0), 
			2 => new IntVec3(0, 0, 1), 
			3 => new IntVec3(1, 0, 3), 
			_ => default(IntVec3), 
		};
	}

	private void ThrowDebugText(string text, bool overwrite = false)
	{
		if (DebugViewSettings.drawShooting || overwrite)
		{
			MoteMaker.ThrowText(caster.DrawPos, caster.Map, text);
		}
	}

	private void ThrowDebugText(string text, IntVec3 c, bool overwrite = false)
	{
		if (DebugViewSettings.drawShooting || overwrite)
		{
			MoteMaker.ThrowText(c.ToVector3Shifted(), caster.Map, text);
		}
	}
}
