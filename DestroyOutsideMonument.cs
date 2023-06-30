using System;
using UnityEngine;

// Token: 0x020003ED RID: 1005
public class DestroyOutsideMonument : FacepunchBehaviour
{
	// Token: 0x170002E2 RID: 738
	// (get) Token: 0x06002295 RID: 8853 RVA: 0x000DF26C File Offset: 0x000DD46C
	private Vector3 OurPos
	{
		get
		{
			return this.baseCombatEntity.transform.position;
		}
	}

	// Token: 0x06002296 RID: 8854 RVA: 0x000DF280 File Offset: 0x000DD480
	protected void OnEnable()
	{
		if (this.ourMonument == null)
		{
			this.ourMonument = this.GetOurMonument();
		}
		if (this.ourMonument == null)
		{
			this.DoOutsideMonument();
			return;
		}
		base.InvokeRandomized(new Action(this.CheckPosition), this.checkEvery, this.checkEvery, this.checkEvery * 0.1f);
	}

	// Token: 0x06002297 RID: 8855 RVA: 0x000DF2E6 File Offset: 0x000DD4E6
	protected void OnDisable()
	{
		base.CancelInvoke(new Action(this.CheckPosition));
	}

	// Token: 0x06002298 RID: 8856 RVA: 0x000DF2FC File Offset: 0x000DD4FC
	private MonumentInfo GetOurMonument()
	{
		foreach (MonumentInfo monumentInfo in TerrainMeta.Path.Monuments)
		{
			if (monumentInfo.IsInBounds(this.OurPos))
			{
				return monumentInfo;
			}
		}
		return null;
	}

	// Token: 0x06002299 RID: 8857 RVA: 0x000DF364 File Offset: 0x000DD564
	private void CheckPosition()
	{
		if (this.ourMonument == null)
		{
			this.DoOutsideMonument();
		}
		if (!this.ourMonument.IsInBounds(this.OurPos))
		{
			this.DoOutsideMonument();
		}
	}

	// Token: 0x0600229A RID: 8858 RVA: 0x000DF393 File Offset: 0x000DD593
	private void DoOutsideMonument()
	{
		this.baseCombatEntity.Kill(BaseNetworkable.DestroyMode.Gib);
	}

	// Token: 0x04001A95 RID: 6805
	[SerializeField]
	private BaseCombatEntity baseCombatEntity;

	// Token: 0x04001A96 RID: 6806
	[SerializeField]
	private float checkEvery = 10f;

	// Token: 0x04001A97 RID: 6807
	private MonumentInfo ourMonument;
}
