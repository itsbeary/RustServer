using System;
using UnityEngine;
using UnityEngine.UI;

// Token: 0x020008D6 RID: 2262
public abstract class UIRoot : MonoBehaviour
{
	// Token: 0x0600376F RID: 14191 RVA: 0x0014CAC4 File Offset: 0x0014ACC4
	private void ToggleRaycasters(bool state)
	{
		for (int i = 0; i < this.graphicRaycasters.Length; i++)
		{
			GraphicRaycaster graphicRaycaster = this.graphicRaycasters[i];
			if (graphicRaycaster.enabled != state)
			{
				graphicRaycaster.enabled = state;
			}
		}
	}

	// Token: 0x06003770 RID: 14192 RVA: 0x000063A5 File Offset: 0x000045A5
	protected virtual void Awake()
	{
	}

	// Token: 0x06003771 RID: 14193 RVA: 0x0014CAFD File Offset: 0x0014ACFD
	protected virtual void Start()
	{
		this.graphicRaycasters = base.GetComponentsInChildren<GraphicRaycaster>(true);
	}

	// Token: 0x06003772 RID: 14194 RVA: 0x0014CB0C File Offset: 0x0014AD0C
	protected void Update()
	{
		this.Refresh();
	}

	// Token: 0x06003773 RID: 14195
	protected abstract void Refresh();

	// Token: 0x040032DD RID: 13021
	private GraphicRaycaster[] graphicRaycasters;

	// Token: 0x040032DE RID: 13022
	public Canvas overlayCanvas;
}
