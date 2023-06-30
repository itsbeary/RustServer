using System;
using System.Collections.Generic;
using Facepunch;
using ProtoBuf;
using UnityEngine;

// Token: 0x020000F7 RID: 247
public class ZiplineArrivalPoint : global::BaseEntity
{
	// Token: 0x0600158A RID: 5514 RVA: 0x000AA49C File Offset: 0x000A869C
	public override void Save(global::BaseNetworkable.SaveInfo info)
	{
		base.Save(info);
		if (info.msg.ZiplineArrival == null)
		{
			info.msg.ZiplineArrival = Pool.Get<ProtoBuf.ZiplineArrivalPoint>();
		}
		info.msg.ZiplineArrival.linePoints = Pool.GetList<VectorData>();
		foreach (Vector3 vector in this.linePositions)
		{
			info.msg.ZiplineArrival.linePoints.Add(vector);
		}
	}

	// Token: 0x0600158B RID: 5515 RVA: 0x000AA51C File Offset: 0x000A871C
	public void SetPositions(List<Vector3> points)
	{
		this.linePositions = new Vector3[points.Count];
		for (int i = 0; i < points.Count; i++)
		{
			this.linePositions[i] = points[i];
		}
	}

	// Token: 0x0600158C RID: 5516 RVA: 0x000AA560 File Offset: 0x000A8760
	public override void Load(global::BaseNetworkable.LoadInfo info)
	{
		base.Load(info);
		if (info.msg.ZiplineArrival != null && this.linePositions == null)
		{
			this.linePositions = new Vector3[info.msg.ZiplineArrival.linePoints.Count];
			for (int i = 0; i < info.msg.ZiplineArrival.linePoints.Count; i++)
			{
				this.linePositions[i] = info.msg.ZiplineArrival.linePoints[i];
			}
		}
	}

	// Token: 0x0600158D RID: 5517 RVA: 0x000AA5F0 File Offset: 0x000A87F0
	public override void ResetState()
	{
		base.ResetState();
		this.linePositions = null;
	}

	// Token: 0x04000DA3 RID: 3491
	public LineRenderer Line;

	// Token: 0x04000DA4 RID: 3492
	private Vector3[] linePositions;
}
