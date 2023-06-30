using System;
using UnityEngine;

// Token: 0x02000473 RID: 1139
public class BasicCar : BaseVehicle
{
	// Token: 0x060025A6 RID: 9638 RVA: 0x000ED499 File Offset: 0x000EB699
	public override float MaxVelocity()
	{
		return 50f;
	}

	// Token: 0x060025A7 RID: 9639 RVA: 0x000ED4A0 File Offset: 0x000EB6A0
	public override Vector3 EyePositionForPlayer(BasePlayer player, Quaternion viewRot)
	{
		if (this.PlayerIsMounted(player))
		{
			return this.driverEye.transform.position;
		}
		return Vector3.zero;
	}

	// Token: 0x060025A8 RID: 9640 RVA: 0x000ED4C4 File Offset: 0x000EB6C4
	public override void ServerInit()
	{
		if (base.isClient)
		{
			return;
		}
		base.ServerInit();
		this.rigidBody = base.GetComponent<Rigidbody>();
		this.rigidBody.centerOfMass = this.centerOfMass.localPosition;
		this.rigidBody.isKinematic = false;
		if (BasicCar.chairtest)
		{
			this.SpawnChairTest();
		}
	}

	// Token: 0x060025A9 RID: 9641 RVA: 0x000ED51C File Offset: 0x000EB71C
	public void SpawnChairTest()
	{
		BaseEntity baseEntity = GameManager.server.CreateEntity(this.chairRef.resourcePath, this.chairAnchorTest.transform.localPosition, default(Quaternion), true);
		baseEntity.Spawn();
		DestroyOnGroundMissing component = baseEntity.GetComponent<DestroyOnGroundMissing>();
		if (component != null)
		{
			component.enabled = false;
		}
		MeshCollider component2 = baseEntity.GetComponent<MeshCollider>();
		if (component2)
		{
			component2.convex = true;
		}
		baseEntity.SetParent(this, false, false);
	}

	// Token: 0x060025AA RID: 9642 RVA: 0x000ED594 File Offset: 0x000EB794
	public override void VehicleFixedUpdate()
	{
		base.VehicleFixedUpdate();
		if (!base.HasDriver())
		{
			this.NoDriverInput();
		}
		this.ConvertInputToThrottle();
		this.DoSteering();
		this.ApplyForceAtWheels();
		base.SetFlag(BaseEntity.Flags.Reserved1, base.HasDriver(), false, true);
		base.SetFlag(BaseEntity.Flags.Reserved2, base.HasDriver() && this.lightsOn, false, true);
	}

	// Token: 0x060025AB RID: 9643 RVA: 0x000ED5F8 File Offset: 0x000EB7F8
	private void DoSteering()
	{
		foreach (BasicCar.VehicleWheel vehicleWheel in this.wheels)
		{
			if (vehicleWheel.steerWheel)
			{
				vehicleWheel.wheelCollider.steerAngle = this.steering;
			}
		}
		base.SetFlag(BaseEntity.Flags.Reserved4, this.steering < -2f, false, true);
		base.SetFlag(BaseEntity.Flags.Reserved5, this.steering > 2f, false, true);
	}

	// Token: 0x060025AC RID: 9644 RVA: 0x000063A5 File Offset: 0x000045A5
	public void ConvertInputToThrottle()
	{
	}

	// Token: 0x060025AD RID: 9645 RVA: 0x000ED66C File Offset: 0x000EB86C
	private void ApplyForceAtWheels()
	{
		if (this.rigidBody == null)
		{
			return;
		}
		Vector3 velocity = this.rigidBody.velocity;
		float num = velocity.magnitude * Vector3.Dot(velocity.normalized, base.transform.forward);
		float num2 = this.brakePedal;
		float num3 = this.gasPedal;
		if (num > 0f && num3 < 0f)
		{
			num2 = 100f;
		}
		else if (num < 0f && num3 > 0f)
		{
			num2 = 100f;
		}
		foreach (BasicCar.VehicleWheel vehicleWheel in this.wheels)
		{
			if (vehicleWheel.wheelCollider.isGrounded)
			{
				if (vehicleWheel.powerWheel)
				{
					vehicleWheel.wheelCollider.motorTorque = num3 * this.motorForceConstant;
				}
				if (vehicleWheel.brakeWheel)
				{
					vehicleWheel.wheelCollider.brakeTorque = num2 * this.brakeForceConstant;
				}
			}
		}
		base.SetFlag(BaseEntity.Flags.Reserved3, num2 >= 100f && this.AnyMounted(), false, true);
	}

