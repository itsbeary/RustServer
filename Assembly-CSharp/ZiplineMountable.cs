using System;
using System.Collections.Generic;
using ConVar;
using Facepunch;
using Facepunch.Rust;
using ProtoBuf;
using UnityEngine;

// Token: 0x020000F9 RID: 249
public class ZiplineMountable : BaseMountable
{
	// Token: 0x06001590 RID: 5520 RVA: 0x000AA600 File Offset: 0x000A8800
	private Vector3 ProcessBezierMovement(float distanceToTravel)
	{
		if (this.linePoints == null)
		{
			return Vector3.zero;
		}
		float num = 0f;
		for (int i = 0; i < this.linePoints.Count - 1; i++)
		{
			float num2 = Vector3.Distance(this.linePoints[i], this.linePoints[i + 1]);
			if (num + num2 > distanceToTravel)
			{
				float num3 = Mathf.Clamp((distanceToTravel - num) / num2, 0f, 1f);
				return Vector3.Lerp(this.linePoints[i], this.linePoints[i + 1], num3);
			}
			num += num2;
		}
		return this.linePoints[this.linePoints.Count - 1];
	}

	// Token: 0x06001591 RID: 5521 RVA: 0x000AA6B4 File Offset: 0x000A88B4
	private Vector3 GetLineEndPoint(bool applyDismountOffset = false)
	{
		if (applyDismountOffset && this.linePoints != null)
		{
			Vector3 normalized = (this.linePoints[this.linePoints.Count - 2] - this.linePoints[this.linePoints.Count - 1]).normalized;
			return this.linePoints[this.linePoints.Count - 1] + normalized * 1.5f;
		}
		List<Vector3> list = this.linePoints;
		if (list == null)
		{
			return Vector3.zero;
		}
		return list[this.linePoints.Count - 1];
	}

	// Token: 0x06001592 RID: 5522 RVA: 0x000AA758 File Offset: 0x000A8958
	private Vector3 GetNextLinePoint(Transform forTransform)
	{
		Vector3 position = forTransform.position;
		Vector3 forward = forTransform.forward;
		for (int i = 1; i < this.linePoints.Count - 1; i++)
		{
			Vector3 normalized = (this.linePoints[i + 1] - position).normalized;
			Vector3 normalized2 = (this.linePoints[i - 1] - position).normalized;
			float num = Vector3.Dot(forward, normalized);
			float num2 = Vector3.Dot(forward, normalized2);
			if (num > 0f && num2 < 0f)
			{
				return this.linePoints[i + 1];
			}
		}
		return this.GetLineEndPoint(false);
	}

	// Token: 0x06001593 RID: 5523 RVA: 0x000AA806 File Offset: 0x000A8A06
	public override void ResetState()
	{
		base.ResetState();
		this.additiveValue = 0f;
		this.currentTravelDistance = 0f;
		this.hasEnded = false;
		this.linePoints = null;
	}

	// Token: 0x06001594 RID: 5524 RVA: 0x000AA832 File Offset: 0x000A8A32
	public override float MaxVelocity()
	{
		return this.MoveSpeed + this.ForwardAdditive;
	}

	// Token: 0x06001595 RID: 5525 RVA: 0x000AA844 File Offset: 0x000A8A44
	public void SetDestination(List<Vector3> targetLinePoints, Vector3 lineStartPos, Quaternion lineStartRot)
	{
		this.linePoints = targetLinePoints;
		this.currentTravelDistance = 0f;
		this.mountTime = 0f;
		GamePhysics.OverlapSphere(base.transform.position, 6f, this.ignoreColliders, 1218511105, QueryTriggerInteraction.Ignore);
		this.startPosition = base.transform.position;
		this.startRotation = base.transform.rotation;
		this.lastSafePosition = this.startPosition;
		this.endPosition = lineStartPos;
		this.endRotation = lineStartRot;
		this.elapsedMoveTime = 0f;
		this.isAnimatingIn = true;
		base.InvokeRepeating(new Action(this.MovePlayerToPosition), 0f, 0f);
		Analytics.Server.UsedZipline();
	}

