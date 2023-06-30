using System;
using UnityEngine;

// Token: 0x0200056B RID: 1387
public class SceneToPrefabTag : MonoBehaviour, IEditorComponent
{
	// Token: 0x040022E4 RID: 8932
	public SceneToPrefabTag.TagType Type;

	// Token: 0x040022E5 RID: 8933
	public int SpecificLOD;

	// Token: 0x02000D6B RID: 3435
	public enum TagType
	{
		// Token: 0x040047DE RID: 18398
		ForceInclude,
		// Token: 0x040047DF RID: 18399
		ForceExclude,
		// Token: 0x040047E0 RID: 18400
		SingleMaterial,
		// Token: 0x040047E1 RID: 18401
		UseSpecificLOD
	}
}
