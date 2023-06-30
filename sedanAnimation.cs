using System;
using UnityEngine;

// Token: 0x02000030 RID: 48
public class sedanAnimation : MonoBehaviour
{
	// Token: 0x06000133 RID: 307 RVA: 0x00007F46 File Offset: 0x00006146
	private void Start()
	{
		this.myRigidbody = base.GetComponent<Rigidbody>();
	}

	// Token: 0x06000134 RID: 308 RVA: 0x00007F54 File Offset: 0x00006154
	private void Update()
	{
		this.DoSteering();
		this.ApplyForceAtWheels();
		this.UpdateTireAnimation();
		this.InputPlayer();
	}

	// Token: 0x06000135 RID: 309 RVA: 0x00007F70 File Offset: 0x00006170
	private void InputPlayer()
	{
		if (Input.GetKey(KeyCode.W))
		{
			this.gasPedal = Mathf.Clamp(this.gasPedal + Time.deltaTime * this.GasLerpTime, -100f, 100f);
			this.brakePedal = Mathf.Lerp(this.brakePedal, 0f, Time.deltaTime * this.GasLerpTime);
		}
		else if (Input.GetKey(KeyCode.S))
		{
			this.gasPedal = Mathf.Clamp(this.gasPedal - Time.deltaTime * this.GasLerpTime, -100f, 100f);
			this.brakePedal = Mathf.Lerp(this.brakePedal, 0f, Time.deltaTime * this.GasLerpTime);
		}
		else
		{
			this.gasPedal = Mathf.Lerp(this.gasPedal, 0f, Time.deltaTime * this.GasLerpTime);
			this.brakePedal = Mathf.Lerp(this.brakePedal, 100f, Time.deltaTime * this.GasLerpTime / 5f);
		}
		if (Input.GetKey(KeyCode.A))
		{
			this.steering = Mathf.Clamp(this.steering - Time.deltaTime * this.SteeringLerpTime, -60f, 60f);
			return;
		}
		if (Input.GetKey(KeyCode.D))
		{
			this.steering = Mathf.Clamp(this.steering + Time.deltaTime * this.SteeringLerpTime, -60f, 60f);
			return;
		}
		this.steering = Mathf.Lerp(this.steering, 0f, Time.deltaTime * this.SteeringLerpTime);
	}

	// Token: 0x06000136 RID: 310 RVA: 0x000080FA File Offset: 0x000062FA
	private void DoSteering()
	{
		this.FL_wheelCollider.steerAngle = this.steering;
		this.FR_wheelCollider.steerAngle = this.steering;
	}

	// Token: 0x06000137 RID: 311 RVA: 0x00008120 File Offset: 0x00006320
	private void ApplyForceAtWheels()
	{
		if (this.FL_wheelCollider.isGrounded)
		{
			this.FL_wheelCollider.motorTorque = this.gasPedal * this.motorForceConstant;
			this.FL_wheelCollider.brakeTorque = this.brakePedal * this.brakeForceConstant;
		}
		if (this.FR_wheelCollider.isGrounded)
		{
			this.FR_wheelCollider.motorTorque = this.gasPedal * this.motorForceConstant;
			this.FR_wheelCollider.brakeTorque = this.brakePedal * this.brakeForceConstant;
		}
		if (this.RL_wheelCollider.isGrounded)
		{
			this.RL_wheelCollider.motorTorque = this.gasPedal * this.motorForceConstant;
			this.RL_wheelCollider.brakeTorque = this.brakePedal * this.brakeForceConstant;
		}
		if (this.RR_wheelCollider.isGrounded)
		{
			this.RR_wheelCollider.motorTorque = this.gasPedal * this.motorForceConstant;
			this.RR_wheelCollider.brakeTorque = this.brakePedal * this.brakeForceConstant;
		}
	}

