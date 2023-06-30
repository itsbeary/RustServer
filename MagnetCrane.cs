using System;
using System.Runtime.CompilerServices;
using ConVar;
using Facepunch;
using Network;
using ProtoBuf;
using Rust;
using UnityEngine;
using UnityEngine.Assertions;

// Token: 0x02000096 RID: 150
public class MagnetCrane : GroundVehicle, CarPhysics<MagnetCrane>.ICar
{
	// Token: 0x06000DC3 RID: 3523 RVA: 0x00074978 File Offset: 0x00072B78
	public override bool OnRpcMessage(global::BasePlayer player, uint rpc, Message msg)
	{
		using (TimeWarning.New("MagnetCrane.OnRpcMessage", 0))
		{
			if (rpc == 1851540757U && player != null)
			{
				Assert.IsTrue(player.isServer, "SV_RPC Message is using a clientside player!");
				if (ConVar.Global.developer > 2)
				{
					Debug.Log("SV_RPCMessage: " + player + " - RPC_OpenFuel ");
				}
				using (TimeWarning.New("RPC_OpenFuel", 0))
				{
					try
					{
						using (TimeWarning.New("Call", 0))
						{
							global::BaseEntity.RPCMessage rpcmessage = new global::BaseEntity.RPCMessage
							{
								connection = msg.connection,
								player = player,
								read = msg.read
							};
							this.RPC_OpenFuel(rpcmessage);
						}
					}
					catch (Exception ex)
					{
						Debug.LogException(ex);
						player.Kick("RPC Error in RPC_OpenFuel");
					}
				}
				return true;
			}
		}
		return base.OnRpcMessage(player, rpc, msg);
	}

	// Token: 0x17000146 RID: 326
	// (get) Token: 0x06000DC4 RID: 3524 RVA: 0x00074A9C File Offset: 0x00072C9C
	public VehicleTerrainHandler.Surface OnSurface
	{
		get
		{
			if (this.serverTerrainHandler == null)
			{
				return VehicleTerrainHandler.Surface.Default;
			}
			return this.serverTerrainHandler.OnSurface;
		}
	}

	// Token: 0x06000DC5 RID: 3525 RVA: 0x00074AB4 File Offset: 0x00072CB4
	public override void ServerInit()
	{
		base.ServerInit();
		base.InvokeRepeating(new Action(this.UpdateParams), 0f, 0.1f);
		this.animator.cullingMode = AnimatorCullingMode.AlwaysAnimate;
		this.animator.updateMode = AnimatorUpdateMode.AnimatePhysics;
		this.myRigidbody.centerOfMass = this.COM.localPosition;
		this.carPhysics = new CarPhysics<MagnetCrane>(this, base.transform, this.rigidBody, this.carSettings);
		this.serverTerrainHandler = new VehicleTerrainHandler(this);
		this.Magnet.SetMagnetEnabled(false, null);
		this.spawnOrigin = base.transform.position;
		this.lastDrivenTime = UnityEngine.Time.realtimeSinceStartup;
		GameObject[] onTriggers = this.OnTriggers;
		for (int i = 0; i < onTriggers.Length; i++)
		{
			onTriggers[i].SetActive(false);
		}
	}

