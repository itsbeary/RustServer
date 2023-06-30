using System;
using Network;
using Rust;
using UnityEngine;

// Token: 0x0200008D RID: 141
public class Kayak : BaseBoat, IPoolVehicle
{
	// Token: 0x06000D2D RID: 3373 RVA: 0x00070EE8 File Offset: 0x0006F0E8
	public override bool OnRpcMessage(BasePlayer player, uint rpc, Message msg)
	{
		using (TimeWarning.New("Kayak.OnRpcMessage", 0))
		{
		}
		return base.OnRpcMessage(player, rpc, msg);
	}

	// Token: 0x06000D2E RID: 3374 RVA: 0x00070F28 File Offset: 0x0006F128
	public override void ServerInit()
	{
		base.ServerInit();
		this.timeSinceLastUsed = 0f;
		base.InvokeRandomized(new Action(this.BoatDecay), UnityEngine.Random.Range(30f, 60f), 60f, 6f);
	}

	// Token: 0x06000D2F RID: 3375 RVA: 0x00070F78 File Offset: 0x0006F178
	public override void OnPlayerMounted()
	{
		base.OnPlayerMounted();
		if (!base.IsInvoking(new Action(this.TravelDistanceUpdate)) && GameInfo.HasAchievements)
		{
			int num = 0;
			foreach (BaseVehicle.MountPointInfo mountPointInfo in base.allMountPoints)
			{
				if (mountPointInfo.mountable != null && mountPointInfo.mountable.AnyMounted())
				{
					num++;
				}
			}
			if (num == 2)
			{
				this.lastTravelPos = base.transform.position.WithY(0f);
				base.InvokeRandomized(new Action(this.TravelDistanceUpdate), 5f, 5f, 3f);
			}
		}
	}

	// Token: 0x06000D30 RID: 3376 RVA: 0x00071050 File Offset: 0x0006F250
	public override void OnPlayerDismounted(BasePlayer player)
	{
		base.OnPlayerDismounted(player);
		if (base.IsInvoking(new Action(this.TravelDistanceUpdate)))
		{
			base.CancelInvoke(new Action(this.TravelDistanceUpdate));
		}
	}

	// Token: 0x06000D31 RID: 3377 RVA: 0x00071080 File Offset: 0x0006F280
	public override void DriverInput(InputState inputState, BasePlayer player)
	{
		this.timeSinceLastUsed = 0f;
		if (this.IsPlayerHoldingPaddle(player))
		{
			int playerSeat = base.GetPlayerSeat(player);
			if (this.playerPaddleCooldowns[playerSeat] > this.maxPaddleFrequency)
			{
				bool flag = inputState.IsDown(BUTTON.BACKWARD);
				bool flag2 = false;
				Vector3 vector = base.transform.forward;
				if (flag)
				{
					vector = -vector;
				}
				float num = this.forwardPaddleForce;
				if (base.NumMounted() >= 2)
				{
					num *= this.multiDriverPaddleForceMultiplier;
				}
				if (inputState.IsDown(BUTTON.LEFT) || inputState.IsDown(BUTTON.FIRE_PRIMARY))
				{
					flag2 = true;
					this.rigidBody.AddForceAtPosition(vector * num, this.GetPaddlePoint(playerSeat, Kayak.PaddleDirection.Left), ForceMode.Impulse);
					this.rigidBody.angularVelocity += -base.transform.up * this.rotatePaddleForce;
					base.ClientRPC<int, int>(null, "OnPaddled", flag ? 2 : 0, playerSeat);
				}
				else if (inputState.IsDown(BUTTON.RIGHT) || inputState.IsDown(BUTTON.FIRE_SECONDARY))
				{
					flag2 = true;
					this.rigidBody.AddForceAtPosition(vector * num, this.GetPaddlePoint(playerSeat, Kayak.PaddleDirection.Right), ForceMode.Impulse);
					this.rigidBody.angularVelocity += base.transform.up * this.rotatePaddleForce;
					base.ClientRPC<int, int>(null, "OnPaddled", flag ? 3 : 1, playerSeat);
				}
				if (flag2)
				{
					this.playerPaddleCooldowns[playerSeat] = 0f;
					if (!flag)
					{
						Vector3 velocity = this.rigidBody.velocity;
						this.rigidBody.velocity = Vector3.Lerp(velocity, vector * velocity.magnitude, 0.4f);
					}
				}
			}
		}
	}

