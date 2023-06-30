using System;
using System.Collections.Generic;
using Facepunch;
using ProtoBuf;
using Rust;
using UnityEngine;

// Token: 0x02000196 RID: 406
public class HalloweenDungeon : BasePortal
{
	// Token: 0x0600182C RID: 6188 RVA: 0x000B5645 File Offset: 0x000B3845
	public virtual float GetLifetime()
	{
		return HalloweenDungeon.lifetime;
	}

	// Token: 0x0600182D RID: 6189 RVA: 0x000B564C File Offset: 0x000B384C
	public override void Load(global::BaseNetworkable.LoadInfo info)
	{
		base.Load(info);
		if (info.fromDisk && info.msg.ioEntity != null)
		{
			this.dungeonInstance.uid = info.msg.ioEntity.genericEntRef3;
			this.secondsUsed = info.msg.ioEntity.genericFloat1;
			this.timeAlive = info.msg.ioEntity.genericFloat2;
		}
	}

	// Token: 0x0600182E RID: 6190 RVA: 0x000B56BC File Offset: 0x000B38BC
	public float GetLifeFraction()
	{
		return Mathf.Clamp01(this.secondsUsed / this.GetLifetime());
	}

	// Token: 0x0600182F RID: 6191 RVA: 0x000B56D0 File Offset: 0x000B38D0
	public void Update()
	{
		if (base.isClient)
		{
			return;
		}
		if (this.secondsUsed > 0f)
		{
			this.secondsUsed += Time.deltaTime;
		}
		this.timeAlive += Time.deltaTime;
		float lifeFraction = this.GetLifeFraction();
		if (this.dungeonInstance.IsValid(true))
		{
			ProceduralDynamicDungeon proceduralDynamicDungeon = this.dungeonInstance.Get(true);
			float num = this.radiationCurve.Evaluate(lifeFraction) * 80f;
			proceduralDynamicDungeon.exitRadiation.RadiationAmountOverride = Mathf.Clamp(num, 0f, float.PositiveInfinity);
		}
		if (lifeFraction >= 1f)
		{
			this.KillIfNoPlayers();
			return;
		}
		if (this.timeAlive > 3600f && this.secondsUsed == 0f)
		{
			this.ClearAllEntitiesInRadius(80f);
			base.Kill(global::BaseNetworkable.DestroyMode.None);
		}
	}

	// Token: 0x06001830 RID: 6192 RVA: 0x000B57A0 File Offset: 0x000B39A0
	public void KillIfNoPlayers()
	{
		if (!this.AnyPlayersInside())
		{
			this.ClearAllEntitiesInRadius(80f);
			base.Kill(global::BaseNetworkable.DestroyMode.None);
		}
	}

	// Token: 0x06001831 RID: 6193 RVA: 0x000B57BC File Offset: 0x000B39BC
	public bool AnyPlayersInside()
	{
		ProceduralDynamicDungeon proceduralDynamicDungeon = this.dungeonInstance.Get(true);
		if (proceduralDynamicDungeon == null)
		{
			this.anyplayers_cached = false;
		}
		else if (Time.time > this.nextPlayerCheckTime)
		{
			this.nextPlayerCheckTime = Time.time + 10f;
			this.anyplayers_cached = global::BaseNetworkable.HasCloseConnections(proceduralDynamicDungeon.transform.position, 80f);
		}
		return this.anyplayers_cached;
	}