	// Token: 0x06000DC6 RID: 3526 RVA: 0x00074B84 File Offset: 0x00072D84
	public override void PlayerServerInput(InputState inputState, global::BasePlayer player)
	{
		base.PlayerServerInput(inputState, player);
		if (!base.IsDriver(player))
		{
			return;
		}
		this.throttleInput = 0f;
		this.steerInput = 0f;
		this.extensionInput = 0f;
		this.yawInput = 0f;
		this.raiseArmInput = 0f;
		if (this.engineController.IsOff)
		{
			if (inputState.IsAnyDown())
			{
				this.engineController.TryStartEngine(player);
			}
		}
		else if (this.engineController.IsOn)
		{
			bool flag = inputState.IsDown(BUTTON.SPRINT);
			if (inputState.IsDown(BUTTON.RELOAD) && UnityEngine.Time.realtimeSinceStartup > this.nextToggleTime)
			{
				this.Magnet.SetMagnetEnabled(!this.Magnet.IsMagnetOn(), player);
				this.nextToggleTime = UnityEngine.Time.realtimeSinceStartup + 0.5f;
			}
			if (flag)
			{
				float speed = base.GetSpeed();
				float num = 0f;
				if (inputState.IsDown(BUTTON.FORWARD))
				{
					num = 1f;
				}
				else if (inputState.IsDown(BUTTON.BACKWARD))
				{
					num = -1f;
				}
				if (speed > 1f && num < 0f)
				{
					this.throttleInput = 0f;
					this.brakeInput = -num;
				}
				else if (speed < -1f && num > 0f)
				{
					this.throttleInput = 0f;
					this.brakeInput = num;
				}
				else
				{
					this.throttleInput = num;
					this.brakeInput = 0f;
				}
				if (inputState.IsDown(BUTTON.RIGHT))
				{
					this.steerInput = -1f;
				}
				if (inputState.IsDown(BUTTON.LEFT))
				{
					this.steerInput = 1f;
				}
			}
			else
			{
				if (inputState.IsDown(BUTTON.LEFT))
				{
					this.yawInput = 1f;
				}
				else if (inputState.IsDown(BUTTON.RIGHT))
				{
					this.yawInput = -1f;
				}
				else if (inputState.IsDown(BUTTON.DUCK))
				{
					float @float = this.animator.GetFloat(MagnetCrane.yawParam);
					if (@float > 0.01f && @float < 0.99f)
					{
						this.yawInput = ((@float <= 0.5f) ? (-1f) : 1f);
					}
				}
				if (inputState.IsDown(BUTTON.FORWARD))
				{
					this.raiseArmInput = 1f;
				}
				else if (inputState.IsDown(BUTTON.BACKWARD))
				{
					this.raiseArmInput = -1f;
				}
			}
			if (inputState.IsDown(BUTTON.FIRE_PRIMARY))
			{
				this.extensionInput = 1f;
			}
			if (inputState.IsDown(BUTTON.FIRE_SECONDARY))
			{
				this.extensionInput = -1f;
			}
		}
		this.handbrakeOn = this.throttleInput == 0f && this.steerInput == 0f;
	}

	// Token: 0x06000DC7 RID: 3527 RVA: 0x00074E15 File Offset: 0x00073015
	public override float MaxVelocity()
	{
		return Mathf.Max(this.GetMaxForwardSpeed() * 1.3f, 30f);
	}

	// Token: 0x06000DC8 RID: 3528 RVA: 0x00074E2D File Offset: 0x0007302D
	public float GetSteerInput()
	{
		return this.steerInput;
	}

	// Token: 0x06000DC9 RID: 3529 RVA: 0x00007A44 File Offset: 0x00005C44
	public bool GetSteerModInput()
	{
		return false;
	}

	// Token: 0x06000DCA RID: 3530 RVA: 0x000063A5 File Offset: 0x000045A5
	public override void OnEngineStartFailed()
	{
	}

	// Token: 0x06000DCB RID: 3531 RVA: 0x00074E35 File Offset: 0x00073035
	public override bool MeetsEngineRequirements()
	{
		return base.HasDriver();
	}

