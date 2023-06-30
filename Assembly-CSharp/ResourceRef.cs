using System;
using UnityEngine;

// Token: 0x0200096B RID: 2411
[Serializable]
public class ResourceRef<T> where T : UnityEngine.Object
{
	// Token: 0x17000493 RID: 1171
	// (get) Token: 0x060039C0 RID: 14784 RVA: 0x00155FAD File Offset: 0x001541AD
	public bool isValid
	{
		get
		{
			return !string.IsNullOrEmpty(this.guid);
		}
	}

	// Token: 0x060039C1 RID: 14785 RVA: 0x00155FBD File Offset: 0x001541BD
	public T Get()
	{
		if (this._cachedObject == null)
		{
			this._cachedObject = GameManifest.GUIDToObject(this.guid) as T;
		}
		return this._cachedObject;
	}

	// Token: 0x17000494 RID: 1172
	// (get) Token: 0x060039C2 RID: 14786 RVA: 0x00155FF3 File Offset: 0x001541F3
	public string resourcePath
	{
		get
		{
			return GameManifest.GUIDToPath(this.guid);
		}
	}

	// Token: 0x17000495 RID: 1173
	// (get) Token: 0x060039C3 RID: 14787 RVA: 0x00156000 File Offset: 0x00154200
	public uint resourceID
	{
		get
		{
			return StringPool.Get(this.resourcePath);
		}
	}

	// Token: 0x04003429 RID: 13353
	public string guid;

	// Token: 0x0400342A RID: 13354
	private T _cachedObject;
}
