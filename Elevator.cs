using System;
using System.Collections.Generic;
using Facepunch;
using ProtoBuf;
using Rust;
using UnityEngine;

// Token: 0x0200010E RID: 270
public class Elevator : global::IOEntity, IFlagNotify
{
	// Token: 0x170001EF RID: 495
	// (get) Token: 0x06001604 RID: 5636 RVA: 0x00007A44 File Offset: 0x00005C44
	protected virtual bool IsStatic
	{
		get
		{
			return false;
		}
	}

	// Token: 0x170001F0 RID: 496
	// (get) Token: 0x06001605 RID: 5637 RVA: 0x000ACADE File Offset: 0x000AACDE
	// (set) Token: 0x06001606 RID: 5638 RVA: 0x000ACAE6 File Offset: 0x000AACE6
	public int Floor { get; set; }

	// Token: 0x170001F1 RID: 497
	// (get) Token: 0x06001607 RID: 5639 RVA: 0x000233C8 File Offset: 0x000215C8
	protected bool IsTop
	{
		get
		{
			return base.HasFlag(global::BaseEntity.Flags.Reserved1);
		}
	}

	// Token: 0x06001608 RID: 5640 RVA: 0x000ACAF0 File Offset: 0x000AACF0
	public override void Load(global::BaseNetworkable.LoadInfo info)
	{
		base.Load(info);
		if (info.msg.elevator != null)
		{
			this.Floor = info.msg.elevator.floor;
		}
		if (this.FloorBlockerVolume != null)
		{
			this.FloorBlockerVolume.SetActive(this.Floor > 0);
		}
	}

	// Token: 0x06001609 RID: 5641 RVA: 0x000ACB4C File Offset: 0x000AAD4C
	public override void OnDeployed(global::BaseEntity parent, global::BasePlayer deployedBy, global::Item fromItem)
	{
		base.OnDeployed(parent, deployedBy, fromItem);
		global::Elevator elevatorInDirection = this.GetElevatorInDirection(global::Elevator.Direction.Down);
		if (elevatorInDirection != null)
		{
			elevatorInDirection.SetFlag(global::BaseEntity.Flags.Reserved1, false, false, true);
			this.Floor = elevatorInDirection.Floor + 1;
		}
		base.SetFlag(global::BaseEntity.Flags.Reserved1, true, false, true);
	}

	// Token: 0x0600160A RID: 5642 RVA: 0x000ACB9D File Offset: 0x000AAD9D
	protected virtual void CallElevator()
	{
		base.EntityLinkBroadcast<global::Elevator, ConstructionSocket>(delegate(global::Elevator elevatorEnt)
		{
			if (elevatorEnt.IsTop)
			{
				float num;
				elevatorEnt.RequestMoveLiftTo(this.Floor, out num, this);
			}
		}, (ConstructionSocket socket) => socket.socketType == ConstructionSocket.Type.Elevator);
	}

	// Token: 0x0600160B RID: 5643 RVA: 0x000ACBD0 File Offset: 0x000AADD0
	public void Server_RaiseLowerElevator(global::Elevator.Direction dir, bool goTopBottom)
	{
		if (base.IsBusy())
		{
			return;
		}
		int num = this.LiftPositionToFloor();
		if (dir != global::Elevator.Direction.Up)
		{
			if (dir == global::Elevator.Direction.Down)
			{
				num--;
				if (goTopBottom)
				{
					num = 0;
				}
			}
		}
		else
		{
			num++;
			if (goTopBottom)
			{
				num = this.Floor;
			}
		}
		float num2;
		this.RequestMoveLiftTo(num, out num2, this);
	}

	// Token: 0x0600160C RID: 5644 RVA: 0x000ACC1C File Offset: 0x000AAE1C
	protected bool RequestMoveLiftTo(int targetFloor, out float timeToTravel, global::Elevator fromElevator)
	{
		timeToTravel = 0f;
		if (base.IsBusy())
		{
			return false;
		}
		if (!this.IsStatic && this.ioEntity != null && !this.ioEntity.IsPowered())
		{
			return false;
		}
		if (!this.IsValidFloor(targetFloor))
		{
			return false;
		}
		if (!this.liftEntity.CanMove())
		{
			return false;
		}
		int num = this.LiftPositionToFloor();
		if (num == targetFloor)
		{
			this.OpenLiftDoors();
			this.OpenDoorsAtFloor(num);
			fromElevator.OpenLiftDoors();
			return false;
		}
		Vector3 worldSpaceFloorPosition = this.GetWorldSpaceFloorPosition(targetFloor);
		if (!GamePhysics.LineOfSight(this.liftEntity.transform.position, worldSpaceFloorPosition, 2097152, null))
		{
			return false;
		}
		this.OnMoveBegin();
		Vector3 vector = base.transform.InverseTransformPoint(worldSpaceFloorPosition);
		timeToTravel = this.TimeToTravelDistance(Mathf.Abs(this.liftEntity.transform.localPosition.y - vector.y));
		LeanTween.moveLocalY(this.liftEntity.gameObject, vector.y, timeToTravel).delay = this.LiftMoveDelay;
		timeToTravel += this.LiftMoveDelay;
		base.SetFlag(global::BaseEntity.Flags.Busy, true, false, true);
		if (targetFloor < this.Floor)
		{
			this.liftEntity.ToggleHurtTrigger(true);
		}
		base.Invoke(new Action(this.ClearBusy), timeToTravel + 1f);
		this.liftEntity.NotifyNewFloor(targetFloor, this.Floor);
		if (this.ioEntity != null)
		{
			this.ioEntity.SetFlag(global::BaseEntity.Flags.Busy, true, false, true);
			this.ioEntity.SendChangedToRoot(true);
		}
		return true;
	}