	// Token: 0x06000DCC RID: 3532 RVA: 0x00074E40 File Offset: 0x00073040
	public override void VehicleFixedUpdate()
	{
		base.VehicleFixedUpdate();
		this.rigidBody.ResetInertiaTensor();
		this.rigidBody.inertiaTensor = Vector3.Lerp(this.rigidBody.inertiaTensor, this.customInertiaTensor, 0.5f);
		float realtimeSinceStartup = UnityEngine.Time.realtimeSinceStartup;
		float num = Mathf.Clamp(realtimeSinceStartup - this.lastFixedUpdateTime, 0f, 0.5f);
		this.lastFixedUpdateTime = realtimeSinceStartup;
		float speed = base.GetSpeed();
		this.carPhysics.FixedUpdate(UnityEngine.Time.fixedDeltaTime, speed);
		this.serverTerrainHandler.FixedUpdate();
		bool flag = base.IsOn();
		if (base.IsOn())
		{
			float num2 = Mathf.Max(Mathf.Abs(this.throttleInput), Mathf.Abs(this.steerInput));
			float num3 = Mathf.Lerp(this.idleFuelPerSec, this.maxFuelPerSec, num2);
			if (!this.Magnet.HasConnectedObject())
			{
				num3 = Mathf.Min(num3, this.maxFuelPerSec * 0.75f);
			}
			this.engineController.TickFuel(num3);
		}
		this.engineController.CheckEngineState();
		if (base.IsOn() != flag)
		{
			GameObject[] onTriggers = this.OnTriggers;
			for (int i = 0; i < onTriggers.Length; i++)
			{
				onTriggers[i].SetActive(base.IsOn());
			}
		}
		if (Vector3.Dot(base.transform.up, Vector3.down) >= 0.4f)
		{
			base.Kill(global::BaseNetworkable.DestroyMode.Gib);
			return;
		}
		if (realtimeSinceStartup > this.lastDrivenTime + 14400f)
		{
			base.Kill(global::BaseNetworkable.DestroyMode.Gib);
			return;
		}
		if (this.spawnOrigin != Vector3.zero && this.maxDistanceFromOrigin != 0f)
		{
			if (Vector3Ex.Distance2D(base.transform.position, this.spawnOrigin) > this.maxDistanceFromOrigin)
			{
				if (Vector3Ex.Distance2D(base.transform.position, this.lastDamagePos) > 6f)
				{
					if (base.GetDriver() != null)
					{
						base.GetDriver().ShowToast(GameTip.Styles.Red_Normal, MagnetCrane.ReturnMessage, Array.Empty<string>());
					}
					base.Hurt(this.MaxHealth() * 0.15f, DamageType.Generic, this, false);
					this.lastDamagePos = base.transform.position;
					this.nextSelfHealTime = realtimeSinceStartup + 3600f;
					Effect.server.Run(this.selfDamageEffect.resourcePath, base.transform.position + Vector3.up * 2f, Vector3.up, null, false);
					return;
				}
			}
			else if (base.healthFraction < 1f && realtimeSinceStartup > this.nextSelfHealTime && base.SecondsSinceAttacked > 600f)
			{
				this.Heal(1000f);
			}
		}
		if (!base.HasDriver() || !base.IsOn())
		{
			this.handbrakeOn = true;
			this.throttleInput = 0f;
			this.steerInput = 0f;
			base.SetFlag(global::BaseEntity.Flags.Reserved10, false, false, true);
			this.Magnet.SetMagnetEnabled(false, null);
		}
		else
		{
			this.lastDrivenTime = realtimeSinceStartup;
			if (this.Magnet.IsMagnetOn() && this.Magnet.HasConnectedObject() && GamePhysics.CheckOBB(this.Magnet.GetConnectedOBB(0.75f), 1084293121, QueryTriggerInteraction.Ignore))
			{
				this.Magnet.SetMagnetEnabled(false, null);
				this.nextToggleTime = realtimeSinceStartup + 2f;
				Effect.server.Run(this.selfDamageEffect.resourcePath, this.Magnet.transform.position, Vector3.up, null, false);
			}
		}
		this.extensionMove = MagnetCrane.<VehicleFixedUpdate>g__UpdateMoveInput|35_0(this.extensionInput, this.extensionMove, 3f, UnityEngine.Time.fixedDeltaTime);
		this.yawMove = MagnetCrane.<VehicleFixedUpdate>g__UpdateMoveInput|35_0(this.yawInput, this.yawMove, 3f, UnityEngine.Time.fixedDeltaTime);
		this.raiseArmMove = MagnetCrane.<VehicleFixedUpdate>g__UpdateMoveInput|35_0(this.raiseArmInput, this.raiseArmMove, 3f, UnityEngine.Time.fixedDeltaTime);
		bool flag2 = this.extensionInput != 0f || this.raiseArmInput != 0f || this.yawInput != 0f;
		base.SetFlag(global::BaseEntity.Flags.Reserved7, flag2, false, true);
		this.magnetDamage.damageEnabled = base.IsOn() && flag2;
		this.extensionArmState += this.extensionInput * this.arm1Speed * num;
		this.raiseArmState += this.raiseArmInput * this.arm2Speed * num;
		this.yawState += this.yawInput * this.turnYawSpeed * num;
		this.yawState %= 1f;
		if (this.yawState < 0f)
		{
			this.yawState += 1f;
		}
		this.extensionArmState = Mathf.Clamp(this.extensionArmState, -1f, 1f);
		this.raiseArmState = Mathf.Clamp(this.raiseArmState, -1f, 1f);
		this.UpdateAnimator(UnityEngine.Time.fixedDeltaTime);
		this.Magnet.MagnetThink(UnityEngine.Time.fixedDeltaTime);
		base.SetFlag(global::BaseEntity.Flags.Reserved10, this.throttleInput != 0f || this.steerInput != 0f, false, true);
	}

