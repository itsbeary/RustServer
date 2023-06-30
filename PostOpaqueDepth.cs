using System;
using UnityEngine;

// Token: 0x02000735 RID: 1845
[ExecuteInEditMode]
[RequireComponent(typeof(CommandBufferManager))]
public class PostOpaqueDepth : MonoBehaviour
{
	// Token: 0x17000439 RID: 1081
	// (get) Token: 0x06003351 RID: 13137 RVA: 0x0013A17E File Offset: 0x0013837E
	public RenderTexture PostOpaque
	{
		get
		{
			return this.postOpaqueDepth;
		}
	}

	// Token: 0x04002A19 RID: 10777
	public RenderTexture postOpaqueDepth;
}
