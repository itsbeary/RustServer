using System;
using UnityEngine;

// Token: 0x0200097C RID: 2428
[Serializable]
public struct ViewModelDrawEvent : IEquatable<ViewModelDrawEvent>
{
	// Token: 0x060039EB RID: 14827 RVA: 0x001568F4 File Offset: 0x00154AF4
	public bool Equals(ViewModelDrawEvent other)
	{
		return object.Equals(this.viewModelRenderer, other.viewModelRenderer) && object.Equals(this.renderer, other.renderer) && this.skipDepthPrePass == other.skipDepthPrePass && object.Equals(this.material, other.material) && this.subMesh == other.subMesh && this.pass == other.pass;
	}

	// Token: 0x060039EC RID: 14828 RVA: 0x00156968 File Offset: 0x00154B68
	public override bool Equals(object obj)
	{
		if (obj is ViewModelDrawEvent)
		{
			ViewModelDrawEvent viewModelDrawEvent = (ViewModelDrawEvent)obj;
			return this.Equals(viewModelDrawEvent);
		}
		return false;
	}

	// Token: 0x060039ED RID: 14829 RVA: 0x00156990 File Offset: 0x00154B90
	public override int GetHashCode()
	{
		return (((((((((((this.viewModelRenderer != null) ? this.viewModelRenderer.GetHashCode() : 0) * 397) ^ ((this.renderer != null) ? this.renderer.GetHashCode() : 0)) * 397) ^ this.skipDepthPrePass.GetHashCode()) * 397) ^ ((this.material != null) ? this.material.GetHashCode() : 0)) * 397) ^ this.subMesh) * 397) ^ this.pass;
	}

	// Token: 0x0400346C RID: 13420
	public ViewModelRenderer viewModelRenderer;

	// Token: 0x0400346D RID: 13421
	public Renderer renderer;

	// Token: 0x0400346E RID: 13422
	public bool skipDepthPrePass;

	// Token: 0x0400346F RID: 13423
	public Material material;

	// Token: 0x04003470 RID: 13424
	public int subMesh;

	// Token: 0x04003471 RID: 13425
	public int pass;
}