	// Token: 0x06000DCD RID: 3533 RVA: 0x0007535C File Offset: 0x0007355C
	public override void Save(global::BaseNetworkable.SaveInfo info)
	{
		base.Save(info);
		info.msg.crane = Facepunch.Pool.Get<Crane>();
		info.msg.crane.arm1 = this.extensionArmState;
		info.msg.crane.arm2 = this.raiseArmState;
		info.msg.crane.yaw = this.yawState;
		info.msg.crane.time = this.GetNetworkTime();
		int num = (int)((byte)((this.carPhysics.TankThrottleLeft + 1f) * 7f));
		byte b = (byte)((this.carPhysics.TankThrottleRight + 1f) * 7f);
		byte b2 = (byte)(num + ((int)b << 4));
		info.msg.crane.treadInput = (int)b2;
	}

	// Token: 0x06000DCE RID: 3534 RVA: 0x00007D2F File Offset: 0x00005F2F
	public void UpdateParams()
	{
		base.SendNetworkUpdate(global::BasePlayer.NetworkQueue.Update);
	}

	// Token: 0x06000DCF RID: 3535 RVA: 0x00075420 File Offset: 0x00073620
	public void LateUpdate()
	{
		if (base.isClient)
		{
			return;
		}
		if (base.HasDriver() && this.IsColliding())
		{
			this.extensionArmState = this.lastExtensionArmState;
			this.raiseArmState = this.lastRaiseArmState;
			this.yawState = this.lastYawState;
			this.extensionInput = -this.extensionInput;
			this.yawInput = -this.yawInput;
			this.raiseArmInput = -this.raiseArmInput;
			this.UpdateAnimator(UnityEngine.Time.deltaTime);
			return;
		}
		this.lastExtensionArmState = this.extensionArmState;
		this.lastRaiseArmState = this.raiseArmState;
		this.lastYawState = this.yawState;
	}

	// Token: 0x06000DD0 RID: 3536 RVA: 0x000754C4 File Offset: 0x000736C4
	public override void OnAttacked(HitInfo info)
	{
		if (base.isServer)
		{
			global::BasePlayer driver = base.GetDriver();
			if (driver != null && info.damageTypes.Has(DamageType.Bullet))
			{
				Capsule capsule = new Capsule(this.driverCollision.transform.position, this.driverCollision.radius, this.driverCollision.height);
				float num = Vector3.Distance(info.PointStart, info.PointEnd);
				Ray ray = new Ray(info.PointStart, Vector3Ex.Direction(info.PointEnd, info.PointStart));
				RaycastHit raycastHit;
				if (capsule.Trace(ray, out raycastHit, 0.05f, num * 1.2f))
				{
					driver.Hurt(info.damageTypes.Total() * 0.15f, DamageType.Bullet, info.Initiator, true);
				}
			}
		}
		base.OnAttacked(info);
	}

