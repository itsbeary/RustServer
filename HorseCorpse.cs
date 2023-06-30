using System;
using Facepunch;
using ProtoBuf;

// Token: 0x02000213 RID: 531
public class HorseCorpse : global::LootableCorpse
{
	// Token: 0x17000253 RID: 595
	// (get) Token: 0x06001BA8 RID: 7080 RVA: 0x000C398F File Offset: 0x000C1B8F
	public override string playerName
	{
		get
		{
			return this.lootPanelTitle.translated;
		}
	}

	// Token: 0x06001BA9 RID: 7081 RVA: 0x000C399C File Offset: 0x000C1B9C
	public override void Save(global::BaseNetworkable.SaveInfo info)
	{
		base.Save(info);
		info.msg.horse = Pool.Get<ProtoBuf.Horse>();
		info.msg.horse.breedIndex = this.breedIndex;
	}

	// Token: 0x06001BAA RID: 7082 RVA: 0x000C39CB File Offset: 0x000C1BCB
	public override void Load(global::BaseNetworkable.LoadInfo info)
	{
		base.Load(info);
		if (info.msg.horse != null)
		{
			this.breedIndex = info.msg.horse.breedIndex;
		}
	}

	// Token: 0x0400138A RID: 5002
	public int breedIndex;

	// Token: 0x0400138B RID: 5003
	public Translate.Phrase lootPanelTitle;
}
