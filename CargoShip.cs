using System;
using System.Collections.Generic;
using Facepunch;
using Network;
using ProtoBuf;
using Rust;
using UnityEngine;

// Token: 0x02000024 RID: 36
public class CargoShip : global::BaseEntity
{
	// Token: 0x060000CC RID: 204 RVA: 0x0000627E File Offset: 0x0000447E
	public override float GetNetworkTime()
	{
		return Time.fixedTime;
	}

	// Token: 0x060000CD RID: 205 RVA: 0x00006285 File Offset: 0x00004485
	public override void Load(global::BaseNetworkable.LoadInfo info)
	{
		base.Load(info);
		if (info.msg.simpleUint != null)
		{
			this.layoutChoice = info.msg.simpleUint.value;
		}
	}

	// Token: 0x060000CE RID: 206 RVA: 0x000062B4 File Offset: 0x000044B4
	public void RefreshActiveLayout()
	{
		for (int i = 0; i < this.layouts.Length; i++)
		{
			this.layouts[i].SetActive((ulong)this.layoutChoice == (ulong)((long)i));
		}
	}

	// Token: 0x060000CF RID: 207 RVA: 0x000062EC File Offset: 0x000044EC
	public void TriggeredEventSpawn()
	{
		Vector3 vector = TerrainMeta.RandomPointOffshore();
		vector.y = TerrainMeta.WaterMap.GetHeight(vector);
		base.transform.position = vector;
		if (!CargoShip.event_enabled || CargoShip.event_duration_minutes == 0f)
		{
			base.Invoke(new Action(this.DelayedDestroy), 1f);
		}
	}

	// Token: 0x060000D0 RID: 208 RVA: 0x00006348 File Offset: 0x00004548
	public void CreateMapMarker()
	{
		if (this.mapMarkerInstance)
		{
			this.mapMarkerInstance.Kill(global::BaseNetworkable.DestroyMode.None);
		}
		global::BaseEntity baseEntity = GameManager.server.CreateEntity(this.mapMarkerEntityPrefab.resourcePath, Vector3.zero, Quaternion.identity, true);
		baseEntity.Spawn();
		baseEntity.SetParent(this, false, false);
		this.mapMarkerInstance = baseEntity;
	}

	// Token: 0x060000D1 RID: 209 RVA: 0x000063A5 File Offset: 0x000045A5
	public void DisableCollisionTest()
	{
	}

	// Token: 0x060000D2 RID: 210 RVA: 0x000063A8 File Offset: 0x000045A8
	public void SpawnCrate(string resourcePath)
	{
		int num = UnityEngine.Random.Range(0, this.crateSpawns.Count);
		Vector3 position = this.crateSpawns[num].position;
		Quaternion rotation = this.crateSpawns[num].rotation;
		this.crateSpawns.Remove(this.crateSpawns[num]);
		global::BaseEntity baseEntity = GameManager.server.CreateEntity(resourcePath, position, rotation, true);
		if (baseEntity)
		{
			baseEntity.enableSaving = false;
			baseEntity.SendMessage("SetWasDropped", SendMessageOptions.DontRequireReceiver);
			baseEntity.Spawn();
			baseEntity.SetParent(this, true, false);
			Rigidbody component = baseEntity.GetComponent<Rigidbody>();
			if (component != null)
			{
				component.isKinematic = true;
			}
		}
	}

	// Token: 0x060000D3 RID: 211 RVA: 0x00006458 File Offset: 0x00004658
	public void RespawnLoot()
	{
		base.InvokeRepeating(new Action(this.PlayHorn), 0f, 8f);
		this.SpawnCrate(this.lockedCratePrefab.resourcePath);
		this.SpawnCrate(this.eliteCratePrefab.resourcePath);
		for (int i = 0; i < 4; i++)
		{
			this.SpawnCrate(this.militaryCratePrefab.resourcePath);
		}
		for (int j = 0; j < 4; j++)
		{
			this.SpawnCrate(this.junkCratePrefab.resourcePath);
		}
		this.lootRoundsPassed++;
		if (this.lootRoundsPassed >= CargoShip.loot_rounds)
		{
			base.CancelInvoke(new Action(this.RespawnLoot));
		}
	}

