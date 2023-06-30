using System;
using System.Collections.Generic;
using Facepunch;
using UnityEngine;

// Token: 0x020004CC RID: 1228
public class DoorManipulator : IOEntity
{
	// Token: 0x06002820 RID: 10272 RVA: 0x0000441C File Offset: 0x0000261C
	public virtual bool PairWithLockedDoors()
	{
		return true;
	}

	// Token: 0x06002821 RID: 10273 RVA: 0x000F9D94 File Offset: 0x000F7F94
	public virtual void SetTargetDoor(Door newTargetDoor)
	{
		UnityEngine.Object @object = this.targetDoor;
		this.targetDoor = newTargetDoor;
		base.SetFlag(BaseEntity.Flags.On, this.targetDoor != null, false, true);
		this.entityRef.Set(newTargetDoor);
		if (@object != this.targetDoor && this.targetDoor != null)
		{
			this.DoAction();
		}
	}

	// Token: 0x06002822 RID: 10274 RVA: 0x000F9DF0 File Offset: 0x000F7FF0
	public virtual void SetupInitialDoorConnection()
	{
		if (this.targetDoor == null && !this.entityRef.IsValid(true))
		{
			this.SetTargetDoor(this.FindDoor(this.PairWithLockedDoors()));
		}
		if (this.targetDoor != null && !this.entityRef.IsValid(true))
		{
			this.entityRef.Set(this.targetDoor);
		}
		if (this.entityRef.IsValid(true) && this.targetDoor == null)
		{
			this.SetTargetDoor(this.entityRef.Get(true).GetComponent<Door>());
		}
	}

	// Token: 0x06002823 RID: 10275 RVA: 0x000F9E8B File Offset: 0x000F808B
	public override void Init()
	{
		base.Init();
		this.SetupInitialDoorConnection();
	}

	// Token: 0x06002824 RID: 10276 RVA: 0x000F9E9C File Offset: 0x000F809C
	public Door FindDoor(bool allowLocked = true)
	{
		List<Door> list = Pool.GetList<Door>();
		Vis.Entities<Door>(base.transform.position, 1f, list, 2097152, QueryTriggerInteraction.Ignore);
		Door door = null;
		float num = float.PositiveInfinity;
		foreach (Door door2 in list)
		{
			if (door2.isServer)
			{
				if (!allowLocked)
				{
					BaseLock baseLock = door2.GetSlot(BaseEntity.Slot.Lock) as BaseLock;
					if (baseLock != null && baseLock.IsLocked())
					{
						continue;
					}
				}
				float num2 = Vector3.Distance(door2.transform.position, base.transform.position);
				if (num2 < num)
				{
					door = door2;
					num = num2;
				}
			}
		}
		Pool.FreeList<Door>(ref list);
		return door;
	}

	// Token: 0x06002825 RID: 10277 RVA: 0x000F9F70 File Offset: 0x000F8170
	public virtual void DoActionDoorMissing()
	{
		this.SetTargetDoor(this.FindDoor(this.PairWithLockedDoors()));
	}

	// Token: 0x06002826 RID: 10278 RVA: 0x000F9F84 File Offset: 0x000F8184
	public void DoAction()
	{
		bool flag = this.IsPowered();
		if (this.targetDoor == null)
		{
			this.DoActionDoorMissing();
		}
		if (this.targetDoor != null)
		{
			if (this.targetDoor.IsBusy())
			{
				base.Invoke(new Action(this.DoAction), 1f);
				return;
			}
			if (this.powerAction == DoorManipulator.DoorEffect.Open)
			{
				if (flag)
				{
					if (!this.targetDoor.IsOpen())
					{
						this.targetDoor.SetOpen(true, false);
						return;
					}
				}
				else if (this.targetDoor.IsOpen())
				{
					this.targetDoor.SetOpen(false, false);
					return;
				}
			}
			else if (this.powerAction == DoorManipulator.DoorEffect.Close)
			{
				if (flag)
				{
					if (this.targetDoor.IsOpen())
					{
						this.targetDoor.SetOpen(false, false);
						return;
					}
				}
				else if (!this.targetDoor.IsOpen())
				{
					this.targetDoor.SetOpen(true, false);
					return;
				}
			}
			else if (this.powerAction == DoorManipulator.DoorEffect.Toggle)
			{
				if (flag && this.toggle)
				{
					this.targetDoor.SetOpen(!this.targetDoor.IsOpen(), false);
					this.toggle = false;
					return;
				}
				if (!this.toggle)
				{
					this.toggle = true;
				}
			}
		}
	}

	// Token: 0x06002827 RID: 10279 RVA: 0x000FA0B0 File Offset: 0x000F82B0
	public override void IOStateChanged(int inputAmount, int inputSlot)
	{
		base.IOStateChanged(inputAmount, inputSlot);
		this.DoAction();
	}

	// Token: 0x06002828 RID: 10280 RVA: 0x000FA0C0 File Offset: 0x000F82C0
	public override void Save(BaseNetworkable.SaveInfo info)
	{
		base.Save(info);
		info.msg.ioEntity.genericEntRef1 = this.entityRef.uid;
		info.msg.ioEntity.genericInt1 = (int)this.powerAction;
	}

	// Token: 0x06002829 RID: 10281 RVA: 0x000FA0FC File Offset: 0x000F82FC
	public override void Load(BaseNetworkable.LoadInfo info)
	{
		base.Load(info);
		if (info.msg.ioEntity != null)
		{
			this.entityRef.uid = info.msg.ioEntity.genericEntRef1;
			this.powerAction = (DoorManipulator.DoorEffect)info.msg.ioEntity.genericInt1;
		}
	}

	// Token: 0x040020A0 RID: 8352
	public EntityRef entityRef;

	// Token: 0x040020A1 RID: 8353
	public Door targetDoor;

	// Token: 0x040020A2 RID: 8354
	public DoorManipulator.DoorEffect powerAction;

	// Token: 0x040020A3 RID: 8355
	private bool toggle = true;

	// Token: 0x02000D36 RID: 3382
	public enum DoorEffect
	{
		// Token: 0x04004720 RID: 18208
		Close,
		// Token: 0x04004721 RID: 18209
		Open,
		// Token: 0x04004722 RID: 18210
		Toggle
	}
}