	// Token: 0x06001596 RID: 5526 RVA: 0x000AA904 File Offset: 0x000A8B04
	private void Update()
	{
		if (this.linePoints == null || base.isClient)
		{
			return;
		}
		if (this.isAnimatingIn)
		{
			return;
		}
		if (this.hasEnded)
		{
			return;
		}
		float num = (this.MoveSpeed + this.additiveValue * this.ForwardAdditive) * Mathf.Clamp(this.mountTime / this.SpeedUpTime, 0f, 1f) * UnityEngine.Time.smoothDeltaTime;
		this.currentTravelDistance += num;
		Vector3 vector = this.ProcessBezierMovement(this.currentTravelDistance);
		List<RaycastHit> list = Facepunch.Pool.GetList<RaycastHit>();
		Vector3 vector2 = vector.WithY(vector.y - this.ZipCollider.height * 0.6f);
		Vector3 vector3 = vector;
		GamePhysics.CapsuleSweep(vector2, vector3, this.ZipCollider.radius, base.transform.forward, num, list, 1218511105, QueryTriggerInteraction.Ignore);
		foreach (RaycastHit raycastHit in list)
		{
			if (!(raycastHit.collider == this.ZipCollider) && !this.ignoreColliders.Contains(raycastHit.collider) && !(raycastHit.collider.GetComponentInParent<PowerlineNode>() != null))
			{
				global::ZiplineMountable componentInParent = raycastHit.collider.GetComponentInParent<global::ZiplineMountable>();
				if (componentInParent != null)
				{
					componentInParent.EndZipline();
				}
				Vector3 vector4;
				if (!this.GetDismountPosition(this._mounted, out vector4))
				{
					base.transform.position = this.lastSafePosition;
				}
				this.EndZipline();
				Facepunch.Pool.FreeList<RaycastHit>(ref list);
				return;
			}
		}
		Facepunch.Pool.FreeList<RaycastHit>(ref list);
		if (Vector3.Distance(vector, this.GetLineEndPoint(false)) < 0.1f)
		{
			base.transform.position = this.GetLineEndPoint(true);
			this.hasEnded = true;
			return;
		}
		if (Vector3.Distance(this.lastSafePosition, base.transform.position) > 0.75f)
		{
			this.lastSafePosition = base.transform.position;
		}
		Vector3 normalized = (vector - base.transform.position.WithY(vector.y)).normalized;
		base.transform.position = Vector3.Lerp(base.transform.position, vector, UnityEngine.Time.deltaTime * 12f);
		base.transform.forward = normalized;
	}

	// Token: 0x06001597 RID: 5527 RVA: 0x000AAB64 File Offset: 0x000A8D64
	public override void PlayerServerInput(InputState inputState, global::BasePlayer player)
	{
		base.PlayerServerInput(inputState, player);
		if (this.linePoints == null)
		{
			return;
		}
		if (this.hasEnded)
		{
			this.EndZipline();
			return;
		}
		Vector3 position = base.transform.position;
		float num = ((this.GetNextLinePoint(base.transform).y < position.y + 0.1f && inputState.IsDown(BUTTON.FORWARD)) ? 1f : 0f);
		this.additiveValue = Mathf.MoveTowards(this.additiveValue, num, (float)Server.tickrate * ((num > 0f) ? 4f : 2f));
		base.SetFlag(global::BaseEntity.Flags.Reserved1, this.additiveValue > 0.5f, false, true);
	}

	// Token: 0x06001598 RID: 5528 RVA: 0x000AAC1B File Offset: 0x000A8E1B
	private void EndZipline()
	{
		this.DismountAllPlayers();
	}

	// Token: 0x06001599 RID: 5529 RVA: 0x000AAC23 File Offset: 0x000A8E23
	public override void OnPlayerDismounted(global::BasePlayer player)
	{
		base.OnPlayerDismounted(player);
		if (!base.IsDestroyed)
		{
			base.Kill(global::BaseNetworkable.DestroyMode.None);
		}
	}

	// Token: 0x0600159A RID: 5530 RVA: 0x000AAC3B File Offset: 0x000A8E3B
	public override bool ValidDismountPosition(global::BasePlayer player, Vector3 disPos)
	{
		this.ZipCollider.enabled = false;
		bool flag = base.ValidDismountPosition(player, disPos);
		this.ZipCollider.enabled = true;
		return flag;
	}

	// Token: 0x0600159B RID: 5531 RVA: 0x000AAC60 File Offset: 0x000A8E60
	public override void Save(global::BaseNetworkable.SaveInfo info)
	{
		base.Save(info);
		if (this.linePoints == null)
		{
			return;
		}
		if (info.msg.ziplineMountable == null)
		{
			info.msg.ziplineMountable = Facepunch.Pool.Get<ProtoBuf.ZiplineMountable>();
		}
		info.msg.ziplineMountable.linePoints = Facepunch.Pool.GetList<VectorData>();
		foreach (Vector3 vector in this.linePoints)
		{
			info.msg.ziplineMountable.linePoints.Add(vector);
		}
	}

