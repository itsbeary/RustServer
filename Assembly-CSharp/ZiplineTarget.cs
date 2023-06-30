using System;
using UnityEngine;

// Token: 0x020000FA RID: 250
public class ZiplineTarget : MonoBehaviour
{
	// Token: 0x0600159F RID: 5535 RVA: 0x000AAEEC File Offset: 0x000A90EC
	public bool IsValidPosition(Vector3 position)
	{
		float num = Vector3.Dot((position - this.Target.position.WithY(position.y)).normalized, this.Target.forward);
		return num >= this.MonumentConnectionDotMin && num <= this.MonumentConnectionDotMax;
	}

	// Token: 0x060015A0 RID: 5536 RVA: 0x000AAF48 File Offset: 0x000A9148
	public bool IsValidChainPoint(Vector3 from, Vector3 to)
	{
		float num = Vector3.Dot((from - this.Target.position.WithY(from.y)).normalized, this.Target.forward);
		float num2 = Vector3.Dot((to - this.Target.position.WithY(from.y)).normalized, this.Target.forward);
		if ((num > 0f && num2 > 0f) || (num < 0f && num2 < 0f))
		{
			return false;
		}
		num2 = Mathf.Abs(num2);
		return num2 >= this.MonumentConnectionDotMin && num2 <= this.MonumentConnectionDotMax;
	}

	// Token: 0x04000DC7 RID: 3527
	public Transform Target;

	// Token: 0x04000DC8 RID: 3528
	public bool IsChainPoint;

	// Token: 0x04000DC9 RID: 3529
	public float MonumentConnectionDotMin = 0.2f;

	// Token: 0x04000DCA RID: 3530
	public float MonumentConnectionDotMax = 1f;
}
