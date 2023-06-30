using System;

// Token: 0x02000969 RID: 2409
public class NetworkedProperty<T> where T : IEquatable<T>
{
	// Token: 0x17000492 RID: 1170
	// (get) Token: 0x060039B9 RID: 14777 RVA: 0x00155F35 File Offset: 0x00154135
	// (set) Token: 0x060039BA RID: 14778 RVA: 0x00155F3D File Offset: 0x0015413D
	public T Value
	{
		get
		{
			return this.val;
		}
		set
		{
			if (!this.val.Equals(value))
			{
				this.val = value;
				if (this.entity.isServer)
				{
					this.entity.SendNetworkUpdate(BasePlayer.NetworkQueue.Update);
				}
			}
		}
	}

	// Token: 0x060039BB RID: 14779 RVA: 0x00155F73 File Offset: 0x00154173
	public NetworkedProperty(BaseEntity entity)
	{
		this.entity = entity;
	}

	// Token: 0x060039BC RID: 14780 RVA: 0x00155F82 File Offset: 0x00154182
	public static implicit operator T(NetworkedProperty<T> value)
	{
		return value.Value;
	}

	// Token: 0x04003427 RID: 13351
	private T val;

	// Token: 0x04003428 RID: 13352
	private BaseEntity entity;
}
