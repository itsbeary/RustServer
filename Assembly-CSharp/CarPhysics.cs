using System;
using System.Runtime.CompilerServices;
using UnityEngine;

// Token: 0x0200047D RID: 1149
public class CarPhysics<TCar> where TCar : BaseVehicle, CarPhysics<TCar>.ICar
{
	// Token: 0x17000317 RID: 791
	// (get) Token: 0x060025E2 RID: 9698 RVA: 0x000EF244 File Offset: 0x000ED444
	// (set) Token: 0x060025E3 RID: 9699 RVA: 0x000EF24C File Offset: 0x000ED44C
	public float DriveWheelVelocity { get; private set; }

	// Token: 0x17000318 RID: 792
	// (get) Token: 0x060025E4 RID: 9700 RVA: 0x000EF255 File Offset: 0x000ED455
	// (set) Token: 0x060025E5 RID: 9701 RVA: 0x000EF25D File Offset: 0x000ED45D
	public float DriveWheelSlip { get; private set; }

	// Token: 0x17000319 RID: 793
	// (get) Token: 0x060025E6 RID: 9702 RVA: 0x000EF266 File Offset: 0x000ED466
	// (set) Token: 0x060025E7 RID: 9703 RVA: 0x000EF26E File Offset: 0x000ED46E
	public float SteerAngle { get; private set; }

	// Token: 0x1700031A RID: 794
	// (get) Token: 0x060025E8 RID: 9704 RVA: 0x000EF277 File Offset: 0x000ED477
	// (set) Token: 0x060025E9 RID: 9705 RVA: 0x000EF27F File Offset: 0x000ED47F
	public float TankThrottleLeft { get; private set; }

	// Token: 0x1700031B RID: 795
	// (get) Token: 0x060025EA RID: 9706 RVA: 0x000EF288 File Offset: 0x000ED488
	// (set) Token: 0x060025EB RID: 9707 RVA: 0x000EF290 File Offset: 0x000ED490
	public float TankThrottleRight { get; private set; }

	// Token: 0x1700031C RID: 796
	// (get) Token: 0x060025EC RID: 9708 RVA: 0x000EF299 File Offset: 0x000ED499
	private bool InSlowSpeedExitMode
	{
		get
		{
			return !this.hasDriver && this.slowSpeedExitFlag;
		}
	}

	// Token: 0x060025ED RID: 9709 RVA: 0x000EF2AC File Offset: 0x000ED4AC
	public CarPhysics(TCar car, Transform transform, Rigidbody rBody, CarSettings vehicleSettings)
	{
		CarPhysics<TCar>.<>c__DisplayClass47_0 CS$<>8__locals1;
		CS$<>8__locals1.transform = transform;
		base..ctor();
		CS$<>8__locals1.<>4__this = this;
		this.car = car;
		this.transform = CS$<>8__locals1.transform;
		this.rBody = rBody;
		this.vehicleSettings = vehicleSettings;
		this.timeSinceWaterCheck = default(TimeSince);
		this.timeSinceWaterCheck = float.MaxValue;
		this.prevLocalCOM = rBody.centerOfMass;
		CarWheel[] wheels = car.GetWheels();
		this.wheelData = new CarPhysics<TCar>.ServerWheelData[wheels.Length];
		for (int i = 0; i < this.wheelData.Length; i++)
		{
			this.wheelData[i] = this.<.ctor>g__AddWheel|47_0(wheels[i], ref CS$<>8__locals1);
		}
		this.midWheelPos = car.GetWheelsMidPos();
		this.wheelData[0].wheel.wheelCollider.ConfigureVehicleSubsteps(1000f, 1, 1);
		this.lastMovingTime = Time.realtimeSinceStartup;
	}

