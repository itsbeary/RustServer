using System;
using UnityEngine;

// Token: 0x02000664 RID: 1636
public static class DecorComponentEx
{
	// Token: 0x06002FBC RID: 12220 RVA: 0x0011FACC File Offset: 0x0011DCCC
	public static void ApplyDecorComponents(this Transform transform, DecorComponent[] components, ref Vector3 pos, ref Quaternion rot, ref Vector3 scale)
	{
		foreach (DecorComponent decorComponent in components)
		{
			if (!decorComponent.isRoot)
			{
				return;
			}
			decorComponent.Apply(ref pos, ref rot, ref scale);
		}
	}

	// Token: 0x06002FBD RID: 12221 RVA: 0x0011FB00 File Offset: 0x0011DD00
	public static void ApplyDecorComponents(this Transform transform, DecorComponent[] components)
	{
		Vector3 position = transform.position;
		Quaternion rotation = transform.rotation;
		Vector3 localScale = transform.localScale;
		transform.ApplyDecorComponents(components, ref position, ref rotation, ref localScale);
		transform.position = position;
		transform.rotation = rotation;
		transform.localScale = localScale;
	}

	// Token: 0x06002FBE RID: 12222 RVA: 0x0011FB44 File Offset: 0x0011DD44
	public static void ApplyDecorComponentsScaleOnly(this Transform transform, DecorComponent[] components)
	{
		Vector3 position = transform.position;
		Quaternion rotation = transform.rotation;
		Vector3 localScale = transform.localScale;
		transform.ApplyDecorComponents(components, ref position, ref rotation, ref localScale);
		transform.localScale = localScale;
	}
}
