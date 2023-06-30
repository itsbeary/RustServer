using System;
using UnityEngine;

// Token: 0x0200069B RID: 1691
public class TerrainCheckGeneratorVolumes : MonoBehaviour, IEditorComponent
{
	// Token: 0x06003031 RID: 12337 RVA: 0x00121C51 File Offset: 0x0011FE51
	protected void OnDrawGizmosSelected()
	{
		Gizmos.color = new Color(0.5f, 0.5f, 0.5f, 1f);
		GizmosUtil.DrawWireCircleY(base.transform.position, this.PlacementRadius);
	}

	// Token: 0x040027DA RID: 10202
	public float PlacementRadius;
}