	// Token: 0x0600159C RID: 5532 RVA: 0x000AAD0C File Offset: 0x000A8F0C
	private void MovePlayerToPosition()
	{
		this.elapsedMoveTime += UnityEngine.Time.deltaTime;
		float num = Mathf.Clamp(this.elapsedMoveTime / this.MountEaseInTime, 0f, 1f);
		Vector3 vector = Vector3.Lerp(this.startPosition, this.endPosition, this.MountPositionCurve.Evaluate(num));
		Quaternion quaternion = Quaternion.Lerp(this.startRotation, this.endRotation, this.MountRotationCurve.Evaluate(num));
		base.transform.localPosition = vector;
		base.transform.localRotation = quaternion;
		if (num >= 1f)
		{
			this.isAnimatingIn = false;
			base.SetFlag(global::BaseEntity.Flags.Reserved2, true, false, true);
			this.mountTime = 0f;
			base.CancelInvoke(new Action(this.MovePlayerToPosition));
		}
	}

	// Token: 0x0600159D RID: 5533 RVA: 0x000AADDC File Offset: 0x000A8FDC
	public override void OnFlagsChanged(global::BaseEntity.Flags old, global::BaseEntity.Flags next)
	{
		base.OnFlagsChanged(old, next);
		if (base.isServer && old.HasFlag(global::BaseEntity.Flags.Busy) && !next.HasFlag(global::BaseEntity.Flags.Busy) && !base.IsDestroyed)
		{
			base.Kill(global::BaseNetworkable.DestroyMode.None);
		}
	}

	// Token: 0x04000DAD RID: 3501
	public float MoveSpeed = 4f;

	// Token: 0x04000DAE RID: 3502
	public float ForwardAdditive = 5f;

	// Token: 0x04000DAF RID: 3503
	public CapsuleCollider ZipCollider;

	// Token: 0x04000DB0 RID: 3504
	public Transform ZiplineGrabRoot;

	// Token: 0x04000DB1 RID: 3505
	public Transform LeftHandIkPoint;

	// Token: 0x04000DB2 RID: 3506
	public Transform RightHandIkPoint;

	// Token: 0x04000DB3 RID: 3507
	public float SpeedUpTime = 0.6f;

	// Token: 0x04000DB4 RID: 3508
	public bool EditorHoldInPlace;

	// Token: 0x04000DB5 RID: 3509
	private List<Vector3> linePoints;

	// Token: 0x04000DB6 RID: 3510
	private const global::BaseEntity.Flags PushForward = global::BaseEntity.Flags.Reserved1;

	// Token: 0x04000DB7 RID: 3511
	public AnimationCurve MountPositionCurve = AnimationCurve.Linear(0f, 0f, 1f, 1f);

	// Token: 0x04000DB8 RID: 3512
	public AnimationCurve MountRotationCurve = AnimationCurve.Linear(0f, 0f, 1f, 1f);

	// Token: 0x04000DB9 RID: 3513
	public float MountEaseInTime = 0.5f;

	// Token: 0x04000DBA RID: 3514
	private const global::BaseEntity.Flags ShowHandle = global::BaseEntity.Flags.Reserved2;

	// Token: 0x04000DBB RID: 3515
	private float additiveValue;

	// Token: 0x04000DBC RID: 3516
	private float currentTravelDistance;

	// Token: 0x04000DBD RID: 3517
	private TimeSince mountTime;

	// Token: 0x04000DBE RID: 3518
	private bool hasEnded;

	// Token: 0x04000DBF RID: 3519
	private List<Collider> ignoreColliders = new List<Collider>();

	// Token: 0x04000DC0 RID: 3520
	private Vector3 lastSafePosition;

	// Token: 0x04000DC1 RID: 3521
	private Vector3 startPosition = Vector3.zero;

	// Token: 0x04000DC2 RID: 3522
	private Vector3 endPosition = Vector3.zero;

	// Token: 0x04000DC3 RID: 3523
	private Quaternion startRotation = Quaternion.identity;

	// Token: 0x04000DC4 RID: 3524
	private Quaternion endRotation = Quaternion.identity;

	// Token: 0x04000DC5 RID: 3525
	private float elapsedMoveTime;

	// Token: 0x04000DC6 RID: 3526
	private bool isAnimatingIn;
}