	// Token: 0x060025EE RID: 9710 RVA: 0x000EF3C0 File Offset: 0x000ED5C0
	public void FixedUpdate(float dt, float speed)
	{
		if (this.rBody.centerOfMass != this.prevLocalCOM)
		{
			this.COMChanged();
		}
		float num = Mathf.Abs(speed);
		this.hasDriver = this.car.HasDriver();
		if (!this.hasDriver && this.hadDriver)
		{
			if (num <= 4f)
			{
				this.slowSpeedExitFlag = true;
			}
		}
		else if (this.hasDriver && !this.hadDriver)
		{
			this.slowSpeedExitFlag = false;
		}
		if ((this.hasDriver || !this.vehicleSettings.canSleep) && this.rBody.IsSleeping())
		{
			this.rBody.WakeUp();
		}
		if (!this.rBody.IsSleeping())
		{
			if ((this.wasSleeping && !this.rBody.isKinematic) || num > 0.25f || Mathf.Abs(this.rBody.angularVelocity.magnitude) > 0.25f)
			{
				this.lastMovingTime = Time.time;
			}
			bool flag = this.vehicleSettings.canSleep && !this.hasDriver && Time.time > this.lastMovingTime + 10f;
			if (flag && (this.car.GetParentEntity() as BaseVehicle).IsValid())
			{
				flag = false;
			}
			if (flag)
			{
				for (int i = 0; i < this.wheelData.Length; i++)
				{
					CarPhysics<TCar>.ServerWheelData serverWheelData = this.wheelData[i];
					serverWheelData.wheelCollider.motorTorque = 0f;
					serverWheelData.wheelCollider.brakeTorque = 0f;
					serverWheelData.wheelCollider.steerAngle = 0f;
				}
				this.rBody.Sleep();
			}
			else
			{
				this.speedAngle = Vector3.Angle(this.rBody.velocity, this.transform.forward) * Mathf.Sign(Vector3.Dot(this.rBody.velocity, this.transform.right));
				float num2 = this.car.GetMaxDriveForce();
				float maxForwardSpeed = this.car.GetMaxForwardSpeed();
				float num3 = (this.car.IsOn() ? this.car.GetThrottleInput() : 0f);
				float steerInput = this.car.GetSteerInput();
				float num4 = (this.InSlowSpeedExitMode ? 1f : this.car.GetBrakeInput());
				float num5 = 1f;
				if (num < 3f)
				{
					num5 = 2.75f;
				}
				else if (num < 9f)
				{
					float num6 = Mathf.InverseLerp(9f, 3f, num);
					num5 = Mathf.Lerp(1f, 2.75f, num6);
				}
				num2 *= num5;
				this.ComputeSteerAngle(num3, steerInput, dt, speed);
				if (this.timeSinceWaterCheck > 0.25f)
				{
					float num7 = this.car.WaterFactor();
					float num8 = 0f;
					TriggerVehicleDrag triggerVehicleDrag;
					if (this.car.FindTrigger<TriggerVehicleDrag>(out triggerVehicleDrag))
					{
						num8 = triggerVehicleDrag.vehicleDrag;
					}
					float num9 = ((num3 != 0f) ? 0f : 0.25f);
					float num10 = Mathf.Max(num7, num8);
					num10 = Mathf.Max(num10, this.car.GetModifiedDrag());
					this.rBody.drag = Mathf.Max(num9, num10);
					this.rBody.angularDrag = num10 * 0.5f;
					this.timeSinceWaterCheck = 0f;
				}
				int num11 = 0;
				float num12 = 0f;
				bool flag2 = !this.hasDriver && this.rBody.velocity.magnitude < 2.5f && this.car.timeSinceLastPush > 2f;
				for (int j = 0; j < this.wheelData.Length; j++)
				{
					CarPhysics<TCar>.ServerWheelData serverWheelData2 = this.wheelData[j];
					serverWheelData2.wheelCollider.motorTorque = 1E-05f;
					if (flag2 && this.car.OnSurface != VehicleTerrainHandler.Surface.Frictionless)
					{
						serverWheelData2.wheelCollider.brakeTorque = 10000f;
					}
					else
					{
						serverWheelData2.wheelCollider.brakeTorque = 0f;
					}
					if (serverWheelData2.wheel.steerWheel)
					{
						serverWheelData2.wheel.wheelCollider.steerAngle = (serverWheelData2.isFrontWheel ? this.SteerAngle : (this.vehicleSettings.rearWheelSteer * -this.SteerAngle));
					}
					this.UpdateSuspension(serverWheelData2);
					if (serverWheelData2.isGrounded)
					{
						num11++;
						num12 += this.wheelData[j].downforce;
					}
				}
				this.AdjustHitForces(num11, num12 / (float)num11);
				for (int k = 0; k < this.wheelData.Length; k++)
				{
					CarPhysics<TCar>.ServerWheelData serverWheelData3 = this.wheelData[k];
					this.UpdateLocalFrame(serverWheelData3, dt);
					this.ComputeTyreForces(serverWheelData3, speed, num2, maxForwardSpeed, num3, steerInput, num4, num5);
					this.ApplyTyreForces(serverWheelData3, num3, steerInput, speed);
				}
				this.ComputeOverallForces();
			}
			this.wasSleeping = false;
		}
		else
		{
			this.wasSleeping = true;
		}
		this.hadDriver = this.hasDriver;
	}

