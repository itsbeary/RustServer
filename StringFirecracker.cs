using System;
using UnityEngine;

// Token: 0x02000145 RID: 325
public class StringFirecracker : TimedExplosive
{
	// Token: 0x0600171E RID: 5918 RVA: 0x000B0828 File Offset: 0x000AEA28
	public override void InitShared()
	{
		base.InitShared();
		if (base.isServer)
		{
			foreach (Rigidbody rigidbody in this.clientParts)
			{
				if (rigidbody != null)
				{
					rigidbody.isKinematic = true;
				}
			}
		}
	}

	// Token: 0x0600171F RID: 5919 RVA: 0x000B086C File Offset: 0x000AEA6C
	public void CreatePinJoint()
	{
		if (this.serverClientJoint != null)
		{
			return;
		}
		this.serverClientJoint = base.gameObject.AddComponent<SpringJoint>();
		this.serverClientJoint.connectedBody = this.clientMiddleBody;
		this.serverClientJoint.autoConfigureConnectedAnchor = false;
		this.serverClientJoint.anchor = Vector3.zero;
		this.serverClientJoint.connectedAnchor = Vector3.zero;
		this.serverClientJoint.minDistance = 0f;
		this.serverClientJoint.maxDistance = 1f;
		this.serverClientJoint.damper = 1000f;
		this.serverClientJoint.spring = 5000f;
		this.serverClientJoint.enableCollision = false;
		this.serverClientJoint.enablePreprocessing = false;
	}

	// Token: 0x04000F6B RID: 3947
	public Rigidbody serverRigidBody;

	// Token: 0x04000F6C RID: 3948
	public Rigidbody clientMiddleBody;

	// Token: 0x04000F6D RID: 3949
	public Rigidbody[] clientParts;

	// Token: 0x04000F6E RID: 3950
	public SpringJoint serverClientJoint;

	// Token: 0x04000F6F RID: 3951
	public Transform clientFirecrackerTransform;
}