	// Token: 0x06000D32 RID: 3378 RVA: 0x00071248 File Offset: 0x0006F448
	private void TravelDistanceUpdate()
	{
		Vector3 vector = base.transform.position.WithY(0f);
		if (GameInfo.HasAchievements)
		{
			float num = Vector3.Distance(this.lastTravelPos, vector) + this.distanceRemainder;
			float num2 = Mathf.Max(Mathf.Floor(num), 0f);
			this.distanceRemainder = num - num2;
			foreach (BaseVehicle.MountPointInfo mountPointInfo in base.allMountPoints)
			{
				if (mountPointInfo.mountable != null && mountPointInfo.mountable.AnyMounted() && (int)num2 > 0)
				{
					mountPointInfo.mountable.GetMounted().stats.Add("kayak_distance_travelled", (int)num2, global::Stats.Steam);
					mountPointInfo.mountable.GetMounted().stats.Save(true);
				}
			}
		}
		this.lastTravelPos = vector;
	}

	// Token: 0x06000D33 RID: 3379 RVA: 0x00007A44 File Offset: 0x00005C44
	public override bool EngineOn()
	{
		return false;
	}

	// Token: 0x06000D34 RID: 3380 RVA: 0x00071348 File Offset: 0x0006F548
	protected override void DoPushAction(BasePlayer player)
	{
		if (base.IsFlipped())
		{
			this.rigidBody.AddRelativeTorque(Vector3.forward * 8f, ForceMode.VelocityChange);
		}
		else
		{
			Vector3 vector = Vector3Ex.Direction2D(player.transform.position + player.eyes.BodyForward() * 3f, player.transform.position);
			vector = (Vector3.up * 0.1f + vector).normalized;
			Vector3 position = base.transform.position;
			float num = 5f;
			if (this.IsInWater())
			{
				num *= 0.75f;
			}
			this.rigidBody.AddForceAtPosition(vector * num, position, ForceMode.VelocityChange);
		}
		if (this.IsInWater())
		{
			if (this.pushWaterEffect.isValid)
			{
				Effect.server.Run(this.pushWaterEffect.resourcePath, this, 0U, Vector3.zero, Vector3.zero, null, false);
			}
		}
		else if (this.pushLandEffect.isValid)
		{
			Effect.server.Run(this.pushLandEffect.resourcePath, this, 0U, Vector3.zero, Vector3.zero, null, false);
		}
		base.WakeUp();
	}

	// Token: 0x06000D35 RID: 3381 RVA: 0x00071470 File Offset: 0x0006F670
	public override void VehicleFixedUpdate()
	{
		base.VehicleFixedUpdate();
		if (this.fixedDragUpdate == null)
		{
			this.fixedDragUpdate = new TimeCachedValue<float>
			{
				refreshCooldown = 0.5f,
				refreshRandomRange = 0.2f,
				updateValue = new Func<float>(this.CalculateDesiredDrag)
			};
		}
		this.rigidBody.drag = this.fixedDragUpdate.Get(false);
	}

	// Token: 0x06000D36 RID: 3382 RVA: 0x000714D8 File Offset: 0x0006F6D8
	private float CalculateDesiredDrag()
	{
		int num = base.NumMounted();
		if (num == 0)
		{
			return 1f;
		}
		if (num < 2)
		{
			return 0.05f;
		}
		return 0.1f;
	}

