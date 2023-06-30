using System;
using Network;
using UnityEngine;

// Token: 0x02000014 RID: 20
public class TreeMarker : BaseEntity
{
	// Token: 0x0600004A RID: 74 RVA: 0x00003200 File Offset: 0x00001400
	public override bool OnRpcMessage(BasePlayer player, uint rpc, Message msg)
	{
		using (TimeWarning.New("TreeMarker.OnRpcMessage", 0))
		{
		}
		return base.OnRpcMessage(player, rpc, msg);
	}

	// Token: 0x0400003D RID: 61
	public GameObjectRef hitEffect;

	// Token: 0x0400003E RID: 62
	public SoundDefinition hitEffectSound;

	// Token: 0x0400003F RID: 63
	public GameObjectRef spawnEffect;

	// Token: 0x04000040 RID: 64
	private Vector3 initialPosition;

	// Token: 0x04000041 RID: 65
	public bool SpherecastOnInit = true;
}
