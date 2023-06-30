using System;
using UnityEngine;

// Token: 0x02000021 RID: 33
public class OreHopper : PercentFullStorageContainer
{
	// Token: 0x060000BD RID: 189 RVA: 0x00005DF7 File Offset: 0x00003FF7
	protected override void OnPercentFullChanged(float newPercentFull)
	{
		this.VisualLerpToOreLevel();
	}

	// Token: 0x060000BE RID: 190 RVA: 0x00005E00 File Offset: 0x00004000
	private void SetVisualOreLevel(float percentFull)
	{
		this._oreScale.y = Mathf.Clamp01(percentFull);
		this.oreOutputMesh.localScale = this._oreScale;
		this.oreOutputMesh.gameObject.SetActive(percentFull > 0f);
		this.visualPercentFull = percentFull;
	}

	// Token: 0x060000BF RID: 191 RVA: 0x00005E4E File Offset: 0x0000404E
	public void VisualLerpToOreLevel()
	{
		if (base.GetPercentFull() == this.visualPercentFull)
		{
			return;
		}
		base.InvokeRepeating(new Action(this.OreVisualLerpUpdate), 0f, 0f);
	}

	// Token: 0x060000C0 RID: 192 RVA: 0x00005E7C File Offset: 0x0000407C
	private void OreVisualLerpUpdate()
	{
		float percentFull = base.GetPercentFull();
		if (Mathf.Abs(this.visualPercentFull - percentFull) < 0.005f)
		{
			this.SetVisualOreLevel(percentFull);
			base.CancelInvoke(new Action(this.OreVisualLerpUpdate));
			return;
		}
		float num = Mathf.Lerp(this.visualPercentFull, percentFull, Time.deltaTime * 1.5f);
		this.SetVisualOreLevel(num);
	}

	// Token: 0x060000C1 RID: 193 RVA: 0x00005EDD File Offset: 0x000040DD
	public override void ServerInit()
	{
		base.ServerInit();
		this.SetVisualOreLevel(base.GetPercentFull());
	}

	// Token: 0x040000C0 RID: 192
	[SerializeField]
	private Transform oreOutputMesh;

	// Token: 0x040000C1 RID: 193
	private float visualPercentFull;

	// Token: 0x040000C2 RID: 194
	private Vector3 _oreScale = new Vector3(1f, 0f, 1f);
}
