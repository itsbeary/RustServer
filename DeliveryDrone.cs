using System;
using System.Runtime.CompilerServices;
using Facepunch;
using ProtoBuf;
using UnityEngine;

// Token: 0x0200016D RID: 365
public class DeliveryDrone : global::Drone
{
	// Token: 0x06001789 RID: 6025 RVA: 0x000B2878 File Offset: 0x000B0A78
	public void Setup(Marketplace marketplace, global::MarketTerminal terminal, global::VendingMachine vendingMachine)
	{
		this.sourceMarketplace.Set(marketplace);
		this.sourceTerminal.Set(terminal);
		this.targetVendingMachine.Set(vendingMachine);
		this._state = global::DeliveryDrone.State.Takeoff;
		this._sinceLastStateChange = 0f;
		this._pickUpTicks = 0;
	}

	// Token: 0x0600178A RID: 6026 RVA: 0x000B28C7 File Offset: 0x000B0AC7
	public override void ServerInit()
	{
		base.ServerInit();
		base.InvokeRandomized(new Action(this.Think), 0f, 0.5f, 0.25f);
		this.CreateMapMarker();
	}

	// Token: 0x0600178B RID: 6027 RVA: 0x000B28F8 File Offset: 0x000B0AF8
	public void CreateMapMarker()
	{
		if (this._mapMarkerInstance != null)
		{
			this._mapMarkerInstance.Kill(global::BaseNetworkable.DestroyMode.None);
		}
		GameManager server = GameManager.server;
		GameObjectRef gameObjectRef = this.mapMarkerPrefab;
		global::BaseEntity baseEntity = server.CreateEntity((gameObjectRef != null) ? gameObjectRef.resourcePath : null, Vector3.zero, Quaternion.identity, true);
		baseEntity.OwnerID = base.OwnerID;
		baseEntity.Spawn();
		baseEntity.SetParent(this, false, false);
		this._mapMarkerInstance = baseEntity;
	}

