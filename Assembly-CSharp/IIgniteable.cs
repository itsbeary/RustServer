using System;
using UnityEngine;

// Token: 0x02000418 RID: 1048
public interface IIgniteable
{
	// Token: 0x0600238A RID: 9098
	void Ignite(Vector3 fromPos);

	// Token: 0x0600238B RID: 9099
	bool CanIgnite();
}
