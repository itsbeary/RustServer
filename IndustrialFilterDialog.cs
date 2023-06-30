using System;
using Rust.UI;
using UnityEngine;
using UnityEngine.UI;

// Token: 0x020004DD RID: 1245
public class IndustrialFilterDialog : UIDialog
{
	// Token: 0x040020BD RID: 8381
	public GameObjectRef ItemPrefab;

	// Token: 0x040020BE RID: 8382
	public Transform ItemParent;

	// Token: 0x040020BF RID: 8383
	public GameObject ItemSearchParent;

	// Token: 0x040020C0 RID: 8384
	public ItemSearchEntry ItemSearchEntryPrefab;

	// Token: 0x040020C1 RID: 8385
	public VirtualItemIcon TargetItemIcon;

	// Token: 0x040020C2 RID: 8386
	public GameObject TargetCategoryRoot;

	// Token: 0x040020C3 RID: 8387
	public RustText TargetCategoryText;

	// Token: 0x040020C4 RID: 8388
	public Image TargetCategoryImage;

	// Token: 0x040020C5 RID: 8389
	public GameObject NoItemsPrompt;

	// Token: 0x040020C6 RID: 8390
	public Rust.UI.Dropdown FilterModeDropdown;

	// Token: 0x040020C7 RID: 8391
	public GameObject[] FilterModeExplanations;

	// Token: 0x040020C8 RID: 8392
	public GameObject FilterModeBlocker;

	// Token: 0x040020C9 RID: 8393
	public RustText FilterCountText;

	// Token: 0x040020CA RID: 8394
	public GameObject BufferRoot;

	// Token: 0x040020CB RID: 8395
	public GameObjectRef BufferItemPrefab;

	// Token: 0x040020CC RID: 8396
	public Transform BufferTransform;

	// Token: 0x040020CD RID: 8397
	public RustButton PasteButton;

	// Token: 0x040020CE RID: 8398
	public GameObject[] RegularCopyPasteButtons;

	// Token: 0x040020CF RID: 8399
	public GameObject[] JsonCopyPasteButtons;
}