	// Token: 0x0600178C RID: 6028 RVA: 0x000B296C File Offset: 0x000B0B6C
	private void Think()
	{
		global::DeliveryDrone.<>c__DisplayClass24_0 CS$<>8__locals1;
		CS$<>8__locals1.<>4__this = this;
		if (this._sinceLastStateChange > this.stateTimeout)
		{
			Debug.LogError("Delivery drone hasn't change state in too long, killing", this);
			this.ForceRemove();
			return;
		}
		global::MarketTerminal marketTerminal;
		if (!this.sourceMarketplace.TryGet(true, out CS$<>8__locals1.marketplace) || !this.sourceTerminal.TryGet(true, out marketTerminal))
		{
			Debug.LogError("Delivery drone's marketplace or terminal was destroyed, killing", this);
			this.ForceRemove();
			return;
		}
		global::VendingMachine vendingMachine;
		if (!this.targetVendingMachine.TryGet(true, out vendingMachine) && this._state <= global::DeliveryDrone.State.AscendBeforeReturn)
		{
			this.<Think>g__SetState|24_7(global::DeliveryDrone.State.ReturnToTerminal, ref CS$<>8__locals1);
		}
		CS$<>8__locals1.currentPosition = base.transform.position;
		float num = this.<Think>g__GetMinimumHeight|24_1(Vector3.zero, ref CS$<>8__locals1);
		if (this._goToY != null)
		{
			if (!this.<Think>g__IsAtGoToY|24_6(ref CS$<>8__locals1))
			{
				this.targetPosition = new Vector3?(CS$<>8__locals1.currentPosition.WithY(this._goToY.Value));
				return;
			}
			this._goToY = null;
			this._sinceLastObstacleBlock = 0f;
			this._minimumYLock = new float?(CS$<>8__locals1.currentPosition.y);
		}
		switch (this._state)
		{
		case global::DeliveryDrone.State.Takeoff:
			this.<Think>g__SetGoalPosition|24_3(CS$<>8__locals1.marketplace.droneLaunchPoint.position + Vector3.up * 15f, ref CS$<>8__locals1);
			if (this.<Think>g__IsAtGoalPosition|24_4(ref CS$<>8__locals1))
			{
				this.<Think>g__SetState|24_7(global::DeliveryDrone.State.FlyToVendingMachine, ref CS$<>8__locals1);
			}
			break;
		case global::DeliveryDrone.State.FlyToVendingMachine:
		{
			bool flag;
			float num2 = this.<Think>g__CalculatePreferredY|24_0(out flag, ref CS$<>8__locals1);
			if (flag && CS$<>8__locals1.currentPosition.y < num2)
			{
				this.<Think>g__SetGoToY|24_5(num2 + this.marginAbovePreferredHeight, ref CS$<>8__locals1);
				return;
			}
			Vector3 vector;
			Vector3 vector2;
			this.config.FindDescentPoints(vendingMachine, num2 + this.marginAbovePreferredHeight, out vector, out vector2);
			this.<Think>g__SetGoalPosition|24_3(vector2, ref CS$<>8__locals1);
			if (this.<Think>g__IsAtGoalPosition|24_4(ref CS$<>8__locals1))
			{
				this.<Think>g__SetState|24_7(global::DeliveryDrone.State.DescendToVendingMachine, ref CS$<>8__locals1);
			}
			break;
		}
		case global::DeliveryDrone.State.DescendToVendingMachine:
		{
			Vector3 vector;
			Vector3 vector3;
			this.config.FindDescentPoints(vendingMachine, CS$<>8__locals1.currentPosition.y, out vector3, out vector);
			this.<Think>g__SetGoalPosition|24_3(vector3, ref CS$<>8__locals1);
			if (this.<Think>g__IsAtGoalPosition|24_4(ref CS$<>8__locals1))
			{
				this.<Think>g__SetState|24_7(global::DeliveryDrone.State.PickUpItems, ref CS$<>8__locals1);
			}
			break;
		}
		case global::DeliveryDrone.State.PickUpItems:
			this._pickUpTicks++;
			if (this._pickUpTicks >= this.pickUpDelayInTicks)
			{
				this.<Think>g__SetState|24_7(global::DeliveryDrone.State.AscendBeforeReturn, ref CS$<>8__locals1);
			}
			break;
		case global::DeliveryDrone.State.AscendBeforeReturn:
		{
			Vector3 vector;
			Vector3 vector4;
			this.config.FindDescentPoints(vendingMachine, num + this.preferredCruiseHeight, out vector, out vector4);
			this.<Think>g__SetGoalPosition|24_3(vector4, ref CS$<>8__locals1);
			if (this.<Think>g__IsAtGoalPosition|24_4(ref CS$<>8__locals1))
			{
				this.<Think>g__SetState|24_7(global::DeliveryDrone.State.ReturnToTerminal, ref CS$<>8__locals1);
			}
			break;
		}
		case global::DeliveryDrone.State.ReturnToTerminal:
		{
			bool flag2;
			float num3 = this.<Think>g__CalculatePreferredY|24_0(out flag2, ref CS$<>8__locals1);
			if (flag2 && CS$<>8__locals1.currentPosition.y < num3)
			{
				this.<Think>g__SetGoToY|24_5(num3 + this.marginAbovePreferredHeight, ref CS$<>8__locals1);
				return;
			}
			Vector3 vector5 = this.<Think>g__LandingPosition|24_2(ref CS$<>8__locals1);
			if (Vector3Ex.Distance2D(CS$<>8__locals1.currentPosition, vector5) < 30f)
			{
				vector5.y = Mathf.Max(vector5.y, num3 + this.marginAbovePreferredHeight);
			}
			else
			{
				vector5.y = num3 + this.marginAbovePreferredHeight;
			}
			this.<Think>g__SetGoalPosition|24_3(vector5, ref CS$<>8__locals1);
			if (this.<Think>g__IsAtGoalPosition|24_4(ref CS$<>8__locals1))
			{
				this.<Think>g__SetState|24_7(global::DeliveryDrone.State.Landing, ref CS$<>8__locals1);
			}
			break;
		}
		case global::DeliveryDrone.State.Landing:
			this.<Think>g__SetGoalPosition|24_3(this.<Think>g__LandingPosition|24_2(ref CS$<>8__locals1), ref CS$<>8__locals1);
			if (this.<Think>g__IsAtGoalPosition|24_4(ref CS$<>8__locals1))
			{
				CS$<>8__locals1.marketplace.ReturnDrone(this);
				this.<Think>g__SetState|24_7(global::DeliveryDrone.State.Invalid, ref CS$<>8__locals1);
			}
			break;
		default:
			this.ForceRemove();
			break;
		}
		if (this._minimumYLock != null)
		{
			if (this._sinceLastObstacleBlock > this.obstacleHeightLockDuration)
			{
				this._minimumYLock = null;
				return;
			}
			if (this.targetPosition != null && this.targetPosition.Value.y < this._minimumYLock.Value)
			{
				this.targetPosition = new Vector3?(this.targetPosition.Value.WithY(this._minimumYLock.Value));
			}
		}
	}

