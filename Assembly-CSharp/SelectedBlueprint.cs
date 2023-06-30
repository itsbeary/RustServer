using System;
using Rust.UI;
using UnityEngine;
using UnityEngine.UI;

// Token: 0x0200081D RID: 2077
public class SelectedBlueprint : SingletonComponent<SelectedBlueprint>, IInventoryChanged
{
	// Token: 0x17000450 RID: 1104
	// (get) Token: 0x060035D3 RID: 13779 RVA: 0x00147574 File Offset: 0x00145774
	public static bool isOpen
	{
		get
		{
			return !(SingletonComponent<SelectedBlueprint>.Instance == null) && SingletonComponent<SelectedBlueprint>.Instance.blueprint != null;
		}
	}

	// Token: 0x04002ECD RID: 11981
	public ItemBlueprint blueprint;

	// Token: 0x04002ECE RID: 11982
	public InputField craftAmountText;

	// Token: 0x04002ECF RID: 11983
	public GameObject ingredientGrid;

	// Token: 0x04002ED0 RID: 11984
	public IconSkinPicker skinPicker;

	// Token: 0x04002ED1 RID: 11985
	public Image iconImage;

	// Token: 0x04002ED2 RID: 11986
	public RustText titleText;

	// Token: 0x04002ED3 RID: 11987
	public RustText descriptionText;

	// Token: 0x04002ED4 RID: 11988
	public CanvasGroup CraftArea;

	// Token: 0x04002ED5 RID: 11989
	public Button CraftButton;

	// Token: 0x04002ED6 RID: 11990
	public RustText CraftingTime;

	// Token: 0x04002ED7 RID: 11991
	public RustText CraftingAmount;

	// Token: 0x04002ED8 RID: 11992
	public Sprite FavouriteOnSprite;

	// Token: 0x04002ED9 RID: 11993
	public Sprite FavouriteOffSprite;

	// Token: 0x04002EDA RID: 11994
	public Image FavouriteButtonStatusMarker;

	// Token: 0x04002EDB RID: 11995
	public GameObject[] workbenchReqs;

	// Token: 0x04002EDC RID: 11996
	private ItemInformationPanel[] informationPanels;
}
