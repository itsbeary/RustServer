using System;
using Facepunch;
using UnityEngine;

// Token: 0x020002D6 RID: 726
public class Model : MonoBehaviour, IPrefabPreProcess
{
	// Token: 0x06001DE6 RID: 7654 RVA: 0x000CCCDB File Offset: 0x000CAEDB
	protected void OnEnable()
	{
		this.skin = -1;
	}

	// Token: 0x06001DE7 RID: 7655 RVA: 0x000CCCE4 File Offset: 0x000CAEE4
	public void BuildBoneDictionary()
	{
		if (this.boneDict != null)
		{
			return;
		}
		this.boneDict = new BoneDictionary(base.transform, this.boneTransforms, this.boneNames);
	}

	// Token: 0x06001DE8 RID: 7656 RVA: 0x000CCD0C File Offset: 0x000CAF0C
	public int GetSkin()
	{
		return this.skin;
	}

	// Token: 0x06001DE9 RID: 7657 RVA: 0x000CCD14 File Offset: 0x000CAF14
	private Transform FindBoneInternal(string name)
	{
		this.BuildBoneDictionary();
		return this.boneDict.FindBone(name, false);
	}

	// Token: 0x06001DEA RID: 7658 RVA: 0x000CCD2C File Offset: 0x000CAF2C
	public Transform FindBone(string name)
	{
		this.BuildBoneDictionary();
		Transform transform = this.rootBone;
		if (string.IsNullOrEmpty(name))
		{
			return transform;
		}
		return this.boneDict.FindBone(name, true);
	}

	// Token: 0x06001DEB RID: 7659 RVA: 0x000CCD60 File Offset: 0x000CAF60
	public Transform FindBone(uint hash)
	{
		this.BuildBoneDictionary();
		Transform transform = this.rootBone;
		if (hash == 0U)
		{
			return transform;
		}
		return this.boneDict.FindBone(hash, true);
	}

	// Token: 0x06001DEC RID: 7660 RVA: 0x000CCD8E File Offset: 0x000CAF8E
	public uint FindBoneID(Transform transform)
	{
		this.BuildBoneDictionary();
		return this.boneDict.FindBoneID(transform);
	}

	// Token: 0x06001DED RID: 7661 RVA: 0x000CCDA2 File Offset: 0x000CAFA2
	public Transform[] GetBones()
	{
		this.BuildBoneDictionary();
		return this.boneDict.transforms;
	}

	// Token: 0x06001DEE RID: 7662 RVA: 0x000CCDB8 File Offset: 0x000CAFB8
	public Transform FindClosestBone(Vector3 worldPos)
	{
		Transform transform = this.rootBone;
		float num = float.MaxValue;
		for (int i = 0; i < this.boneTransforms.Length; i++)
		{
			Transform transform2 = this.boneTransforms[i];
			if (!(transform2 == null))
			{
				float num2 = Vector3.Distance(transform2.position, worldPos);
				if (num2 < num)
				{
					transform = transform2;
					num = num2;
				}
			}
		}
		return transform;
	}

	// Token: 0x06001DEF RID: 7663 RVA: 0x000CCE10 File Offset: 0x000CB010
	public void PreProcess(IPrefabProcessor process, GameObject rootObj, string name, bool serverside, bool clientside, bool bundling)
	{
		if (this == null)
		{
			return;
		}
		if (this.animator == null)
		{
			this.animator = base.GetComponent<Animator>();
		}
		if (this.rootBone == null)
		{
			this.rootBone = base.transform;
		}
		this.boneTransforms = this.rootBone.GetComponentsInChildren<Transform>(true);
		this.boneNames = new string[this.boneTransforms.Length];
		for (int i = 0; i < this.boneTransforms.Length; i++)
		{
			this.boneNames[i] = this.boneTransforms[i].name;
		}
	}

	// Token: 0x040016E4 RID: 5860
	public SphereCollider collision;

	// Token: 0x040016E5 RID: 5861
	public Transform rootBone;

	// Token: 0x040016E6 RID: 5862
	public Transform headBone;

	// Token: 0x040016E7 RID: 5863
	public Transform eyeBone;

	// Token: 0x040016E8 RID: 5864
	public Animator animator;

	// Token: 0x040016E9 RID: 5865
	public Skeleton skeleton;

	// Token: 0x040016EA RID: 5866
	[HideInInspector]
	public Transform[] boneTransforms;

	// Token: 0x040016EB RID: 5867
	[HideInInspector]
	public string[] boneNames;

	// Token: 0x040016EC RID: 5868
	internal BoneDictionary boneDict;

	// Token: 0x040016ED RID: 5869
	internal int skin;
}
