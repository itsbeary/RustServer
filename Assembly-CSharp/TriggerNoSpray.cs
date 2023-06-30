using System;
using UnityEngine;

// Token: 0x02000591 RID: 1425
public class TriggerNoSpray : TriggerBase
{
	// Token: 0x06002BAE RID: 11182 RVA: 0x001094BC File Offset: 0x001076BC
	private void OnEnable()
	{
		this.cachedTransform = base.transform;
		this.cachedBounds = new OBB(this.cachedTransform, new Bounds(this.TriggerCollider.center, this.TriggerCollider.size));
	}

	// Token: 0x06002BAF RID: 11183 RVA: 0x001094F8 File Offset: 0x001076F8
	internal override GameObject InterestedInObject(GameObject obj)
	{
		BaseEntity baseEntity = obj.ToBaseEntity();
		if (baseEntity == null)
		{
			return null;
		}
		if (baseEntity.ToPlayer() == null)
		{
			return null;
		}
		return baseEntity.gameObject;
	}

	// Token: 0x06002BB0 RID: 11184 RVA: 0x0010952D File Offset: 0x0010772D
	public bool IsPositionValid(Vector3 worldPosition)
	{
		return !this.cachedBounds.Contains(worldPosition);
	}

	// Token: 0x04002394 RID: 9108
	public BoxCollider TriggerCollider;

	// Token: 0x04002395 RID: 9109
	private OBB cachedBounds;

	// Token: 0x04002396 RID: 9110
	private Transform cachedTransform;
}