	// Token: 0x06000DD1 RID: 3537 RVA: 0x000755A0 File Offset: 0x000737A0
	public override void OnKilled(HitInfo info)
	{
		if (base.HasDriver())
		{
			base.GetDriver().Hurt(10000f, DamageType.Blunt, info.Initiator, false);
		}
		if (this.explosionEffect.isValid)
		{
			Effect.server.Run(this.explosionEffect.resourcePath, this.explosionPoint.position, Vector3.up, null, false);
		}
		base.OnKilled(info);
	}

	// Token: 0x06000DD2 RID: 3538 RVA: 0x00075604 File Offset: 0x00073804
	public bool IsColliding()
	{
		foreach (Transform transform in this.collisionTestingPoints)
		{
			if (transform.gameObject.activeSelf)
			{
				Vector3 position = transform.position;
				Quaternion rotation = transform.rotation;
				if (GamePhysics.CheckOBB(new OBB(position, new Vector3(transform.localScale.x, transform.localScale.y, transform.localScale.z), rotation), 1084293121, QueryTriggerInteraction.Ignore))
				{
					return true;
				}
			}
		}
		return false;
	}

	// Token: 0x06000DD3 RID: 3539 RVA: 0x00075684 File Offset: 0x00073884
	public float GetMaxDriveForce()
	{
		return (float)this.engineKW * 10f;
	}

	// Token: 0x06000DD4 RID: 3540 RVA: 0x00075694 File Offset: 0x00073894
	public float GetAdjustedDriveForce(float absSpeed, float topSpeed)
	{
		float num = MathEx.BiasedLerp(1f - absSpeed / topSpeed, 0.5f);
		num = Mathf.Lerp(num, 1f, Mathf.Abs(this.steerInput));
		return this.GetMaxDriveForce() * num;
	}

	// Token: 0x06000DD5 RID: 3541 RVA: 0x000756D4 File Offset: 0x000738D4
	public CarWheel[] GetWheels()
	{
		return this.wheels;
	}

	// Token: 0x06000DD6 RID: 3542 RVA: 0x00029EBC File Offset: 0x000280BC
	public float GetWheelsMidPos()
	{
		return 0f;
	}

	// Token: 0x06000DD7 RID: 3543 RVA: 0x000756DC File Offset: 0x000738DC
	public void UpdateAnimator(float dt)
	{
		this.animator.SetFloat("Arm_01", this.extensionArmState);
		this.animator.SetFloat("Arm_02", this.raiseArmState);
		this.animator.SetFloat("Yaw", this.yawState);
	}

	// Token: 0x06000DD8 RID: 3544 RVA: 0x0007572C File Offset: 0x0007392C
	[global::BaseEntity.RPC_Server]
	public void RPC_OpenFuel(global::BaseEntity.RPCMessage msg)
	{
		global::BasePlayer player = msg.player;
		if (!this.CanBeLooted(player))
		{
			return;
		}
		this.GetFuelSystem().LootFuel(player);
	}

	// Token: 0x17000147 RID: 327
	// (get) Token: 0x06000DD9 RID: 3545 RVA: 0x00075756 File Offset: 0x00073956
	public override float DriveWheelVelocity
	{
		get
		{
			return base.GetSpeed();
		}
	}

	// Token: 0x06000DDA RID: 3546 RVA: 0x0007575E File Offset: 0x0007395E
	public override float GetThrottleInput()
	{
		if (base.isServer)
		{
			return this.throttleInput;
		}
		throw new NotImplementedException("We don't know magnet crane throttle input on the client.");
	}

	// Token: 0x06000DDB RID: 3547 RVA: 0x00075779 File Offset: 0x00073979
	public override float GetBrakeInput()
	{
		if (!base.isServer)
		{
			throw new NotImplementedException("We don't know magnet crane brake input on the client.");
		}
		if (this.handbrakeOn)
		{
			return 1f;
		}
		return this.brakeInput;
	}

