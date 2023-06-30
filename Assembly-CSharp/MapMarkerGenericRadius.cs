using System;
using Network;
using ProtoBuf;
using UnityEngine;

// Token: 0x02000099 RID: 153
public class MapMarkerGenericRadius : MapMarker
{
	// Token: 0x06000DF9 RID: 3577 RVA: 0x00076124 File Offset: 0x00074324
	public override bool OnRpcMessage(global::BasePlayer player, uint rpc, Message msg)
	{
		using (TimeWarning.New("MapMarkerGenericRadius.OnRpcMessage", 0))
		{
		}
		return base.OnRpcMessage(player, rpc, msg);
	}

	// Token: 0x06000DFA RID: 3578 RVA: 0x00076164 File Offset: 0x00074364
	public void SendUpdate(bool fullUpdate = true)
	{
		float a = this.color1.a;
		Vector3 vector = new Vector3(this.color1.r, this.color1.g, this.color1.b);
		Vector3 vector2 = new Vector3(this.color2.r, this.color2.g, this.color2.b);
		base.ClientRPC<Vector3, float, Vector3, float, float>(null, "MarkerUpdate", vector, a, vector2, this.alpha, this.radius);
	}

	// Token: 0x06000DFB RID: 3579 RVA: 0x000761E8 File Offset: 0x000743E8
	public override AppMarker GetAppMarkerData()
	{
		AppMarker appMarkerData = base.GetAppMarkerData();
		appMarkerData.radius = this.radius;
		appMarkerData.color1 = this.color1;
		appMarkerData.color2 = this.color2;
		appMarkerData.alpha = this.alpha;
		return appMarkerData;
	}

	// Token: 0x04000908 RID: 2312
	public float radius;

	// Token: 0x04000909 RID: 2313
	public Color color1;

	// Token: 0x0400090A RID: 2314
	public Color color2;

	// Token: 0x0400090B RID: 2315
	public float alpha;
}
