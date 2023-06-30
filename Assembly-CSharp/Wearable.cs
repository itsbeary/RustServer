using System;
using System.Collections.Generic;
using System.Linq;
using Facepunch;
using UnityEngine;

// Token: 0x020005AA RID: 1450
public class Wearable : MonoBehaviour, IItemSetup, IPrefabPreProcess
{
	// Token: 0x06002C3C RID: 11324 RVA: 0x000063A5 File Offset: 0x000045A5
	public void OnItemSetup(Item item)
	{
	}

	// Token: 0x06002C3D RID: 11325 RVA: 0x0010BF9C File Offset: 0x0010A19C
	public virtual void PreProcess(IPrefabProcessor preProcess, GameObject rootObj, string name, bool serverside, bool clientside, bool bundling)
	{
		foreach (LODGroup lodgroup in base.GetComponentsInChildren<LODGroup>(true))
		{
			lodgroup.SetLODs(Wearable.emptyLOD);
			preProcess.RemoveComponent(lodgroup);
		}
	}

	// Token: 0x06002C3E RID: 11326 RVA: 0x0010BFD8 File Offset: 0x0010A1D8
	public void CacheComponents()
	{
		this.playerModelHairCap = base.GetComponent<PlayerModelHairCap>();
		this.playerModelHair = base.GetComponent<PlayerModelHair>();
		this.wearableReplacementByRace = base.GetComponent<WearableReplacementByRace>();
		this.wearableShadowLod = base.GetComponent<WearableShadowLod>();
		base.GetComponentsInChildren<Renderer>(true, this.renderers);
		base.GetComponentsInChildren<PlayerModelSkin>(true, this.playerModelSkins);
		base.GetComponentsInChildren<BoneRetarget>(true, this.boneRetargets);
		base.GetComponentsInChildren<SkinnedMeshRenderer>(true, this.skinnedRenderers);
		base.GetComponentsInChildren<SkeletonSkin>(true, this.skeletonSkins);
		base.GetComponentsInChildren<ComponentInfo>(true, this.componentInfos);
		this.RenderersLod0 = this.renderers.Where((Renderer x) => x.gameObject.name.EndsWith("0")).ToArray<Renderer>();
		this.RenderersLod1 = this.renderers.Where((Renderer x) => x.gameObject.name.EndsWith("1")).ToArray<Renderer>();
		this.RenderersLod2 = this.renderers.Where((Renderer x) => x.gameObject.name.EndsWith("2")).ToArray<Renderer>();
		this.RenderersLod3 = this.renderers.Where((Renderer x) => x.gameObject.name.EndsWith("3")).ToArray<Renderer>();
		this.RenderersLod4 = this.renderers.Where((Renderer x) => x.gameObject.name.EndsWith("4")).ToArray<Renderer>();
		foreach (Renderer renderer in this.renderers)
		{
			renderer.gameObject.AddComponent<ObjectMotionVectorFix>();
			renderer.motionVectorGenerationMode = MotionVectorGenerationMode.ForceNoMotion;
		}
	}

	// Token: 0x06002C3F RID: 11327 RVA: 0x0010C1BC File Offset: 0x0010A3BC
	public void StripRig(IPrefabProcessor preProcess, SkinnedMeshRenderer skinnedMeshRenderer)
	{
		if (this.disableRigStripping)
		{
			return;
		}
		Transform transform = skinnedMeshRenderer.FindRig();
		if (transform != null)
		{
			List<Transform> list = Pool.GetList<Transform>();
			transform.GetComponentsInChildren<Transform>(list);
			for (int i = list.Count - 1; i >= 0; i--)
			{
				if (preProcess != null)
				{
					preProcess.NominateForDeletion(list[i].gameObject);
				}
				else
				{
					UnityEngine.Object.DestroyImmediate(list[i].gameObject);
				}
			}
			Pool.FreeList<Transform>(ref list);
		}
	}

	// Token: 0x06002C40 RID: 11328 RVA: 0x000063A5 File Offset: 0x000045A5
	public void SetupRendererCache(IPrefabProcessor preProcess)
	{
	}

