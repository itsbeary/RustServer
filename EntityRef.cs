using System;
using UnityEngine;

// Token: 0x020003F1 RID: 1009
public struct EntityRef
{
	// Token: 0x060022BA RID: 8890 RVA: 0x000DF900 File Offset: 0x000DDB00
	public bool IsSet()
	{
		return this.id_cached.IsValid;
	}

	// Token: 0x060022BB RID: 8891 RVA: 0x000DF90D File Offset: 0x000DDB0D
	public bool IsValid(bool serverside)
	{
		return this.Get(serverside).IsValid();
	}

	// Token: 0x060022BC RID: 8892 RVA: 0x000DF91B File Offset: 0x000DDB1B
	public void Set(BaseEntity ent)
	{
		this.ent_cached = ent;
		this.id_cached = default(NetworkableId);
		if (this.ent_cached.IsValid())
		{
			this.id_cached = this.ent_cached.net.ID;
		}
	}

	// Token: 0x060022BD RID: 8893 RVA: 0x000DF954 File Offset: 0x000DDB54
	public BaseEntity Get(bool serverside)
	{
		if (this.ent_cached == null && this.id_cached.IsValid)
		{
			if (serverside)
			{
				this.ent_cached = BaseNetworkable.serverEntities.Find(this.id_cached) as BaseEntity;
			}
			else
			{
				Debug.LogWarning("EntityRef: Looking for clientside entities on pure server!");
			}
		}
		if (!this.ent_cached.IsValid())
		{
			this.ent_cached = null;
		}
		return this.ent_cached;
	}

	// Token: 0x170002E4 RID: 740
	// (get) Token: 0x060022BE RID: 8894 RVA: 0x000DF9C0 File Offset: 0x000DDBC0
	// (set) Token: 0x060022BF RID: 8895 RVA: 0x000DF9EC File Offset: 0x000DDBEC
	public NetworkableId uid
	{
		get
		{
			if (this.ent_cached.IsValid())
			{
				this.id_cached = this.ent_cached.net.ID;
			}
			return this.id_cached;
		}
		set
		{
			this.id_cached = value;
			if (!this.id_cached.IsValid)
			{
				this.ent_cached = null;
				return;
			}
			if (this.ent_cached.IsValid() && this.ent_cached.net.ID == this.id_cached)
			{
				return;
			}
			this.ent_cached = null;
		}
	}

	// Token: 0x04001AA3 RID: 6819
	internal BaseEntity ent_cached;

	// Token: 0x04001AA4 RID: 6820
	internal NetworkableId id_cached;
}