	// Token: 0x0600160D RID: 5645 RVA: 0x000ACDA9 File Offset: 0x000AAFA9
	protected virtual void OpenLiftDoors()
	{
		this.NotifyLiftEntityDoorsOpen(true);
	}

	// Token: 0x0600160E RID: 5646 RVA: 0x000063A5 File Offset: 0x000045A5
	protected virtual void OnMoveBegin()
	{
	}

	// Token: 0x0600160F RID: 5647 RVA: 0x000ACDB2 File Offset: 0x000AAFB2
	private float TimeToTravelDistance(float distance)
	{
		return distance / this.LiftSpeedPerMetre;
	}

	// Token: 0x06001610 RID: 5648 RVA: 0x000ACDBC File Offset: 0x000AAFBC
	protected virtual Vector3 GetWorldSpaceFloorPosition(int targetFloor)
	{
		int num = this.Floor - targetFloor;
		Vector3 vector = Vector3.up * ((float)num * this.FloorHeight);
		vector.y -= 1f;
		return base.transform.position - vector;
	}

	// Token: 0x170001F2 RID: 498
	// (get) Token: 0x06001611 RID: 5649 RVA: 0x000ACE07 File Offset: 0x000AB007
	protected virtual float FloorHeight
	{
		get
		{
			return 3f;
		}
	}

	// Token: 0x06001612 RID: 5650 RVA: 0x000ACE10 File Offset: 0x000AB010
	protected virtual void ClearBusy()
	{
		base.SetFlag(global::BaseEntity.Flags.Busy, false, false, true);
		if (this.liftEntity != null)
		{
			this.liftEntity.ToggleHurtTrigger(false);
		}
		if (this.ioEntity != null)
		{
			this.ioEntity.SetFlag(global::BaseEntity.Flags.Busy, false, false, true);
			this.ioEntity.SendChangedToRoot(true);
		}
	}

	// Token: 0x06001613 RID: 5651 RVA: 0x000ACE72 File Offset: 0x000AB072
	protected virtual bool IsValidFloor(int targetFloor)
	{
		return targetFloor <= this.Floor && targetFloor >= 0;
	}

	// Token: 0x06001614 RID: 5652 RVA: 0x000ACE88 File Offset: 0x000AB088
	private global::Elevator GetElevatorInDirection(global::Elevator.Direction dir)
	{
		EntityLink entityLink = base.FindLink((dir == global::Elevator.Direction.Down) ? "elevator/sockets/elevator-male" : "elevator/sockets/elevator-female");
		if (entityLink != null && !entityLink.IsEmpty())
		{
			global::BaseEntity owner = entityLink.connections[0].owner;
			global::Elevator elevator;
			if (owner != null && owner.isServer && (elevator = owner as global::Elevator) != null && elevator != this)
			{
				return elevator;
			}
		}
		return null;
	}

	// Token: 0x06001615 RID: 5653 RVA: 0x000ACEF0 File Offset: 0x000AB0F0
	public void UpdateChildEntities(bool isTop)
	{
		if (isTop)
		{
			if (this.liftEntity == null)
			{
				this.FindExistingLiftChild();
			}
			if (this.liftEntity == null)
			{
				this.liftEntity = GameManager.server.CreateEntity(this.LiftEntityPrefab.resourcePath, this.GetWorldSpaceFloorPosition(this.Floor), this.LiftRoot.rotation, true) as ElevatorLift;
				this.liftEntity.SetParent(this, true, false);
				this.liftEntity.Spawn();
			}
			if (this.ioEntity == null)
			{
				this.FindExistingIOChild();
			}
			if (this.ioEntity == null && this.IoEntityPrefab.isValid)
			{
				this.ioEntity = GameManager.server.CreateEntity(this.IoEntityPrefab.resourcePath, this.IoEntitySpawnPoint.position, this.IoEntitySpawnPoint.rotation, true) as global::IOEntity;
				this.ioEntity.SetParent(this, true, false);
				this.ioEntity.Spawn();
				return;
			}
		}
		else
		{
			if (this.liftEntity != null)
			{
				this.liftEntity.Kill(global::BaseNetworkable.DestroyMode.None);
			}
			if (this.ioEntity != null)
			{
				this.ioEntity.Kill(global::BaseNetworkable.DestroyMode.None);
			}
		}
	}

