using System;
using UnityEngine;

// Token: 0x020001B7 RID: 439
public class FishingBobber : BaseCombatEntity
{
	// Token: 0x1700021E RID: 542
	// (get) Token: 0x0600190F RID: 6415 RVA: 0x000B8ECF File Offset: 0x000B70CF
	// (set) Token: 0x06001910 RID: 6416 RVA: 0x000B8ED7 File Offset: 0x000B70D7
	public float TireAmount { get; private set; }

	// Token: 0x06001911 RID: 6417 RVA: 0x000B8EE0 File Offset: 0x000B70E0
	public override void ServerInit()
	{
		this.myRigidBody.centerOfMass = this.centerOfMass.localPosition;
		base.ServerInit();
	}

	// Token: 0x06001912 RID: 6418 RVA: 0x000B8F00 File Offset: 0x000B7100
	public void InitialiseBobber(BasePlayer forPlayer, WaterBody forBody, Vector3 targetPos)
	{
		this.initialDirection = forPlayer.eyes.HeadForward().WithY(0f);
		this.spawnPosition = base.transform.position;
		this.initialTargetPosition = targetPos;
		this.initialCastTime = 0f;
		this.initialDistance = Vector3.Distance(targetPos, forPlayer.transform.position.WithY(targetPos.y));
		base.InvokeRepeating(new Action(this.ProcessInitialCast), 0f, 0f);
	}

	// Token: 0x06001913 RID: 6419 RVA: 0x000B8F90 File Offset: 0x000B7190
	private void ProcessInitialCast()
	{
		float num = 0.8f;
		if (this.initialCastTime > num)
		{
			base.transform.position = this.initialTargetPosition;
			base.CancelInvoke(new Action(this.ProcessInitialCast));
			return;
		}
		float num2 = this.initialCastTime / num;
		Vector3 vector = Vector3.Lerp(this.spawnPosition, this.initialTargetPosition, 0.5f);
		vector.y += 1.5f;
		Vector3 vector2 = Vector3.Lerp(Vector3.Lerp(this.spawnPosition, vector, num2), Vector3.Lerp(vector, this.initialTargetPosition, num2), num2);
		base.transform.position = vector2;
	}

	// Token: 0x06001914 RID: 6420 RVA: 0x000B9038 File Offset: 0x000B7238
	public void ServerMovementUpdate(bool inputLeft, bool inputRight, bool inputBack, ref BaseFishingRod.FishState state, Vector3 playerPos, ItemModFishable fishableModifier)
	{
		Vector3 normalized = (playerPos - base.transform.position).normalized;
		Vector3 vector = Vector3.zero;
		this.bobberForcePingPong = Mathf.Clamp(Mathf.PingPong(Time.time, 2f), 0.2f, 2f);
		if (state.Contains(BaseFishingRod.FishState.PullingLeft))
		{
			vector = base.transform.right * (Time.deltaTime * this.HorizontalMoveSpeed * this.bobberForcePingPong * fishableModifier.MoveMultiplier * (inputRight ? 0.5f : 1f));
		}
		if (state.Contains(BaseFishingRod.FishState.PullingRight))
		{
			vector = -base.transform.right * (Time.deltaTime * this.HorizontalMoveSpeed * this.bobberForcePingPong * fishableModifier.MoveMultiplier * (inputLeft ? 0.5f : 1f));
		}
		if (state.Contains(BaseFishingRod.FishState.PullingBack))
		{
			vector += -base.transform.forward * (Time.deltaTime * this.PullAwayMoveSpeed * this.bobberForcePingPong * fishableModifier.MoveMultiplier * (inputBack ? 0.5f : 1f));
		}
		if (inputLeft || inputRight)
		{
			float num = 0.8f;
			if ((inputLeft && state == BaseFishingRod.FishState.PullingRight) || (inputRight && state == BaseFishingRod.FishState.PullingLeft))
			{
				num = 1.25f;
			}
			this.TireAmount += Time.deltaTime * num;
		}
		else
		{
			this.TireAmount -= Time.deltaTime * 0.1f;
		}
		if (inputLeft && !state.Contains(BaseFishingRod.FishState.PullingLeft))
		{
			vector += base.transform.right * (Time.deltaTime * this.SidewaysInputForce);
		}
		else if (inputRight && !state.Contains(BaseFishingRod.FishState.PullingRight))
		{
			vector += -base.transform.right * (Time.deltaTime * this.SidewaysInputForce);
		}
		if (inputBack)
		{
			float num2 = Mathx.RemapValClamped(this.TireAmount, 0f, 5f, 1f, 3f);
			vector += normalized * (this.ReelInMoveSpeed * fishableModifier.ReelInSpeedMultiplier * num2 * Time.deltaTime);
		}
		base.transform.LookAt(playerPos.WithY(base.transform.position.y));
		Vector3 vector2 = base.transform.position + vector;
		if (!this.IsDirectionValid(vector2, vector.magnitude, playerPos))
		{
			state = state.FlipHorizontal();
			return;
		}
		base.transform.position = vector2;
	}

	// Token: 0x06001915 RID: 6421 RVA: 0x000B92D4 File Offset: 0x000B74D4
	private bool IsDirectionValid(Vector3 pos, float checkLength, Vector3 playerPos)
	{
		if (Vector3.Angle((pos - playerPos).normalized.WithY(0f), this.initialDirection) > 60f)
		{
			return false;
		}
		Vector3 position = base.transform.position;
		RaycastHit raycastHit;
		return !GamePhysics.Trace(new Ray(position, (pos - position).normalized), 0.1f, out raycastHit, checkLength, 1218511105, QueryTriggerInteraction.UseGlobal, null);
	}

	// Token: 0x0400119C RID: 4508
	public Transform centerOfMass;

	// Token: 0x0400119D RID: 4509
	public Rigidbody myRigidBody;

	// Token: 0x0400119E RID: 4510
	public Transform lineAttachPoint;

	// Token: 0x0400119F RID: 4511
	public Transform bobberRoot;

	// Token: 0x040011A0 RID: 4512
	public const BaseEntity.Flags CaughtFish = BaseEntity.Flags.Reserved1;

	// Token: 0x040011A1 RID: 4513
	public float HorizontalMoveSpeed = 1f;

	// Token: 0x040011A2 RID: 4514
	public float PullAwayMoveSpeed = 1f;

	// Token: 0x040011A3 RID: 4515
	public float SidewaysInputForce = 1f;

	// Token: 0x040011A4 RID: 4516
	public float ReelInMoveSpeed = 1f;

	// Token: 0x040011A5 RID: 4517
	private float bobberForcePingPong;

	// Token: 0x040011A6 RID: 4518
	private Vector3 initialDirection;

	// Token: 0x040011A8 RID: 4520
	private Vector3 initialTargetPosition;

	// Token: 0x040011A9 RID: 4521
	private Vector3 spawnPosition;

	// Token: 0x040011AA RID: 4522
	private TimeSince initialCastTime;

	// Token: 0x040011AB RID: 4523
	private float initialDistance;
}
