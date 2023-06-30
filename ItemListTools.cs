using System;
using System.Collections.Generic;
using System.Linq;
using Rust.UI;
using TMPro;
using UnityEngine;
using UnityEngine.UI;

// Token: 0x0200030E RID: 782
public class ItemListTools : MonoBehaviour
{
	// Token: 0x06001EDF RID: 7903 RVA: 0x000D2117 File Offset: 0x000D0317
	public void OnPanelOpened()
	{
		this.CacheAllItems();
		this.Refresh();
		this.searchInputText.InputField.ActivateInputField();
	}

	// Token: 0x06001EE0 RID: 7904 RVA: 0x000D2135 File Offset: 0x000D0335
	private void OnOpenDevTools()
	{
		this.searchInputText.InputField.ActivateInputField();
	}

	// Token: 0x06001EE1 RID: 7905 RVA: 0x000D2147 File Offset: 0x000D0347
	private void CacheAllItems()
	{
		if (this.allItems != null)
		{
			return;
		}
		this.allItems = from x in ItemManager.GetItemDefinitions()
			orderby x.displayName.translated
			select x;
	}

	// Token: 0x06001EE2 RID: 7906 RVA: 0x000D2181 File Offset: 0x000D0381
	public void Refresh()
	{
		this.RebuildCategories();
	}

	// Token: 0x06001EE3 RID: 7907 RVA: 0x000D218C File Offset: 0x000D038C
	private void RebuildCategories()
	{
		for (int i = 0; i < this.categoryButton.transform.parent.childCount; i++)
		{
			Transform child = this.categoryButton.transform.parent.GetChild(i);
			if (!(child == this.categoryButton.transform))
			{
				GameManager.Destroy(child.gameObject, 0f);
			}
		}
		this.categoryButton.SetActive(true);
		foreach (IGrouping<ItemCategory, ItemDefinition> grouping in from x in ItemManager.GetItemDefinitions()
			group x by x.category into x
			orderby x.First<ItemDefinition>().category
			select x)
		{
			GameObject gameObject = UnityEngine.Object.Instantiate<GameObject>(this.categoryButton);
			gameObject.transform.SetParent(this.categoryButton.transform.parent, false);
			gameObject.GetComponentInChildren<TextMeshProUGUI>().text = grouping.First<ItemDefinition>().category.ToString();
			Button btn = gameObject.GetComponentInChildren<Button>();
			ItemDefinition[] itemArray = grouping.ToArray<ItemDefinition>();
			btn.onClick.AddListener(delegate
			{
				if (this.lastCategory)
				{
					this.lastCategory.interactable = true;
				}
				this.lastCategory = btn;
				this.lastCategory.interactable = false;
				this.SwitchItemCategory(itemArray);
			});
			if (this.lastCategory == null)
			{
				this.lastCategory = btn;
				this.lastCategory.interactable = false;
				this.SwitchItemCategory(itemArray);
			}
		}
		this.categoryButton.SetActive(false);
	}

	// Token: 0x06001EE4 RID: 7908 RVA: 0x000D235C File Offset: 0x000D055C
	private void SwitchItemCategory(ItemDefinition[] defs)
	{
		this.currentItems = defs.OrderBy((ItemDefinition x) => x.displayName.translated);
		this.searchInputText.Text = "";
		this.FilterItems(null);
	}

	// Token: 0x06001EE5 RID: 7909 RVA: 0x000D23AC File Offset: 0x000D05AC
	public void FilterItems(string searchText)
	{
		if (this.itemButton == null)
		{
			return;
		}
		for (int i = 0; i < this.itemButton.transform.parent.childCount; i++)
		{
			Transform child = this.itemButton.transform.parent.GetChild(i);
			if (!(child == this.itemButton.transform))
			{
				GameManager.Destroy(child.gameObject, 0f);
			}
		}
		this.itemButton.SetActive(true);
		bool flag = !string.IsNullOrEmpty(searchText);
		string text = (flag ? searchText.ToLower() : null);
		IEnumerable<ItemDefinition> enumerable = (flag ? this.allItems : this.currentItems);
		int num = 0;
		foreach (ItemDefinition itemDefinition in enumerable)
		{
			if (!itemDefinition.hidden && (!flag || itemDefinition.displayName.translated.ToLower().Contains(text)))
			{
				GameObject gameObject = UnityEngine.Object.Instantiate<GameObject>(this.itemButton);
				gameObject.transform.SetParent(this.itemButton.transform.parent, false);
				gameObject.GetComponentInChildren<TextMeshProUGUI>().text = itemDefinition.displayName.translated;
				gameObject.GetComponentInChildren<ItemButtonTools>().itemDef = itemDefinition;
				gameObject.GetComponentInChildren<ItemButtonTools>().image.sprite = itemDefinition.iconSprite;
				num++;
				if (num >= 160)
				{
					break;
				}
			}
		}
		this.itemButton.SetActive(false);
	}

	// Token: 0x040017C8 RID: 6088
	public GameObject categoryButton;

	// Token: 0x040017C9 RID: 6089
	public GameObject itemButton;

	// Token: 0x040017CA RID: 6090
	public RustInput searchInputText;

	// Token: 0x040017CB RID: 6091
	internal Button lastCategory;

	// Token: 0x040017CC RID: 6092
	private IOrderedEnumerable<ItemDefinition> currentItems;

	// Token: 0x040017CD RID: 6093
	private IOrderedEnumerable<ItemDefinition> allItems;
}
