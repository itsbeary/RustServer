using System;
using UnityEngine;

// Token: 0x020006BD RID: 1725
public abstract class ProceduralComponent : MonoBehaviour
{
	// Token: 0x1700040B RID: 1035
	// (get) Token: 0x060031B9 RID: 12729 RVA: 0x00007A44 File Offset: 0x00005C44
	public virtual bool RunOnCache
	{
		get
		{
			return false;
		}
	}

	// Token: 0x060031BA RID: 12730 RVA: 0x00129AD2 File Offset: 0x00127CD2
	public bool ShouldRun()
	{
		return (!World.Cached || this.RunOnCache) && (this.Mode & ProceduralComponent.Realm.Server) != (ProceduralComponent.Realm)0;
	}

	// Token: 0x060031BB RID: 12731
	public abstract void Process(uint seed);

	// Token: 0x04002863 RID: 10339
	[InspectorFlags]
	public ProceduralComponent.Realm Mode = (ProceduralComponent.Realm)(-1);

	// Token: 0x04002864 RID: 10340
	public string Description = "Procedural Component";

	// Token: 0x02000DF7 RID: 3575
	public enum Realm
	{
		// Token: 0x04004A34 RID: 18996
		Client = 1,
		// Token: 0x04004A35 RID: 18997
		Server
	}
}