	// Token: 0x060025EF RID: 9711 RVA: 0x000EF8F0 File Offset: 0x000EDAF0
	public bool IsGrounded()
	{
		int num = 0;
		for (int i = 0; i < this.wheelData.Length; i++)
		{
			if (this.wheelData[i].isGrounded)
			{
				num++;
			}
			if (num >= 2)
			{
				return true;
			}
		}
		return false;
	}

	// Token: 0x060025F0 RID: 9712 RVA: 0x000EF92C File Offset: 0x000EDB2C
	private void COMChanged()
	{
		for (int i = 0; i < this.wheelData.Length; i++)
		{
			CarPhysics<TCar>.ServerWheelData serverWheelData = this.wheelData[i];
			serverWheelData.forceDistance = this.GetWheelForceDistance(serverWheelData.wheel.wheelCollider);
		}
		this.prevLocalCOM = this.rBody.centerOfMass;
	}

	// Token: 0x060025F1 RID: 9713 RVA: 0x000EF980 File Offset: 0x000EDB80
	private void ComputeSteerAngle(float throttleInput, float steerInput, float dt, float speed)
	{
		if (this.vehicleSettings.tankSteering)
		{
			this.SteerAngle = 0f;
			this.ComputeTankSteeringThrottle(throttleInput, steerInput, speed);
			return;
		}
		float num = this.vehicleSettings.maxSteerAngle * steerInput;
		float num2 = Mathf.InverseLerp(0f, this.vehicleSettings.minSteerLimitSpeed, speed);
		if (this.vehicleSettings.steeringLimit)
		{
			float num3 = Mathf.Lerp(this.vehicleSettings.maxSteerAngle, this.vehicleSettings.minSteerLimitAngle, num2);
			num = Mathf.Clamp(num, -num3, num3);
		}
		float num4 = 0f;
		if (this.vehicleSettings.steeringAssist)
		{
			float num5 = Mathf.InverseLerp(0.1f, 3f, speed);
			num4 = this.speedAngle * this.vehicleSettings.steeringAssistRatio * num5 * Mathf.InverseLerp(2f, 3f, Mathf.Abs(this.speedAngle));
		}
		float num6 = Mathf.Clamp(num + num4, -this.vehicleSettings.maxSteerAngle, this.vehicleSettings.maxSteerAngle);
		if (this.SteerAngle != num6)
		{
			float num7 = 1f - num2 * 0.7f;
			float num9;
			if ((this.SteerAngle == 0f || Mathf.Sign(num6) == Mathf.Sign(this.SteerAngle)) && Mathf.Abs(num6) > Mathf.Abs(this.SteerAngle))
			{
				float num8 = this.SteerAngle / this.vehicleSettings.maxSteerAngle;
				num9 = Mathf.Lerp(this.vehicleSettings.steerMinLerpSpeed * num7, this.vehicleSettings.steerMaxLerpSpeed * num7, num8 * num8);
			}
			else
			{
				num9 = this.vehicleSettings.steerReturnLerpSpeed * num7;
			}
			if (this.car.GetSteerModInput())
			{
				num9 *= 1.5f;
			}
			this.SteerAngle = Mathf.MoveTowards(this.SteerAngle, num6, dt * num9);
		}
	}

	// Token: 0x060025F2 RID: 9714 RVA: 0x000EFB54 File Offset: 0x000EDD54
	private float GetWheelForceDistance(WheelCollider col)
	{
		return this.rBody.centerOfMass.y - this.transform.InverseTransformPoint(col.transform.position).y + col.radius + (1f - col.suspensionSpring.targetPosition) * col.suspensionDistance;
	}