	// Token: 0x06000DDC RID: 3548 RVA: 0x000757A4 File Offset: 0x000739A4
	public override void Load(global::BaseNetworkable.LoadInfo info)
	{
		base.Load(info);
		if (info.msg.crane != null && base.isServer)
		{
			this.yawState = info.msg.crane.yaw;
			this.extensionArmState = info.msg.crane.arm1;
			this.raiseArmState = info.msg.crane.arm2;
		}
	}

	// Token: 0x06000DDD RID: 3549 RVA: 0x0007580F File Offset: 0x00073A0F
	public override float GetMaxForwardSpeed()
	{
		return 13f;
	}

	// Token: 0x06000DDE RID: 3550 RVA: 0x00075816 File Offset: 0x00073A16
	public override bool CanBeLooted(global::BasePlayer player)
	{
		return base.CanBeLooted(player) && (this.PlayerIsMounted(player) || !base.IsOn());
	}

	// Token: 0x06000DE1 RID: 3553 RVA: 0x00075928 File Offset: 0x00073B28
	[CompilerGenerated]
	internal static float <VehicleFixedUpdate>g__UpdateMoveInput|35_0(float input, float move, float slowRate, float dt)
	{
		if (input != 0f)
		{
			return input;
		}
		return Mathf.MoveTowards(move, 0f, dt * slowRate);
	}

	// Token: 0x040008C4 RID: 2244
	private float steerInput;

	// Token: 0x040008C5 RID: 2245
	private float throttleInput;

	// Token: 0x040008C6 RID: 2246
	private float brakeInput;

	// Token: 0x040008C7 RID: 2247
	private float yawInput;

	// Token: 0x040008C8 RID: 2248
	private float extensionInput;

	// Token: 0x040008C9 RID: 2249
	private float raiseArmInput;

	// Token: 0x040008CA RID: 2250
	private float extensionMove;

	// Token: 0x040008CB RID: 2251
	private float yawMove;

	// Token: 0x040008CC RID: 2252
	private float raiseArmMove;

	// Token: 0x040008CD RID: 2253
	private float nextToggleTime;

	// Token: 0x040008CE RID: 2254
	private Vector3 spawnOrigin = Vector3.zero;

	// Token: 0x040008CF RID: 2255
	private float lastExtensionArmState;

	// Token: 0x040008D0 RID: 2256
	private float lastRaiseArmState;

	// Token: 0x040008D1 RID: 2257
	private float lastYawState;

	// Token: 0x040008D2 RID: 2258
	private bool handbrakeOn = true;

	// Token: 0x040008D3 RID: 2259
	private float nextSelfHealTime;

	// Token: 0x040008D4 RID: 2260
	private Vector3 lastDamagePos = Vector3.zero;

	// Token: 0x040008D5 RID: 2261
	private float lastDrivenTime;

	// Token: 0x040008D6 RID: 2262
	private float lastFixedUpdateTime;

	// Token: 0x040008D7 RID: 2263
	private CarPhysics<MagnetCrane> carPhysics;

	// Token: 0x040008D8 RID: 2264
	private VehicleTerrainHandler serverTerrainHandler;

	// Token: 0x040008D9 RID: 2265
	private Vector3 customInertiaTensor = new Vector3(25000f, 11000f, 19000f);

	// Token: 0x040008DA RID: 2266
	private float extensionArmState;

	// Token: 0x040008DB RID: 2267
	private float raiseArmState;

	// Token: 0x040008DC RID: 2268
	private float yawState = 1f;

	// Token: 0x040008DD RID: 2269
	[Header("Magnet Crane")]
	public Animator animator;

	// Token: 0x040008DE RID: 2270
	[SerializeField]
	private Transform COM;

	// Token: 0x040008DF RID: 2271
	[SerializeField]
	private float arm1Speed = 0.01f;

