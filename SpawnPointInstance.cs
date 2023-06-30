using System;
using Rust;
using UnityEngine;

// Token: 0x0200057F RID: 1407
public class SpawnPointInstance : MonoBehaviour
{
	// Token: 0x06002B30 RID: 11056 RVA: 0x00106886 File Offset: 0x00104A86
	public void Notify()
	{
		if (!this.parentSpawnPointUser.IsUnityNull<ISpawnPointUser>())
		{
			this.parentSpawnPointUser.ObjectSpawned(this);
		}
		if (this.parentSpawnPoint)
		{
			this.parentSpawnPoint.ObjectSpawned(this);
		}
	}

	// Token: 0x06002B31 RID: 11057 RVA: 0x001068BA File Offset: 0x00104ABA
	public void Retire()
	{
		if (!this.parentSpawnPointUser.IsUnityNull<ISpawnPointUser>())
		{
			this.parentSpawnPointUser.ObjectRetired(this);
		}
		if (this.parentSpawnPoint)
		{
			this.parentSpawnPoint.ObjectRetired(this);
		}
	}

	// Token: 0x06002B32 RID: 11058 RVA: 0x001068EE File Offset: 0x00104AEE
	protected void OnDestroy()
	{
		if (Rust.Application.isQuitting)
		{
			return;
		}
		this.Retire();
	}

	// Token: 0x04002346 RID: 9030
	internal ISpawnPointUser parentSpawnPointUser;

	// Token: 0x04002347 RID: 9031
	internal BaseSpawnPoint parentSpawnPoint;
}
