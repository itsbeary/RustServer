using System;
using Network;
using UnityEngine;

// Token: 0x020000C8 RID: 200
public class SantaSleigh : BaseEntity
{
	// Token: 0x060011E6 RID: 4582 RVA: 0x0009116C File Offset: 0x0008F36C
	public override bool OnRpcMessage(BasePlayer player, uint rpc, Message msg)
	{
		using (TimeWarning.New("SantaSleigh.OnRpcMessage", 0))
		{
		}
		return base.OnRpcMessage(player, rpc, msg);
	}

	// Token: 0x060011E7 RID: 4583 RVA: 0x0000627E File Offset: 0x0000447E
	public override float GetNetworkTime()
	{
		return Time.fixedTime;
	}

	// Token: 0x060011E8 RID: 4584 RVA: 0x000911AC File Offset: 0x0008F3AC
	public void InitDropPosition(Vector3 newDropPosition)
	{
		this.dropPosition = newDropPosition;
		this.dropPosition.y = 0f;
	}

	// Token: 0x060011E9 RID: 4585 RVA: 0x000911C8 File Offset: 0x0008F3C8
	public override void ServerInit()
	{
		base.ServerInit();
		if (this.dropPosition == Vector3.zero)
		{
			this.dropPosition = this.RandomDropPosition();
		}
		this.UpdateDropPosition(this.dropPosition);
		base.Invoke(new Action(this.SendHoHoHo), 0f);
	}

	// Token: 0x060011EA RID: 4586 RVA: 0x0009121C File Offset: 0x0008F41C
	public void SendHoHoHo()
	{
		base.Invoke(new Action(this.SendHoHoHo), this.hohohospacing + UnityEngine.Random.Range(0f, this.hohoho_additional_spacing));
		base.ClientRPC(null, "ClientPlayHoHoHo");
	}

	// Token: 0x060011EB RID: 4587 RVA: 0x00091254 File Offset: 0x0008F454
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

	// Token: 0x060011EC RID: 4588 RVA: 0x000912C0 File Offset: 0x0008F4C0
	public void UpdateDropPosition(Vector3 newDropPosition)
	{
		float x = TerrainMeta.Size.x;
		float num = SantaSleigh.altitudeAboveTerrain;
		this.startPos = Vector3Ex.Range(-1f, 1f);
		this.startPos.y = 0f;
		this.startPos.Normalize();
		this.startPos *= x * 1.25f;
		this.startPos.y = num;
		this.endPos = this.startPos * -1f;
		this.endPos.y = this.startPos.y;
		this.startPos += newDropPosition;
		this.endPos += newDropPosition;
		this.secondsToTake = Vector3.Distance(this.startPos, this.endPos) / 25f;
		this.secondsToTake *= UnityEngine.Random.Range(0.95f, 1.05f);
		base.transform.SetPositionAndRotation(this.startPos, Quaternion.LookRotation(this.endPos - this.startPos));
		this.dropPosition = newDropPosition;
	}