	// Token: 0x0600178D RID: 6029 RVA: 0x000B2D70 File Offset: 0x000B0F70
	private void ForceRemove()
	{
		Marketplace marketplace;
		if (this.sourceMarketplace.TryGet(true, out marketplace))
		{
			marketplace.ReturnDrone(this);
			return;
		}
		base.Kill(global::BaseNetworkable.DestroyMode.None);
	}

	// Token: 0x0600178E RID: 6030 RVA: 0x000B2D9C File Offset: 0x000B0F9C
	public override void Save(global::BaseNetworkable.SaveInfo info)
	{
		base.Save(info);
		if (info.forDisk)
		{
			info.msg.deliveryDrone = Pool.Get<ProtoBuf.DeliveryDrone>();
			info.msg.deliveryDrone.marketplaceId = this.sourceMarketplace.uid;
			info.msg.deliveryDrone.terminalId = this.sourceTerminal.uid;
			info.msg.deliveryDrone.vendingMachineId = this.targetVendingMachine.uid;
			info.msg.deliveryDrone.state = (int)this._state;
		}
	}

	// Token: 0x0600178F RID: 6031 RVA: 0x000B2E30 File Offset: 0x000B1030
	public override void Load(global::BaseNetworkable.LoadInfo info)
	{
		base.Load(info);
		if (info.msg.deliveryDrone != null)
		{
			this.sourceMarketplace = new EntityRef<Marketplace>(info.msg.deliveryDrone.marketplaceId);
			this.sourceTerminal = new EntityRef<global::MarketTerminal>(info.msg.deliveryDrone.terminalId);
			this.targetVendingMachine = new EntityRef<global::VendingMachine>(info.msg.deliveryDrone.vendingMachineId);
			this._state = (global::DeliveryDrone.State)info.msg.deliveryDrone.state;
		}
	}

	// Token: 0x06001790 RID: 6032 RVA: 0x00007A44 File Offset: 0x00005C44
	public override bool CanControl(ulong playerID)
	{
		return false;
	}

