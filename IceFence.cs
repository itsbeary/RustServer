using System;
using UnityEngine;

// Token: 0x02000184 RID: 388
public class IceFence : GraveyardFence
{
	// Token: 0x060017E5 RID: 6117 RVA: 0x000B4270 File Offset: 0x000B2470
	public int GetStyleFromID()
	{
		uint num = (uint)this.net.ID.Value;
		return SeedRandom.Range(ref num, 0, this.styles.Length);
	}

	// Token: 0x060017E6 RID: 6118 RVA: 0x000B429F File Offset: 0x000B249F
	public override void ServerInit()
	{
		base.ServerInit();
		this.InitStyle();
		this.UpdatePillars();
	}

	// Token: 0x060017E7 RID: 6119 RVA: 0x000B42B3 File Offset: 0x000B24B3
	public void InitStyle()
	{
		if (this.init)
		{
			return;
		}
		this.SetStyle(this.GetStyleFromID());
	}

	// Token: 0x060017E8 RID: 6120 RVA: 0x000B42CC File Offset: 0x000B24CC
	public void SetStyle(int style)
	{
		GameObject[] array = this.styles;
		for (int i = 0; i < array.Length; i++)
		{
			array[i].gameObject.SetActive(false);
		}
		this.styles[style].gameObject.SetActive(true);
	}

	// Token: 0x060017E9 RID: 6121 RVA: 0x000B430F File Offset: 0x000B250F
	public override void UpdatePillars()
	{
		base.UpdatePillars();
	}

	// Token: 0x040010A9 RID: 4265
	public GameObject[] styles;

	// Token: 0x040010AA RID: 4266
	private bool init;

	// Token: 0x040010AB RID: 4267
	public AdaptMeshToTerrain snowMesh;
}
