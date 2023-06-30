using System;
using UnityEngine;
using UnityEngine.Rendering;

// Token: 0x020008AC RID: 2220
[ExecuteInEditMode]
public class CameraEx : MonoBehaviour
{
	// Token: 0x0400321A RID: 12826
	public bool overrideAmbientLight;

	// Token: 0x0400321B RID: 12827
	public AmbientMode ambientMode;

	// Token: 0x0400321C RID: 12828
	public Color ambientGroundColor;

	// Token: 0x0400321D RID: 12829
	public Color ambientEquatorColor;

	// Token: 0x0400321E RID: 12830
	public Color ambientLight;

	// Token: 0x0400321F RID: 12831
	public float ambientIntensity;

	// Token: 0x04003220 RID: 12832
	public ReflectionProbe reflectionProbe;

	// Token: 0x04003221 RID: 12833
	internal Color old_ambientLight;

	// Token: 0x04003222 RID: 12834
	internal Color old_ambientGroundColor;

	// Token: 0x04003223 RID: 12835
	internal Color old_ambientEquatorColor;

	// Token: 0x04003224 RID: 12836
	internal float old_ambientIntensity;

	// Token: 0x04003225 RID: 12837
	internal AmbientMode old_ambientMode;

	// Token: 0x04003226 RID: 12838
	public float aspect;
}
