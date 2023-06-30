using System;
using UnityEngine;
using UnityEngine.AI;

// Token: 0x02000207 RID: 519
public class FishNavigator : BaseNavigator
{
	// Token: 0x1700024D RID: 589
	// (get) Token: 0x06001B5B RID: 7003 RVA: 0x000C2117 File Offset: 0x000C0317
	// (set) Token: 0x06001B5C RID: 7004 RVA: 0x000C211F File Offset: 0x000C031F
	public BaseNpc NPC { get; private set; }

	// Token: 0x06001B5D RID: 7005 RVA: 0x000C2128 File Offset: 0x000C0328
	public override void Init(BaseCombatEntity entity, NavMeshAgent agent)
	{
		base.Init(entity, agent);
		this.NPC = entity as BaseNpc;
	}

	// Token: 0x06001B5E RID: 7006 RVA: 0x000C213E File Offset: 0x000C033E
	protected override bool SetCustomDestination(Vector3 pos, float speedFraction = 1f, float updateInterval = 0f)
	{
		if (!base.SetCustomDestination(pos, speedFraction, updateInterval))
		{
			return false;
		}
		base.Destination = pos;
		return true;
	}

	// Token: 0x06001B5F RID: 7007 RVA: 0x000C2158 File Offset: 0x000C0358
	protected override void UpdatePositionAndRotation(Vector3 moveToPosition, float delta)
	{
		base.transform.position = Vector3.MoveTowards(base.transform.position, moveToPosition, this.GetTargetSpeed() * delta);
		base.BaseEntity.ServerPosition = base.transform.localPosition;
		if (base.ReachedPosition(moveToPosition))
		{
			base.Stop();
			return;
		}
		this.UpdateRotation(moveToPosition, delta);
	}

	// Token: 0x06001B60 RID: 7008 RVA: 0x000C21B7 File Offset: 0x000C03B7
	private void UpdateRotation(Vector3 moveToPosition, float delta)
	{
		base.BaseEntity.ServerRotation = Quaternion.LookRotation(Vector3Ex.Direction(moveToPosition, base.transform.position));
	}
}