	// Token: 0x060025F3 RID: 9715 RVA: 0x000EFBB0 File Offset: 0x000EDDB0
	private void UpdateSuspension(CarPhysics<TCar>.ServerWheelData wd)
	{
		wd.isGrounded = wd.wheelCollider.GetGroundHit(out wd.hit);
		wd.origin = wd.wheelColliderTransform.TransformPoint(wd.wheelCollider.center);
		RaycastHit raycastHit;
		if (wd.isGrounded && GamePhysics.Trace(new Ray(wd.origin, -wd.wheelColliderTransform.up), 0f, out raycastHit, wd.wheelCollider.suspensionDistance + wd.wheelCollider.radius, 1235321089, QueryTriggerInteraction.Ignore, null))
		{
			wd.hit.point = raycastHit.point;
			wd.hit.normal = raycastHit.normal;
		}
		if (wd.isGrounded)
		{
			if (wd.hit.force < 0f)
			{
				wd.hit.force = 0f;
			}
			wd.downforce = wd.hit.force;
			return;
		}
		wd.downforce = 0f;
	}

	// Token: 0x060025F4 RID: 9716 RVA: 0x000EFCAC File Offset: 0x000EDEAC
	private void AdjustHitForces(int groundedWheels, float neutralForcePerWheel)
	{
		float num = neutralForcePerWheel * 0.25f;
		for (int i = 0; i < this.wheelData.Length; i++)
		{
			CarPhysics<TCar>.ServerWheelData serverWheelData = this.wheelData[i];
			if (serverWheelData.isGrounded && serverWheelData.downforce < num)
			{
				if (groundedWheels == 1)
				{
					serverWheelData.downforce = num;
				}
				else
				{
					float num2 = (num - serverWheelData.downforce) / (float)(groundedWheels - 1);
					serverWheelData.downforce = num;
					for (int j = 0; j < this.wheelData.Length; j++)
					{
						CarPhysics<TCar>.ServerWheelData serverWheelData2 = this.wheelData[j];
						if (serverWheelData2.isGrounded && serverWheelData2.downforce > num)
						{
							float num3 = Mathf.Min(num2, serverWheelData2.downforce - num);
							serverWheelData2.downforce -= num3;
						}
					}
				}
			}
		}
	}

	// Token: 0x060025F5 RID: 9717 RVA: 0x000EFD70 File Offset: 0x000EDF70
	private void UpdateLocalFrame(CarPhysics<TCar>.ServerWheelData wd, float dt)
	{
		if (!wd.isGrounded)
		{
			wd.hit.point = wd.origin - wd.wheelColliderTransform.up * (wd.wheelCollider.suspensionDistance + wd.wheelCollider.radius);
			wd.hit.normal = wd.wheelColliderTransform.up;
			wd.hit.collider = null;
		}
		Vector3 pointVelocity = this.rBody.GetPointVelocity(wd.hit.point);
		wd.velocity = pointVelocity - Vector3.Project(pointVelocity, wd.hit.normal);
		wd.localVelocity.y = Vector3.Dot(wd.hit.forwardDir, wd.velocity);
		wd.localVelocity.x = Vector3.Dot(wd.hit.sidewaysDir, wd.velocity);
		if (!wd.isGrounded)
		{
			wd.localRigForce = Vector2.zero;
			return;
		}
		float num = Mathf.InverseLerp(1f, 0.25f, wd.velocity.sqrMagnitude);
		Vector2 vector3;
		if (num > 0f)
		{
			float num2 = Vector3.Dot(Vector3.up, wd.hit.normal);
			Vector3 vector2;
			if (num2 > 1E-06f)
			{
				Vector3 vector = Vector3.up * wd.downforce / num2;
				vector2 = vector - Vector3.Project(vector, wd.hit.normal);
			}
			else
			{
				vector2 = Vector3.up * 100000f;
			}
			vector3.y = Vector3.Dot(wd.hit.forwardDir, vector2);
			vector3.x = Vector3.Dot(wd.hit.sidewaysDir, vector2);
			vector3 *= num;
		}
		else
		{
			vector3 = Vector2.zero;
		}
		Vector2 vector4 = -(Mathf.Clamp(wd.downforce / -Physics.gravity.y, 0f, wd.wheelCollider.sprungMass) * 0.5f) * wd.localVelocity / dt;
		wd.localRigForce = vector4 + vector3;
	}