	// Token: 0x06000D37 RID: 3383 RVA: 0x00071504 File Offset: 0x0006F704
	private void BoatDecay()
	{
		BaseBoat.WaterVehicleDecay(this, 60f, this.timeSinceLastUsed, MotorRowboat.outsidedecayminutes, MotorRowboat.deepwaterdecayminutes);
	}

	// Token: 0x06000D38 RID: 3384 RVA: 0x00071526 File Offset: 0x0006F726
	public override bool CanPickup(BasePlayer player)
	{
		return !base.HasDriver() && base.CanPickup(player);
	}

	// Token: 0x06000D39 RID: 3385 RVA: 0x00071539 File Offset: 0x0006F739
	public bool IsPlayerHoldingPaddle(BasePlayer player)
	{
		return player.GetHeldEntity() != null && player.GetHeldEntity().GetItem().info == this.OarItem;
	}

	// Token: 0x06000D3A RID: 3386 RVA: 0x00071568 File Offset: 0x0006F768
	private Vector3 GetPaddlePoint(int index, Kayak.PaddleDirection direction)
	{
		index = Mathf.Clamp(index, 0, this.mountPoints.Count);
		Vector3 pos = this.mountPoints[index].pos;
		if (direction == Kayak.PaddleDirection.Left)
		{
			pos.x -= 1f;
		}
		else if (direction == Kayak.PaddleDirection.Right)
		{
			pos.x += 1f;
		}
		pos.y -= 0.2f;
		return base.transform.TransformPoint(pos);
	}

	// Token: 0x06000D3B RID: 3387 RVA: 0x000715E0 File Offset: 0x0006F7E0
	private bool IsInWater()
	{
		return base.isServer && this.buoyancy.timeOutOfWater < 0.1f;
	}

	// Token: 0x04000868 RID: 2152
	public ItemDefinition OarItem;

	// Token: 0x04000869 RID: 2153
	public float maxPaddleFrequency = 0.5f;

	// Token: 0x0400086A RID: 2154
	public float forwardPaddleForce = 5f;

	// Token: 0x0400086B RID: 2155
	public float multiDriverPaddleForceMultiplier = 0.75f;

	// Token: 0x0400086C RID: 2156
	public float rotatePaddleForce = 3f;

	// Token: 0x0400086D RID: 2157
	public GameObjectRef forwardSplashEffect;

	// Token: 0x0400086E RID: 2158
	public GameObjectRef backSplashEffect;

	// Token: 0x0400086F RID: 2159
	public ParticleSystem moveSplashEffect;

	// Token: 0x04000870 RID: 2160
	public float animationLerpSpeed = 6f;

	// Token: 0x04000871 RID: 2161
	[Header("Audio")]
	public BlendedSoundLoops waterLoops;

	// Token: 0x04000872 RID: 2162
	public float waterSoundSpeedDivisor = 10f;

	// Token: 0x04000873 RID: 2163
	public GameObjectRef pushLandEffect;

	// Token: 0x04000874 RID: 2164
	public GameObjectRef pushWaterEffect;

	// Token: 0x04000875 RID: 2165
	public PlayerModel.MountPoses noPaddlePose;

	// Token: 0x04000876 RID: 2166
	private TimeSince[] playerPaddleCooldowns = new TimeSince[2];

	// Token: 0x04000877 RID: 2167
	private TimeCachedValue<float> fixedDragUpdate;

	// Token: 0x04000878 RID: 2168
	private TimeSince timeSinceLastUsed;

	// Token: 0x04000879 RID: 2169
	private const float DECAY_TICK_TIME = 60f;

	// Token: 0x0400087A RID: 2170
	private Vector3 lastTravelPos;

	// Token: 0x0400087B RID: 2171
	private float distanceRemainder;

	// Token: 0x02000BEB RID: 3051
	private enum PaddleDirection
	{
		// Token: 0x040041D4 RID: 16852
		Left,
		// Token: 0x040041D5 RID: 16853
		Right,
		// Token: 0x040041D6 RID: 16854
		LeftBack,
		// Token: 0x040041D7 RID: 16855
		RightBack
	}
}
