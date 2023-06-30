using System;
using UnityEngine;

// Token: 0x02000919 RID: 2329
public class RigidbodyInfo : PrefabAttribute, IClientComponent
{
	// Token: 0x06003823 RID: 14371 RVA: 0x0014E389 File Offset: 0x0014C589
	protected override Type GetIndexedType()
	{
		return typeof(RigidbodyInfo);
	}

	// Token: 0x06003824 RID: 14372 RVA: 0x0014E398 File Offset: 0x0014C598
	protected override void AttributeSetup(GameObject rootObj, string name, bool serverside, bool clientside, bool bundling)
	{
		base.AttributeSetup(rootObj, name, serverside, clientside, bundling);
		Rigidbody component = rootObj.GetComponent<Rigidbody>();
		if (component == null)
		{
			Debug.LogError(base.GetType().Name + ": RigidbodyInfo couldn't find a rigidbody on " + name + "! If a RealmedRemove is removing it, make sure this script is above the RealmedRemove script so that this gets processed first.");
			return;
		}
		this.mass = component.mass;
		this.drag = component.drag;
		this.angularDrag = component.angularDrag;
	}

	// Token: 0x0400338F RID: 13199
	[NonSerialized]
	public float mass;

	// Token: 0x04003390 RID: 13200
	[NonSerialized]
	public float drag;

	// Token: 0x04003391 RID: 13201
	[NonSerialized]
	public float angularDrag;
}