	// Token: 0x06000138 RID: 312 RVA: 0x00008224 File Offset: 0x00006424
	private void UpdateTireAnimation()
	{
		float num = Vector3.Dot(this.myRigidbody.velocity, this.myRigidbody.transform.forward);
		if (this.FL_wheelCollider.isGrounded)
		{
			this.FL_shock.localPosition = new Vector3(this.FL_shock.localPosition.x, this.shockRestingPosY + this.GetShockHeightDelta(this.FL_wheelCollider), this.FL_shock.localPosition.z);
			this.FL_wheel.localEulerAngles = new Vector3(this.FL_wheel.localEulerAngles.x, this.FL_wheel.localEulerAngles.y, this.FL_wheel.localEulerAngles.z - num * Time.deltaTime * this.wheelSpinConstant);
		}
		else
		{
			this.FL_shock.localPosition = Vector3.Lerp(this.FL_shock.localPosition, new Vector3(this.FL_shock.localPosition.x, this.shockRestingPosY, this.FL_shock.localPosition.z), Time.deltaTime * 2f);
		}
		if (this.FR_wheelCollider.isGrounded)
		{
			this.FR_shock.localPosition = new Vector3(this.FR_shock.localPosition.x, this.shockRestingPosY + this.GetShockHeightDelta(this.FR_wheelCollider), this.FR_shock.localPosition.z);
			this.FR_wheel.localEulerAngles = new Vector3(this.FR_wheel.localEulerAngles.x, this.FR_wheel.localEulerAngles.y, this.FR_wheel.localEulerAngles.z - num * Time.deltaTime * this.wheelSpinConstant);
		}
		else
		{
			this.FR_shock.localPosition = Vector3.Lerp(this.FR_shock.localPosition, new Vector3(this.FR_shock.localPosition.x, this.shockRestingPosY, this.FR_shock.localPosition.z), Time.deltaTime * 2f);
		}
		if (this.RL_wheelCollider.isGrounded)
		{
			this.RL_shock.localPosition = new Vector3(this.RL_shock.localPosition.x, this.shockRestingPosY + this.GetShockHeightDelta(this.RL_wheelCollider), this.RL_shock.localPosition.z);
			this.RL_wheel.localEulerAngles = new Vector3(this.RL_wheel.localEulerAngles.x, this.RL_wheel.localEulerAngles.y, this.RL_wheel.localEulerAngles.z - num * Time.deltaTime * this.wheelSpinConstant);
		}
		else
		{
			this.RL_shock.localPosition = Vector3.Lerp(this.RL_shock.localPosition, new Vector3(this.RL_shock.localPosition.x, this.shockRestingPosY, this.RL_shock.localPosition.z), Time.deltaTime * 2f);
		}
		if (this.RR_wheelCollider.isGrounded)
		{
			this.RR_shock.localPosition = new Vector3(this.RR_shock.localPosition.x, this.shockRestingPosY + this.GetShockHeightDelta(this.RR_wheelCollider), this.RR_shock.localPosition.z);
			this.RR_wheel.localEulerAngles = new Vector3(this.RR_wheel.localEulerAngles.x, this.RR_wheel.localEulerAngles.y, this.RR_wheel.localEulerAngles.z - num * Time.deltaTime * this.wheelSpinConstant);
		}
		else
		{
			this.RR_shock.localPosition = Vector3.Lerp(this.RR_shock.localPosition, new Vector3(this.RR_shock.localPosition.x, this.shockRestingPosY, this.RR_shock.localPosition.z), Time.deltaTime * 2f);
		}
		foreach (Transform transform in this.frontAxles)
		{
			transform.localEulerAngles = new Vector3(this.steering, transform.localEulerAngles.y, transform.localEulerAngles.z);
		}
	}

	// Token: 0x06000139 RID: 313 RVA: 0x00008668 File Offset: 0x00006868
	private float GetShockHeightDelta(WheelCollider wheel)
	{
		int mask = LayerMask.GetMask(new string[] { "Terrain", "World", "Construction" });
		RaycastHit raycastHit;
		Physics.Linecast(wheel.transform.position, wheel.transform.position - Vector3.up * 10f, out raycastHit, mask);
		return Mathx.RemapValClamped(raycastHit.distance, this.traceDistanceNeutralPoint - this.shockDistance, this.traceDistanceNeutralPoint + this.shockDistance, this.shockDistance * 0.75f, -0.75f * this.shockDistance);
	}

	// Token: 0x0400016D RID: 365
	public Transform[] frontAxles;

	// Token: 0x0400016E RID: 366
	public Transform FL_shock;

	// Token: 0x0400016F RID: 367
	public Transform FL_wheel;

	// Token: 0x04000170 RID: 368
	public Transform FR_shock;

	// Token: 0x04000171 RID: 369
	public Transform FR_wheel;

	// Token: 0x04000172 RID: 370
	public Transform RL_shock;

	// Token: 0x04000173 RID: 371
	public Transform RL_wheel;

	// Token: 0x04000174 RID: 372
	public Transform RR_shock;

	// Token: 0x04000175 RID: 373
	public Transform RR_wheel;

	// Token: 0x04000176 RID: 374
	public WheelCollider FL_wheelCollider;

	// Token: 0x04000177 RID: 375
	public WheelCollider FR_wheelCollider;

	// Token: 0x04000178 RID: 376
	public WheelCollider RL_wheelCollider;

	// Token: 0x04000179 RID: 377
	public WheelCollider RR_wheelCollider;

	// Token: 0x0400017A RID: 378
	public Transform steeringWheel;

	// Token: 0x0400017B RID: 379
	public float motorForceConstant = 150f;

	// Token: 0x0400017C RID: 380
	public float brakeForceConstant = 500f;

	// Token: 0x0400017D RID: 381
	public float brakePedal;

	// Token: 0x0400017E RID: 382
	public float gasPedal;

	// Token: 0x0400017F RID: 383
	public float steering;

	// Token: 0x04000180 RID: 384
	private Rigidbody myRigidbody;

	// Token: 0x04000181 RID: 385
	public float GasLerpTime = 20f;

	// Token: 0x04000182 RID: 386
	public float SteeringLerpTime = 20f;

	// Token: 0x04000183 RID: 387
	private float wheelSpinConstant = 120f;

	// Token: 0x04000184 RID: 388
	private float shockRestingPosY = -0.27f;

	// Token: 0x04000185 RID: 389
	private float shockDistance = 0.3f;

	// Token: 0x04000186 RID: 390
	private float traceDistanceNeutralPoint = 0.7f;
}
