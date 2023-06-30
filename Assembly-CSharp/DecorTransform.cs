using System;
using UnityEngine;

// Token: 0x0200066D RID: 1645
public class DecorTransform : DecorComponent
{
	// Token: 0x06002FD1 RID: 12241 RVA: 0x0012015C File Offset: 0x0011E35C
	public override void Apply(ref Vector3 pos, ref Quaternion rot, ref Vector3 scale)
	{
		pos += rot * Vector3.Scale(scale, this.Position);
		rot = Quaternion.Euler(this.Rotation) * rot;
		scale = Vector3.Scale(scale, this.Scale);
	}

	// Token: 0x0400273A RID: 10042
	public Vector3 Position = new Vector3(0f, 0f, 0f);

	// Token: 0x0400273B RID: 10043
	public Vector3 Rotation = new Vector3(0f, 0f, 0f);

	// Token: 0x0400273C RID: 10044
	public Vector3 Scale = new Vector3(1f, 1f, 1f);
}
