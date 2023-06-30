using System;
using Network;
using UnityEngine;

// Token: 0x02000078 RID: 120
public class Flashbang : TimedExplosive
{
	// Token: 0x06000B47 RID: 2887 RVA: 0x0006543C File Offset: 0x0006363C
	public override bool OnRpcMessage(BasePlayer player, uint rpc, Message msg)
	{
		using (TimeWarning.New("Flashbang.OnRpcMessage", 0))
		{
		}
		return base.OnRpcMessage(player, rpc, msg);
	}

	// Token: 0x06000B48 RID: 2888 RVA: 0x0006547C File Offset: 0x0006367C
	public override void Explode()
	{
		base.ClientRPC<Vector3>(null, "Client_DoFlash", base.transform.position);
		base.Explode();
	}

	// Token: 0x06000B49 RID: 2889 RVA: 0x00029C50 File Offset: 0x00027E50
	public void DelayedDestroy()
	{
		base.Kill(BaseNetworkable.DestroyMode.Gib);
	}

	// Token: 0x04000769 RID: 1897
	public SoundDefinition deafLoopDef;

	// Token: 0x0400076A RID: 1898
	public float flashReductionPerSecond = 1f;

	// Token: 0x0400076B RID: 1899
	public float flashToAdd = 3f;

	// Token: 0x0400076C RID: 1900
	public float flashMinRange = 5f;

	// Token: 0x0400076D RID: 1901
	public float flashMaxRange = 10f;
}
