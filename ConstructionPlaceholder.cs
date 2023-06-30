using System;
using UnityEngine;
using UnityEngine.Rendering;

// Token: 0x02000257 RID: 599
[ExecuteInEditMode]
public class ConstructionPlaceholder : PrefabAttribute, IPrefabPreProcess
{
	// Token: 0x06001C85 RID: 7301 RVA: 0x000C6A90 File Offset: 0x000C4C90
	protected override void AttributeSetup(GameObject rootObj, string name, bool serverside, bool clientside, bool bundling)
	{
		base.AttributeSetup(rootObj, name, serverside, clientside, bundling);
		if (clientside)
		{
			if (this.renderer)
			{
				UnityEngine.Object component = rootObj.GetComponent<MeshFilter>();
				MeshRenderer meshRenderer = rootObj.GetComponent<MeshRenderer>();
				if (!component)
				{
					rootObj.AddComponent<MeshFilter>().sharedMesh = this.mesh;
				}
				if (!meshRenderer)
				{
					meshRenderer = rootObj.AddComponent<MeshRenderer>();
					meshRenderer.sharedMaterial = this.material;
					meshRenderer.shadowCastingMode = ShadowCastingMode.Off;
				}
			}
			if (this.collider && !rootObj.GetComponent<MeshCollider>())
			{
				rootObj.AddComponent<MeshCollider>().sharedMesh = this.mesh;
			}
		}
	}

	// Token: 0x06001C86 RID: 7302 RVA: 0x000C6B23 File Offset: 0x000C4D23
	protected override Type GetIndexedType()
	{
		return typeof(ConstructionPlaceholder);
	}

	// Token: 0x04001524 RID: 5412
	public Mesh mesh;

	// Token: 0x04001525 RID: 5413
	public Material material;

	// Token: 0x04001526 RID: 5414
	public bool renderer;

	// Token: 0x04001527 RID: 5415
	public bool collider;
}