	// Token: 0x060025AE RID: 9646 RVA: 0x000ED778 File Offset: 0x000EB978
	public void NoDriverInput()
	{
		if (BasicCar.chairtest)
		{
			this.gasPedal = Mathf.Sin(Time.time) * 50f;
			return;
		}
		this.gasPedal = 0f;
		this.brakePedal = Mathf.Lerp(this.brakePedal, 100f, Time.deltaTime * this.GasLerpTime / 5f);
	}

	// Token: 0x060025AF RID: 9647 RVA: 0x000ED7D6 File Offset: 0x000EB9D6
	public override void PlayerServerInput(InputState inputState, BasePlayer player)
	{
		if (base.IsDriver(player))
		{
			this.DriverInput(inputState, player);
		}
	}

	// Token: 0x060025B0 RID: 9648 RVA: 0x000ED7EC File Offset: 0x000EB9EC
	public void DriverInput(InputState inputState, BasePlayer player)
	{
		if (inputState.IsDown(BUTTON.FORWARD))
		{
			this.gasPedal = 100f;
			this.brakePedal = 0f;
		}
		else if (inputState.IsDown(BUTTON.BACKWARD))
		{
			this.gasPedal = -30f;
			this.brakePedal = 0f;
		}
		else
		{
			this.gasPedal = 0f;
			this.brakePedal = 30f;
		}
		if (inputState.IsDown(BUTTON.LEFT))
		{
			this.steering = -60f;
			return;
		}
		if (inputState.IsDown(BUTTON.RIGHT))
		{
			this.steering = 60f;
			return;
		}
		this.steering = 0f;
	}

	// Token: 0x060025B1 RID: 9649 RVA: 0x000ED887 File Offset: 0x000EBA87
	public override void LightToggle(BasePlayer player)
	{
		if (base.IsDriver(player))
		{
			this.lightsOn = !this.lightsOn;
		}
	}

	// Token: 0x04001DBB RID: 7611
	public BasicCar.VehicleWheel[] wheels;

	// Token: 0x04001DBC RID: 7612
	public float brakePedal;

	// Token: 0x04001DBD RID: 7613
	public float gasPedal;

	// Token: 0x04001DBE RID: 7614
	public float steering;

	// Token: 0x04001DBF RID: 7615
	public Transform centerOfMass;

	// Token: 0x04001DC0 RID: 7616
	public Transform steeringWheel;

	// Token: 0x04001DC1 RID: 7617
	public float motorForceConstant = 150f;

	// Token: 0x04001DC2 RID: 7618
	public float brakeForceConstant = 500f;

	// Token: 0x04001DC3 RID: 7619
	public float GasLerpTime = 20f;

	// Token: 0x04001DC4 RID: 7620
	public float SteeringLerpTime = 20f;

	// Token: 0x04001DC5 RID: 7621
	public Transform driverEye;

	// Token: 0x04001DC6 RID: 7622
	public GameObjectRef chairRef;

	// Token: 0x04001DC7 RID: 7623
	public Transform chairAnchorTest;

	// Token: 0x04001DC8 RID: 7624
	public SoundPlayer idleLoopPlayer;

	// Token: 0x04001DC9 RID: 7625
	public Transform engineOffset;

	// Token: 0x04001DCA RID: 7626
	public SoundDefinition engineSoundDef;

	// Token: 0x04001DCB RID: 7627
	private static bool chairtest;

	// Token: 0x04001DCC RID: 7628
	private float throttle;

	// Token: 0x04001DCD RID: 7629
	private float brake;

	// Token: 0x04001DCE RID: 7630
	private bool lightsOn = true;

	// Token: 0x02000D10 RID: 3344
	[Serializable]
	public class VehicleWheel
	{
		// Token: 0x04004699 RID: 18073
		public Transform shock;

		// Token: 0x0400469A RID: 18074
		public WheelCollider wheelCollider;

		// Token: 0x0400469B RID: 18075
		public Transform wheel;

		// Token: 0x0400469C RID: 18076
		public Transform axle;

		// Token: 0x0400469D RID: 18077
		public bool steerWheel;

		// Token: 0x0400469E RID: 18078
		public bool brakeWheel = true;

		// Token: 0x0400469F RID: 18079
		public bool powerWheel = true;
	}
}
