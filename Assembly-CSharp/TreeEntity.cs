using System;
using ConVar;
using Facepunch.Rust;
using Network;
using UnityEngine;

// Token: 0x020000E6 RID: 230
public class TreeEntity : ResourceEntity, IPrefabPreProcess
{
	// Token: 0x0600145B RID: 5211 RVA: 0x000A0B94 File Offset: 0x0009ED94
	public override bool OnRpcMessage(BasePlayer player, uint rpc, Message msg)
	{
		using (TimeWarning.New("TreeEntity.OnRpcMessage", 0))
		{
		}
		return base.OnRpcMessage(player, rpc, msg);
	}

	// Token: 0x0600145C RID: 5212 RVA: 0x000A0BD4 File Offset: 0x0009EDD4
	public override void ResetState()
	{
		base.ResetState();
	}

	// Token: 0x0600145D RID: 5213 RVA: 0x00006CA5 File Offset: 0x00004EA5
	public override float BoundsPadding()
	{
		return 1f;
	}

	// Token: 0x0600145E RID: 5214 RVA: 0x000A0BDC File Offset: 0x0009EDDC
	public override void ServerInit()
	{
		base.ServerInit();
		this.lastDirection = (float)((UnityEngine.Random.Range(0, 2) == 0) ? (-1) : 1);
		TreeManager.OnTreeSpawned(this);
	}

	// Token: 0x0600145F RID: 5215 RVA: 0x000A0BFE File Offset: 0x0009EDFE
	internal override void DoServerDestroy()
	{
		base.DoServerDestroy();
		this.CleanupMarker();
		TreeManager.OnTreeDestroyed(this);
	}

	// Token: 0x06001460 RID: 5216 RVA: 0x000A0C14 File Offset: 0x0009EE14
	public bool DidHitMarker(HitInfo info)
	{
		if (this.xMarker == null)
		{
			return false;
		}
		if (PrefabAttribute.server.Find<TreeMarkerData>(this.prefabID) != null)
		{
			Bounds bounds = new Bounds(this.xMarker.transform.position, Vector3.one * 0.2f);
			if (bounds.Contains(info.HitPositionWorld))
			{
				return true;
			}
		}
		else
		{
			Vector3 vector = Vector3Ex.Direction2D(base.transform.position, this.xMarker.transform.position);
			Vector3 attackNormal = info.attackNormal;
			float num = Vector3.Dot(vector, attackNormal);
			float num2 = Vector3.Distance(this.xMarker.transform.position, info.HitPositionWorld);
			if (num >= 0.3f && num2 <= 0.2f)
			{
				return true;
			}
		}
		return false;
	}

	// Token: 0x06001461 RID: 5217 RVA: 0x000A0CDB File Offset: 0x0009EEDB
	public void StartBonusGame()
	{
		if (base.IsInvoking(new Action(this.StopBonusGame)))
		{
			base.CancelInvoke(new Action(this.StopBonusGame));
		}
		base.Invoke(new Action(this.StopBonusGame), 60f);
	}

	// Token: 0x06001462 RID: 5218 RVA: 0x000A0D1A File Offset: 0x0009EF1A
	public void StopBonusGame()
	{
		this.CleanupMarker();
		this.lastHitTime = 0f;
		this.currentBonusLevel = 0;
	}

	// Token: 0x06001463 RID: 5219 RVA: 0x000A0D34 File Offset: 0x0009EF34
	public bool BonusActive()
	{
		return this.xMarker != null;
	}

	// Token: 0x06001464 RID: 5220 RVA: 0x000A0D44 File Offset: 0x0009EF44
	private void DoBirds()
	{
		if (base.isClient)
		{
			return;
		}
		if (UnityEngine.Time.realtimeSinceStartup < this.nextBirdTime)
		{
			return;
		}
		if (this.bounds.extents.y < 6f)
		{
			return;
		}
		uint num = (uint)this.net.ID.Value + this.birdCycleIndex;
		if (SeedRandom.Range(ref num, 0, 2) == 0)
		{
			Effect.server.Run("assets/prefabs/npc/birds/birdemission.prefab", base.transform.position + Vector3.up * UnityEngine.Random.Range(this.bounds.extents.y * 0.65f, this.bounds.extents.y * 0.9f), Vector3.up, null, false);
		}
		this.birdCycleIndex += 1U;
		this.nextBirdTime = UnityEngine.Time.realtimeSinceStartup + 90f;
	}

