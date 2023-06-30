using System;
using UnityEngine;

// Token: 0x020004E5 RID: 1253
public interface IRFObject
{
	// Token: 0x060028AA RID: 10410
	Vector3 GetPosition();

	// Token: 0x060028AB RID: 10411
	float GetMaxRange();

	// Token: 0x060028AC RID: 10412
	void RFSignalUpdate(bool on);

	// Token: 0x060028AD RID: 10413
	int GetFrequency();
}
