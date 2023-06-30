using System;
using UnityEngine;

// Token: 0x02000273 RID: 627
public class NeighbourSocket : Socket_Base
{
	// Token: 0x06001CE7 RID: 7399 RVA: 0x000C8543 File Offset: 0x000C6743
	private void OnDrawGizmosSelected()
	{
		Gizmos.matrix = base.transform.localToWorldMatrix;
		Gizmos.DrawWireCube(this.selectCenter, this.selectSize);
	}

	// Token: 0x06001CE8 RID: 7400 RVA: 0x00007A44 File Offset: 0x00005C44
	public override bool TestTarget(Construction.Target target)
	{
		return false;
	}

	// Token: 0x06001CE9 RID: 7401 RVA: 0x000C8568 File Offset: 0x000C6768
	public override bool CanConnect(Vector3 position, Quaternion rotation, Socket_Base socket, Vector3 socketPosition, Quaternion socketRotation)
	{
		if (!base.CanConnect(position, rotation, socket, socketPosition, socketRotation))
		{
			return false;
		}
		OBB selectBounds = base.GetSelectBounds(position, rotation);
		OBB selectBounds2 = socket.GetSelectBounds(socketPosition, socketRotation);
		return selectBounds.Intersects(selectBounds2);
	}
}