	// Token: 0x06001465 RID: 5221 RVA: 0x000A0E24 File Offset: 0x0009F024
	public override void OnAttacked(HitInfo info)
	{
		bool canGather = info.CanGather;
		float num = UnityEngine.Time.time - this.lastHitTime;
		this.lastHitTime = UnityEngine.Time.time;
		this.DoBirds();
		if (!this.hasBonusGame || !canGather || info.Initiator == null || (this.BonusActive() && !this.DidHitMarker(info)))
		{
			base.OnAttacked(info);
			return;
		}
		if (this.xMarker != null && !info.DidGather && info.gatherScale > 0f)
		{
			this.xMarker.ClientRPC<int>(null, "MarkerHit", this.currentBonusLevel);
			this.currentBonusLevel++;
			info.gatherScale = 1f + Mathf.Clamp((float)this.currentBonusLevel * 0.125f, 0f, 1f);
		}
		Vector3 vector = ((this.xMarker != null) ? this.xMarker.transform.position : info.HitPositionWorld);
		this.CleanupMarker();
		TreeMarkerData treeMarkerData = PrefabAttribute.server.Find<TreeMarkerData>(this.prefabID);
		if (treeMarkerData != null)
		{
			Vector3 vector3;
			Vector3 vector2 = treeMarkerData.GetNearbyPoint(base.transform.InverseTransformPoint(vector), ref this.lastHitMarkerIndex, out vector3);
			vector2 = base.transform.TransformPoint(vector2);
			Quaternion quaternion = QuaternionEx.LookRotationNormal(base.transform.TransformDirection(vector3), default(Vector3));
			this.xMarker = GameManager.server.CreateEntity("assets/content/nature/treesprefabs/trees/effects/tree_marking_nospherecast.prefab", vector2, quaternion, true);
		}
		else
		{
			Vector3 vector4 = Vector3Ex.Direction2D(base.transform.position, vector);
			Vector3 vector5 = Vector3.Cross(vector4, Vector3.up);
			float num2 = this.lastDirection;
			float num3 = UnityEngine.Random.Range(0.5f, 0.5f);
			Vector3 vector6 = Vector3.Lerp(-vector4, vector5 * num2, num3);
			Vector3 vector7 = base.transform.InverseTransformDirection(vector6.normalized) * 2.5f;
			vector7 = base.transform.InverseTransformPoint(this.GetCollider().ClosestPoint(base.transform.TransformPoint(vector7)));
			Vector3 vector8 = base.transform.TransformPoint(vector7);
			Vector3 vector9 = base.transform.InverseTransformPoint(info.HitPositionWorld);
			vector7.y = vector9.y;
			Vector3 vector10 = base.transform.InverseTransformPoint(info.Initiator.CenterPoint());
			float num4 = Mathf.Max(0.75f, vector10.y);
			float num5 = vector10.y + 0.5f;
			vector7.y = Mathf.Clamp(vector7.y + UnityEngine.Random.Range(0.1f, 0.2f) * ((UnityEngine.Random.Range(0, 2) == 0) ? (-1f) : 1f), num4, num5);
			Vector3 vector11 = Vector3Ex.Direction2D(base.transform.position, vector8);
			Vector3 vector12 = vector11;
			vector11 = base.transform.InverseTransformDirection(vector11);
			Quaternion quaternion2 = QuaternionEx.LookRotationNormal(-vector11, Vector3.zero);
			vector7 = base.transform.TransformPoint(vector7);
			quaternion2 = QuaternionEx.LookRotationNormal(-vector12, Vector3.zero);
			vector7 = this.GetCollider().ClosestPoint(vector7);
			Line line = new Line(this.GetCollider().transform.TransformPoint(new Vector3(0f, 10f, 0f)), this.GetCollider().transform.TransformPoint(new Vector3(0f, -10f, 0f)));
			quaternion2 = QuaternionEx.LookRotationNormal(-Vector3Ex.Direction(line.ClosestPoint(vector7), vector7), default(Vector3));
			this.xMarker = GameManager.server.CreateEntity("assets/content/nature/treesprefabs/trees/effects/tree_marking.prefab", vector7, quaternion2, true);
		}
		this.xMarker.Spawn();
		if (num > 5f)
		{
			this.StartBonusGame();
		}
		base.OnAttacked(info);
		if (this.health > 0f)
		{
			this.lastAttackDamage = info.damageTypes.Total();
			int num6 = Mathf.CeilToInt(this.health / this.lastAttackDamage);
			if (num6 < 2)
			{
				base.ClientRPC<int>(null, "CrackSound", 1);
				return;
			}
			if (num6 < 5)
			{
				base.ClientRPC<int>(null, "CrackSound", 0);
			}
		}
	}

