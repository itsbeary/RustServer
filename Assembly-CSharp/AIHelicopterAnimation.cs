using System;
using UnityEngine;

// Token: 0x0200041F RID: 1055
public class AIHelicopterAnimation : MonoBehaviour
{
	// Token: 0x060023B1 RID: 9137 RVA: 0x000E3F04 File Offset: 0x000E2104
	public void Awake()
	{
		this.lastPosition = base.transform.position;
	}

	// Token: 0x060023B2 RID: 9138 RVA: 0x000E3F17 File Offset: 0x000E2117
	public Vector3 GetMoveDirection()
	{
		return this._ai.GetMoveDirection();
	}

	// Token: 0x060023B3 RID: 9139 RVA: 0x000E3F24 File Offset: 0x000E2124
	public float GetMoveSpeed()
	{
		return this._ai.GetMoveSpeed();
	}

	// Token: 0x060023B4 RID: 9140 RVA: 0x000E3F34 File Offset: 0x000E2134
	public void Update()
	{
		this.lastPosition = base.transform.position;
		Vector3 moveDirection = this.GetMoveDirection();
		float moveSpeed = this.GetMoveSpeed();
		float num = 0.25f + Mathf.Clamp01(moveSpeed / this._ai.maxSpeed) * 0.75f;
		this.smoothRateOfChange = Mathf.Lerp(this.smoothRateOfChange, moveSpeed - this.oldMoveSpeed, Time.deltaTime * 5f);
		this.oldMoveSpeed = moveSpeed;
		float num2 = Vector3.Angle(moveDirection, base.transform.forward);
		float num3 = Vector3.Angle(moveDirection, -base.transform.forward);
		float num4 = 1f - Mathf.Clamp01(num2 / this.degreeMax);
		float num5 = 1f - Mathf.Clamp01(num3 / this.degreeMax);
		float num6 = (num4 - num5) * num;
		float num7 = Mathf.Lerp(this.lastForwardBackScalar, num6, Time.deltaTime * 2f);
		this.lastForwardBackScalar = num7;
		float num8 = Vector3.Angle(moveDirection, base.transform.right);
		float num9 = Vector3.Angle(moveDirection, -base.transform.right);
		float num10 = 1f - Mathf.Clamp01(num8 / this.degreeMax);
		float num11 = 1f - Mathf.Clamp01(num9 / this.degreeMax);
		float num12 = (num10 - num11) * num;
		float num13 = Mathf.Lerp(this.lastStrafeScalar, num12, Time.deltaTime * 2f);
		this.lastStrafeScalar = num13;
		Vector3 zero = Vector3.zero;
		zero.x += num7 * this.swayAmount;
		zero.z -= num13 * this.swayAmount;
		Quaternion quaternion = Quaternion.identity;
		quaternion = Quaternion.Euler(zero.x, zero.y, zero.z);
		this._ai.helicopterBase.rotorPivot.transform.localRotation = quaternion;
	}

	// Token: 0x04001BBB RID: 7099
	public PatrolHelicopterAI _ai;

	// Token: 0x04001BBC RID: 7100
	public float swayAmount = 1f;

	// Token: 0x04001BBD RID: 7101
	public float lastStrafeScalar;

	// Token: 0x04001BBE RID: 7102
	public float lastForwardBackScalar;

	// Token: 0x04001BBF RID: 7103
	public float degreeMax = 90f;

	// Token: 0x04001BC0 RID: 7104
	public Vector3 lastPosition = Vector3.zero;

	// Token: 0x04001BC1 RID: 7105
	public float oldMoveSpeed;

	// Token: 0x04001BC2 RID: 7106
	public float smoothRateOfChange;

	// Token: 0x04001BC3 RID: 7107
	public float flareAmount;
}
