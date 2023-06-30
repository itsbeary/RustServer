using System;
using UnityEngine;

// Token: 0x02000288 RID: 648
public class StabilitySocket : Socket_Base
{
	// Token: 0x06001D39 RID: 7481 RVA: 0x000C8543 File Offset: 0x000C6743
	private void OnDrawGizmosSelected()
	{
		Gizmos.matrix = base.transform.localToWorldMatrix;
		Gizmos.DrawWireCube(this.selectCenter, this.selectSize);
	}

	// Token: 0x06001D3A RID: 7482 RVA: 0x00007A44 File Offset: 0x00005C44
	public override bool TestTarget(Construction.Target target)
	{
		return false;
	}

	// Token: 0x06001D3B RID: 7483 RVA: 0x000C9FFC File Offset: 0x000C81FC
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

	// Token: 0x040015C5 RID: 5573
	[Range(0f, 1f)]
	public float support = 1f;
}
