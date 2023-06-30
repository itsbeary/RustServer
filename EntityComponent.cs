using System;
using UnityEngine;

// Token: 0x020003C6 RID: 966
public class EntityComponent<T> : EntityComponentBase where T : BaseEntity
{
	// Token: 0x170002CD RID: 717
	// (get) Token: 0x060021C9 RID: 8649 RVA: 0x000DC4A7 File Offset: 0x000DA6A7
	protected T baseEntity
	{
		get
		{
			if (this._baseEntity == null)
			{
				this.UpdateBaseEntity();
			}
			return this._baseEntity;
		}
	}

	// Token: 0x060021CA RID: 8650 RVA: 0x000DC4C8 File Offset: 0x000DA6C8
	protected void UpdateBaseEntity()
	{
		if (!this)
		{
			return;
		}
		if (!base.gameObject)
		{
			return;
		}
		this._baseEntity = base.gameObject.ToBaseEntity() as T;
	}

	// Token: 0x060021CB RID: 8651 RVA: 0x000DC4FC File Offset: 0x000DA6FC
	protected override BaseEntity GetBaseEntity()
	{
		return this.baseEntity;
	}

	// Token: 0x04001A30 RID: 6704
	[NonSerialized]
	private T _baseEntity;
}