	// Token: 0x06001792 RID: 6034 RVA: 0x000B2F14 File Offset: 0x000B1114
	[CompilerGenerated]
	private float <Think>g__CalculatePreferredY|24_0(out bool isBlocked, ref global::DeliveryDrone.<>c__DisplayClass24_0 A_2)
	{
		Vector3 vector;
		float num;
		this.body.velocity.WithY(0f).ToDirectionAndMagnitude(out vector, out num);
		if (num >= 0.5f)
		{
			float num2 = num * 2f;
			float num3 = this.<Think>g__GetMinimumHeight|24_1(Vector3.zero, ref A_2);
			float num4 = this.<Think>g__GetMinimumHeight|24_1(new Vector3(0f, 0f, num2 / 2f), ref A_2);
			float num5 = this.<Think>g__GetMinimumHeight|24_1(new Vector3(0f, 0f, num2), ref A_2);
			float num6 = Mathf.Max(Mathf.Max(num3, num4), num5) + this.preferredCruiseHeight;
			Quaternion quaternion = Quaternion.FromToRotation(Vector3.forward, vector);
			Vector3 vector2 = this.config.halfExtents.WithZ(num2 / 2f);
			Vector3 vector3 = (A_2.currentPosition.WithY(num6) + quaternion * new Vector3(0f, 0f, vector2.z / 2f)).WithY(num6 + 1000f);
			RaycastHit raycastHit;
			isBlocked = Physics.BoxCast(vector3, vector2, Vector3.down, out raycastHit, quaternion, 1000f, this.config.layerMask);
			float num8;
			if (isBlocked)
			{
				Ray ray = new Ray(vector3, Vector3.down);
				Vector3 vector4 = ray.ClosestPoint(raycastHit.point);
				float num7 = Vector3.Distance(ray.origin, vector4);
				num8 = num6 + (1000f - num7) + this.preferredHeightAboveObstacle;
			}
			else
			{
				num8 = num6;
			}
			return num8;
		}
		float num9 = this.<Think>g__GetMinimumHeight|24_1(Vector3.zero, ref A_2) + this.preferredCruiseHeight;
		Vector3 vector5 = A_2.currentPosition.WithY(num9 + 1000f);
		A_2.currentPosition.WithY(num9);
		RaycastHit raycastHit2;
		isBlocked = Physics.Raycast(vector5, Vector3.down, out raycastHit2, 1000f, this.config.layerMask);
		if (!isBlocked)
		{
			return num9;
		}
		return num9 + (1000f - raycastHit2.distance) + this.preferredHeightAboveObstacle;
	}

	// Token: 0x06001793 RID: 6035 RVA: 0x000B3104 File Offset: 0x000B1304
	[CompilerGenerated]
	private float <Think>g__GetMinimumHeight|24_1(Vector3 offset, ref global::DeliveryDrone.<>c__DisplayClass24_0 A_2)
	{
		Vector3 vector = base.transform.TransformPoint(offset);
		float height = TerrainMeta.HeightMap.GetHeight(vector);
		float height2 = WaterSystem.GetHeight(vector);
		return Mathf.Max(height, height2);
	}

	// Token: 0x06001794 RID: 6036 RVA: 0x000B3136 File Offset: 0x000B1336
	[CompilerGenerated]
	private Vector3 <Think>g__LandingPosition|24_2(ref global::DeliveryDrone.<>c__DisplayClass24_0 A_1)
	{
		return A_1.marketplace.droneLaunchPoint.position;
	}

	// Token: 0x06001795 RID: 6037 RVA: 0x000B3148 File Offset: 0x000B1348
	[CompilerGenerated]
	private void <Think>g__SetGoalPosition|24_3(Vector3 position, ref global::DeliveryDrone.<>c__DisplayClass24_0 A_2)
	{
		this._goToY = null;
		this._stateGoalPosition = new Vector3?(position);
		this.targetPosition = new Vector3?(position);
	}

	// Token: 0x06001796 RID: 6038 RVA: 0x000B316E File Offset: 0x000B136E
	[CompilerGenerated]
	private bool <Think>g__IsAtGoalPosition|24_4(ref global::DeliveryDrone.<>c__DisplayClass24_0 A_1)
	{
		return this._stateGoalPosition != null && Vector3.Distance(this._stateGoalPosition.Value, A_1.currentPosition) < this.targetPositionTolerance;
	}

