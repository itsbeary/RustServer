using System;
using UnityEngine;

// Token: 0x02000417 RID: 1047
public class F15 : BaseCombatEntity
{
	// Token: 0x170002F4 RID: 756
	// (get) Token: 0x06002380 RID: 9088 RVA: 0x000349F2 File Offset: 0x00032BF2
	protected override float PositionTickRate
	{
		get
		{
			return 0.05f;
		}
	}

	// Token: 0x170002F5 RID: 757
	// (get) Token: 0x06002381 RID: 9089 RVA: 0x0000441C File Offset: 0x0000261C
	protected override bool PositionTickFixedTime
	{
		get
		{
			return true;
		}
	}

	// Token: 0x06002382 RID: 9090 RVA: 0x0000627E File Offset: 0x0000447E
	public override float GetNetworkTime()
	{
		return Time.fixedTime;
	}

	// Token: 0x06002383 RID: 9091 RVA: 0x000E2944 File Offset: 0x000E0B44
	public float GetDesiredAltitude()
	{
		Vector3 vector = base.transform.position + base.transform.forward * 200f;
		return (TerrainMeta.HeightMap.GetHeight(base.transform.position) + TerrainMeta.HeightMap.GetHeight(vector) + TerrainMeta.HeightMap.GetHeight(vector + Vector3.right * 50f) + TerrainMeta.HeightMap.GetHeight(vector - Vector3.right * 50f) + TerrainMeta.HeightMap.GetHeight(vector + Vector3.forward * 50f) + TerrainMeta.HeightMap.GetHeight(vector - Vector3.forward * 50f)) / 6f + this.defaultAltitude;
	}

	// Token: 0x06002384 RID: 9092 RVA: 0x000E2A28 File Offset: 0x000E0C28
	public override void ServerInit()
	{
		base.ServerInit();
		base.Invoke(new Action(this.RetireToSunset), 600f);
		this.movePosition = base.transform.position;
		this.movePosition.y = this.defaultAltitude;
		base.transform.position = this.movePosition;
	}

	// Token: 0x06002385 RID: 9093 RVA: 0x000E2A85 File Offset: 0x000E0C85
	public void RetireToSunset()
	{
		this.isRetiring = true;
		this.movePosition = new Vector3(10000f, this.defaultAltitude, 10000f);
	}

	// Token: 0x06002386 RID: 9094 RVA: 0x000E2AAC File Offset: 0x000E0CAC
	public void PickNewPatrolPoint()
	{
		this.movePosition = this.pathFinder.GetRandomPatrolPoint();
		float num = 0f;
		if (TerrainMeta.HeightMap != null)
		{
			num = TerrainMeta.HeightMap.GetHeight(this.movePosition);
		}
		this.movePosition.y = num + this.defaultAltitude;
	}

	// Token: 0x06002387 RID: 9095 RVA: 0x000E2B04 File Offset: 0x000E0D04
	private void FixedUpdate()
	{
		if (base.isClient)
		{
			return;
		}
		if (this.isRetiring && Vector3.Distance(base.transform.position, Vector3.zero) > 4900f)
		{
			base.Invoke(new Action(this.DelayedDestroy), 0f);
		}
		if (base.IsInvoking(new Action(this.DelayedDestroy)))
		{
			return;
		}
		this.altitude = Mathf.Lerp(this.altitude, this.GetDesiredAltitude(), Time.fixedDeltaTime * 0.25f);
		if (Vector3Ex.Distance2D(this.movePosition, base.transform.position) < 10f)
		{
			this.PickNewPatrolPoint();
			this.blockTurningFor = 6f;
		}
		this.blockTurningFor -= Time.fixedDeltaTime;
		bool flag = this.blockTurningFor > 0f;
		this.movePosition.y = this.altitude;
		Vector3 vector = Vector3Ex.Direction(this.movePosition, base.transform.position);
		if (flag)
		{
			Vector3 position = base.transform.position;
			position.y = this.altitude;
			Vector3 vector2 = QuaternionEx.LookRotationForcedUp(base.transform.forward, Vector3.up) * Vector3.forward;
			vector = Vector3Ex.Direction(position + vector2 * 2000f, base.transform.position);
		}
		Vector3 vector3 = Vector3.Lerp(base.transform.forward, vector, Time.fixedDeltaTime * this.turnRate);
		base.transform.forward = vector3;
		bool flag2 = Vector3.Dot(base.transform.right, vector) > 0.55f;
		bool flag3 = Vector3.Dot(-base.transform.right, vector) > 0.55f;
		base.SetFlag(BaseEntity.Flags.Reserved1, flag2, false, true);
		base.SetFlag(BaseEntity.Flags.Reserved2, flag3, false, true);
		if (flag3 || flag2)
		{
			this.turnSeconds += Time.fixedDeltaTime;
		}
		else
		{
			this.turnSeconds = 0f;
		}
		if (this.turnSeconds > 10f)
		{
			this.turnSeconds = 0f;
			this.blockTurningFor = 8f;
		}
		base.transform.position += base.transform.forward * this.speed * Time.fixedDeltaTime;
		this.nextMissileTime = Time.realtimeSinceStartup + 10f;
	}

	// Token: 0x06002388 RID: 9096 RVA: 0x00003384 File Offset: 0x00001584
	public void DelayedDestroy()
	{
		base.Kill(BaseNetworkable.DestroyMode.None);
	}

	// Token: 0x04001B64 RID: 7012
	public float speed = 150f;

	// Token: 0x04001B65 RID: 7013
	public float defaultAltitude = 150f;

	// Token: 0x04001B66 RID: 7014
	public float altitude = 250f;

	// Token: 0x04001B67 RID: 7015
	public float altitudeLerpSpeed = 30f;

	// Token: 0x04001B68 RID: 7016
	public float turnRate = 1f;

	// Token: 0x04001B69 RID: 7017
	public float flybySoundLengthUntilMax = 4.5f;

	// Token: 0x04001B6A RID: 7018
	public SoundPlayer flybySound;

	// Token: 0x04001B6B RID: 7019
	public GameObject body;

	// Token: 0x04001B6C RID: 7020
	public float rollSpeed = 1f;

	// Token: 0x04001B6D RID: 7021
	protected Vector3 movePosition;

	// Token: 0x04001B6E RID: 7022
	public GameObjectRef missilePrefab;

	// Token: 0x04001B6F RID: 7023
	private float nextMissileTime;

	// Token: 0x04001B70 RID: 7024
	public float blockTurningFor;

	// Token: 0x04001B71 RID: 7025
	private bool isRetiring;

	// Token: 0x04001B72 RID: 7026
	private CH47PathFinder pathFinder = new CH47PathFinder();

	// Token: 0x04001B73 RID: 7027
	private float turnSeconds;
}
