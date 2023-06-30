using System;

// Token: 0x020003F2 RID: 1010
public struct EntityRef<T> where T : BaseEntity
{
	// Token: 0x060022C0 RID: 8896 RVA: 0x000DFA48 File Offset: 0x000DDC48
	public EntityRef(NetworkableId uid)
	{
		this.entityRef = new EntityRef
		{
			uid = uid
		};
	}

	// Token: 0x170002E5 RID: 741
	// (get) Token: 0x060022C1 RID: 8897 RVA: 0x000DFA6C File Offset: 0x000DDC6C
	public bool IsSet
	{
		get
		{
			return this.entityRef.IsSet();
		}
	}

	// Token: 0x060022C2 RID: 8898 RVA: 0x000DFA79 File Offset: 0x000DDC79
	public bool IsValid(bool serverside)
	{
		return this.Get(serverside).IsValid();
	}

	// Token: 0x060022C3 RID: 8899 RVA: 0x000DFA8C File Offset: 0x000DDC8C
	public void Set(T entity)
	{
		this.entityRef.Set(entity);
	}

	// Token: 0x060022C4 RID: 8900 RVA: 0x000DFAA0 File Offset: 0x000DDCA0
	public T Get(bool serverside)
	{
		BaseEntity baseEntity = this.entityRef.Get(serverside);
		if (baseEntity == null)
		{
			return default(T);
		}
		T t;
		if ((t = baseEntity as T) == null)
		{
			this.Set(default(T));
			return default(T);
		}
		return t;
	}

	// Token: 0x060022C5 RID: 8901 RVA: 0x000DFAF5 File Offset: 0x000DDCF5
	public bool TryGet(bool serverside, out T entity)
	{
		entity = this.Get(serverside);
		return entity != null;
	}

	// Token: 0x170002E6 RID: 742
	// (get) Token: 0x060022C6 RID: 8902 RVA: 0x000DFB12 File Offset: 0x000DDD12
	// (set) Token: 0x060022C7 RID: 8903 RVA: 0x000DFB1F File Offset: 0x000DDD1F
	public NetworkableId uid
	{
		get
		{
			return this.entityRef.uid;
		}
		set
		{
			this.entityRef.uid = value;
		}
	}

	// Token: 0x04001AA5 RID: 6821
	private EntityRef entityRef;
}
