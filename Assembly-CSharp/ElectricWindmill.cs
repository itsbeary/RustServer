using System;
using Facepunch;
using ProtoBuf;
using UnityEngine;

// Token: 0x02000138 RID: 312
public class ElectricWindmill : global::IOEntity
{
	// Token: 0x060016EC RID: 5868 RVA: 0x000AFC16 File Offset: 0x000ADE16
	public override int MaximalPowerOutput()
	{
		return this.maxPowerGeneration;
	}

	// Token: 0x060016ED RID: 5869 RVA: 0x0000441C File Offset: 0x0000261C
	public override bool IsRootEntity()
	{
		return true;
	}

	// Token: 0x060016EE RID: 5870 RVA: 0x000AFC20 File Offset: 0x000ADE20
	public float GetWindSpeedScale()
	{
		float num = Time.time / 600f;
		float num2 = base.transform.position.x / 512f;
		float num3 = base.transform.position.z / 512f;
		float num4 = Mathf.PerlinNoise(num2 + num, num3 + num * 0.1f);
		float height = TerrainMeta.HeightMap.GetHeight(base.transform.position);
		float num5 = base.transform.position.y - height;
		if (num5 < 0f)
		{
			num5 = 0f;
		}
		return Mathf.Clamp01(Mathf.InverseLerp(0f, 50f, num5) * 0.5f + num4);
	}

	// Token: 0x060016EF RID: 5871 RVA: 0x000AFCCF File Offset: 0x000ADECF
	public override void Load(global::BaseNetworkable.LoadInfo info)
	{
		base.Load(info);
	}

	// Token: 0x060016F0 RID: 5872 RVA: 0x000AFCD8 File Offset: 0x000ADED8
	public override void ServerInit()
	{
		base.ServerInit();
		base.InvokeRandomized(new Action(this.WindUpdate), 1f, 20f, 2f);
	}

	// Token: 0x060016F1 RID: 5873 RVA: 0x000AFD04 File Offset: 0x000ADF04
	public override void Save(global::BaseNetworkable.SaveInfo info)
	{
		base.Save(info);
		if (!info.forDisk)
		{
			if (info.msg.ioEntity == null)
			{
				info.msg.ioEntity = Pool.Get<ProtoBuf.IOEntity>();
			}
			info.msg.ioEntity.genericFloat1 = Time.time;
			info.msg.ioEntity.genericFloat2 = this.serverWindSpeed;
		}
	}

	// Token: 0x060016F2 RID: 5874 RVA: 0x000AFD68 File Offset: 0x000ADF68
	public bool AmIVisible()
	{
		int num = 15;
		Vector3 vector = base.transform.position + Vector3.up * 6f;
		if (!base.IsVisible(vector + base.transform.up * (float)num, (float)(num + 1)))
		{
			return false;
		}
		Vector3 windAimDir = this.GetWindAimDir(Time.time);
		return base.IsVisible(vector + windAimDir * (float)num, (float)(num + 1));
	}

	// Token: 0x060016F3 RID: 5875 RVA: 0x000AFDE8 File Offset: 0x000ADFE8
	public void WindUpdate()
	{
		this.serverWindSpeed = this.GetWindSpeedScale();
		if (!this.AmIVisible())
		{
			this.serverWindSpeed = 0f;
		}
		int num = Mathf.FloorToInt((float)this.maxPowerGeneration * this.serverWindSpeed);
		bool flag = this.currentEnergy != num;
		this.currentEnergy = num;
		if (flag)
		{
			this.MarkDirty();
		}
		base.SendNetworkUpdate(global::BasePlayer.NetworkQueue.Update);
	}

	// Token: 0x060016F4 RID: 5876 RVA: 0x000668BB File Offset: 0x00064ABB
	public override int GetPassthroughAmount(int outputSlot = 0)
	{
		if (outputSlot != 0)
		{
			return 0;
		}
		return this.currentEnergy;
	}

	// Token: 0x060016F5 RID: 5877 RVA: 0x000AFE4C File Offset: 0x000AE04C
	public Vector3 GetWindAimDir(float time)
	{
		float num = time / 3600f * 360f;
		int num2 = 10;
		Vector3 vector = new Vector3(Mathf.Sin(num * 0.017453292f) * (float)num2, 0f, Mathf.Cos(num * 0.017453292f) * (float)num2);
		return vector.normalized;
	}

	// Token: 0x04000F1D RID: 3869
	public Animator animator;

	// Token: 0x04000F1E RID: 3870
	public int maxPowerGeneration = 100;

	// Token: 0x04000F1F RID: 3871
	public Transform vaneRot;

	// Token: 0x04000F20 RID: 3872
	public SoundDefinition wooshSound;

	// Token: 0x04000F21 RID: 3873
	public Transform wooshOrigin;

	// Token: 0x04000F22 RID: 3874
	public float targetSpeed;

	// Token: 0x04000F23 RID: 3875
	private float serverWindSpeed;
}
