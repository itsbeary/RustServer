using System;
using UnityEngine;

// Token: 0x02000286 RID: 646
public class Socket_Specific_Female : Socket_Base
{
	// Token: 0x06001D31 RID: 7473 RVA: 0x000C9D44 File Offset: 0x000C7F44
	private void OnDrawGizmos()
	{
		Gizmos.matrix = base.transform.localToWorldMatrix;
		Gizmos.color = Color.red;
		Gizmos.DrawLine(Vector3.zero, Vector3.forward * 0.2f);
		Gizmos.color = Color.blue;
		Gizmos.DrawLine(Vector3.zero, Vector3.right * 0.1f);
		Gizmos.color = Color.green;
		Gizmos.DrawLine(Vector3.zero, Vector3.up * 0.1f);
		Gizmos.DrawIcon(base.transform.position, "light_circle_green.png", false);
	}

	// Token: 0x06001D32 RID: 7474 RVA: 0x000C8543 File Offset: 0x000C6743
	private void OnDrawGizmosSelected()
	{
		Gizmos.matrix = base.transform.localToWorldMatrix;
		Gizmos.DrawWireCube(this.selectCenter, this.selectSize);
	}

	// Token: 0x06001D33 RID: 7475 RVA: 0x000C9DE0 File Offset: 0x000C7FE0
	public bool CanAccept(Socket_Specific socket)
	{
		foreach (string text in this.allowedMaleSockets)
		{
			if (socket.targetSocketName == text)
			{
				return true;
			}
		}
		return false;
	}

	// Token: 0x040015C0 RID: 5568
	public int rotationDegrees;

	// Token: 0x040015C1 RID: 5569
	public int rotationOffset;

	// Token: 0x040015C2 RID: 5570
	public string[] allowedMaleSockets;
}
