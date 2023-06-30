using System;
using System.Collections.Generic;
using ConVar;
using Facepunch;
using Rust;
using UnityEngine;

// Token: 0x020004E3 RID: 1251
public class PuzzleReset : FacepunchBehaviour
{
	// Token: 0x0600289D RID: 10397 RVA: 0x000FB07A File Offset: 0x000F927A
	public float GetResetSpacing()
	{
		return this.timeBetweenResets * (this.scaleWithServerPopulation ? (1f - SpawnHandler.PlayerLerp(Spawn.min_rate, Spawn.max_rate)) : 1f);
	}

	// Token: 0x0600289E RID: 10398 RVA: 0x000FB0A7 File Offset: 0x000F92A7
	public void Start()
	{
		if (this.timeBetweenResets != float.PositiveInfinity)
		{
			this.ResetTimer();
		}
	}

	// Token: 0x0600289F RID: 10399 RVA: 0x000FB0BC File Offset: 0x000F92BC
	public void ResetTimer()
	{
		this.resetTimeElapsed = 0f;
		base.CancelInvoke(new Action(this.ResetTick));
		base.InvokeRandomized(new Action(this.ResetTick), UnityEngine.Random.Range(0f, 1f), this.resetTickTime, 0.5f);
	}

	// Token: 0x060028A0 RID: 10400 RVA: 0x000FB112 File Offset: 0x000F9312
	public bool PassesResetCheck()
	{
		if (!this.playersBlockReset)
		{
			return true;
		}
		if (this.CheckSleepingAIZForPlayers)
		{
			return this.AIZSleeping();
		}
		return !this.PlayersWithinDistance();
	}

	// Token: 0x060028A1 RID: 10401 RVA: 0x000FB138 File Offset: 0x000F9338
	private bool AIZSleeping()
	{
		if (this.zone != null)
		{
			if (!this.zone.PointInside(base.transform.position))
			{
				this.zone = AIInformationZone.GetForPoint(base.transform.position, true);
			}
		}
		else
		{
			this.zone = AIInformationZone.GetForPoint(base.transform.position, true);
		}
		return !(this.zone == null) && this.zone.Sleeping;
	}

	// Token: 0x060028A2 RID: 10402 RVA: 0x000FB1B6 File Offset: 0x000F93B6
	private bool PlayersWithinDistance()
	{
		return PuzzleReset.AnyPlayersWithinDistance(this.playerDetectionOrigin, this.playerDetectionRadius);
	}

	// Token: 0x060028A3 RID: 10403 RVA: 0x000FB1CC File Offset: 0x000F93CC
	public static bool AnyPlayersWithinDistance(Transform origin, float radius)
	{
		foreach (BasePlayer basePlayer in BasePlayer.activePlayerList)
		{
			if (!basePlayer.IsSleeping() && basePlayer.IsAlive() && Vector3.Distance(basePlayer.transform.position, origin.position) < radius)
			{
				return true;
			}
		}
		return false;
	}

	// Token: 0x060028A4 RID: 10404 RVA: 0x000FB248 File Offset: 0x000F9448
	public void ResetTick()
	{
		if (this.PassesResetCheck())
		{
			this.resetTimeElapsed += this.resetTickTime;
		}
		if (this.resetTimeElapsed > this.GetResetSpacing())
		{
			this.resetTimeElapsed = 0f;
			this.DoReset();
		}
	}

	// Token: 0x060028A5 RID: 10405 RVA: 0x000FB284 File Offset: 0x000F9484
	public void CleanupSleepers()
	{
		if (this.playerDetectionOrigin == null || BasePlayer.sleepingPlayerList == null)
		{
			return;
		}
		for (int i = BasePlayer.sleepingPlayerList.Count - 1; i >= 0; i--)
		{
			BasePlayer basePlayer = BasePlayer.sleepingPlayerList[i];
			if (!(basePlayer == null) && basePlayer.IsSleeping() && Vector3.Distance(basePlayer.transform.position, this.playerDetectionOrigin.position) <= this.playerDetectionRadius)
			{
				basePlayer.Hurt(1000f, DamageType.Suicide, basePlayer, false);
			}
		}
	}

