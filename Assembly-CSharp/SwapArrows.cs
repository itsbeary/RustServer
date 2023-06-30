using System;
using Rust;
using UnityEngine;

// Token: 0x02000977 RID: 2423
public class SwapArrows : MonoBehaviour, IClientComponent
{
	// Token: 0x060039DC RID: 14812 RVA: 0x0015664D File Offset: 0x0015484D
	public void SelectArrowType(int iType)
	{
		this.HideAllArrowHeads();
		if (iType < this.arrowModels.Length)
		{
			this.arrowModels[iType].SetActive(true);
		}
	}

	// Token: 0x060039DD RID: 14813 RVA: 0x00156670 File Offset: 0x00154870
	public void HideAllArrowHeads()
	{
		GameObject[] array = this.arrowModels;
		for (int i = 0; i < array.Length; i++)
		{
			array[i].SetActive(false);
		}
	}

	// Token: 0x060039DE RID: 14814 RVA: 0x0015669C File Offset: 0x0015489C
	public void UpdateAmmoType(ItemDefinition ammoType, bool hidden = false)
	{
		if (hidden)
		{
			this.wasHidden = hidden;
			this.HideAllArrowHeads();
			return;
		}
		if (this.curAmmoType == ammoType.shortname && hidden == this.wasHidden)
		{
			return;
		}
		this.curAmmoType = ammoType.shortname;
		this.wasHidden = hidden;
		string text = this.curAmmoType;
		if (!(text == "ammo_arrow"))
		{
			if (text == "arrow.bone")
			{
				this.SelectArrowType(0);
				return;
			}
			if (text == "arrow.fire")
			{
				this.SelectArrowType(1);
				return;
			}
			if (text == "arrow.hv")
			{
				this.SelectArrowType(2);
				return;
			}
			if (text == "ammo_arrow_poison")
			{
				this.SelectArrowType(3);
				return;
			}
			if (text == "ammo_arrow_stone")
			{
				this.SelectArrowType(4);
				return;
			}
		}
		this.HideAllArrowHeads();
	}

	// Token: 0x060039DF RID: 14815 RVA: 0x0015676D File Offset: 0x0015496D
	private void Cleanup()
	{
		this.HideAllArrowHeads();
		this.curAmmoType = "";
	}

	// Token: 0x060039E0 RID: 14816 RVA: 0x00156780 File Offset: 0x00154980
	public void OnDisable()
	{
		if (Rust.Application.isQuitting)
		{
			return;
		}
		this.Cleanup();
	}

	// Token: 0x060039E1 RID: 14817 RVA: 0x00156790 File Offset: 0x00154990
	public void OnEnable()
	{
		this.Cleanup();
	}

	// Token: 0x04003460 RID: 13408
	public GameObject[] arrowModels;

	// Token: 0x04003461 RID: 13409
	[NonSerialized]
	private string curAmmoType = "";

	// Token: 0x04003462 RID: 13410
	private bool wasHidden;

	// Token: 0x02000EDA RID: 3802
	public enum ArrowType
	{
		// Token: 0x04004D90 RID: 19856
		One,
		// Token: 0x04004D91 RID: 19857
		Two,
		// Token: 0x04004D92 RID: 19858
		Three,
		// Token: 0x04004D93 RID: 19859
		Four
	}
}
