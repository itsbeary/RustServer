using System;

namespace UnityEngine.Rendering.PostProcessing
{
	// Token: 0x02000AA3 RID: 2723
	public sealed class PropertySheet
	{
		// Token: 0x17000586 RID: 1414
		// (get) Token: 0x060040C0 RID: 16576 RVA: 0x0017D195 File Offset: 0x0017B395
		// (set) Token: 0x060040C1 RID: 16577 RVA: 0x0017D19D File Offset: 0x0017B39D
		public MaterialPropertyBlock properties { get; private set; }

		// Token: 0x17000587 RID: 1415
		// (get) Token: 0x060040C2 RID: 16578 RVA: 0x0017D1A6 File Offset: 0x0017B3A6
		// (set) Token: 0x060040C3 RID: 16579 RVA: 0x0017D1AE File Offset: 0x0017B3AE
		internal Material material { get; private set; }

		// Token: 0x060040C4 RID: 16580 RVA: 0x0017D1B7 File Offset: 0x0017B3B7
		internal PropertySheet(Material material)
		{
			this.material = material;
			this.properties = new MaterialPropertyBlock();
		}

		// Token: 0x060040C5 RID: 16581 RVA: 0x0017D1D1 File Offset: 0x0017B3D1
		public void ClearKeywords()
		{
			this.material.shaderKeywords = null;
		}

		// Token: 0x060040C6 RID: 16582 RVA: 0x0017D1DF File Offset: 0x0017B3DF
		public void EnableKeyword(string keyword)
		{
			this.material.EnableKeyword(keyword);
		}

		// Token: 0x060040C7 RID: 16583 RVA: 0x0017D1ED File Offset: 0x0017B3ED
		public void DisableKeyword(string keyword)
		{
			this.material.DisableKeyword(keyword);
		}

		// Token: 0x060040C8 RID: 16584 RVA: 0x0017D1FB File Offset: 0x0017B3FB
		internal void Release()
		{
			RuntimeUtilities.Destroy(this.material);
			this.material = null;
		}
	}
}
