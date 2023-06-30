using System;
using Facepunch;
using ProtoBuf;
using UnityEngine;

// Token: 0x020004E8 RID: 1256
public class CargoPlane : global::BaseEntity
{
	// Token: 0x060028BC RID: 10428 RVA: 0x000FB96B File Offset: 0x000F9B6B
	public override void ServerInit()
	{
		base.ServerInit();
		this.Initialize();
	}

	// Token: 0x060028BD RID: 10429 RVA: 0x000FB979 File Offset: 0x000F9B79
	public override void PostServerLoad()
	{
		base.PostServerLoad();
		if (this.dropPosition == Vector3.zero)
		{
			this.Initialize();
		}
	}

	// Token: 0x060028BE RID: 10430 RVA: 0x000FB999 File Offset: 0x000F9B99
	private void Initialize()
	{
		if (this.dropPosition == Vector3.zero)
		{
			this.dropPosition = this.RandomDropPosition();
		}
		this.UpdateDropPosition(this.dropPosition);
	}

	// Token: 0x060028BF RID: 10431 RVA: 0x000FB9C5 File Offset: 0x000F9BC5
	public void InitDropPosition(Vector3 newDropPosition)
	{
		this.dropPosition = newDropPosition;
		this.dropPosition.y = 0f;
	}

	// Token: 0x060028C0 RID: 10432 RVA: 0x000FB9E0 File Offset: 0x000F9BE0
	public Vector3 RandomDropPosition()
	{
		Vector3 vector = Vector3.zero;
		float num = 100f;
		float x = TerrainMeta.Size.x;
		do
		{
			vector = Vector3Ex.Range(-(x / 3f), x / 3f);
		}
		while (this.filter.GetFactor(vector, true) == 0f && (num -= 1f) > 0f);
		vector.y = 0f;
		return vector;
	}

	// Token: 0x060028C1 RID: 10433 RVA: 0x000FBA4C File Offset: 0x000F9C4C
	public void UpdateDropPosition(Vector3 newDropPosition)
	{
		float x = TerrainMeta.Size.x;
		float num = TerrainMeta.HighestPoint.y + 250f;
		this.startPos = Vector3Ex.Range(-1f, 1f);
		this.startPos.y = 0f;
		this.startPos.Normalize();
		this.startPos *= x * 2f;
		this.startPos.y = num;
		this.endPos = this.startPos * -1f;
		this.endPos.y = this.startPos.y;
		this.startPos += newDropPosition;
		this.endPos += newDropPosition;
		this.secondsToTake = Vector3.Distance(this.startPos, this.endPos) / 50f;
		this.secondsToTake *= UnityEngine.Random.Range(0.95f, 1.05f);
		base.transform.position = this.startPos;
		base.transform.rotation = Quaternion.LookRotation(this.endPos - this.startPos);
		this.dropPosition = newDropPosition;
	}

	// Token: 0x060028C2 RID: 10434 RVA: 0x000FBB8C File Offset: 0x000F9D8C
	private void Update()
	{
		if (!base.isServer)
		{
			return;
		}
		this.secondsTaken += Time.deltaTime;
		float num = Mathf.InverseLerp(0f, this.secondsToTake, this.secondsTaken);
		if (!this.dropped && num >= 0.5f)
		{
			this.dropped = true;
			global::BaseEntity baseEntity = GameManager.server.CreateEntity(this.prefabDrop.resourcePath, base.transform.position, default(Quaternion), true);
			if (baseEntity)
			{
				baseEntity.globalBroadcast = true;
				baseEntity.Spawn();
			}
		}
		base.transform.position = Vector3.Lerp(this.startPos, this.endPos, num);
		base.transform.hasChanged = true;
		if (num >= 1f)
		{
			base.Kill(global::BaseNetworkable.DestroyMode.None);
		}
	}

	// Token: 0x060028C3 RID: 10435 RVA: 0x000FBC5C File Offset: 0x000F9E5C
	public override void Save(global::BaseNetworkable.SaveInfo info)
	{
		base.Save(info);
		if (base.isServer && info.forDisk)
		{
			info.msg.cargoPlane = Pool.Get<ProtoBuf.CargoPlane>();
			info.msg.cargoPlane.startPos = this.startPos;
			info.msg.cargoPlane.endPos = this.endPos;
			info.msg.cargoPlane.secondsToTake = this.secondsToTake;
			info.msg.cargoPlane.secondsTaken = this.secondsTaken;
			info.msg.cargoPlane.dropped = this.dropped;
			info.msg.cargoPlane.dropPosition = this.dropPosition;
		}
	}

	// Token: 0x060028C4 RID: 10436 RVA: 0x000FBD1C File Offset: 0x000F9F1C
	public override void Load(global::BaseNetworkable.LoadInfo info)
	{
		base.Load(info);
		if (base.isServer && info.fromDisk && info.msg.cargoPlane != null)
		{
			this.startPos = info.msg.cargoPlane.startPos;
			this.endPos = info.msg.cargoPlane.endPos;
			this.secondsToTake = info.msg.cargoPlane.secondsToTake;
			this.secondsTaken = info.msg.cargoPlane.secondsTaken;
			this.dropped = info.msg.cargoPlane.dropped;
			this.dropPosition = info.msg.cargoPlane.dropPosition;
		}
	}

	// Token: 0x04002106 RID: 8454
	public GameObjectRef prefabDrop;

	// Token: 0x04002107 RID: 8455
	public SpawnFilter filter;

	// Token: 0x04002108 RID: 8456
	private Vector3 startPos;

	// Token: 0x04002109 RID: 8457
	private Vector3 endPos;

	// Token: 0x0400210A RID: 8458
	private float secondsToTake;

	// Token: 0x0400210B RID: 8459
	private float secondsTaken;

	// Token: 0x0400210C RID: 8460
	private bool dropped;

	// Token: 0x0400210D RID: 8461
	private Vector3 dropPosition = Vector3.zero;
}
