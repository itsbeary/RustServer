using System;
using UnityEngine;

// Token: 0x020006EF RID: 1775
public abstract class ProceduralObject : MonoBehaviour
{
	// Token: 0x06003254 RID: 12884 RVA: 0x001366F0 File Offset: 0x001348F0
	protected void Awake()
	{
		if (!(SingletonComponent<WorldSetup>.Instance == null))
		{
			if (SingletonComponent<WorldSetup>.Instance.ProceduralObjects == null)
			{
				Debug.LogError("WorldSetup.Instance.ProceduralObjects is null.", this);
				return;
			}
			SingletonComponent<WorldSetup>.Instance.ProceduralObjects.Add(this);
		}
	}

	// Token: 0x06003255 RID: 12885
	public abstract void Process();
}