	// Token: 0x040023D4 RID: 9172
	[global::InspectorFlags]
	public Wearable.RemoveSkin removeSkin;

	// Token: 0x040023D5 RID: 9173
	[global::InspectorFlags]
	public Wearable.RemoveSkin removeSkinFirstPerson;

	// Token: 0x040023D6 RID: 9174
	[global::InspectorFlags]
	public Wearable.RemoveHair removeHair;

	// Token: 0x040023D7 RID: 9175
	[global::InspectorFlags]
	public Wearable.DeformHair deformHair;

	// Token: 0x040023D8 RID: 9176
	[global::InspectorFlags]
	public Wearable.OccupationSlots occupationUnder;

	// Token: 0x040023D9 RID: 9177
	[global::InspectorFlags]
	public Wearable.OccupationSlots occupationOver;

	// Token: 0x040023DA RID: 9178
	public bool showCensorshipCube;

	// Token: 0x040023DB RID: 9179
	public bool showCensorshipCubeBreasts;

	// Token: 0x040023DC RID: 9180
	public bool forceHideCensorshipBreasts;

	// Token: 0x040023DD RID: 9181
	public string followBone;

	// Token: 0x040023DE RID: 9182
	public bool disableRigStripping;

	// Token: 0x040023DF RID: 9183
	public bool overrideDownLimit;

	// Token: 0x040023E0 RID: 9184
	public float downLimit = 70f;

	// Token: 0x040023E1 RID: 9185
	[HideInInspector]
	public PlayerModelHair playerModelHair;

	// Token: 0x040023E2 RID: 9186
	[HideInInspector]
	public PlayerModelHairCap playerModelHairCap;

	// Token: 0x040023E3 RID: 9187
	[HideInInspector]
	public WearableReplacementByRace wearableReplacementByRace;

	// Token: 0x040023E4 RID: 9188
	[HideInInspector]
	public WearableShadowLod wearableShadowLod;

	// Token: 0x040023E5 RID: 9189
	[HideInInspector]
	public List<Renderer> renderers = new List<Renderer>();

	// Token: 0x040023E6 RID: 9190
	[HideInInspector]
	public List<PlayerModelSkin> playerModelSkins = new List<PlayerModelSkin>();

	// Token: 0x040023E7 RID: 9191
	[HideInInspector]
	public List<BoneRetarget> boneRetargets = new List<BoneRetarget>();

	// Token: 0x040023E8 RID: 9192
	[HideInInspector]
	public List<SkinnedMeshRenderer> skinnedRenderers = new List<SkinnedMeshRenderer>();

	// Token: 0x040023E9 RID: 9193
	[HideInInspector]
	public List<SkeletonSkin> skeletonSkins = new List<SkeletonSkin>();

	// Token: 0x040023EA RID: 9194
	[HideInInspector]
	public List<ComponentInfo> componentInfos = new List<ComponentInfo>();

	// Token: 0x040023EB RID: 9195
	public bool HideInEyesView;

	// Token: 0x040023EC RID: 9196
	[Header("First Person Legs")]
	[Tooltip("If this is true, we'll hide this item in the first person view. Usually done for items that you definitely won't see in first person view, like facemasks and hats.")]
	public bool HideInFirstPerson;

	// Token: 0x040023ED RID: 9197
	[Tooltip("Use this if the clothing item clips into the player view. It'll push the chest legs model backwards.")]
	[Range(0f, 5f)]
	public float ExtraLeanBack;

	// Token: 0x040023EE RID: 9198
	[Tooltip("Enable this to check for BoneRetargets which need to be preserved in first person view")]
	public bool PreserveBones;

	// Token: 0x040023EF RID: 9199
	public Renderer[] RenderersLod0;

	// Token: 0x040023F0 RID: 9200
	public Renderer[] RenderersLod1;

	// Token: 0x040023F1 RID: 9201
	public Renderer[] RenderersLod2;

	// Token: 0x040023F2 RID: 9202
	public Renderer[] RenderersLod3;

	// Token: 0x040023F3 RID: 9203
	public Renderer[] RenderersLod4;

