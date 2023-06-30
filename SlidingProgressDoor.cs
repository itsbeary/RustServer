using System;
using Facepunch;
using ProtoBuf;
using UnityEngine;

// Token: 0x020004E1 RID: 1249
public class SlidingProgressDoor : ProgressDoor
{
	// Token: 0x06002890 RID: 10384 RVA: 0x000FAD3C File Offset: 0x000F8F3C
	public override void Spawn()
	{
		base.Spawn();
		base.InvokeRepeating(new Action(this.ServerUpdate), 0f, 0.1f);
		if (this.vehiclePhysBox != null)
		{
			this.vehiclePhysBox.gameObject.SetActive(false);
		}
	}

	// Token: 0x06002891 RID: 10385 RVA: 0x000FAD8A File Offset: 0x000F8F8A
	public override void NoEnergy()
	{
		base.NoEnergy();
	}

	// Token: 0x06002892 RID: 10386 RVA: 0x000FAD92 File Offset: 0x000F8F92
	public override void AddEnergy(float amount)
	{
		this.lastEnergyTime = Time.time;
		base.AddEnergy(amount);
	}

	// Token: 0x06002893 RID: 10387 RVA: 0x000FADA8 File Offset: 0x000F8FA8
	public void ServerUpdate()
	{
		if (base.isServer)
		{
			if (this.lastServerUpdateTime == 0f)
			{
				this.lastServerUpdateTime = Time.realtimeSinceStartup;
			}
			float num = Time.realtimeSinceStartup - this.lastServerUpdateTime;
			this.lastServerUpdateTime = Time.realtimeSinceStartup;
			if (Time.time > this.lastEnergyTime + 0.333f)
			{
				float num2 = this.energyForOpen * num / this.secondsToClose;
				float num3 = Mathf.Min(this.storedEnergy, num2);
				if (this.vehiclePhysBox != null)
				{
					this.vehiclePhysBox.gameObject.SetActive(num3 > 0f && this.storedEnergy > 0f);
					if (this.vehiclePhysBox.gameObject.activeSelf && this.vehiclePhysBox.ContentsCount > 0)
					{
						num3 = 0f;
					}
				}
				this.storedEnergy -= num3;
				this.storedEnergy = Mathf.Clamp(this.storedEnergy, 0f, this.energyForOpen);
				if (num3 > 0f)
				{
					foreach (global::IOEntity.IOSlot ioslot in this.outputs)
					{
						if (ioslot.connectedTo.Get(true) != null)
						{
							ioslot.connectedTo.Get(true).IOInput(this, this.ioType, -num3, ioslot.connectedToSlot);
						}
					}
				}
			}
			this.UpdateProgress();
		}
	}

	// Token: 0x06002894 RID: 10388 RVA: 0x000FAF10 File Offset: 0x000F9110
	public override void UpdateProgress()
	{
		Vector3 localPosition = this.doorObject.transform.localPosition;
		float num = this.storedEnergy / this.energyForOpen;
		Vector3 vector = Vector3.Lerp(this.closedPosition, this.openPosition, num);
		this.doorObject.transform.localPosition = vector;
		if (base.isServer)
		{
			bool flag = Vector3.Distance(localPosition, vector) > 0.01f;
			base.SetFlag(global::BaseEntity.Flags.Reserved1, flag, false, true);
			if (flag)
			{
				base.SendNetworkUpdate(global::BasePlayer.NetworkQueue.Update);
			}
		}
	}

	// Token: 0x06002895 RID: 10389 RVA: 0x000FAF8F File Offset: 0x000F918F
	public override void Load(global::BaseNetworkable.LoadInfo info)
	{
		base.Load(info);
		ProtoBuf.SphereEntity sphereEntity = info.msg.sphereEntity;
	}

	// Token: 0x06002896 RID: 10390 RVA: 0x000FAFA4 File Offset: 0x000F91A4
	public override void Save(global::BaseNetworkable.SaveInfo info)
	{
		base.Save(info);
		info.msg.sphereEntity = Pool.Get<ProtoBuf.SphereEntity>();
		info.msg.sphereEntity.radius = this.storedEnergy;
	}

	// Token: 0x040020DF RID: 8415
	public Vector3 openPosition;

	// Token: 0x040020E0 RID: 8416
	public Vector3 closedPosition;

	// Token: 0x040020E1 RID: 8417
	public GameObject doorObject;

	// Token: 0x040020E2 RID: 8418
	public TriggerVehiclePush vehiclePhysBox;

	// Token: 0x040020E3 RID: 8419
	private float lastEnergyTime;

	// Token: 0x040020E4 RID: 8420
	private float lastServerUpdateTime;
}
