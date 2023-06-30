using System;
using System.Collections.Generic;
using Facepunch;
using UnityEngine;

// Token: 0x020002E3 RID: 739
public class Ragdoll : BaseMonoBehaviour, IPrefabPreProcess
{
	// Token: 0x06001E2C RID: 7724 RVA: 0x000CDDF8 File Offset: 0x000CBFF8
	public void PreProcess(IPrefabProcessor preProcess, GameObject rootObj, string name, bool serverside, bool clientside, bool bundling)
	{
		if (!clientside)
		{
			return;
		}
		this.joints.Clear();
		this.characterJoints.Clear();
		this.configurableJoints.Clear();
		this.rigidbodies.Clear();
		base.GetComponentsInChildren<Joint>(true, this.joints);
		base.GetComponentsInChildren<CharacterJoint>(true, this.characterJoints);
		base.GetComponentsInChildren<ConfigurableJoint>(true, this.configurableJoints);
		base.GetComponentsInChildren<Rigidbody>(true, this.rigidbodies);
	}

	// Token: 0x04001744 RID: 5956
	public Transform eyeTransform;

	// Token: 0x04001745 RID: 5957
	public Transform centerBone;

	// Token: 0x04001746 RID: 5958
	public Rigidbody primaryBody;

	// Token: 0x04001747 RID: 5959
	public PhysicMaterial physicMaterial;

	// Token: 0x04001748 RID: 5960
	public SpringJoint corpseJoint;

	// Token: 0x04001749 RID: 5961
	public Skeleton skeleton;

	// Token: 0x0400174A RID: 5962
	public Model model;

	// Token: 0x0400174B RID: 5963
	public List<Joint> joints = new List<Joint>();

	// Token: 0x0400174C RID: 5964
	public List<CharacterJoint> characterJoints = new List<CharacterJoint>();

	// Token: 0x0400174D RID: 5965
	public List<ConfigurableJoint> configurableJoints = new List<ConfigurableJoint>();

	// Token: 0x0400174E RID: 5966
	public List<Rigidbody> rigidbodies = new List<Rigidbody>();

	// Token: 0x0400174F RID: 5967
	public GameObject GibEffect;
}
