using System;
using Rust;
using UnityEngine;

// Token: 0x020004A2 RID: 1186
public class VehicleLiftOccupantTrigger : TriggerBase
{
	// Token: 0x17000337 RID: 823
	// (get) Token: 0x06002706 RID: 9990 RVA: 0x000F4740 File Offset: 0x000F2940
	// (set) Token: 0x06002707 RID: 9991 RVA: 0x000F4748 File Offset: 0x000F2948
	public ModularCar carOccupant { get; private set; }

	// Token: 0x06002708 RID: 9992 RVA: 0x000F4751 File Offset: 0x000F2951
	protected override void OnDisable()
	{
		if (Rust.Application.isQuitting)
		{
			return;
		}
		base.OnDisable();
		if (this.carOccupant != null)
		{
			this.carOccupant = null;
		}
	}

	// Token: 0x06002709 RID: 9993 RVA: 0x000F4778 File Offset: 0x000F2978
	internal override GameObject InterestedInObject(GameObject obj)
	{
		if (base.InterestedInObject(obj) == null)
		{
			return null;
		}
		BaseEntity baseEntity = obj.ToBaseEntity();
		if (baseEntity == null || baseEntity.isClient)
		{
			return null;
		}
		if (!(baseEntity is ModularCar))
		{
			return null;
		}
		return obj;
	}

	// Token: 0x0600270A RID: 9994 RVA: 0x000F47BB File Offset: 0x000F29BB
	internal override void OnEntityEnter(BaseEntity ent)
	{
		base.OnEntityEnter(ent);
		if (this.carOccupant == null && ent.isServer)
		{
			this.carOccupant = (ModularCar)ent;
		}
	}

	// Token: 0x0600270B RID: 9995 RVA: 0x000F47E8 File Offset: 0x000F29E8
	internal override void OnEntityLeave(BaseEntity ent)
	{
		base.OnEntityLeave(ent);
		if (this.carOccupant == ent)
		{
			this.carOccupant = null;
			if (this.entityContents != null && this.entityContents.Count > 0)
			{
				foreach (BaseEntity baseEntity in this.entityContents)
				{
					if (baseEntity != null)
					{
						this.carOccupant = (ModularCar)baseEntity;
						break;
					}
				}
			}
		}
	}
}