	// Token: 0x06001832 RID: 6194 RVA: 0x000B5828 File Offset: 0x000B3A28
	private void ClearAllEntitiesInRadius(float radius)
	{
		ProceduralDynamicDungeon proceduralDynamicDungeon = this.dungeonInstance.Get(true);
		if (proceduralDynamicDungeon == null)
		{
			return;
		}
		List<global::BaseEntity> list = Pool.GetList<global::BaseEntity>();
		Vis.Entities<global::BaseEntity>(proceduralDynamicDungeon.transform.position, radius, list, -1, QueryTriggerInteraction.Collide);
		foreach (global::BaseEntity baseEntity in list)
		{
			if (baseEntity.IsValid() && !baseEntity.IsDestroyed)
			{
				global::LootableCorpse lootableCorpse;
				if ((lootableCorpse = baseEntity as global::LootableCorpse) != null)
				{
					lootableCorpse.blockBagDrop = true;
				}
				baseEntity.Kill(global::BaseNetworkable.DestroyMode.None);
			}
		}
		Pool.FreeList<global::BaseEntity>(ref list);
	}

	// Token: 0x06001833 RID: 6195 RVA: 0x000B58D4 File Offset: 0x000B3AD4
	public override void Save(global::BaseNetworkable.SaveInfo info)
	{
		base.Save(info);
		if (info.msg.ioEntity == null)
		{
			info.msg.ioEntity = Pool.Get<ProtoBuf.IOEntity>();
		}
		info.msg.ioEntity.genericEntRef3 = this.dungeonInstance.uid;
		info.msg.ioEntity.genericFloat1 = this.secondsUsed;
		info.msg.ioEntity.genericFloat2 = this.timeAlive;
	}

	// Token: 0x06001834 RID: 6196 RVA: 0x000B594C File Offset: 0x000B3B4C
	public override void PostServerLoad()
	{
		base.PostServerLoad();
		this.timeAlive += UnityEngine.Random.Range(0f, 60f);
	}

	// Token: 0x06001835 RID: 6197 RVA: 0x000B5970 File Offset: 0x000B3B70
	public override void UsePortal(global::BasePlayer player)
	{
		if (this.GetLifeFraction() > 0.8f)
		{
			player.ShowToast(GameTip.Styles.Blue_Normal, this.collapsePhrase, Array.Empty<string>());
			return;
		}
		if (player.isMounted)
		{
			player.ShowToast(GameTip.Styles.Blue_Normal, this.mountPhrase, Array.Empty<string>());
			return;
		}
		if (this.secondsUsed == 0f)
		{
			this.secondsUsed = 1f;
		}
		base.UsePortal(player);
	}

	// Token: 0x06001836 RID: 6198 RVA: 0x000639C2 File Offset: 0x00061BC2
	public override void Spawn()
	{
		base.Spawn();
	}

	// Token: 0x06001837 RID: 6199 RVA: 0x000B59D8 File Offset: 0x000B3BD8
	public override void ServerInit()
	{
		base.ServerInit();
		if (!Rust.Application.isLoadingSave)
		{
			this.timeAlive = UnityEngine.Random.Range(0f, 60f);
			this.SpawnSubEntities();
		}
		this.localEntryExitPos.DropToGround(false, 10f);
		this.localEntryExitPos.transform.position += Vector3.up * 0.05f;
		base.Invoke(new Action(this.CheckBlocked), 0.25f);
	}

	// Token: 0x06001838 RID: 6200 RVA: 0x000B5A60 File Offset: 0x000B3C60
	public void CheckBlocked()
	{
		float num = 0.5f;
		float num2 = 1.8f;
		Vector3 position = this.localEntryExitPos.position;
		Vector3 vector = position + new Vector3(0f, num, 0f);
		Vector3 vector2 = position + new Vector3(0f, num2 - num, 0f);
		if (Physics.CheckCapsule(vector, vector2, num, 1537286401))
		{
			base.Kill(global::BaseNetworkable.DestroyMode.None);
		}
	}