	// Token: 0x060025F6 RID: 9718 RVA: 0x000EFF88 File Offset: 0x000EE188
	private void ComputeTyreForces(CarPhysics<TCar>.ServerWheelData wd, float speed, float maxDriveForce, float maxSpeed, float throttleInput, float steerInput, float brakeInput, float driveForceMultiplier)
	{
		float num = Mathf.Abs(speed);
		if (this.vehicleSettings.tankSteering && brakeInput == 0f)
		{
			if (wd.isLeftWheel)
			{
				throttleInput = this.TankThrottleLeft;
			}
			else
			{
				throttleInput = this.TankThrottleRight;
			}
		}
		float num2 = (wd.wheel.powerWheel ? throttleInput : 0f);
		wd.hasThrottleInput = num2 != 0f;
		float num3 = this.vehicleSettings.maxDriveSlip;
		if (Mathf.Sign(num2) != Mathf.Sign(wd.localVelocity.y))
		{
			num3 -= wd.localVelocity.y * Mathf.Sign(num2);
		}
		float num4 = Mathf.Abs(num2);
		float num5 = -this.vehicleSettings.rollingResistance + num4 * (1f + this.vehicleSettings.rollingResistance) - brakeInput * (1f - this.vehicleSettings.rollingResistance);
		if (this.InSlowSpeedExitMode || num5 < 0f || maxDriveForce == 0f)
		{
			num5 *= -1f;
			wd.isBraking = true;
		}
		else
		{
			num5 *= Mathf.Sign(num2);
			wd.isBraking = false;
		}
		float num7;
		if (wd.isBraking)
		{
			float num6 = Mathf.Clamp(this.car.GetMaxForwardSpeed() * this.vehicleSettings.brakeForceMultiplier, 10f * this.vehicleSettings.brakeForceMultiplier, 50f * this.vehicleSettings.brakeForceMultiplier);
			num6 += this.rBody.mass * 1.5f;
			num7 = num5 * num6;
		}
		else
		{
			num7 = this.ComputeDriveForce(speed, num, num5 * maxDriveForce, maxDriveForce, maxSpeed, driveForceMultiplier);
		}
		if (wd.isGrounded)
		{
			wd.tyreSlip.x = wd.localVelocity.x;
			wd.tyreSlip.y = wd.localVelocity.y - wd.angularVelocity * wd.wheelCollider.radius;
			float num8;
			switch (this.car.OnSurface)
			{
			case VehicleTerrainHandler.Surface.Road:
				num8 = 1f;
				goto IL_230;
			case VehicleTerrainHandler.Surface.Ice:
				num8 = 0.25f;
				goto IL_230;
			case VehicleTerrainHandler.Surface.Frictionless:
				num8 = 0f;
				goto IL_230;
			}
			num8 = 0.75f;
			IL_230:
			float num9 = wd.wheel.tyreFriction * wd.downforce * num8;
			float num10 = 0f;
			if (!wd.isBraking)
			{
				num10 = Mathf.Min(Mathf.Abs(num7 * wd.tyreSlip.x) / num9, num3);
				if (num7 != 0f && num10 < 0.1f)
				{
					num10 = 0.1f;
				}
			}
			if (Mathf.Abs(wd.tyreSlip.y) < num10)
			{
				wd.tyreSlip.y = num10 * Mathf.Sign(wd.tyreSlip.y);
			}
			Vector2 vector = -num9 * wd.tyreSlip.normalized;
			vector.x = Mathf.Abs(vector.x) * 1.5f;
			vector.y = Mathf.Abs(vector.y);
			wd.tyreForce.x = Mathf.Clamp(wd.localRigForce.x, -vector.x, vector.x);
			if (wd.isBraking)
			{
				float num11 = Mathf.Min(vector.y, num7);
				wd.tyreForce.y = Mathf.Clamp(wd.localRigForce.y, -num11, num11);
			}
			else
			{
				wd.tyreForce.y = Mathf.Clamp(num7, -vector.y, vector.y);
			}
		}
		else
		{
			wd.tyreSlip = Vector2.zero;
			wd.tyreForce = Vector2.zero;
		}
		if (wd.isGrounded)
		{
			float num12;
			if (wd.isBraking)
			{
				num12 = 0f;
			}
			else
			{
				float driveForceToMaxSlip = this.vehicleSettings.driveForceToMaxSlip;
				num12 = Mathf.Clamp01((Mathf.Abs(num7) - Mathf.Abs(wd.tyreForce.y)) / driveForceToMaxSlip) * num3 * Mathf.Sign(num7);
			}
			wd.angularVelocity = (wd.localVelocity.y + num12) / wd.wheelCollider.radius;
			return;
		}
		float num13 = 50f;
		float num14 = 10f;
		if (num2 > 0f)
		{
			wd.angularVelocity += num13 * num2;
		}
		else
		{
			wd.angularVelocity -= num14;
		}
		wd.angularVelocity -= num13 * brakeInput;
		wd.angularVelocity = Mathf.Clamp(wd.angularVelocity, 0f, maxSpeed / wd.wheelCollider.radius);
	}

