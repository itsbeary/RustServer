using System;
using UnityEngine;

// Token: 0x02000536 RID: 1334
public class CopyLODValues : MonoBehaviour, IEditorComponent
{
	// Token: 0x06002A18 RID: 10776 RVA: 0x00101B07 File Offset: 0x000FFD07
	public bool CanCopy()
	{
		return this.source != null && this.destination != null;
	}

	// Token: 0x06002A19 RID: 10777 RVA: 0x00101B28 File Offset: 0x000FFD28
	public void Copy()
	{
		if (!this.CanCopy())
		{
			return;
		}
		LOD[] lods = this.source.GetLODs();
		if (this.scale)
		{
			float num = this.destination.size / this.source.size;
			for (int i = 0; i < lods.Length; i++)
			{
				LOD[] array = lods;
				int num2 = i;
				array[num2].screenRelativeTransitionHeight = array[num2].screenRelativeTransitionHeight * num;
			}
		}
		LOD[] lods2 = this.destination.GetLODs();
		int num3 = 0;
		while (num3 < lods2.Length && num3 < lods.Length)
		{
			int num4 = ((num3 == lods2.Length - 1) ? (lods.Length - 1) : num3);
			lods2[num3].screenRelativeTransitionHeight = lods[num4].screenRelativeTransitionHeight;
			Debug.Log(string.Format("Set destination LOD {0} to {1}", num3, lods2[num3].screenRelativeTransitionHeight));
			num3++;
		}
		this.destination.SetLODs(lods2);
	}

	// Token: 0x04002253 RID: 8787
	[SerializeField]
	private LODGroup source;

	// Token: 0x04002254 RID: 8788
	[SerializeField]
	private LODGroup destination;

	// Token: 0x04002255 RID: 8789
	[Tooltip("Is false, exact values are copied. If true, values are scaled based on LODGroup size, so the changeover point will match.")]
	[SerializeField]
	private bool scale = true;
}
