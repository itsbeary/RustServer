using System;
using UnityEngine;

// Token: 0x02000978 RID: 2424
public class SwapKeycard : MonoBehaviour
{
	// Token: 0x060039E3 RID: 14819 RVA: 0x001567AC File Offset: 0x001549AC
	public void UpdateAccessLevel(int level)
	{
		GameObject[] array = this.accessLevels;
		for (int i = 0; i < array.Length; i++)
		{
			array[i].SetActive(false);
		}
		this.accessLevels[level - 1].SetActive(true);
	}

	// Token: 0x060039E4 RID: 14820 RVA: 0x001567E8 File Offset: 0x001549E8
	public void SetRootActive(int index)
	{
		for (int i = 0; i < this.accessLevels.Length; i++)
		{
			this.accessLevels[i].SetActive(i == index);
		}
	}

	// Token: 0x04003463 RID: 13411
	public GameObject[] accessLevels;
}
