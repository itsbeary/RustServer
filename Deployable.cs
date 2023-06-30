using System;
using UnityEngine;

// Token: 0x02000503 RID: 1283
public class Deployable : PrefabAttribute
{
	// Token: 0x0600296D RID: 10605 RVA: 0x000FEBEE File Offset: 0x000FCDEE
	protected override void AttributeSetup(GameObject rootObj, string name, bool serverside, bool clientside, bool bundling)
	{
		base.AttributeSetup(rootObj, name, serverside, clientside, bundling);
		this.bounds = rootObj.GetComponent<BaseEntity>().bounds;
	}

	// Token: 0x0600296E RID: 10606 RVA: 0x000FEC0E File Offset: 0x000FCE0E
	protected override Type GetIndexedType()
	{
		return typeof(Deployable);
	}

	// Token: 0x0400217E RID: 8574
	public Mesh guideMesh;

	// Token: 0x0400217F RID: 8575
	public Vector3 guideMeshScale = Vector3.one;

	// Token: 0x04002180 RID: 8576
	public bool guideLights = true;

	// Token: 0x04002181 RID: 8577
	public bool wantsInstanceData;

	// Token: 0x04002182 RID: 8578
	public bool copyInventoryFromItem;

	// Token: 0x04002183 RID: 8579
	public bool setSocketParent;

	// Token: 0x04002184 RID: 8580
	public bool toSlot;

	// Token: 0x04002185 RID: 8581
	public BaseEntity.Slot slot;

	// Token: 0x04002186 RID: 8582
	public GameObjectRef placeEffect;

	// Token: 0x04002187 RID: 8583
	[NonSerialized]
	public Bounds bounds;
}