	// Token: 0x060000D4 RID: 212 RVA: 0x0000650C File Offset: 0x0000470C
	public void SpawnSubEntities()
	{
		if (!Rust.Application.isLoadingSave)
		{
			global::BaseEntity baseEntity = GameManager.server.CreateEntity(this.escapeBoatPrefab.resourcePath, this.escapeBoatPoint.position, this.escapeBoatPoint.rotation, true);
			if (baseEntity)
			{
				baseEntity.SetParent(this, true, false);
				baseEntity.Spawn();
				RHIB component = baseEntity.GetComponent<RHIB>();
				component.SetToKinematic();
				if (component)
				{
					component.AddFuel(50);
				}
			}
		}
		global::MicrophoneStand microphoneStand = GameManager.server.CreateEntity(this.microphonePrefab.resourcePath, this.microphonePoint.position, this.microphonePoint.rotation, true) as global::MicrophoneStand;
		if (microphoneStand)
		{
			microphoneStand.enableSaving = false;
			microphoneStand.SetParent(this, true, false);
			microphoneStand.Spawn();
			microphoneStand.SpawnChildEntity();
			global::IOEntity ioentity = microphoneStand.ioEntity.Get(true);
			foreach (Transform transform in this.speakerPoints)
			{
				global::IOEntity ioentity2 = GameManager.server.CreateEntity(this.speakerPrefab.resourcePath, transform.position, transform.rotation, true) as global::IOEntity;
				ioentity2.enableSaving = false;
				ioentity2.SetParent(this, true, false);
				ioentity2.Spawn();
				ioentity.outputs[0].connectedTo.Set(ioentity2);
				ioentity2.inputs[0].connectedTo.Set(ioentity);
				ioentity = ioentity2;
			}
			microphoneStand.ioEntity.Get(true).MarkDirtyForceUpdateOutputs();
		}
	}

	// Token: 0x060000D5 RID: 213 RVA: 0x0000668C File Offset: 0x0000488C
	protected override void OnChildAdded(global::BaseEntity child)
	{
		base.OnChildAdded(child);
		RHIB rhib;
		if (base.isServer && Rust.Application.isLoadingSave && (rhib = child as RHIB) != null)
		{
			Vector3 localPosition = rhib.transform.localPosition;
			Vector3 vector = base.transform.InverseTransformPoint(this.escapeBoatPoint.transform.position);
			if (Vector3.Distance(localPosition, vector) < 1f)
			{
				rhib.SetToKinematic();
			}
		}
	}

	// Token: 0x060000D6 RID: 214 RVA: 0x000066F3 File Offset: 0x000048F3
	public override void Save(global::BaseNetworkable.SaveInfo info)
	{
		base.Save(info);
		info.msg.simpleUint = Pool.Get<SimpleUInt>();
		info.msg.simpleUint.value = this.layoutChoice;
	}

	// Token: 0x060000D7 RID: 215 RVA: 0x00006722 File Offset: 0x00004922
	public override void PostServerLoad()
	{
		base.PostServerLoad();
		this.RefreshActiveLayout();
	}

	// Token: 0x060000D8 RID: 216 RVA: 0x00006730 File Offset: 0x00004930
	public void PlayHorn()
	{
		base.ClientRPC(null, "DoHornSound");
		this.hornCount++;
		if (this.hornCount >= 3)
		{
			this.hornCount = 0;
			base.CancelInvoke(new Action(this.PlayHorn));
		}
	}

	// Token: 0x060000D9 RID: 217 RVA: 0x0000676E File Offset: 0x0000496E
	public override void Spawn()
	{
		if (!Rust.Application.isLoadingSave)
		{
			this.layoutChoice = (uint)UnityEngine.Random.Range(0, this.layouts.Length);
			base.SendNetworkUpdate(global::BasePlayer.NetworkQueue.Update);
			this.RefreshActiveLayout();
		}
		base.Spawn();
	}

	// Token: 0x060000DA RID: 218 RVA: 0x000067A0 File Offset: 0x000049A0
	public override void ServerInit()
	{
		base.ServerInit();
		base.Invoke(new Action(this.FindInitialNode), 2f);
		base.InvokeRepeating(new Action(this.BuildingCheck), 1f, 5f);
		base.InvokeRepeating(new Action(this.RespawnLoot), 10f, 60f * CargoShip.loot_round_spacing_minutes);
		base.Invoke(new Action(this.DisableCollisionTest), 10f);
		float height = TerrainMeta.WaterMap.GetHeight(base.transform.position);
		Vector3 vector = base.transform.InverseTransformPoint(this.waterLine.transform.position);
		base.transform.position = new Vector3(base.transform.position.x, height - vector.y, base.transform.position.z);
		this.SpawnSubEntities();
		base.Invoke(new Action(this.StartEgress), 60f * CargoShip.event_duration_minutes);
		this.CreateMapMarker();
	}

	// Token: 0x060000DB RID: 219 RVA: 0x000068B4 File Offset: 0x00004AB4
	public void UpdateRadiation()
	{
		this.currentRadiation += 1f;
		TriggerRadiation[] componentsInChildren = this.radiation.GetComponentsInChildren<TriggerRadiation>();
		for (int i = 0; i < componentsInChildren.Length; i++)
		{
			componentsInChildren[i].RadiationAmountOverride = this.currentRadiation;
		}
	}

