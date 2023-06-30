using System;
using System.Collections.Generic;
using System.Linq;
using UnityEngine;

// Token: 0x02000564 RID: 1380
[CreateAssetMenu(menuName = "Rust/Skeleton Properties")]
public class SkeletonProperties : ScriptableObject
{
	// Token: 0x06002A94 RID: 10900 RVA: 0x0010374C File Offset: 0x0010194C
	public void OnValidate()
	{
		if (this.boneReference == null)
		{
			Debug.LogWarning("boneReference is null on " + base.name, this);
			return;
		}
		List<SkeletonProperties.BoneProperty> list = this.bones.ToList<SkeletonProperties.BoneProperty>();
		using (List<Transform>.Enumerator enumerator = this.boneReference.transform.GetAllChildren().GetEnumerator())
		{
			while (enumerator.MoveNext())
			{
				Transform child = enumerator.Current;
				if (list.All((SkeletonProperties.BoneProperty x) => x.bone != child.gameObject))
				{
					list.Add(new SkeletonProperties.BoneProperty
					{
						bone = child.gameObject,
						name = new Translate.Phrase("", "")
						{
							token = child.name.ToLower(),
							english = child.name.ToLower()
						}
					});
				}
			}
		}
		this.bones = list.ToArray();
	}

	// Token: 0x06002A95 RID: 10901 RVA: 0x00103864 File Offset: 0x00101A64
	private void BuildDictionary()
	{
		this.quickLookup = new Dictionary<uint, SkeletonProperties.BoneProperty>();
		if (this.boneReference == null)
		{
			Debug.LogWarning("boneReference is null on " + base.name, this);
			return;
		}
		foreach (SkeletonProperties.BoneProperty boneProperty in this.bones)
		{
			if (boneProperty == null || boneProperty.bone == null || boneProperty.bone.name == null)
			{
				Debug.LogWarning("Bone error in SkeletonProperties.BuildDictionary for " + this.boneReference.name);
			}
			else
			{
				uint num = StringPool.Get(boneProperty.bone.name);
				if (!this.quickLookup.ContainsKey(num))
				{
					this.quickLookup.Add(num, boneProperty);
				}
				else
				{
					string name = boneProperty.bone.name;
					string name2 = this.quickLookup[num].bone.name;
					Debug.LogWarning(string.Concat(new object[] { "Duplicate bone id ", num, " for ", name, " and ", name2 }));
				}
			}
		}
	}

	// Token: 0x06002A96 RID: 10902 RVA: 0x0010398C File Offset: 0x00101B8C
	public SkeletonProperties.BoneProperty FindBone(uint id)
	{
		if (this.quickLookup == null)
		{
			this.BuildDictionary();
		}
		SkeletonProperties.BoneProperty boneProperty = null;
		if (!this.quickLookup.TryGetValue(id, out boneProperty))
		{
			return null;
		}
		return boneProperty;
	}

	// Token: 0x040022D2 RID: 8914
	public GameObject boneReference;

	// Token: 0x040022D3 RID: 8915
	[BoneProperty]
	public SkeletonProperties.BoneProperty[] bones;

	// Token: 0x040022D4 RID: 8916
	[NonSerialized]
	private Dictionary<uint, SkeletonProperties.BoneProperty> quickLookup;

	// Token: 0x02000D62 RID: 3426
	[Serializable]
	public class BoneProperty
	{
		// Token: 0x040047BA RID: 18362
		public GameObject bone;

		// Token: 0x040047BB RID: 18363
		public Translate.Phrase name;

		// Token: 0x040047BC RID: 18364
		public HitArea area;
	}
}
