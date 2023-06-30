using System;
using UnityEngine;

// Token: 0x02000118 RID: 280
public class PlayerDetectionTrigger : TriggerBase
{
	// Token: 0x06001672 RID: 5746 RVA: 0x000AE3F4 File Offset: 0x000AC5F4
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
		return baseEntity.gameObject;
	}

	// Token: 0x06001673 RID: 5747 RVA: 0x000AE437 File Offset: 0x000AC637
	internal override void OnObjects()
	{
		base.OnObjects();
		this.myDetector.OnObjects();
	}

	// Token: 0x06001674 RID: 5748 RVA: 0x000AE44A File Offset: 0x000AC64A
	internal override void OnEmpty()
	{
		base.OnEmpty();
		this.myDetector.OnEmpty();
	}

	// Token: 0x04000E9B RID: 3739
	public BaseDetector myDetector;
}
