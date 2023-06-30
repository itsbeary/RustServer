using System;
using System.Collections.Generic;
using UnityEngine;

// Token: 0x02000298 RID: 664
public class BoneDictionary
{
	// Token: 0x17000269 RID: 617
	// (get) Token: 0x06001D5E RID: 7518 RVA: 0x000CA9F3 File Offset: 0x000C8BF3
	public int Count
	{
		get
		{
			return this.transforms.Length;
		}
	}

	// Token: 0x06001D5F RID: 7519 RVA: 0x000CAA00 File Offset: 0x000C8C00
	public BoneDictionary(Transform rootBone)
	{
		this.transform = rootBone;
		this.transforms = rootBone.GetComponentsInChildren<Transform>(true);
		this.names = new string[this.transforms.Length];
		for (int i = 0; i < this.transforms.Length; i++)
		{
			Transform transform = this.transforms[i];
			if (transform != null)
			{
				this.names[i] = transform.name;
			}
		}
		this.BuildBoneDictionary();
	}

	// Token: 0x06001D60 RID: 7520 RVA: 0x000CAA9C File Offset: 0x000C8C9C
	public BoneDictionary(Transform rootBone, Transform[] boneTransforms, string[] boneNames)
	{
		this.transform = rootBone;
		this.transforms = boneTransforms;
		this.names = boneNames;
		this.BuildBoneDictionary();
	}

	// Token: 0x06001D61 RID: 7521 RVA: 0x000CAAF0 File Offset: 0x000C8CF0
	private void BuildBoneDictionary()
	{
		for (int i = 0; i < this.transforms.Length; i++)
		{
			Transform transform = this.transforms[i];
			string text = this.names[i];
			uint num = StringPool.Get(text);
			if (!this.nameDict.ContainsKey(text))
			{
				this.nameDict.Add(text, transform);
			}
			if (!this.hashDict.ContainsKey(num))
			{
				this.hashDict.Add(num, transform);
			}
			if (transform != null && !this.transformDict.ContainsKey(transform))
			{
				this.transformDict.Add(transform, num);
			}
		}
	}

	// Token: 0x06001D62 RID: 7522 RVA: 0x000CAB88 File Offset: 0x000C8D88
	public Transform FindBone(string name, bool defaultToRoot = true)
	{
		Transform transform = null;
		if (this.nameDict.TryGetValue(name, out transform))
		{
			return transform;
		}
		if (!defaultToRoot)
		{
			return null;
		}
		return this.transform;
	}

	// Token: 0x06001D63 RID: 7523 RVA: 0x000CABB4 File Offset: 0x000C8DB4
	public Transform FindBone(uint hash, bool defaultToRoot = true)
	{
		Transform transform = null;
		if (this.hashDict.TryGetValue(hash, out transform))
		{
			return transform;
		}
		if (!defaultToRoot)
		{
			return null;
		}
		return this.transform;
	}

	// Token: 0x06001D64 RID: 7524 RVA: 0x000CABE0 File Offset: 0x000C8DE0
	public uint FindBoneID(Transform transform)
	{
		uint num;
		if (!this.transformDict.TryGetValue(transform, out num))
		{
			return StringPool.closest;
		}
		return num;
	}

	// Token: 0x04001607 RID: 5639
	public Transform transform;

	// Token: 0x04001608 RID: 5640
	public Transform[] transforms;

	// Token: 0x04001609 RID: 5641
	public string[] names;

	// Token: 0x0400160A RID: 5642
	private Dictionary<string, Transform> nameDict = new Dictionary<string, Transform>(StringComparer.OrdinalIgnoreCase);

	// Token: 0x0400160B RID: 5643
	private Dictionary<uint, Transform> hashDict = new Dictionary<uint, Transform>();

	// Token: 0x0400160C RID: 5644
	private Dictionary<Transform, uint> transformDict = new Dictionary<Transform, uint>();
}
