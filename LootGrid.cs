using System;
using System.Collections.Generic;
using UnityEngine;

// Token: 0x02000847 RID: 2119
public class LootGrid : MonoBehaviour
{
	// Token: 0x06003608 RID: 13832 RVA: 0x001478F4 File Offset: 0x00145AF4
	public void CreateInventory(ItemContainerSource inventory, int? slots = null, int? offset = null)
	{
		foreach (ItemIcon itemIcon in this._icons)
		{
			UnityEngine.Object.Destroy(itemIcon.gameObject);
		}
		this._icons.Clear();
		this.Inventory = inventory;
		this.Count = slots ?? this.Count;
		this.Offset = offset ?? this.Offset;
		for (int i = 0; i < this.Count; i++)
		{
			ItemIcon component = UnityEngine.Object.Instantiate<GameObject>(this.ItemIconPrefab, base.transform).GetComponent<ItemIcon>();
			component.slot = this.Offset + i;
			component.emptySlotBackgroundSprite = this.BackgroundImage ?? component.emptySlotBackgroundSprite;
			component.containerSource = inventory;
			this._icons.Add(component);
		}
	}

	// Token: 0x04002FA0 RID: 12192
	public int Container;

	// Token: 0x04002FA1 RID: 12193
	public int Offset;

	// Token: 0x04002FA2 RID: 12194
	public int Count = 1;

	// Token: 0x04002FA3 RID: 12195
	public GameObject ItemIconPrefab;

	// Token: 0x04002FA4 RID: 12196
	public Sprite BackgroundImage;

	// Token: 0x04002FA5 RID: 12197
	public ItemContainerSource Inventory;

	// Token: 0x04002FA6 RID: 12198
	private List<ItemIcon> _icons = new List<ItemIcon>();
}
