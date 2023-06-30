using System;
using System.Collections.Generic;
using Facepunch;
using ProtoBuf;
using UnityEngine;

// Token: 0x0200060E RID: 1550
[Serializable]
public class ItemAmount : ISerializationCallbackReceiver
{
	// Token: 0x06002DF8 RID: 11768 RVA: 0x00114A93 File Offset: 0x00112C93
	public ItemAmount(ItemDefinition item = null, float amt = 0f)
	{
		this.itemDef = item;
		this.amount = amt;
		this.startAmount = this.amount;
	}

	// Token: 0x170003C7 RID: 967
	// (get) Token: 0x06002DF9 RID: 11769 RVA: 0x00114AB5 File Offset: 0x00112CB5
	public int itemid
	{
		get
		{
			if (this.itemDef == null)
			{
				return 0;
			}
			return this.itemDef.itemid;
		}
	}

	// Token: 0x06002DFA RID: 11770 RVA: 0x00114AD2 File Offset: 0x00112CD2
	public virtual float GetAmount()
	{
		return this.amount;
	}

	// Token: 0x06002DFB RID: 11771 RVA: 0x00114ADA File Offset: 0x00112CDA
	public virtual void OnAfterDeserialize()
	{
		this.startAmount = this.amount;
	}

	// Token: 0x06002DFC RID: 11772 RVA: 0x000063A5 File Offset: 0x000045A5
	public virtual void OnBeforeSerialize()
	{
	}

	// Token: 0x06002DFD RID: 11773 RVA: 0x00114AE8 File Offset: 0x00112CE8
	public static ItemAmountList SerialiseList(List<ItemAmount> list)
	{
		ItemAmountList itemAmountList = Pool.Get<ItemAmountList>();
		itemAmountList.amount = Pool.GetList<float>();
		itemAmountList.itemID = Pool.GetList<int>();
		foreach (ItemAmount itemAmount in list)
		{
			itemAmountList.amount.Add(itemAmount.amount);
			itemAmountList.itemID.Add(itemAmount.itemid);
		}
		return itemAmountList;
	}

	// Token: 0x06002DFE RID: 11774 RVA: 0x00114B70 File Offset: 0x00112D70
	public static void DeserialiseList(List<ItemAmount> target, ItemAmountList source)
	{
		target.Clear();
		if (source.amount.Count != source.itemID.Count)
		{
			return;
		}
		for (int i = 0; i < source.amount.Count; i++)
		{
			target.Add(new ItemAmount(ItemManager.FindItemDefinition(source.itemID[i]), source.amount[i]));
		}
	}

	// Token: 0x040025BE RID: 9662
	[ItemSelector(ItemCategory.All)]
	public ItemDefinition itemDef;

	// Token: 0x040025BF RID: 9663
	public float amount;

	// Token: 0x040025C0 RID: 9664
	[NonSerialized]
	public float startAmount;
}