	// Token: 0x060000DC RID: 220 RVA: 0x000068FC File Offset: 0x00004AFC
	public void StartEgress()
	{
		if (this.egressing)
		{
			return;
		}
		this.egressing = true;
		base.CancelInvoke(new Action(this.PlayHorn));
		this.radiation.SetActive(true);
		base.SetFlag(global::BaseEntity.Flags.Reserved8, true, false, true);
		base.InvokeRepeating(new Action(this.UpdateRadiation), 10f, 1f);
		base.Invoke(new Action(this.DelayedDestroy), 60f * CargoShip.egress_duration_minutes);
	}

	// Token: 0x060000DD RID: 221 RVA: 0x00003384 File Offset: 0x00001584
	public void DelayedDestroy()
	{
		base.Kill(global::BaseNetworkable.DestroyMode.None);
	}

	// Token: 0x060000DE RID: 222 RVA: 0x0000697E File Offset: 0x00004B7E
	public void FindInitialNode()
	{
		this.targetNodeIndex = this.GetClosestNodeToUs();
	}

	// Token: 0x060000DF RID: 223 RVA: 0x0000698C File Offset: 0x00004B8C
	public void BuildingCheck()
	{
		List<global::DecayEntity> list = Pool.GetList<global::DecayEntity>();
		Vis.Entities<global::DecayEntity>(this.WorldSpaceBounds(), list, 2097152, QueryTriggerInteraction.Collide);
		foreach (global::DecayEntity decayEntity in list)
		{
			if (decayEntity.isServer && decayEntity.IsAlive())
			{
				decayEntity.Kill(global::BaseNetworkable.DestroyMode.Gib);
			}
		}
		Pool.FreeList<global::DecayEntity>(ref list);
	}

	// Token: 0x060000E0 RID: 224 RVA: 0x00006A0C File Offset: 0x00004C0C
	public void FixedUpdate()
	{
		if (base.isClient)
		{
			return;
		}
		this.UpdateMovement();
	}

	// Token: 0x060000E1 RID: 225 RVA: 0x00006A20 File Offset: 0x00004C20
	public void UpdateMovement()
	{
		if (TerrainMeta.Path.OceanPatrolFar == null || TerrainMeta.Path.OceanPatrolFar.Count == 0)
		{
			return;
		}
		if (this.targetNodeIndex == -1)
		{
			return;
		}
		Vector3 vector = TerrainMeta.Path.OceanPatrolFar[this.targetNodeIndex];
		if (this.egressing)
		{
			vector = base.transform.position + (base.transform.position - Vector3.zero).normalized * 10000f;
		}
		Vector3 normalized = (vector - base.transform.position).normalized;
		float num = Vector3.Dot(base.transform.forward, normalized);
		float num2 = Mathf.InverseLerp(0f, 1f, num);
		float num3 = Vector3.Dot(base.transform.right, normalized);
		float num4 = 2.5f;
		float num5 = Mathf.InverseLerp(0.05f, 0.5f, Mathf.Abs(num3));
		this.turnScale = Mathf.Lerp(this.turnScale, num5, Time.deltaTime * 0.2f);
		float num6 = (float)((num3 < 0f) ? (-1) : 1);
		this.currentTurnSpeed = num4 * this.turnScale * num6;
		base.transform.Rotate(Vector3.up, Time.deltaTime * this.currentTurnSpeed, Space.World);
		this.currentThrottle = Mathf.Lerp(this.currentThrottle, num2, Time.deltaTime * 0.2f);
		this.currentVelocity = base.transform.forward * (8f * this.currentThrottle);
		base.transform.position += this.currentVelocity * Time.deltaTime;
		if (Vector3.Distance(base.transform.position, vector) < 80f)
		{
			this.targetNodeIndex++;
			if (this.targetNodeIndex >= TerrainMeta.Path.OceanPatrolFar.Count)
			{
				this.targetNodeIndex = 0;
			}
		}
	}

	// Token: 0x060000E2 RID: 226 RVA: 0x00006C28 File Offset: 0x00004E28
	public int GetClosestNodeToUs()
	{
		int num = 0;
		float num2 = float.PositiveInfinity;
		for (int i = 0; i < TerrainMeta.Path.OceanPatrolFar.Count; i++)
		{
			Vector3 vector = TerrainMeta.Path.OceanPatrolFar[i];
			float num3 = Vector3.Distance(base.transform.position, vector);
			if (num3 < num2)
			{
				num = i;
				num2 = num3;
			}
		}
		return num;
	}

	// Token: 0x060000E3 RID: 227 RVA: 0x00006C86 File Offset: 0x00004E86
	public override Vector3 GetLocalVelocityServer()
	{
		return this.currentVelocity;
	}

	// Token: 0x060000E4 RID: 228 RVA: 0x00006C8E File Offset: 0x00004E8E
	public override Quaternion GetAngularVelocityServer()
	{
		return Quaternion.Euler(0f, this.currentTurnSpeed, 0f);
	}

