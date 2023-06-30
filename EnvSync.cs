using System;
using Facepunch;
using ProtoBuf;
using UnityEngine;

// Token: 0x02000416 RID: 1046
public class EnvSync : PointEntity
{
	// Token: 0x0600237B RID: 9083 RVA: 0x000E2856 File Offset: 0x000E0A56
	public override void ServerInit()
	{
		base.ServerInit();
		base.InvokeRepeating(new Action(this.UpdateNetwork), 5f, 5f);
	}

	// Token: 0x0600237C RID: 9084 RVA: 0x00007D2F File Offset: 0x00005F2F
	private void UpdateNetwork()
	{
		base.SendNetworkUpdate(global::BasePlayer.NetworkQueue.Update);
	}

	// Token: 0x0600237D RID: 9085 RVA: 0x000E287C File Offset: 0x000E0A7C
	public override void Save(global::BaseNetworkable.SaveInfo info)
	{
		base.Save(info);
		info.msg.environment = Pool.Get<ProtoBuf.Environment>();
		if (TOD_Sky.Instance)
		{
			info.msg.environment.dateTime = TOD_Sky.Instance.Cycle.DateTime.ToBinary();
		}
		info.msg.environment.engineTime = Time.realtimeSinceStartup;
	}

	// Token: 0x0600237E RID: 9086 RVA: 0x000E28E8 File Offset: 0x000E0AE8
	public override void Load(global::BaseNetworkable.LoadInfo info)
	{
		base.Load(info);
		if (info.msg.environment == null)
		{
			return;
		}
		if (!TOD_Sky.Instance)
		{
			return;
		}
		if (base.isServer)
		{
			TOD_Sky.Instance.Cycle.DateTime = DateTime.FromBinary(info.msg.environment.dateTime);
		}
	}

	// Token: 0x04001B62 RID: 7010
	private const float syncInterval = 5f;

	// Token: 0x04001B63 RID: 7011
	private const float syncIntervalInv = 0.2f;
}