	// Token: 0x040023F4 RID: 9204
	public Renderer[] SkipInFirstPersonLegs;

	// Token: 0x040023F5 RID: 9205
	public WearableNotify[] Notifies;

	// Token: 0x040023F6 RID: 9206
	private static LOD[] emptyLOD = new LOD[1];

	// Token: 0x040023F7 RID: 9207
	public Wearable.PartRandomizer[] randomParts;

	// Token: 0x02000D7D RID: 3453
	[Flags]
	public enum RemoveSkin
	{
		// Token: 0x0400481A RID: 18458
		Torso = 1,
		// Token: 0x0400481B RID: 18459
		Feet = 2,
		// Token: 0x0400481C RID: 18460
		Hands = 4,
		// Token: 0x0400481D RID: 18461
		Legs = 8,
		// Token: 0x0400481E RID: 18462
		Head = 16
	}

	// Token: 0x02000D7E RID: 3454
	[Flags]
	public enum RemoveHair
	{
		// Token: 0x04004820 RID: 18464
		Head = 1,
		// Token: 0x04004821 RID: 18465
		Eyebrow = 2,
		// Token: 0x04004822 RID: 18466
		Facial = 4,
		// Token: 0x04004823 RID: 18467
		Armpit = 8,
		// Token: 0x04004824 RID: 18468
		Pubic = 16
	}

	// Token: 0x02000D7F RID: 3455
	[Flags]
	public enum DeformHair
	{
		// Token: 0x04004826 RID: 18470
		None = 0,
		// Token: 0x04004827 RID: 18471
		BaseballCap = 1,
		// Token: 0x04004828 RID: 18472
		BoonieHat = 2,
		// Token: 0x04004829 RID: 18473
		CandleHat = 3,
		// Token: 0x0400482A RID: 18474
		MinersHat = 4,
		// Token: 0x0400482B RID: 18475
		WoodHelmet = 5
	}

	// Token: 0x02000D80 RID: 3456
	[Flags]
	public enum OccupationSlots
	{
		// Token: 0x0400482D RID: 18477
		HeadTop = 1,
		// Token: 0x0400482E RID: 18478
		Face = 2,
		// Token: 0x0400482F RID: 18479
		HeadBack = 4,
		// Token: 0x04004830 RID: 18480
		TorsoFront = 8,
		// Token: 0x04004831 RID: 18481
		TorsoBack = 16,
		// Token: 0x04004832 RID: 18482
		LeftShoulder = 32,
		// Token: 0x04004833 RID: 18483
		RightShoulder = 64,
		// Token: 0x04004834 RID: 18484
		LeftArm = 128,
		// Token: 0x04004835 RID: 18485
		RightArm = 256,
		// Token: 0x04004836 RID: 18486
		LeftHand = 512,
		// Token: 0x04004837 RID: 18487
		RightHand = 1024,
		// Token: 0x04004838 RID: 18488
		Groin = 2048,
		// Token: 0x04004839 RID: 18489
		Bum = 4096,
		// Token: 0x0400483A RID: 18490
		LeftKnee = 8192,
		// Token: 0x0400483B RID: 18491
		RightKnee = 16384,
		// Token: 0x0400483C RID: 18492
		LeftLeg = 32768,
		// Token: 0x0400483D RID: 18493
		RightLeg = 65536,
		// Token: 0x0400483E RID: 18494
		LeftFoot = 131072,
		// Token: 0x0400483F RID: 18495
		RightFoot = 262144,
		// Token: 0x04004840 RID: 18496
		Mouth = 524288,
		// Token: 0x04004841 RID: 18497
		Eyes = 1048576
	}

	// Token: 0x02000D81 RID: 3457
	[Serializable]
	public struct PartRandomizer
	{
		// Token: 0x04004842 RID: 18498
		public Wearable.PartCollection[] groups;
	}

	// Token: 0x02000D82 RID: 3458
	[Serializable]
	public struct PartCollection
	{
		// Token: 0x04004843 RID: 18499
		public GameObject[] parts;
	}
}