	// Token: 0x060025F7 RID: 9719 RVA: 0x000F0414 File Offset: 0x000EE614
	private void ComputeTankSteeringThrottle(float throttleInput, float steerInput, float speed)
	{
		this.TankThrottleLeft = throttleInput;
		this.TankThrottleRight = throttleInput;
		float tankSteerInvert = this.GetTankSteerInvert(throttleInput, speed);
		if (throttleInput == 0f)
		{
			this.TankThrottleLeft = -steerInput;
			this.TankThrottleRight = steerInput;
			return;
		}
		if (steerInput > 0f)
		{
			this.TankThrottleLeft = Mathf.Lerp(throttleInput, -1f * tankSteerInvert, steerInput);
			this.TankThrottleRight = Mathf.Lerp(throttleInput, 1f * tankSteerInvert, steerInput);
			return;
		}
		if (steerInput < 0f)
		{
			this.TankThrottleLeft = Mathf.Lerp(throttleInput, 1f * tankSteerInvert, -steerInput);
			this.TankThrottleRight = Mathf.Lerp(throttleInput, -1f * tankSteerInvert, -steerInput);
		}
	}

	// Token: 0x060025F8 RID: 9720 RVA: 0x000F04B4 File Offset: 0x000EE6B4
	private float ComputeDriveForce(float speed, float absSpeed, float demandedForce, float maxForce, float maxForwardSpeed, float driveForceMultiplier)
	{
		float num = ((speed >= 0f) ? maxForwardSpeed : (maxForwardSpeed * this.vehicleSettings.reversePercentSpeed));
		if (absSpeed < num)
		{
			if ((speed >= 0f || demandedForce <= 0f) && (speed <= 0f || demandedForce >= 0f))
			{
				maxForce = this.car.GetAdjustedDriveForce(absSpeed, maxForwardSpeed) * driveForceMultiplier;
			}
			return Mathf.Clamp(demandedForce, -maxForce, maxForce);
		}
		float num2 = maxForce * Mathf.Max(1f - absSpeed / num, -1f) * Mathf.Sign(speed);
		if ((speed < 0f && demandedForce > 0f) || (speed > 0f && demandedForce < 0f))
		{
			num2 = Mathf.Clamp(num2 + demandedForce, -maxForce, maxForce);
		}
		return num2;
	}

	// Token: 0x060025F9 RID: 9721 RVA: 0x000F0574 File Offset: 0x000EE774
	private void ComputeOverallForces()
	{
		this.DriveWheelVelocity = 0f;
		this.DriveWheelSlip = 0f;
		int num = 0;
		for (int i = 0; i < this.wheelData.Length; i++)
		{
			CarPhysics<TCar>.ServerWheelData serverWheelData = this.wheelData[i];
			if (serverWheelData.wheel.powerWheel)
			{
				this.DriveWheelVelocity += serverWheelData.angularVelocity;
				if (serverWheelData.isGrounded)
				{
					float num2 = CarPhysics<TCar>.ComputeCombinedSlip(serverWheelData.localVelocity, serverWheelData.tyreSlip);
					this.DriveWheelSlip += num2;
				}
				num++;
			}
		}
		if (num > 0)
		{
			this.DriveWheelVelocity /= (float)num;
			this.DriveWheelSlip /= (float)num;
		}
	}

