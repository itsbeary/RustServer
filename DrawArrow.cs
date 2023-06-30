using System;
using UnityEngine;

// Token: 0x0200030A RID: 778
public class DrawArrow : MonoBehaviour
{
	// Token: 0x06001ED3 RID: 7891 RVA: 0x000D1ED8 File Offset: 0x000D00D8
	private void OnDrawGizmos()
	{
		Vector3 forward = base.transform.forward;
		Vector3 up = Camera.current.transform.up;
		Vector3 position = base.transform.position;
		Vector3 vector = base.transform.position + forward * this.length;
		Gizmos.color = this.color;
		Gizmos.DrawLine(position, vector);
		Gizmos.DrawLine(vector, vector + up * this.arrowLength - forward * this.arrowLength);
		Gizmos.DrawLine(vector, vector - up * this.arrowLength - forward * this.arrowLength);
		Gizmos.DrawLine(vector + up * this.arrowLength - forward * this.arrowLength, vector - up * this.arrowLength - forward * this.arrowLength);
	}

	// Token: 0x040017C1 RID: 6081
	public Color color = new Color(1f, 1f, 1f, 1f);

	// Token: 0x040017C2 RID: 6082
	public float length = 0.2f;

	// Token: 0x040017C3 RID: 6083
	public float arrowLength = 0.02f;
}
