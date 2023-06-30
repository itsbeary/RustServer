using System;
using UnityEngine;

// Token: 0x02000680 RID: 1664
public class BiomeVisuals : MonoBehaviour
{
	// Token: 0x06002FF9 RID: 12281 RVA: 0x00120A18 File Offset: 0x0011EC18
	protected void Start()
	{
		int num = ((TerrainMeta.BiomeMap != null) ? TerrainMeta.BiomeMap.GetBiomeMaxType(base.transform.position, -1) : 2);
		switch (num)
		{
		case 1:
			this.SetChoice(this.Arid);
			return;
		case 2:
			this.SetChoice(this.Temperate);
			return;
		case 3:
			break;
		case 4:
			this.SetChoice(this.Tundra);
			return;
		default:
			if (num != 8)
			{
				return;
			}
			this.SetChoice(this.Arctic);
			break;
		}
	}

	// Token: 0x06002FFA RID: 12282 RVA: 0x00120A9C File Offset: 0x0011EC9C
	private void SetChoice(GameObject selection)
	{
		bool flag = !base.gameObject.SupportsPoolingInParent();
		this.ApplyChoice(selection, this.Arid, flag);
		this.ApplyChoice(selection, this.Temperate, flag);
		this.ApplyChoice(selection, this.Tundra, flag);
		this.ApplyChoice(selection, this.Arctic, flag);
		if (selection != null)
		{
			selection.SetActive(true);
		}
		GameManager.Destroy(this, 0f);
	}

	// Token: 0x06002FFB RID: 12283 RVA: 0x00120B0B File Offset: 0x0011ED0B
	private void ApplyChoice(GameObject selection, GameObject target, bool shouldDestroy)
	{
		if (target != null && target != selection)
		{
			if (shouldDestroy)
			{
				GameManager.Destroy(target, 0f);
				return;
			}
			target.SetActive(false);
		}
	}

	// Token: 0x04002789 RID: 10121
	public GameObject Arid;

	// Token: 0x0400278A RID: 10122
	public GameObject Temperate;

	// Token: 0x0400278B RID: 10123
	public GameObject Tundra;

	// Token: 0x0400278C RID: 10124
	public GameObject Arctic;
}
