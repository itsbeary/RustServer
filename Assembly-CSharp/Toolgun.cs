using System;
using Network;
using UnityEngine;

// Token: 0x020000E1 RID: 225
public class Toolgun : Hammer
{
	// Token: 0x060013B3 RID: 5043 RVA: 0x0009DE6C File Offset: 0x0009C06C
	public override bool OnRpcMessage(BasePlayer player, uint rpc, Message msg)
	{
		using (TimeWarning.New("Toolgun.OnRpcMessage", 0))
		{
		}
		return base.OnRpcMessage(player, rpc, msg);
	}

	// Token: 0x060013B4 RID: 5044 RVA: 0x0009DEAC File Offset: 0x0009C0AC
	public override void DoAttackShared(HitInfo info)
	{
		if (base.isServer)
		{
			base.ClientRPC<Vector3, Vector3>(null, "EffectSpawn", info.HitPositionWorld, info.HitNormalWorld);
		}
		base.DoAttackShared(info);
	}

	// Token: 0x04000C3B RID: 3131
	public GameObjectRef attackEffect;

	// Token: 0x04000C3C RID: 3132
	public GameObjectRef beamEffect;

	// Token: 0x04000C3D RID: 3133
	public GameObjectRef beamImpactEffect;

	// Token: 0x04000C3E RID: 3134
	public GameObjectRef errorEffect;

	// Token: 0x04000C3F RID: 3135
	public GameObjectRef beamEffectClassic;

	// Token: 0x04000C40 RID: 3136
	public GameObjectRef beamImpactEffectClassic;

	// Token: 0x04000C41 RID: 3137
	public Transform muzzlePoint;
}
