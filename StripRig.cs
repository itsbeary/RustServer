using System;
using System.Collections.Generic;
using Facepunch;
using UnityEngine;

// Token: 0x02000582 RID: 1410
public class StripRig : MonoBehaviour, IPrefabPreProcess
{
	// Token: 0x06002B59 RID: 11097 RVA: 0x00107868 File Offset: 0x00105A68
	public void PreProcess(IPrefabProcessor preProcess, GameObject rootObj, string name, bool serverside, bool clientside, bool bundling)
	{
		if (this.root && ((serverside && this.fromServer) || (clientside && this.fromClient)))
		{
			SkinnedMeshRenderer component = base.GetComponent<SkinnedMeshRenderer>();
			this.Strip(preProcess, component);
		}
		preProcess.RemoveComponent(this);
	}

	// Token: 0x06002B5A RID: 11098 RVA: 0x001078B0 File Offset: 0x00105AB0
	public void Strip(IPrefabProcessor preProcess, SkinnedMeshRenderer skinnedMeshRenderer)
	{
		List<Transform> list = Pool.GetList<Transform>();
		this.root.GetComponentsInChildren<Transform>(list);
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

	// Token: 0x0400235C RID: 9052
	public Transform root;

	// Token: 0x0400235D RID: 9053
	public bool fromClient;

	// Token: 0x0400235E RID: 9054
	public bool fromServer;
}
