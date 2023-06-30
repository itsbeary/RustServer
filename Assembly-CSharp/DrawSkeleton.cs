using System;
using UnityEngine;

// Token: 0x0200030B RID: 779
public class DrawSkeleton : MonoBehaviour
{
	// Token: 0x06001ED5 RID: 7893 RVA: 0x000D2014 File Offset: 0x000D0214
	private void OnDrawGizmos()
	{
		Gizmos.color = Color.white;
		DrawSkeleton.DrawTransform(base.transform);
	}

	// Token: 0x06001ED6 RID: 7894 RVA: 0x000D202C File Offset: 0x000D022C
	private static void DrawTransform(Transform t)
	{
		for (int i = 0; i < t.childCount; i++)
		{
			Gizmos.DrawLine(t.position, t.GetChild(i).position);
			DrawSkeleton.DrawTransform(t.GetChild(i));
		}
	}
}