	// Token: 0x040008E0 RID: 2272
	[SerializeField]
	private float arm2Speed = 0.01f;

	// Token: 0x040008E1 RID: 2273
	[SerializeField]
	private float turnYawSpeed = 0.01f;

	// Token: 0x040008E2 RID: 2274
	[SerializeField]
	private BaseMagnet Magnet;

	// Token: 0x040008E3 RID: 2275
	[SerializeField]
	private MagnetCraneAudio mcAudio;

	// Token: 0x040008E4 RID: 2276
	[SerializeField]
	private Rigidbody myRigidbody;

	// Token: 0x040008E5 RID: 2277
	[SerializeField]
	private Transform[] collisionTestingPoints;

	// Token: 0x040008E6 RID: 2278
	[SerializeField]
	private float maxDistanceFromOrigin;

	// Token: 0x040008E7 RID: 2279
	[SerializeField]
	private GameObjectRef selfDamageEffect;

	// Token: 0x040008E8 RID: 2280
	[SerializeField]
	private GameObjectRef explosionEffect;

	// Token: 0x040008E9 RID: 2281
	[SerializeField]
	private Transform explosionPoint;

	// Token: 0x040008EA RID: 2282
	[SerializeField]
	private CapsuleCollider driverCollision;

	// Token: 0x040008EB RID: 2283
	[SerializeField]
	private Transform leftHandTarget;

	// Token: 0x040008EC RID: 2284
	[SerializeField]
	private Transform rightHandTarget;

	// Token: 0x040008ED RID: 2285
	[SerializeField]
	private Transform leftFootTarget;

	// Token: 0x040008EE RID: 2286
	[SerializeField]
	private Transform rightFootTarget;

	// Token: 0x040008EF RID: 2287
	[SerializeField]
	private float idleFuelPerSec;

	// Token: 0x040008F0 RID: 2288
	[SerializeField]
	private float maxFuelPerSec;

	// Token: 0x040008F1 RID: 2289
	[SerializeField]
	private GameObject[] OnTriggers;

	// Token: 0x040008F2 RID: 2290
	[SerializeField]
	private TriggerHurtEx magnetDamage;

	// Token: 0x040008F3 RID: 2291
	[SerializeField]
	private int engineKW = 250;

	// Token: 0x040008F4 RID: 2292
	[SerializeField]
	private CarWheel[] wheels;

	// Token: 0x040008F5 RID: 2293
	[SerializeField]
	private CarSettings carSettings;

	// Token: 0x040008F6 RID: 2294
	[SerializeField]
	private ParticleSystem exhaustInner;

	// Token: 0x040008F7 RID: 2295
	[SerializeField]
	private ParticleSystem exhaustOuter;

	// Token: 0x040008F8 RID: 2296
	[SerializeField]
	private EmissionToggle lightToggle;

	// Token: 0x040008F9 RID: 2297
	public static readonly Translate.Phrase ReturnMessage = new Translate.Phrase("junkyardcrane.return", "Return to the Junkyard. Excessive damage will occur.");

	// Token: 0x040008FA RID: 2298
	private const global::BaseEntity.Flags Flag_ArmMovement = global::BaseEntity.Flags.Reserved7;

	// Token: 0x040008FB RID: 2299
	private const global::BaseEntity.Flags Flag_BaseMovementInput = global::BaseEntity.Flags.Reserved10;

	// Token: 0x040008FC RID: 2300
	private static int leftTreadParam = Animator.StringToHash("left tread movement");

	// Token: 0x040008FD RID: 2301
	private static int rightTreadParam = Animator.StringToHash("right tread movement");

	// Token: 0x040008FE RID: 2302
	private static int yawParam = Animator.StringToHash("Yaw");

	// Token: 0x040008FF RID: 2303
	private static int arm1Param = Animator.StringToHash("Arm_01");

	// Token: 0x04000900 RID: 2304
	private static int arm2Param = Animator.StringToHash("Arm_02");
}
