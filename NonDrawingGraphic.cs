using System;
using UnityEngine.UI;

// Token: 0x020008E8 RID: 2280
public class NonDrawingGraphic : Graphic
{
	// Token: 0x06003798 RID: 14232 RVA: 0x000063A5 File Offset: 0x000045A5
	public override void SetMaterialDirty()
	{
	}

	// Token: 0x06003799 RID: 14233 RVA: 0x000063A5 File Offset: 0x000045A5
	public override void SetVerticesDirty()
	{
	}

	// Token: 0x0600379A RID: 14234 RVA: 0x0014CD9E File Offset: 0x0014AF9E
	protected override void OnPopulateMesh(VertexHelper vh)
	{
		vh.Clear();
	}
}
