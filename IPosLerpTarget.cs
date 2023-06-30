using System;
using System.Collections.Generic;
using Rust.Interpolation;
using UnityEngine;

// Token: 0x020002DF RID: 735
public interface IPosLerpTarget : ILerpInfo
{
	// Token: 0x06001E07 RID: 7687
	float GetInterpolationInertia();

	// Token: 0x06001E08 RID: 7688
	Vector3 GetNetworkPosition();

	// Token: 0x06001E09 RID: 7689
	Quaternion GetNetworkRotation();

	// Token: 0x06001E0A RID: 7690
	void SetNetworkPosition(Vector3 pos);

	// Token: 0x06001E0B RID: 7691
	void SetNetworkRotation(Quaternion rot);

	// Token: 0x06001E0C RID: 7692
	void DrawInterpolationState(Interpolator<TransformSnapshot>.Segment segment, List<TransformSnapshot> entries);

	// Token: 0x06001E0D RID: 7693
	void LerpIdleDisable();
}
