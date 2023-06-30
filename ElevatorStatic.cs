using System;
using System.Collections.Generic;
using Facepunch;
using UnityEngine;

// Token: 0x02000111 RID: 273
public class ElevatorStatic : Elevator
{
	// Token: 0x170001F3 RID: 499
	// (get) Token: 0x0600162A RID: 5674 RVA: 0x0000441C File Offset: 0x0000261C
	protected override bool IsStatic
	{
		get
		{
			return true;
		}
	}

	// Token: 0x0600162B RID: 5675 RVA: 0x000AD5A8 File Offset: 0x000AB7A8
	public override void Spawn()
	{
		base.Spawn();
		base.SetFlag(BaseEntity.Flags.Reserved2, true, false, true);
		base.SetFlag(BaseEntity.Flags.Reserved1, this.StaticTop, false, true);
		if (!base.IsTop)
		{
			return;
		}
		List<RaycastHit> list = Pool.GetList<RaycastHit>();
		GamePhysics.TraceAll(new Ray(base.transform.position, -Vector3.up), 0f, list, 200f, 262144, QueryTriggerInteraction.Collide, null);
		foreach (RaycastHit raycastHit in list)
		{
			if (raycastHit.transform.parent != null)
			{
				ElevatorStatic component = raycastHit.transform.parent.GetComponent<ElevatorStatic>();
				if (component != null && component != this && component.isServer)
				{
					this.floorPositions.Add(component);
				}
			}
		}
		Pool.FreeList<RaycastHit>(ref list);
		this.floorPositions.Reverse();
		base.Floor = this.floorPositions.Count;
		for (int i = 0; i < this.floorPositions.Count; i++)
		{
			this.floorPositions[i].SetFloorDetails(i, this);
		}
	}

	// Token: 0x0600162C RID: 5676 RVA: 0x000AD6F4 File Offset: 0x000AB8F4
	public override void PostMapEntitySpawn()
	{
		base.PostMapEntitySpawn();
		base.UpdateChildEntities(base.IsTop);
	}

	// Token: 0x0600162D RID: 5677 RVA: 0x000AD708 File Offset: 0x000AB908
	protected override bool IsValidFloor(int targetFloor)
	{
		return targetFloor >= 0 && targetFloor <= base.Floor;
	}

	// Token: 0x0600162E RID: 5678 RVA: 0x000AD71C File Offset: 0x000AB91C
	protected override Vector3 GetWorldSpaceFloorPosition(int targetFloor)
	{
		if (targetFloor == base.Floor)
		{
			return base.transform.position + Vector3.up * 1f;
		}
		Vector3 position = base.transform.position;
		position.y = this.floorPositions[targetFloor].transform.position.y + 1f;
		return position;
	}

	// Token: 0x0600162F RID: 5679 RVA: 0x000AD787 File Offset: 0x000AB987
	public void SetFloorDetails(int floor, ElevatorStatic owner)
	{
		this.ownerElevator = owner;
		base.Floor = floor;
	}

	// Token: 0x06001630 RID: 5680 RVA: 0x000AD798 File Offset: 0x000AB998
	protected override void CallElevator()
	{
		if (this.ownerElevator != null)
		{
			float num;
			this.ownerElevator.RequestMoveLiftTo(base.Floor, out num, this);
			return;
		}
		if (base.IsTop)
		{
			float num2;
			base.RequestMoveLiftTo(base.Floor, out num2, this);
		}
	}

	// Token: 0x06001631 RID: 5681 RVA: 0x000AD7E1 File Offset: 0x000AB9E1
	private ElevatorStatic ElevatorAtFloor(int floor)
	{
		if (floor == base.Floor)
		{
			return this;
		}
		if (floor >= 0 && floor < this.floorPositions.Count)
		{
			return this.floorPositions[floor];
		}
		return null;
	}

	// Token: 0x06001632 RID: 5682 RVA: 0x000AD80E File Offset: 0x000ABA0E
	protected override void OpenDoorsAtFloor(int floor)
	{
		base.OpenDoorsAtFloor(floor);
		if (floor == this.floorPositions.Count)
		{
			this.OpenLiftDoors();
			return;
		}
		this.floorPositions[floor].OpenLiftDoors();
	}

	// Token: 0x06001633 RID: 5683 RVA: 0x000AD840 File Offset: 0x000ABA40
	protected override void OnMoveBegin()
	{
		base.OnMoveBegin();
		ElevatorStatic elevatorStatic = this.ElevatorAtFloor(base.LiftPositionToFloor());
		if (elevatorStatic != null)
		{
			elevatorStatic.OnLiftLeavingFloor();
		}
		base.NotifyLiftEntityDoorsOpen(false);
	}

	// Token: 0x06001634 RID: 5684 RVA: 0x000AD876 File Offset: 0x000ABA76
	private void OnLiftLeavingFloor()
	{
		this.ClearPowerOutput();
		if (base.IsInvoking(new Action(this.ClearPowerOutput)))
		{
			base.CancelInvoke(new Action(this.ClearPowerOutput));
		}
	}

	// Token: 0x06001635 RID: 5685 RVA: 0x000AD8A4 File Offset: 0x000ABAA4
	protected override void ClearBusy()
	{
		base.ClearBusy();
		ElevatorStatic elevatorStatic = this.ElevatorAtFloor(base.LiftPositionToFloor());
		if (elevatorStatic != null)
		{
			elevatorStatic.OnLiftArrivedAtFloor();
		}
		base.NotifyLiftEntityDoorsOpen(true);
	}

	// Token: 0x06001636 RID: 5686 RVA: 0x000AD8DA File Offset: 0x000ABADA
	protected override void OpenLiftDoors()
	{
		base.OpenLiftDoors();
		this.OnLiftArrivedAtFloor();
	}

	// Token: 0x06001637 RID: 5687 RVA: 0x000AD8E8 File Offset: 0x000ABAE8
	private void OnLiftArrivedAtFloor()
	{
		base.SetFlag(BaseEntity.Flags.Reserved3, true, false, true);
		this.MarkDirty();
		base.Invoke(new Action(this.ClearPowerOutput), 10f);
	}

	// Token: 0x06001638 RID: 5688 RVA: 0x00088B46 File Offset: 0x00086D46
	private void ClearPowerOutput()
	{
		base.SetFlag(BaseEntity.Flags.Reserved3, false, false, true);
		this.MarkDirty();
	}

	// Token: 0x06001639 RID: 5689 RVA: 0x000AD915 File Offset: 0x000ABB15
	public override int GetPassthroughAmount(int outputSlot = 0)
	{
		if (!base.HasFlag(BaseEntity.Flags.Reserved3))
		{
			return 0;
		}
		return 1;
	}

	// Token: 0x0600163A RID: 5690 RVA: 0x000AD927 File Offset: 0x000ABB27
	public override void Load(BaseNetworkable.LoadInfo info)
	{
		base.Load(info);
		if (info.fromDisk)
		{
			base.SetFlag(BaseEntity.Flags.Reserved3, false, false, true);
		}
	}

	// Token: 0x04000E80 RID: 3712
	public bool StaticTop;

	// Token: 0x04000E81 RID: 3713
	private const BaseEntity.Flags LiftRecentlyArrived = BaseEntity.Flags.Reserved3;

	// Token: 0x04000E82 RID: 3714
	private List<ElevatorStatic> floorPositions = new List<ElevatorStatic>();

	// Token: 0x04000E83 RID: 3715
	private ElevatorStatic ownerElevator;
}
