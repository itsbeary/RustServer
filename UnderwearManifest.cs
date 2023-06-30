using System;
using System.Collections.Generic;
using UnityEngine;

// Token: 0x0200076B RID: 1899
[CreateAssetMenu(menuName = "Rust/Underwear Manifest")]
public class UnderwearManifest : ScriptableObject
{
	// Token: 0x060034B5 RID: 13493 RVA: 0x00145267 File Offset: 0x00143467
	public static UnderwearManifest Get()
	{
		if (UnderwearManifest.instance == null)
		{
			UnderwearManifest.instance = Resources.Load<UnderwearManifest>("UnderwearManifest");
		}
		return UnderwearManifest.instance;
	}

	// Token: 0x060034B6 RID: 13494 RVA: 0x0014528C File Offset: 0x0014348C
	public void PrintManifest()
	{
		Debug.Log("MANIFEST CONTENTS");
		foreach (Underwear underwear in this.underwears)
		{
			Debug.Log(string.Concat(new object[]
			{
				"Underwear name : ",
				underwear.shortname,
				" underwear ID : ",
				underwear.GetID()
			}));
		}
	}

	// Token: 0x060034B7 RID: 13495 RVA: 0x0014531C File Offset: 0x0014351C
	public Underwear GetUnderwear(uint id)
	{
		foreach (Underwear underwear in this.underwears)
		{
			if (underwear.GetID() == id)
			{
				return underwear;
			}
		}
		return null;
	}

	// Token: 0x04002B53 RID: 11091
	public static UnderwearManifest instance;

	// Token: 0x04002B54 RID: 11092
	public List<Underwear> underwears;
}