	// Token: 0x06001797 RID: 6039 RVA: 0x000B319D File Offset: 0x000B139D
	[CompilerGenerated]
	private void <Think>g__SetGoToY|24_5(float y, ref global::DeliveryDrone.<>c__DisplayClass24_0 A_2)
	{
		this._goToY = new float?(y);
		this.targetPosition = new Vector3?(A_2.currentPosition.WithY(y));
	}

	// Token: 0x06001798 RID: 6040 RVA: 0x000B31C2 File Offset: 0x000B13C2
	[CompilerGenerated]
	private bool <Think>g__IsAtGoToY|24_6(ref global::DeliveryDrone.<>c__DisplayClass24_0 A_1)
	{
		return this._goToY != null && Mathf.Abs(this._goToY.Value - A_1.currentPosition.y) < this.targetPositionTolerance;
	}

	// Token: 0x06001799 RID: 6041 RVA: 0x000B31F8 File Offset: 0x000B13F8
	[CompilerGenerated]
	private void <Think>g__SetState|24_7(global::DeliveryDrone.State newState, ref global::DeliveryDrone.<>c__DisplayClass24_0 A_2)
	{
		this._state = newState;
		this._sinceLastStateChange = 0f;
		this._pickUpTicks = 0;
		this._stateGoalPosition = null;
		this._goToY = null;
		base.SetFlag(global::BaseEntity.Flags.Reserved1, this._state >= global::DeliveryDrone.State.AscendBeforeReturn, false, true);
	}

	// Token: 0x0400102C RID: 4140
	[Header("Delivery Drone")]
	public float stateTimeout = 300f;

	// Token: 0x0400102D RID: 4141
	public float targetPositionTolerance = 1f;

	// Token: 0x0400102E RID: 4142
	public float preferredCruiseHeight = 20f;

	// Token: 0x0400102F RID: 4143
	public float preferredHeightAboveObstacle = 5f;

	// Token: 0x04001030 RID: 4144
	public float marginAbovePreferredHeight = 3f;

	// Token: 0x04001031 RID: 4145
	public float obstacleHeightLockDuration = 3f;

	// Token: 0x04001032 RID: 4146
	public int pickUpDelayInTicks = 3;

	// Token: 0x04001033 RID: 4147
	public DeliveryDroneConfig config;

	// Token: 0x04001034 RID: 4148
	public GameObjectRef mapMarkerPrefab;

	// Token: 0x04001035 RID: 4149
	public EntityRef<Marketplace> sourceMarketplace;

	// Token: 0x04001036 RID: 4150
	public EntityRef<global::MarketTerminal> sourceTerminal;

	// Token: 0x04001037 RID: 4151
	public EntityRef<global::VendingMachine> targetVendingMachine;

	// Token: 0x04001038 RID: 4152
	private global::DeliveryDrone.State _state;

	// Token: 0x04001039 RID: 4153
	private RealTimeSince _sinceLastStateChange;

	// Token: 0x0400103A RID: 4154
	private Vector3? _stateGoalPosition;

	// Token: 0x0400103B RID: 4155
	private float? _goToY;

	// Token: 0x0400103C RID: 4156
	private TimeSince _sinceLastObstacleBlock;

	// Token: 0x0400103D RID: 4157
	private float? _minimumYLock;

	// Token: 0x0400103E RID: 4158
	private int _pickUpTicks;

	// Token: 0x0400103F RID: 4159
	private global::BaseEntity _mapMarkerInstance;

	// Token: 0x02000C3F RID: 3135
	private enum State
	{
		// Token: 0x04004315 RID: 17173
		Invalid,
		// Token: 0x04004316 RID: 17174
		Takeoff,
		// Token: 0x04004317 RID: 17175
		FlyToVendingMachine,
		// Token: 0x04004318 RID: 17176
		DescendToVendingMachine,
		// Token: 0x04004319 RID: 17177
		PickUpItems,
		// Token: 0x0400431A RID: 17178
		AscendBeforeReturn,
		// Token: 0x0400431B RID: 17179
		ReturnToTerminal,
		// Token: 0x0400431C RID: 17180
		Landing
	}
}
