using System;
using UnityEngine;

// Token: 0x020001A7 RID: 423
public class BradleyMoveTest : MonoBehaviour
{
	// Token: 0x060018BD RID: 6333 RVA: 0x000B838C File Offset: 0x000B658C
	public void Awake()
	{
		this.Initialize();
	}

	// Token: 0x060018BE RID: 6334 RVA: 0x000B8394 File Offset: 0x000B6594
	public void Initialize()
	{
		this.myRigidBody.centerOfMass = this.centerOfMass.localPosition;
		this.destination = base.transform.position;
	}

	// Token: 0x060018BF RID: 6335 RVA: 0x000B83BD File Offset: 0x000B65BD
	public void SetDestination(Vector3 dest)
	{
		this.destination = dest;
	}

	// Token: 0x060018C0 RID: 6336 RVA: 0x000B83C8 File Offset: 0x000B65C8
	public void FixedUpdate()
	{
		Vector3 velocity = this.myRigidBody.velocity;
		this.SetDestination(this.followTest.transform.position);
		float num = Vector3.Distance(base.transform.position, this.destination);
		if (num > this.stoppingDist)
		{
			Vector3 zero = Vector3.zero;
			float num2 = Vector3.Dot(zero, base.transform.right);
			float num3 = Vector3.Dot(zero, -base.transform.right);
			float num4 = Vector3.Dot(zero, base.transform.right);
			if (Vector3.Dot(zero, -base.transform.forward) > num4)
			{
				if (num2 >= num3)
				{
					this.turning = 1f;
				}
				else
				{
					this.turning = -1f;
				}
			}
			else
			{
				this.turning = num4;
			}
			this.throttle = Mathf.InverseLerp(this.stoppingDist, 30f, num);
		}
		this.throttle = Mathf.Clamp(this.throttle, -1f, 1f);
		float num5 = this.throttle;
		float num6 = this.throttle;
		if (this.turning > 0f)
		{
			num6 = -this.turning;
			num5 = this.turning;
		}
		else if (this.turning < 0f)
		{
			num5 = this.turning;
			num6 = this.turning * -1f;
		}
		this.ApplyBrakes(this.brake ? 1f : 0f);
		float num7 = this.throttle;
		num5 = Mathf.Clamp(num5 + num7, -1f, 1f);
		num6 = Mathf.Clamp(num6 + num7, -1f, 1f);
		this.AdjustFriction();
		float num8 = Mathf.InverseLerp(3f, 1f, velocity.magnitude * Mathf.Abs(Vector3.Dot(velocity.normalized, base.transform.forward)));
		float num9 = Mathf.Lerp(this.moveForceMax, this.turnForce, num8);
		this.SetMotorTorque(num5, false, num9);
		this.SetMotorTorque(num6, true, num9);
	}

	// Token: 0x060018C1 RID: 6337 RVA: 0x000B85CB File Offset: 0x000B67CB
	public void ApplyBrakes(float amount)
	{
		this.ApplyBrakeTorque(amount, true);
		this.ApplyBrakeTorque(amount, false);
	}

	// Token: 0x060018C2 RID: 6338 RVA: 0x000B85E0 File Offset: 0x000B67E0
	public float GetMotorTorque(bool rightSide)
	{
		float num = 0f;
		foreach (WheelCollider wheelCollider in rightSide ? this.rightWheels : this.leftWheels)
		{
			num += wheelCollider.motorTorque;
		}
		return num / (float)this.rightWheels.Length;
	}

	// Token: 0x060018C3 RID: 6339 RVA: 0x000B8630 File Offset: 0x000B6830
	public void SetMotorTorque(float newThrottle, bool rightSide, float torqueAmount)
	{
		newThrottle = Mathf.Clamp(newThrottle, -1f, 1f);
		float num = torqueAmount * newThrottle;
		WheelCollider[] array = (rightSide ? this.rightWheels : this.leftWheels);
		for (int i = 0; i < array.Length; i++)
		{
			array[i].motorTorque = num;
		}
	}

	// Token: 0x060018C4 RID: 6340 RVA: 0x000B867C File Offset: 0x000B687C
	public void ApplyBrakeTorque(float amount, bool rightSide)
	{
		WheelCollider[] array = (rightSide ? this.rightWheels : this.leftWheels);
		for (int i = 0; i < array.Length; i++)
		{
			array[i].brakeTorque = this.brakeForce * amount;
		}
	}

	// Token: 0x060018C5 RID: 6341 RVA: 0x000063A5 File Offset: 0x000045A5
	public void AdjustFriction()
	{
	}

	// Token: 0x04001160 RID: 4448
	public WheelCollider[] leftWheels;

	// Token: 0x04001161 RID: 4449
	public WheelCollider[] rightWheels;

	// Token: 0x04001162 RID: 4450
	public float moveForceMax = 2000f;

	// Token: 0x04001163 RID: 4451
	public float brakeForce = 100f;

	// Token: 0x04001164 RID: 4452
	public float throttle = 1f;

	// Token: 0x04001165 RID: 4453
	public float turnForce = 2000f;

	// Token: 0x04001166 RID: 4454
	public float sideStiffnessMax = 1f;

	// Token: 0x04001167 RID: 4455
	public float sideStiffnessMin = 0.5f;

	// Token: 0x04001168 RID: 4456
	public Transform centerOfMass;

	// Token: 0x04001169 RID: 4457
	public float turning;

	// Token: 0x0400116A RID: 4458
	public bool brake;

	// Token: 0x0400116B RID: 4459
	public Rigidbody myRigidBody;

	// Token: 0x0400116C RID: 4460
	public Vector3 destination;

	// Token: 0x0400116D RID: 4461
	public float stoppingDist = 5f;

	// Token: 0x0400116E RID: 4462
	public GameObject followTest;
}