	// Token: 0x060025FA RID: 9722 RVA: 0x000F0624 File Offset: 0x000EE824
	private static float ComputeCombinedSlip(Vector2 localVelocity, Vector2 tyreSlip)
	{
		float magnitude = localVelocity.magnitude;
		if (magnitude > 0.01f)
		{
			float num = tyreSlip.x * localVelocity.x / magnitude;
			float y = tyreSlip.y;
			return Mathf.Sqrt(num * num + y * y);
		}
		return tyreSlip.magnitude;
	}

	// Token: 0x060025FB RID: 9723 RVA: 0x000F066C File Offset: 0x000EE86C
	private void ApplyTyreForces(CarPhysics<TCar>.ServerWheelData wd, float throttleInput, float steerInput, float speed)
	{
		if (wd.isGrounded)
		{
			Vector3 vector = wd.hit.forwardDir * wd.tyreForce.y;
			Vector3 vector2 = wd.hit.sidewaysDir * wd.tyreForce.x;
			Vector3 sidewaysForceAppPoint = this.GetSidewaysForceAppPoint(wd, wd.hit.point);
			this.rBody.AddForceAtPosition(vector, wd.hit.point, ForceMode.Force);
			this.rBody.AddForceAtPosition(vector2, sidewaysForceAppPoint, ForceMode.Force);
		}
	}

	// Token: 0x060025FC RID: 9724 RVA: 0x000F06F4 File Offset: 0x000EE8F4
	private Vector3 GetSidewaysForceAppPoint(CarPhysics<TCar>.ServerWheelData wd, Vector3 contactPoint)
	{
		Vector3 vector = contactPoint + wd.wheelColliderTransform.up * this.vehicleSettings.antiRoll * wd.forceDistance;
		float num = (wd.wheel.steerWheel ? this.SteerAngle : 0f);
		if (num != 0f && Mathf.Sign(num) != Mathf.Sign(wd.tyreSlip.x))
		{
			vector += wd.wheelColliderTransform.forward * this.midWheelPos * (this.vehicleSettings.handlingBias - 0.5f);
		}
		return vector;
	}

	// Token: 0x060025FD RID: 9725 RVA: 0x000F07A0 File Offset: 0x000EE9A0
	private float GetTankSteerInvert(float throttleInput, float speed)
	{
		float num = 1f;
		if (throttleInput < 0f && speed < 1.75f)
		{
			num = -1f;
		}
		else if (throttleInput == 0f && speed < -1f)
		{
			num = -1f;
		}
		else if (speed < -1f)
		{
			num = -1f;
		}
		return num;
	}

	// Token: 0x060025FE RID: 9726 RVA: 0x000F07F4 File Offset: 0x000EE9F4
	[CompilerGenerated]
	private CarPhysics<TCar>.ServerWheelData <.ctor>g__AddWheel|47_0(CarWheel wheel, ref CarPhysics<TCar>.<>c__DisplayClass47_0 A_2)
	{
		CarPhysics<TCar>.ServerWheelData serverWheelData = new CarPhysics<TCar>.ServerWheelData();
		serverWheelData.wheelCollider = wheel.wheelCollider;
		serverWheelData.wheelColliderTransform = wheel.wheelCollider.transform;
		serverWheelData.forceDistance = this.GetWheelForceDistance(wheel.wheelCollider);
		serverWheelData.wheel = wheel;
		serverWheelData.wheelCollider.sidewaysFriction = this.zeroFriction;
		serverWheelData.wheelCollider.forwardFriction = this.zeroFriction;
		Vector3 vector = A_2.transform.InverseTransformPoint(wheel.wheelCollider.transform.position);
		serverWheelData.isFrontWheel = vector.z > 0f;
		serverWheelData.isLeftWheel = vector.x < 0f;
		return serverWheelData;
	}

	// Token: 0x04001E54 RID: 7764
	private readonly CarPhysics<TCar>.ServerWheelData[] wheelData;

	// Token: 0x04001E55 RID: 7765
	private readonly TCar car;

	// Token: 0x04001E56 RID: 7766
	private readonly Transform transform;

	// Token: 0x04001E57 RID: 7767
	private readonly Rigidbody rBody;

	// Token: 0x04001E58 RID: 7768
	private readonly CarSettings vehicleSettings;

	// Token: 0x04001E59 RID: 7769
	private float speedAngle;

	// Token: 0x04001E5A RID: 7770
	private bool wasSleeping = true;

	// Token: 0x04001E5B RID: 7771
	private bool hasDriver;

