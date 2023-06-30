using System;
using UnityEngine;
using UnityEngine.AI;

// Token: 0x02000209 RID: 521
public class NPCPlayerNavigator : BaseNavigator
{
	// Token: 0x1700024F RID: 591
	// (get) Token: 0x06001B6E RID: 7022 RVA: 0x000C24A3 File Offset: 0x000C06A3
	// (set) Token: 0x06001B6F RID: 7023 RVA: 0x000C24AB File Offset: 0x000C06AB
	public NPCPlayer NPCPlayerEntity { get; private set; }

	// Token: 0x06001B70 RID: 7024 RVA: 0x000C24B4 File Offset: 0x000C06B4
	public override void Init(BaseCombatEntity entity, NavMeshAgent agent)
	{
		base.Init(entity, agent);
		this.NPCPlayerEntity = entity as NPCPlayer;
	}

	// Token: 0x06001B71 RID: 7025 RVA: 0x000C24CA File Offset: 0x000C06CA
	protected override bool CanEnableNavMeshNavigation()
	{
		return base.CanEnableNavMeshNavigation() && (!this.NPCPlayerEntity.isMounted || this.CanNavigateMounted);
	}

	// Token: 0x06001B72 RID: 7026 RVA: 0x000C24F0 File Offset: 0x000C06F0
	protected override bool CanUpdateMovement()
	{
		if (!base.CanUpdateMovement())
		{
			return false;
		}
		if (this.NPCPlayerEntity.IsWounded())
		{
			return false;
		}
		if (base.CurrentNavigationType == BaseNavigator.NavigationType.NavMesh && (this.NPCPlayerEntity.IsDormant || !this.NPCPlayerEntity.syncPosition) && base.Agent.enabled)
		{
			base.SetDestination(this.NPCPlayerEntity.ServerPosition, 1f, 0f, 0f);
			return false;
		}
		return true;
	}

	// Token: 0x06001B73 RID: 7027 RVA: 0x000C256C File Offset: 0x000C076C
	protected override void UpdatePositionAndRotation(Vector3 moveToPosition, float delta)
	{
		base.UpdatePositionAndRotation(moveToPosition, delta);
		if (this.overrideFacingDirectionMode == BaseNavigator.OverrideFacingDirectionMode.None)
		{
			if (base.CurrentNavigationType == BaseNavigator.NavigationType.NavMesh)
			{
				this.NPCPlayerEntity.SetAimDirection(base.Agent.desiredVelocity.normalized);
				return;
			}
			if (base.CurrentNavigationType == BaseNavigator.NavigationType.AStar || base.CurrentNavigationType == BaseNavigator.NavigationType.Base)
			{
				this.NPCPlayerEntity.SetAimDirection(Vector3Ex.Direction2D(moveToPosition, base.transform.position));
			}
		}
	}

	// Token: 0x06001B74 RID: 7028 RVA: 0x000C25E0 File Offset: 0x000C07E0
	public override void ApplyFacingDirectionOverride()
	{
		base.ApplyFacingDirectionOverride();
		if (this.overrideFacingDirectionMode == BaseNavigator.OverrideFacingDirectionMode.None)
		{
			return;
		}
		if (this.overrideFacingDirectionMode == BaseNavigator.OverrideFacingDirectionMode.Direction)
		{
			this.NPCPlayerEntity.SetAimDirection(this.facingDirectionOverride);
			return;
		}
		if (this.facingDirectionEntity != null)
		{
			Vector3 aimDirection = NPCPlayerNavigator.GetAimDirection(this.NPCPlayerEntity, this.facingDirectionEntity);
			this.facingDirectionOverride = aimDirection;
			this.NPCPlayerEntity.SetAimDirection(this.facingDirectionOverride);
		}
	}

	// Token: 0x06001B75 RID: 7029 RVA: 0x000C2650 File Offset: 0x000C0850
	private static Vector3 GetAimDirection(BasePlayer aimingPlayer, BaseEntity target)
	{
		if (target == null)
		{
			return Vector3Ex.Direction2D(aimingPlayer.transform.position + aimingPlayer.eyes.BodyForward() * 1000f, aimingPlayer.transform.position);
		}
		if (Vector3Ex.Distance2D(aimingPlayer.transform.position, target.transform.position) <= 0.75f)
		{
			return Vector3Ex.Direction2D(target.transform.position, aimingPlayer.transform.position);
		}
		return (NPCPlayerNavigator.TargetAimPositionOffset(target) - aimingPlayer.eyes.position).normalized;
	}

	// Token: 0x06001B76 RID: 7030 RVA: 0x000C26F8 File Offset: 0x000C08F8
	private static Vector3 TargetAimPositionOffset(BaseEntity target)
	{
		BasePlayer basePlayer = target as BasePlayer;
		if (!(basePlayer != null))
		{
			return target.CenterPoint();
		}
		if (basePlayer.IsSleeping() || basePlayer.IsWounded())
		{
			return basePlayer.transform.position + Vector3.up * 0.1f;
		}
		return basePlayer.eyes.position - Vector3.up * 0.15f;
	}
}