	// Token: 0x060028A6 RID: 10406 RVA: 0x000FB30C File Offset: 0x000F950C
	public void DoReset()
	{
		this.CleanupSleepers();
		IOEntity component = base.GetComponent<IOEntity>();
		if (component != null)
		{
			PuzzleReset.ResetIOEntRecursive(component, UnityEngine.Time.frameCount);
			component.MarkDirty();
		}
		else if (this.resetPositions != null)
		{
			foreach (Vector3 vector in this.resetPositions)
			{
				Vector3 vector2 = base.transform.TransformPoint(vector);
				List<IOEntity> list = Facepunch.Pool.GetList<IOEntity>();
				global::Vis.Entities<IOEntity>(vector2, 0.5f, list, 1235288065, QueryTriggerInteraction.Ignore);
				foreach (IOEntity ioentity in list)
				{
					if (ioentity.IsRootEntity() && ioentity.isServer)
					{
						PuzzleReset.ResetIOEntRecursive(ioentity, UnityEngine.Time.frameCount);
						ioentity.MarkDirty();
					}
				}
				Facepunch.Pool.FreeList<IOEntity>(ref list);
			}
		}
		List<SpawnGroup> list2 = Facepunch.Pool.GetList<SpawnGroup>();
		global::Vis.Components<SpawnGroup>(base.transform.position, 1f, list2, 262144, QueryTriggerInteraction.Collide);
		foreach (SpawnGroup spawnGroup in list2)
		{
			if (!(spawnGroup == null))
			{
				spawnGroup.Clear();
				spawnGroup.DelayedSpawn();
			}
		}
		Facepunch.Pool.FreeList<SpawnGroup>(ref list2);
		foreach (GameObject gameObject in this.resetObjects)
		{
			if (gameObject != null)
			{
				gameObject.SendMessage("OnPuzzleReset", SendMessageOptions.DontRequireReceiver);
			}
		}
		if (this.broadcastResetMessage)
		{
			foreach (BasePlayer basePlayer in BasePlayer.activePlayerList)
			{
				if (!basePlayer.IsNpc && basePlayer.IsConnected)
				{
					basePlayer.ShowToast(GameTip.Styles.Server_Event, this.resetPhrase, Array.Empty<string>());
				}
			}
		}
	}

	// Token: 0x060028A7 RID: 10407 RVA: 0x000FB51C File Offset: 0x000F971C
	public static void ResetIOEntRecursive(IOEntity target, int resetIndex)
	{
		if (target.lastResetIndex == resetIndex)
		{
			return;
		}
		target.lastResetIndex = resetIndex;
		target.ResetIOState();
		foreach (IOEntity.IOSlot ioslot in target.outputs)
		{
			if (ioslot.connectedTo.Get(true) != null && ioslot.connectedTo.Get(true) != target)
			{
				PuzzleReset.ResetIOEntRecursive(ioslot.connectedTo.Get(true), resetIndex);
			}
		}
	}

	// Token: 0x040020E8 RID: 8424
	public SpawnGroup[] respawnGroups;

	// Token: 0x040020E9 RID: 8425
	public IOEntity[] resetEnts;

	// Token: 0x040020EA RID: 8426
	public GameObject[] resetObjects;

	// Token: 0x040020EB RID: 8427
	public bool playersBlockReset;

	// Token: 0x040020EC RID: 8428
	public bool CheckSleepingAIZForPlayers;

	// Token: 0x040020ED RID: 8429
	public float playerDetectionRadius;

	// Token: 0x040020EE RID: 8430
	public float playerHeightDetectionMinMax = -1f;

	// Token: 0x040020EF RID: 8431
	public Transform playerDetectionOrigin;

	// Token: 0x040020F0 RID: 8432
	public float timeBetweenResets = 30f;

	// Token: 0x040020F1 RID: 8433
	public bool scaleWithServerPopulation;

	// Token: 0x040020F2 RID: 8434
	[HideInInspector]
	public Vector3[] resetPositions;

	// Token: 0x040020F3 RID: 8435
	public bool broadcastResetMessage;

	// Token: 0x040020F4 RID: 8436
	public Translate.Phrase resetPhrase;

	// Token: 0x040020F5 RID: 8437
	private AIInformationZone zone;

	// Token: 0x040020F6 RID: 8438
	private float resetTimeElapsed;

	// Token: 0x040020F7 RID: 8439
	private float resetTickTime = 10f;
}