	// Token: 0x06001616 RID: 5654 RVA: 0x000AD030 File Offset: 0x000AB230
	private void FindExistingIOChild()
	{
		using (List<global::BaseEntity>.Enumerator enumerator = this.children.GetEnumerator())
		{
			while (enumerator.MoveNext())
			{
				global::IOEntity ioentity;
				if ((ioentity = enumerator.Current as global::IOEntity) != null)
				{
					this.ioEntity = ioentity;
					break;
				}
			}
		}
	}

	// Token: 0x06001617 RID: 5655 RVA: 0x000AD090 File Offset: 0x000AB290
	public override void Save(global::BaseNetworkable.SaveInfo info)
	{
		base.Save(info);
		if (info.msg.elevator == null)
		{
			info.msg.elevator = Pool.Get<ProtoBuf.Elevator>();
		}
		info.msg.elevator.floor = this.Floor;
	}

	// Token: 0x06001618 RID: 5656 RVA: 0x000AD0CC File Offset: 0x000AB2CC
	protected int LiftPositionToFloor()
	{
		Vector3 position = this.liftEntity.transform.position;
		int num = -1;
		float num2 = float.MaxValue;
		for (int i = 0; i <= this.Floor; i++)
		{
			float num3 = Vector3.Distance(this.GetWorldSpaceFloorPosition(i), position);
			if (num3 < num2)
			{
				num2 = num3;
				num = i;
			}
		}
		return num;
	}

	// Token: 0x06001619 RID: 5657 RVA: 0x000AD11D File Offset: 0x000AB31D
	public override void DestroyShared()
	{
		this.Cleanup();
		base.DestroyShared();
	}

	// Token: 0x0600161A RID: 5658 RVA: 0x000AD12C File Offset: 0x000AB32C
	private void Cleanup()
	{
		global::Elevator elevatorInDirection = this.GetElevatorInDirection(global::Elevator.Direction.Down);
		if (elevatorInDirection != null)
		{
			elevatorInDirection.SetFlag(global::BaseEntity.Flags.Reserved1, true, false, true);
		}
		global::Elevator elevatorInDirection2 = this.GetElevatorInDirection(global::Elevator.Direction.Up);
		if (elevatorInDirection2 != null)
		{
			elevatorInDirection2.Kill(global::BaseNetworkable.DestroyMode.Gib);
		}
	}

	// Token: 0x0600161B RID: 5659 RVA: 0x000AD170 File Offset: 0x000AB370
	public override void PostServerLoad()
	{
		base.PostServerLoad();
		base.SetFlag(global::BaseEntity.Flags.Busy, false, false, true);
		this.UpdateChildEntities(this.IsTop);
		if (this.ioEntity != null)
		{
			this.ioEntity.SetFlag(global::BaseEntity.Flags.Busy, false, false, true);
		}
	}

	// Token: 0x0600161C RID: 5660 RVA: 0x000AD1BE File Offset: 0x000AB3BE
	public override void UpdateHasPower(int inputAmount, int inputSlot)
	{
		base.UpdateHasPower(inputAmount, inputSlot);
		if (inputAmount > 0 && this.previousPowerAmount[inputSlot] == 0)
		{
			this.CallElevator();
		}
		this.previousPowerAmount[inputSlot] = inputAmount;
	}

	// Token: 0x0600161D RID: 5661 RVA: 0x000AD1E5 File Offset: 0x000AB3E5
	private void OnPhysicsNeighbourChanged()
	{
		if (this.IsStatic)
		{
			return;
		}
		if (this.GetElevatorInDirection(global::Elevator.Direction.Down) == null && !this.HasFloorSocketConnection())
		{
			base.Kill(global::BaseNetworkable.DestroyMode.Gib);
		}
	}

	// Token: 0x0600161E RID: 5662 RVA: 0x000AD210 File Offset: 0x000AB410
	private bool HasFloorSocketConnection()
	{
		EntityLink entityLink = base.FindLink("elevator/sockets/block-male");
		return entityLink != null && !entityLink.IsEmpty();
	}

	// Token: 0x0600161F RID: 5663 RVA: 0x000AD238 File Offset: 0x000AB438
	public void NotifyLiftEntityDoorsOpen(bool state)
	{
		if (this.liftEntity != null)
		{
			using (List<global::BaseEntity>.Enumerator enumerator = this.liftEntity.children.GetEnumerator())
			{
				while (enumerator.MoveNext())
				{
					Door door;
					if ((door = enumerator.Current as Door) != null)
					{
						door.SetOpen(state, false);
					}
				}
			}
		}
	}

