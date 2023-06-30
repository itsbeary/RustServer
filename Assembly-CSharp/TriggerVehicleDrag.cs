using System;
using UnityEngine;

// Token: 0x020002F2 RID: 754
public class TriggerVehicleDrag : TriggerBase, IServerComponent
{
	// Token: 0x06001E51 RID: 7761 RVA: 0x000CEA40 File Offset: 0x000CCC40
	internal override GameObject InterestedInObject(GameObject obj)
	{
		obj = base.InterestedInObject(obj);
		if (obj == null)
		{
			return null;
		}
		BaseEntity baseEntity = obj.ToBaseEntity();
		if (baseEntity == null)
		{
			return null;
		}
		if (baseEntity.isClient)
		{
			return null;
		}
		if (this.losEyes != null)
		{
			if (this.entityContents != null && this.entityContents.Contains(baseEntity))
			{
				return baseEntity.gameObject;
			}
			if (!baseEntity.IsVisible(this.losEyes.transform.position, baseEntity.CenterPoint(), float.PositiveInfinity))
			{
				return null;
			}
		}
		return baseEntity.gameObject;
	}

	// Token: 0x04001786 RID: 6022
	[Tooltip("If set, the entering object must have line of sight to this transform to be added, note this is only checked on entry")]
	public Transform losEyes;

	// Token: 0x04001787 RID: 6023
	public float vehicleDrag;
}