	// Token: 0x06001839 RID: 6201 RVA: 0x000B5ACC File Offset: 0x000B3CCC
	public static Vector3 GetDungeonSpawnPoint()
	{
		float num = Mathf.Floor(TerrainMeta.Size.x / 200f);
		float num2 = 1000f;
		Vector3 zero = Vector3.zero;
		zero.x = -Mathf.Min(TerrainMeta.Size.x * 0.5f, 4000f) + 200f;
		zero.y = 1025f;
		zero.z = -Mathf.Min(TerrainMeta.Size.z * 0.5f, 4000f) + 200f;
		Vector3 zero2 = Vector3.zero;
		int num3 = 0;
		while ((float)num3 < num2)
		{
			int num4 = 0;
			while ((float)num4 < num)
			{
				Vector3 vector = zero + new Vector3((float)num4 * 200f, (float)num3 * 100f, 0f);
				bool flag = false;
				foreach (ProceduralDynamicDungeon proceduralDynamicDungeon in ProceduralDynamicDungeon.dungeons)
				{
					if (proceduralDynamicDungeon != null && proceduralDynamicDungeon.isServer && Vector3.Distance(proceduralDynamicDungeon.transform.position, vector) < 10f)
					{
						flag = true;
						break;
					}
				}
				if (!flag)
				{
					return vector;
				}
				num4++;
			}
			num3++;
		}
		return Vector3.zero;
	}

	// Token: 0x0600183A RID: 6202 RVA: 0x000B5C2C File Offset: 0x000B3E2C
	internal override void DoServerDestroy()
	{
		base.DoServerDestroy();
		if (this.dungeonInstance.IsValid(true))
		{
			this.dungeonInstance.Get(true).Kill(global::BaseNetworkable.DestroyMode.None);
		}
	}

	// Token: 0x0600183B RID: 6203 RVA: 0x00003384 File Offset: 0x00001584
	public void DelayedDestroy()
	{
		base.Kill(global::BaseNetworkable.DestroyMode.None);
	}

	// Token: 0x0600183C RID: 6204 RVA: 0x000B5C54 File Offset: 0x000B3E54
	public void SpawnSubEntities()
	{
		Vector3 dungeonSpawnPoint = HalloweenDungeon.GetDungeonSpawnPoint();
		if (dungeonSpawnPoint == Vector3.zero)
		{
			Debug.LogError("No dungeon spawn point");
			base.Invoke(new Action(this.DelayedDestroy), 5f);
			return;
		}
		global::BaseEntity baseEntity = GameManager.server.CreateEntity(this.dungeonPrefab.resourcePath, dungeonSpawnPoint, Quaternion.identity, true);
		ProceduralDynamicDungeon component = baseEntity.GetComponent<ProceduralDynamicDungeon>();
		component.mapOffset = base.transform.position - dungeonSpawnPoint;
		baseEntity.Spawn();
		this.dungeonInstance.Set(component);
		BasePortal exitPortal = component.GetExitPortal();
		this.targetPortal = exitPortal;
		exitPortal.targetPortal = this;
		base.LinkPortal();
		exitPortal.LinkPortal();
	}

	// Token: 0x04001100 RID: 4352
	public GameObjectRef dungeonPrefab;

	// Token: 0x04001101 RID: 4353
	public EntityRef<ProceduralDynamicDungeon> dungeonInstance;

	// Token: 0x04001102 RID: 4354
	[ServerVar(Help = "Population active on the server", ShowInAdminUI = true)]
	public static float population = 0f;

	// Token: 0x04001103 RID: 4355
	[ServerVar(Help = "How long each active dungeon should last before dying", ShowInAdminUI = true)]
	public static float lifetime = 600f;

	// Token: 0x04001104 RID: 4356
	private float secondsUsed;

	// Token: 0x04001105 RID: 4357
	private float timeAlive;

	// Token: 0x04001106 RID: 4358
	public AnimationCurve radiationCurve;

	// Token: 0x04001107 RID: 4359
	public Translate.Phrase collapsePhrase;

	// Token: 0x04001108 RID: 4360
	public Translate.Phrase mountPhrase;

	// Token: 0x04001109 RID: 4361
	private bool anyplayers_cached;

	// Token: 0x0400110A RID: 4362
	private float nextPlayerCheckTime = float.NegativeInfinity;
}
