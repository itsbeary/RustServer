using System;
using UnityEngine;
using UnityEngine.UI;

// Token: 0x02000889 RID: 2185
public class ToggleTerrainRenderer : MonoBehaviour
{
	// Token: 0x06003694 RID: 13972 RVA: 0x00149675 File Offset: 0x00147875
	protected void OnEnable()
	{
		if (Terrain.activeTerrain)
		{
			this.toggleControl.isOn = Terrain.activeTerrain.drawHeightmap;
		}
	}

	// Token: 0x06003695 RID: 13973 RVA: 0x00149698 File Offset: 0x00147898
	public void OnToggleChanged()
	{
		if (Terrain.activeTerrain)
		{
			Terrain.activeTerrain.drawHeightmap = this.toggleControl.isOn;
		}
	}

	// Token: 0x06003696 RID: 13974 RVA: 0x001496BB File Offset: 0x001478BB
	protected void OnValidate()
	{
		if (this.textControl)
		{
			this.textControl.text = "Terrain Renderer";
		}
	}

	// Token: 0x0400315C RID: 12636
	public Toggle toggleControl;

	// Token: 0x0400315D RID: 12637
	public Text textControl;
}
