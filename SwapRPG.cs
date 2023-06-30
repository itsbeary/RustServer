using System;
using UnityEngine;

// Token: 0x02000979 RID: 2425
public class SwapRPG : MonoBehaviour
{
	// Token: 0x060039E6 RID: 14822 RVA: 0x0015681C File Offset: 0x00154A1C
	public void SelectRPGType(int iType)
	{
		GameObject[] array = this.rpgModels;
		for (int i = 0; i < array.Length; i++)
		{
			array[i].SetActive(false);
		}
		this.rpgModels[iType].SetActive(true);
	}

	// Token: 0x060039E7 RID: 14823 RVA: 0x00156858 File Offset: 0x00154A58
	public void UpdateAmmoType(ItemDefinition ammoType)
	{
		if (this.curAmmoType == ammoType.shortname)
		{
			return;
		}
		this.curAmmoType = ammoType.shortname;
		string text = this.curAmmoType;
		if (!(text == "ammo.rocket.basic"))
		{
			if (text == "ammo.rocket.fire")
			{
				this.SelectRPGType(1);
				return;
			}
			if (text == "ammo.rocket.hv")
			{
				this.SelectRPGType(2);
				return;
			}
			if (text == "ammo.rocket.smoke")
			{
				this.SelectRPGType(3);
				return;
			}
		}
		this.SelectRPGType(0);
	}

	// Token: 0x060039E8 RID: 14824 RVA: 0x000063A5 File Offset: 0x000045A5
	private void Start()
	{
	}

	// Token: 0x04003464 RID: 13412
	public GameObject[] rpgModels;

	// Token: 0x04003465 RID: 13413
	[NonSerialized]
	private string curAmmoType = "";

	// Token: 0x02000EDB RID: 3803
	public enum RPGType
	{
		// Token: 0x04004D95 RID: 19861
		One,
		// Token: 0x04004D96 RID: 19862
		Two,
		// Token: 0x04004D97 RID: 19863
		Three,
		// Token: 0x04004D98 RID: 19864
		Four
	}
}
