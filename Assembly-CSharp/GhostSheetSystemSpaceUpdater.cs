using System;
using EZhex1991.EZSoftBone;
using UnityEngine;

// Token: 0x020000FE RID: 254
public class GhostSheetSystemSpaceUpdater : MonoBehaviour, IClientComponent
{
	// Token: 0x060015A8 RID: 5544 RVA: 0x000AB103 File Offset: 0x000A9303
	public void Awake()
	{
		this.ezSoftBones = base.GetComponents<EZSoftBone>();
		this.player = base.gameObject.ToBaseEntity() as BasePlayer;
	}

	// Token: 0x060015A9 RID: 5545 RVA: 0x000AB128 File Offset: 0x000A9328
	public void Update()
	{
		if (this.ezSoftBones == null || this.ezSoftBones.Length == 0 || this.player == null)
		{
			return;
		}
		BaseMountable mounted = this.player.GetMounted();
		if (mounted != null)
		{
			this.SetSimulateSpace(mounted.transform, false);
			return;
		}
		BaseEntity parentEntity = this.player.GetParentEntity();
		if (parentEntity != null)
		{
			this.SetSimulateSpace(parentEntity.transform, true);
			return;
		}
		this.SetSimulateSpace(null, true);
	}

	// Token: 0x060015AA RID: 5546 RVA: 0x000AB1A4 File Offset: 0x000A93A4
	private void SetSimulateSpace(Transform transform, bool collisionEnabled)
	{
		for (int i = 0; i < this.ezSoftBones.Length; i++)
		{
			EZSoftBone ezsoftBone = this.ezSoftBones[i];
			ezsoftBone.simulateSpace = transform;
			ezsoftBone.collisionEnabled = collisionEnabled;
		}
	}

	// Token: 0x04000DE0 RID: 3552
	private EZSoftBone[] ezSoftBones;

	// Token: 0x04000DE1 RID: 3553
	private BasePlayer player;
}
