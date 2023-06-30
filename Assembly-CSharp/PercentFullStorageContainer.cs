using System;
using Facepunch;
using ProtoBuf;

// Token: 0x020003DC RID: 988
public class PercentFullStorageContainer : StorageContainer
{
	// Token: 0x06002217 RID: 8727 RVA: 0x000DD7CE File Offset: 0x000DB9CE
	public bool IsFull()
	{
		return this.GetPercentFull() == 1f;
	}

	// Token: 0x06002218 RID: 8728 RVA: 0x000DD7DD File Offset: 0x000DB9DD
	public bool IsEmpty()
	{
		return this.GetPercentFull() == 0f;
	}

	// Token: 0x06002219 RID: 8729 RVA: 0x000063A5 File Offset: 0x000045A5
	protected virtual void OnPercentFullChanged(float newPercentFull)
	{
	}

	// Token: 0x0600221A RID: 8730 RVA: 0x000DD7EC File Offset: 0x000DB9EC
	public float GetPercentFull()
	{
		if (base.isServer)
		{
			float num = 0f;
			if (base.inventory != null)
			{
				foreach (global::Item item in base.inventory.itemList)
				{
					num += (float)item.amount / (float)item.MaxStackable();
				}
				num /= (float)base.inventory.capacity;
			}
			return num;
		}
		return 0f;
	}

	// Token: 0x0600221B RID: 8731 RVA: 0x000DD87C File Offset: 0x000DBA7C
	public override void Load(global::BaseNetworkable.LoadInfo info)
	{
		base.Load(info);
		SimpleFloat simpleFloat = info.msg.simpleFloat;
	}

	// Token: 0x0600221C RID: 8732 RVA: 0x000DD891 File Offset: 0x000DBA91
	public override void Save(global::BaseNetworkable.SaveInfo info)
	{
		base.Save(info);
		info.msg.simpleFloat = Pool.Get<SimpleFloat>();
		info.msg.simpleFloat.value = this.GetPercentFull();
	}

	// Token: 0x0600221D RID: 8733 RVA: 0x000DD8C0 File Offset: 0x000DBAC0
	protected override void OnInventoryDirty()
	{
		base.OnInventoryDirty();
		float percentFull = this.GetPercentFull();
		if (percentFull != this.prevPercentFull)
		{
			this.OnPercentFullChanged(percentFull);
			base.SendNetworkUpdate(global::BasePlayer.NetworkQueue.Update);
			this.prevPercentFull = percentFull;
		}
	}

	// Token: 0x04001A65 RID: 6757
	private float prevPercentFull = -1f;
}