	// Token: 0x04001E5C RID: 7772
	private bool hadDriver;

	// Token: 0x04001E5D RID: 7773
	private float lastMovingTime = float.MinValue;

	// Token: 0x04001E5E RID: 7774
	private WheelFrictionCurve zeroFriction = new WheelFrictionCurve
	{
		stiffness = 0f
	};

	// Token: 0x04001E5F RID: 7775
	private Vector3 prevLocalCOM;

	// Token: 0x04001E60 RID: 7776
	private readonly float midWheelPos;

	// Token: 0x04001E61 RID: 7777
	private const bool WHEEL_HIT_CORRECTION = true;

	// Token: 0x04001E62 RID: 7778
	private const float SLEEP_SPEED = 0.25f;

	// Token: 0x04001E63 RID: 7779
	private const float SLEEP_DELAY = 10f;

	// Token: 0x04001E64 RID: 7780
	private const float AIR_DRAG = 0.25f;

	// Token: 0x04001E65 RID: 7781
	private const float DEFAULT_GROUND_GRIP = 0.75f;

	// Token: 0x04001E66 RID: 7782
	private const float ROAD_GROUND_GRIP = 1f;

	// Token: 0x04001E67 RID: 7783
	private const float ICE_GROUND_GRIP = 0.25f;

	// Token: 0x04001E68 RID: 7784
	private bool slowSpeedExitFlag;

	// Token: 0x04001E69 RID: 7785
	private const float SLOW_SPEED_EXIT_SPEED = 4f;

	// Token: 0x04001E6A RID: 7786
	private TimeSince timeSinceWaterCheck;

	// Token: 0x02000D12 RID: 3346
	public interface ICar
	{
		// Token: 0x170006AE RID: 1710
		// (get) Token: 0x0600503C RID: 20540
		VehicleTerrainHandler.Surface OnSurface { get; }

		// Token: 0x0600503D RID: 20541
		float GetThrottleInput();

		// Token: 0x0600503E RID: 20542
		float GetBrakeInput();

		// Token: 0x0600503F RID: 20543
		float GetSteerInput();

		// Token: 0x06005040 RID: 20544
		bool GetSteerModInput();

		// Token: 0x06005041 RID: 20545
		float GetMaxForwardSpeed();

		// Token: 0x06005042 RID: 20546
		float GetMaxDriveForce();

		// Token: 0x06005043 RID: 20547
		float GetAdjustedDriveForce(float absSpeed, float topSpeed);

		// Token: 0x06005044 RID: 20548
		float GetModifiedDrag();

		// Token: 0x06005045 RID: 20549
		CarWheel[] GetWheels();

		// Token: 0x06005046 RID: 20550
		float GetWheelsMidPos();
	}

	// Token: 0x02000D13 RID: 3347
	private class ServerWheelData
	{
		// Token: 0x040046A4 RID: 18084
		public CarWheel wheel;

		// Token: 0x040046A5 RID: 18085
		public Transform wheelColliderTransform;

		// Token: 0x040046A6 RID: 18086
		public WheelCollider wheelCollider;

		// Token: 0x040046A7 RID: 18087
		public bool isGrounded;

		// Token: 0x040046A8 RID: 18088
		public float downforce;

		// Token: 0x040046A9 RID: 18089
		public float forceDistance;

		// Token: 0x040046AA RID: 18090
		public WheelHit hit;

		// Token: 0x040046AB RID: 18091
		public Vector2 localRigForce;

		// Token: 0x040046AC RID: 18092
		public Vector2 localVelocity;

		// Token: 0x040046AD RID: 18093
		public float angularVelocity;

		// Token: 0x040046AE RID: 18094
		public Vector3 origin;

		// Token: 0x040046AF RID: 18095
		public Vector2 tyreForce;

		// Token: 0x040046B0 RID: 18096
		public Vector2 tyreSlip;

		// Token: 0x040046B1 RID: 18097
		public Vector3 velocity;

		// Token: 0x040046B2 RID: 18098
		public bool isBraking;

		// Token: 0x040046B3 RID: 18099
		public bool hasThrottleInput;

		// Token: 0x040046B4 RID: 18100
		public bool isFrontWheel;

		// Token: 0x040046B5 RID: 18101
		public bool isLeftWheel;
	}
}
