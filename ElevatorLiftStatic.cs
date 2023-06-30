using System;
using System.Collections.Generic;
using UnityEngine;

// Token: 0x02000110 RID: 272
public class ElevatorLiftStatic : ElevatorLift
{
	// Token: 0x06001627 RID: 5671 RVA: 0x000AD4A8 File Offset: 0x000AB6A8
	public override void ServerInit()
	{
		base.ServerInit();
		if (this.ElevatorDoorRef.isValid && this.ElevatorDoorLocation != null)
		{
			using (List<BaseEntity>.Enumerator enumerator = this.children.GetEnumerator())
			{
				while (enumerator.MoveNext())
				{
					if (enumerator.Current is Door)
					{
						return;
					}
				}
			}
			BaseEntity baseEntity = GameManager.server.CreateEntity(this.ElevatorDoorRef.resourcePath, this.ElevatorDoorLocation.localPosition, this.ElevatorDoorLocation.localRotation, true);
			baseEntity.SetParent(this, false, false);
			baseEntity.Spawn();
			base.SetFlag(BaseEntity.Flags.Reserved3, false, false, false);
			base.SetFlag(BaseEntity.Flags.Reserved4, true, false, true);
		}
	}

	// Token: 0x06001628 RID: 5672 RVA: 0x000AD57C File Offset: 0x000AB77C
	public override void NotifyNewFloor(int newFloor, int totalFloors)
	{
		base.NotifyNewFloor(newFloor, totalFloors);
		base.SetFlag(BaseEntity.Flags.Reserved3, newFloor < totalFloors, false, true);
		base.SetFlag(BaseEntity.Flags.Reserved4, newFloor > 0, false, true);
	}

	// Token: 0x04000E7B RID: 3707
	public GameObjectRef ElevatorDoorRef;

	// Token: 0x04000E7C RID: 3708
	public Transform ElevatorDoorLocation;

	// Token: 0x04000E7D RID: 3709
	public bool BlockPerFloorMovement;

	// Token: 0x04000E7E RID: 3710
	private const BaseEntity.Flags CanGoUp = BaseEntity.Flags.Reserved3;

	// Token: 0x04000E7F RID: 3711
	private const BaseEntity.Flags CanGoDown = BaseEntity.Flags.Reserved4;
}
