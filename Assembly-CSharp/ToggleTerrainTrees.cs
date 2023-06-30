using System;
using UnityEngine;
using UnityEngine.UI;

// Token: 0x0200088A RID: 2186
public class ToggleTerrainTrees : MonoBehaviour
{
	// Token: 0x06003698 RID: 13976 RVA: 0x001496DA File Offset: 0x001478DA
	protected void OnEnable()
	{
		if (Terrain.activeTerrain)
		{
			this.toggleControl.isOn = Terrain.activeTerrain.drawTreesAndFoliage;
		}
	}

	// Token: 0x06003699 RID: 13977 RVA: 0x001496FD File Offset: 0x001478FD
	public void OnToggleChanged()
	{
		if (Terrain.activeTerrain)
		{
			Terrain.activeTerrain.drawTreesAndFoliage = this.toggleControl.isOn;
		}
	}

	// Token: 0x0600369A RID: 13978 RVA: 0x00149720 File Offset: 0x00147920
	protected void OnValidate()
	{
		if (this.textControl)
		{
			this.textControl.text = "Terrain Trees";
		}
	}

	// Token: 0x0400315E RID: 12638
	public Toggle toggleControl;

	// Token: 0x0400315F RID: 12639
	public Text textControl;
}
