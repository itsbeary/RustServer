using System;
using System.Collections.Generic;
using Facepunch;

// Token: 0x020003EF RID: 1007
public class EntityLink : Pool.IPooled
{
	// Token: 0x170002E3 RID: 739
	// (get) Token: 0x060022A9 RID: 8873 RVA: 0x000DF6D4 File Offset: 0x000DD8D4
	public string name
	{
		get
		{
			return this.socket.socketName;
		}
	}

	// Token: 0x060022AA RID: 8874 RVA: 0x000DF6E1 File Offset: 0x000DD8E1
	public void Setup(BaseEntity owner, Socket_Base socket)
	{
		this.owner = owner;
		this.socket = socket;
		if (socket.monogamous)
		{
			this.capacity = 1;
		}
	}

	// Token: 0x060022AB RID: 8875 RVA: 0x000DF700 File Offset: 0x000DD900
	public void EnterPool()
	{
		this.owner = null;
		this.socket = null;
		this.capacity = int.MaxValue;
	}

	// Token: 0x060022AC RID: 8876 RVA: 0x000063A5 File Offset: 0x000045A5
	public void LeavePool()
	{
	}

	// Token: 0x060022AD RID: 8877 RVA: 0x000DF71B File Offset: 0x000DD91B
	public bool Contains(EntityLink entity)
	{
		return this.connections.Contains(entity);
	}

	// Token: 0x060022AE RID: 8878 RVA: 0x000DF729 File Offset: 0x000DD929
	public void Add(EntityLink entity)
	{
		this.connections.Add(entity);
	}

	// Token: 0x060022AF RID: 8879 RVA: 0x000DF737 File Offset: 0x000DD937
	public void Remove(EntityLink entity)
	{
		this.connections.Remove(entity);
	}

	// Token: 0x060022B0 RID: 8880 RVA: 0x000DF748 File Offset: 0x000DD948
	public void Clear()
	{
		for (int i = 0; i < this.connections.Count; i++)
		{
			this.connections[i].Remove(this);
		}
		this.connections.Clear();
	}

	// Token: 0x060022B1 RID: 8881 RVA: 0x000DF788 File Offset: 0x000DD988
	public bool IsEmpty()
	{
		return this.connections.Count == 0;
	}

	// Token: 0x060022B2 RID: 8882 RVA: 0x000DF798 File Offset: 0x000DD998
	public bool IsOccupied()
	{
		return this.connections.Count >= this.capacity;
	}

	// Token: 0x060022B3 RID: 8883 RVA: 0x000DF7B0 File Offset: 0x000DD9B0
	public bool IsMale()
	{
		return this.socket.male;
	}

	// Token: 0x060022B4 RID: 8884 RVA: 0x000DF7BD File Offset: 0x000DD9BD
	public bool IsFemale()
	{
		return this.socket.female;
	}

	// Token: 0x060022B5 RID: 8885 RVA: 0x000DF7CC File Offset: 0x000DD9CC
	public bool CanConnect(EntityLink link)
	{
		return !this.IsOccupied() && link != null && !link.IsOccupied() && this.socket.CanConnect(this.owner.transform.position, this.owner.transform.rotation, link.socket, link.owner.transform.position, link.owner.transform.rotation);
	}

	// Token: 0x04001A9F RID: 6815
	public BaseEntity owner;

	// Token: 0x04001AA0 RID: 6816
	public Socket_Base socket;

	// Token: 0x04001AA1 RID: 6817
	public List<EntityLink> connections = new List<EntityLink>(8);

	// Token: 0x04001AA2 RID: 6818
	public int capacity = int.MaxValue;
}
