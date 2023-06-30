using System;
using UnityEngine;

// Token: 0x020002CF RID: 719
public class MaterialParameterToggle : MonoBehaviour
{
	// Token: 0x040016AE RID: 5806
	[InspectorFlags]
	public MaterialParameterToggle.ToggleMode Toggle;

	// Token: 0x040016AF RID: 5807
	public Renderer[] TargetRenderers = new Renderer[0];

	// Token: 0x040016B0 RID: 5808
	[ColorUsage(true, true)]
	public Color EmissionColor;

	// Token: 0x02000CAF RID: 3247
	[Flags]
	public enum ToggleMode
	{
		// Token: 0x040044E0 RID: 17632
		Detail = 0,
		// Token: 0x040044E1 RID: 17633
		Emission = 1
	}
}
