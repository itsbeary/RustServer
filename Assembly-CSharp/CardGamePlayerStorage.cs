using System;
using Facepunch;
using ProtoBuf;

// Token: 0x0200040F RID: 1039
public class CardGamePlayerStorage : StorageContainer
{
	// Token: 0x0600236A RID: 9066 RVA: 0x000E2484 File Offset: 0x000E0684
	public BaseCardGameEntity GetCardGameEntity()
	{
		global::BaseEntity baseEntity = this.cardTableRef.Get(base.isServer);
		if (baseEntity != null && baseEntity.IsValid())
		{
			return baseEntity as BaseCardGameEntity;
		}
		return null;
	}

	// Token: 0x0600236B RID: 9067 RVA: 0x000E24BC File Offset: 0x000E06BC
	public override void Load(global::BaseNetworkable.LoadInfo info)
	{
		base.Load(info);
		if (info.msg.simpleUID != null)
		{
			this.cardTableRef.uid = info.msg.simpleUID.uid;
		}
	}

	// Token: 0x0600236C RID: 9068 RVA: 0x000E24F0 File Offset: 0x000E06F0
	protected override void OnInventoryDirty()
	{
		base.OnInventoryDirty();
		BaseCardGameEntity cardGameEntity = this.GetCardGameEntity();
		if (cardGameEntity != null)
		{
			cardGameEntity.PlayerStorageChanged();
		}
	}

	// Token: 0x0600236D RID: 9069 RVA: 0x000E2519 File Offset: 0x000E0719
	public override void Save(global::BaseNetworkable.SaveInfo info)
	{
		base.Save(info);
		info.msg.simpleUID = Pool.Get<SimpleUID>();
		info.msg.simpleUID.uid = this.cardTableRef.uid;
	}

	// Token: 0x0600236E RID: 9070 RVA: 0x000E254D File Offset: 0x000E074D
	public void SetCardTable(BaseCardGameEntity cardGameEntity)
	{
		this.cardTableRef.Set(cardGameEntity);
	}

	// Token: 0x04001B40 RID: 6976
	private EntityRef cardTableRef;
}