	// Token: 0x06001620 RID: 5664 RVA: 0x000063A5 File Offset: 0x000045A5
	protected virtual void OpenDoorsAtFloor(int floor)
	{
	}

	// Token: 0x06001621 RID: 5665 RVA: 0x000AD2A8 File Offset: 0x000AB4A8
	public override void OnFlagsChanged(global::BaseEntity.Flags old, global::BaseEntity.Flags next)
	{
		base.OnFlagsChanged(old, next);
		if (!Rust.Application.isLoading && base.isServer && old.HasFlag(global::BaseEntity.Flags.Reserved1) != next.HasFlag(global::BaseEntity.Flags.Reserved1))
		{
			this.UpdateChildEntities(next.HasFlag(global::BaseEntity.Flags.Reserved1));
		}
		if (old.HasFlag(global::BaseEntity.Flags.Busy) != next.HasFlag(global::BaseEntity.Flags.Busy))
		{
			if (this.liftEntity == null)
			{
				this.FindExistingLiftChild();
			}
			if (this.liftEntity != null)
			{
				this.liftEntity.ToggleMovementCollider(!next.HasFlag(global::BaseEntity.Flags.Busy));
			}
		}
		if (old.HasFlag(global::BaseEntity.Flags.Reserved1) != next.HasFlag(global::BaseEntity.Flags.Reserved1) && this.FloorBlockerVolume != null)
		{
			this.FloorBlockerVolume.SetActive(next.HasFlag(global::BaseEntity.Flags.Reserved1));
		}
	}

	// Token: 0x06001622 RID: 5666 RVA: 0x000AD3E0 File Offset: 0x000AB5E0
	private void FindExistingLiftChild()
	{
		using (List<global::BaseEntity>.Enumerator enumerator = this.children.GetEnumerator())
		{
			while (enumerator.MoveNext())
			{
				ElevatorLift elevatorLift;
				if ((elevatorLift = enumerator.Current as ElevatorLift) != null)
				{
					this.liftEntity = elevatorLift;
					break;
				}
			}
		}
	}

	// Token: 0x06001623 RID: 5667 RVA: 0x000AD440 File Offset: 0x000AB640
	public void OnFlagToggled(bool state)
	{
		if (base.isServer)
		{
			base.SetFlag(global::BaseEntity.Flags.Reserved2, state, false, true);
		}
	}

	// Token: 0x04000E67 RID: 3687
	public Transform LiftRoot;

	// Token: 0x04000E68 RID: 3688
	public GameObjectRef LiftEntityPrefab;

	// Token: 0x04000E69 RID: 3689
	public GameObjectRef IoEntityPrefab;

	// Token: 0x04000E6A RID: 3690
	public Transform IoEntitySpawnPoint;

	// Token: 0x04000E6B RID: 3691
	public GameObject FloorBlockerVolume;

	// Token: 0x04000E6C RID: 3692
	public float LiftSpeedPerMetre = 1f;

	// Token: 0x04000E6D RID: 3693
	public GameObject[] PoweredObjects;

	// Token: 0x04000E6E RID: 3694
	public MeshRenderer PoweredMesh;

	// Token: 0x04000E6F RID: 3695
	[ColorUsage(true, true)]
	public Color PoweredLightColour;

	// Token: 0x04000E70 RID: 3696
	[ColorUsage(true, true)]
	public Color UnpoweredLightColour;

	// Token: 0x04000E71 RID: 3697
	public SkinnedMeshRenderer[] CableRenderers;

	// Token: 0x04000E72 RID: 3698
	public LODGroup CableLod;

	// Token: 0x04000E73 RID: 3699
	public Transform CableRoot;

	// Token: 0x04000E74 RID: 3700
	public float LiftMoveDelay;

	// Token: 0x04000E76 RID: 3702
	protected const global::BaseEntity.Flags TopFloorFlag = global::BaseEntity.Flags.Reserved1;

	// Token: 0x04000E77 RID: 3703
	public const global::BaseEntity.Flags ElevatorPowered = global::BaseEntity.Flags.Reserved2;

	// Token: 0x04000E78 RID: 3704
	private ElevatorLift liftEntity;

	// Token: 0x04000E79 RID: 3705
	private global::IOEntity ioEntity;

	// Token: 0x04000E7A RID: 3706
	private int[] previousPowerAmount = new int[2];

	// Token: 0x02000C32 RID: 3122
	public enum Direction
	{
		// Token: 0x040042E2 RID: 17122
		Up,
		// Token: 0x040042E3 RID: 17123
		Down
	}
}
