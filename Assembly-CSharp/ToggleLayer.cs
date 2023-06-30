using System;
using TMPro;
using UnityEngine;
using UnityEngine.UI;

// Token: 0x02000887 RID: 2183
public class ToggleLayer : MonoBehaviour, IClientComponent
{
	// Token: 0x0600368F RID: 13967 RVA: 0x001495BC File Offset: 0x001477BC
	protected void OnEnable()
	{
		if (MainCamera.mainCamera)
		{
			this.toggleControl.isOn = (MainCamera.mainCamera.cullingMask & this.layer.Mask) != 0;
		}
	}

	// Token: 0x06003690 RID: 13968 RVA: 0x001495F0 File Offset: 0x001477F0
	public void OnToggleChanged()
	{
		if (MainCamera.mainCamera)
		{
			if (this.toggleControl.isOn)
			{
				MainCamera.mainCamera.cullingMask |= this.layer.Mask;
				return;
			}
			MainCamera.mainCamera.cullingMask &= ~this.layer.Mask;
		}
	}

	// Token: 0x06003691 RID: 13969 RVA: 0x00149650 File Offset: 0x00147850
	protected void OnValidate()
	{
		if (this.textControl)
		{
			this.textControl.text = this.layer.Name;
		}
	}

	// Token: 0x04003159 RID: 12633
	public Toggle toggleControl;

	// Token: 0x0400315A RID: 12634
	public TextMeshProUGUI textControl;

	// Token: 0x0400315B RID: 12635
	public LayerSelect layer;
}