	// Token: 0x06001466 RID: 5222 RVA: 0x000A124C File Offset: 0x0009F44C
	public void CleanupMarker()
	{
		if (this.xMarker)
		{
			this.xMarker.Kill(BaseNetworkable.DestroyMode.None);
		}
		this.xMarker = null;
	}

	// Token: 0x06001467 RID: 5223 RVA: 0x000A1270 File Offset: 0x0009F470
	public Collider GetCollider()
	{
		if (base.isServer)
		{
			if (!(this.serverCollider == null))
			{
				return this.serverCollider;
			}
			return base.GetComponentInChildren<CapsuleCollider>();
		}
		else
		{
			if (!(this.clientCollider == null))
			{
				return this.clientCollider;
			}
			return base.GetComponent<Collider>();
		}
	}

	// Token: 0x06001468 RID: 5224 RVA: 0x000A12BC File Offset: 0x0009F4BC
	public override void OnKilled(HitInfo info)
	{
		if (this.isKilled)
		{
			return;
		}
		this.isKilled = true;
		this.CleanupMarker();
		Analytics.Server.TreeKilled(info.WeaponPrefab);
		if (this.fallOnKilled)
		{
			Collider collider = this.GetCollider();
			if (collider)
			{
				collider.enabled = false;
			}
			Vector3 vector = info.attackNormal;
			if (vector == Vector3.zero)
			{
				vector = Vector3Ex.Direction2D(base.transform.position, info.PointStart);
			}
			base.ClientRPC<Vector3>(null, "TreeFall", vector);
			base.Invoke(new Action(this.DelayedKill), this.fallDuration + 1f);
			return;
		}
		this.DelayedKill();
	}

	// Token: 0x06001469 RID: 5225 RVA: 0x00003384 File Offset: 0x00001584
	public void DelayedKill()
	{
		base.Kill(BaseNetworkable.DestroyMode.None);
	}

	// Token: 0x0600146A RID: 5226 RVA: 0x000A1365 File Offset: 0x0009F565
	public override void PreProcess(IPrefabProcessor preProcess, GameObject rootObj, string name, bool serverside, bool clientside, bool bundling)
	{
		base.PreProcess(preProcess, rootObj, name, serverside, clientside, bundling);
		if (serverside)
		{
			this.globalBroadcast = ConVar.Tree.global_broadcast;
		}
	}

	// Token: 0x04000CC3 RID: 3267
	[Header("Falling")]
	public bool fallOnKilled = true;

	// Token: 0x04000CC4 RID: 3268
	public float fallDuration = 1.5f;

	// Token: 0x04000CC5 RID: 3269
	public GameObjectRef fallStartSound;

	// Token: 0x04000CC6 RID: 3270
	public GameObjectRef fallImpactSound;

	// Token: 0x04000CC7 RID: 3271
	public GameObjectRef fallImpactParticles;

	// Token: 0x04000CC8 RID: 3272
	public SoundDefinition fallLeavesLoopDef;

	// Token: 0x04000CC9 RID: 3273
	[NonSerialized]
	public bool[] usedHeights = new bool[20];

	// Token: 0x04000CCA RID: 3274
	public bool impactSoundPlayed;

	// Token: 0x04000CCB RID: 3275
	private float treeDistanceUponFalling;

	// Token: 0x04000CCC RID: 3276
	public GameObjectRef prefab;

	// Token: 0x04000CCD RID: 3277
	public bool hasBonusGame = true;

	// Token: 0x04000CCE RID: 3278
	public GameObjectRef bonusHitEffect;

	// Token: 0x04000CCF RID: 3279
	public GameObjectRef bonusHitSound;

	// Token: 0x04000CD0 RID: 3280
	public Collider serverCollider;

	// Token: 0x04000CD1 RID: 3281
	public Collider clientCollider;

	// Token: 0x04000CD2 RID: 3282
	public SoundDefinition smallCrackSoundDef;

	// Token: 0x04000CD3 RID: 3283
	public SoundDefinition medCrackSoundDef;

	// Token: 0x04000CD4 RID: 3284
	private float lastAttackDamage;

	// Token: 0x04000CD5 RID: 3285
	[NonSerialized]
	protected BaseEntity xMarker;

	// Token: 0x04000CD6 RID: 3286
	private int currentBonusLevel;

	// Token: 0x04000CD7 RID: 3287
	private float lastDirection = -1f;

	// Token: 0x04000CD8 RID: 3288
	private float lastHitTime;

	// Token: 0x04000CD9 RID: 3289
	private int lastHitMarkerIndex = -1;

	// Token: 0x04000CDA RID: 3290
	private float nextBirdTime;

	// Token: 0x04000CDB RID: 3291
	private uint birdCycleIndex;
}