	// Token: 0x060000E5 RID: 229 RVA: 0x00006CA5 File Offset: 0x00004EA5
	public override float InheritedVelocityScale()
	{
		return 1f;
	}

	// Token: 0x060000E6 RID: 230 RVA: 0x0000441C File Offset: 0x0000261C
	public override bool BlocksWaterFor(global::BasePlayer player)
	{
		return true;
	}

	// Token: 0x060000E7 RID: 231 RVA: 0x0000441C File Offset: 0x0000261C
	public override bool SupportsChildDeployables()
	{
		return true;
	}

	// Token: 0x060000E8 RID: 232 RVA: 0x0000441C File Offset: 0x0000261C
	public override bool ForceDeployableSetParent()
	{
		return true;
	}

	// Token: 0x060000E9 RID: 233 RVA: 0x00006CAC File Offset: 0x00004EAC
	public override bool OnRpcMessage(global::BasePlayer player, uint rpc, Message msg)
	{
		using (TimeWarning.New("CargoShip.OnRpcMessage", 0))
		{
		}
		return base.OnRpcMessage(player, rpc, msg);
	}

	// Token: 0x040000C7 RID: 199
	public int targetNodeIndex = -1;

	// Token: 0x040000C8 RID: 200
	public GameObject wakeParent;

	// Token: 0x040000C9 RID: 201
	public GameObjectRef scientistTurretPrefab;

	// Token: 0x040000CA RID: 202
	public Transform[] scientistSpawnPoints;

	// Token: 0x040000CB RID: 203
	public List<Transform> crateSpawns;

	// Token: 0x040000CC RID: 204
	public GameObjectRef lockedCratePrefab;

	// Token: 0x040000CD RID: 205
	public GameObjectRef militaryCratePrefab;

	// Token: 0x040000CE RID: 206
	public GameObjectRef eliteCratePrefab;

	// Token: 0x040000CF RID: 207
	public GameObjectRef junkCratePrefab;

	// Token: 0x040000D0 RID: 208
	public Transform waterLine;

	// Token: 0x040000D1 RID: 209
	public Transform rudder;

	// Token: 0x040000D2 RID: 210
	public Transform propeller;

	// Token: 0x040000D3 RID: 211
	public GameObjectRef escapeBoatPrefab;

	// Token: 0x040000D4 RID: 212
	public Transform escapeBoatPoint;

	// Token: 0x040000D5 RID: 213
	public GameObjectRef microphonePrefab;

	// Token: 0x040000D6 RID: 214
	public Transform microphonePoint;

	// Token: 0x040000D7 RID: 215
	public GameObjectRef speakerPrefab;

	// Token: 0x040000D8 RID: 216
	public Transform[] speakerPoints;

	// Token: 0x040000D9 RID: 217
	public GameObject radiation;

	// Token: 0x040000DA RID: 218
	public GameObjectRef mapMarkerEntityPrefab;

	// Token: 0x040000DB RID: 219
	public GameObject hornOrigin;

	// Token: 0x040000DC RID: 220
	public SoundDefinition hornDef;

	// Token: 0x040000DD RID: 221
	public CargoShipSounds cargoShipSounds;

	// Token: 0x040000DE RID: 222
	public GameObject[] layouts;

	// Token: 0x040000DF RID: 223
	public GameObjectRef playerTest;

	// Token: 0x040000E0 RID: 224
	private uint layoutChoice;

	// Token: 0x040000E1 RID: 225
	[ServerVar]
	public static bool event_enabled = true;

	// Token: 0x040000E2 RID: 226
	[ServerVar]
	public static float event_duration_minutes = 50f;

	// Token: 0x040000E3 RID: 227
	[ServerVar]
	public static float egress_duration_minutes = 10f;

	// Token: 0x040000E4 RID: 228
	[ServerVar]
	public static int loot_rounds = 3;

	// Token: 0x040000E5 RID: 229
	[ServerVar]
	public static float loot_round_spacing_minutes = 10f;

	// Token: 0x040000E6 RID: 230
	private global::BaseEntity mapMarkerInstance;

	// Token: 0x040000E7 RID: 231
	private Vector3 currentVelocity = Vector3.zero;

	// Token: 0x040000E8 RID: 232
	private float currentThrottle;

	// Token: 0x040000E9 RID: 233
	private float currentTurnSpeed;

	// Token: 0x040000EA RID: 234
	private float turnScale;

	// Token: 0x040000EB RID: 235
	private int lootRoundsPassed;

	// Token: 0x040000EC RID: 236
	private int hornCount;

	// Token: 0x040000ED RID: 237
	private float currentRadiation;

	// Token: 0x040000EE RID: 238
	private bool egressing;
}