	// Token: 0x060011ED RID: 4589 RVA: 0x000913EC File Offset: 0x0008F5EC
	private void FixedUpdate()
	{
		if (!base.isServer)
		{
			return;
		}
		Vector3 vector = base.transform.position;
		Quaternion quaternion = base.transform.rotation;
		this.secondsTaken += Time.deltaTime;
		float num = Mathf.InverseLerp(0f, this.secondsToTake, this.secondsTaken);
		if (!this.dropped && num >= 0.5f)
		{
			this.dropped = true;
			BaseEntity baseEntity = GameManager.server.CreateEntity(this.prefabDrop.resourcePath, this.dropOrigin.transform.position, default(Quaternion), true);
			if (baseEntity)
			{
				baseEntity.globalBroadcast = true;
				baseEntity.Spawn();
			}
		}
		vector = Vector3.Lerp(this.startPos, this.endPos, num);
		Vector3 normalized = (this.endPos - this.startPos).normalized;
		Vector3 vector2 = Vector3.zero;
		if (this.swimScale != Vector3.zero)
		{
			if (this.swimRandom == 0f)
			{
				this.swimRandom = UnityEngine.Random.Range(0f, 20f);
			}
			float num2 = Time.time + this.swimRandom;
			vector2 = new Vector3(Mathf.Sin(num2 * this.swimSpeed.x) * this.swimScale.x, Mathf.Cos(num2 * this.swimSpeed.y) * this.swimScale.y, Mathf.Sin(num2 * this.swimSpeed.z) * this.swimScale.z);
			vector2 = base.transform.InverseTransformDirection(vector2);
			vector += vector2 * this.appliedSwimScale;
		}
		quaternion = Quaternion.LookRotation(normalized) * Quaternion.Euler(Mathf.Cos(Time.time * this.swimSpeed.y) * this.appliedSwimRotation, 0f, Mathf.Sin(Time.time * this.swimSpeed.x) * this.appliedSwimRotation);
		Vector3 vector3 = vector;
		float height = TerrainMeta.HeightMap.GetHeight(vector3 + base.transform.forward * 30f);
		float height2 = TerrainMeta.HeightMap.GetHeight(vector3);
		float num3 = Mathf.Max(height, height2);
		float num4 = Mathf.Max(SantaSleigh.desiredAltitude, num3 + SantaSleigh.altitudeAboveTerrain);
		vector3.y = Mathf.Lerp(base.transform.position.y, num4, Time.fixedDeltaTime * 0.5f);
		vector = vector3;
		base.transform.hasChanged = true;
		if (num >= 1f)
		{
			base.Kill(BaseNetworkable.DestroyMode.None);
		}
		base.transform.SetPositionAndRotation(vector, quaternion);
	}

	// Token: 0x060011EE RID: 4590 RVA: 0x00091698 File Offset: 0x0008F898
	[ServerVar]
	public static void drop(ConsoleSystem.Arg arg)
	{
		BasePlayer basePlayer = arg.Player();
		if (!basePlayer)
		{
			return;
		}
		Debug.Log("Santa Inbound");
		BaseEntity baseEntity = GameManager.server.CreateEntity("assets/prefabs/misc/xmas/sleigh/santasleigh.prefab", default(Vector3), default(Quaternion), true);
		if (baseEntity)
		{
			baseEntity.GetComponent<SantaSleigh>().InitDropPosition(basePlayer.transform.position + new Vector3(0f, 10f, 0f));
			baseEntity.Spawn();
		}
	}

	// Token: 0x04000B2C RID: 2860
	public GameObjectRef prefabDrop;

	// Token: 0x04000B2D RID: 2861
	public SpawnFilter filter;

	// Token: 0x04000B2E RID: 2862
	public Transform dropOrigin;

	// Token: 0x04000B2F RID: 2863
	[ServerVar]
	public static float altitudeAboveTerrain = 50f;

	// Token: 0x04000B30 RID: 2864
	[ServerVar]
	public static float desiredAltitude = 60f;

	// Token: 0x04000B31 RID: 2865
	public Light bigLight;

	// Token: 0x04000B32 RID: 2866
	public SoundPlayer hohoho;

	// Token: 0x04000B33 RID: 2867
	public float hohohospacing = 4f;

	// Token: 0x04000B34 RID: 2868
	public float hohoho_additional_spacing = 2f;

	// Token: 0x04000B35 RID: 2869
	private Vector3 startPos;

	// Token: 0x04000B36 RID: 2870
	private Vector3 endPos;

	// Token: 0x04000B37 RID: 2871
	private float secondsToTake;

	// Token: 0x04000B38 RID: 2872
	private float secondsTaken;

	// Token: 0x04000B39 RID: 2873
	private bool dropped;

	// Token: 0x04000B3A RID: 2874
	private Vector3 dropPosition = Vector3.zero;

	// Token: 0x04000B3B RID: 2875
	public Vector3 swimScale;

	// Token: 0x04000B3C RID: 2876
	public Vector3 swimSpeed;

	// Token: 0x04000B3D RID: 2877
	private float swimRandom;

	// Token: 0x04000B3E RID: 2878
	public float appliedSwimScale = 1f;

	// Token: 0x04000B3F RID: 2879
	public float appliedSwimRotation = 20f;

	// Token: 0x04000B40 RID: 2880
	private const string path = "assets/prefabs/misc/xmas/sleigh/santasleigh.prefab";
}
