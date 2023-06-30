using System;
using UnityEngine;

// Token: 0x0200067F RID: 1663
public class DungeonVolume : MonoBehaviour
{
	// Token: 0x06002FF6 RID: 12278 RVA: 0x00120924 File Offset: 0x0011EB24
	public OBB GetBounds(Vector3 position, Quaternion rotation)
	{
		position += rotation * (base.transform.localRotation * this.bounds.center + base.transform.localPosition);
		return new OBB(position, this.bounds.size, rotation * base.transform.localRotation);
	}

	// Token: 0x06002FF7 RID: 12279 RVA: 0x0012098C File Offset: 0x0011EB8C
	public OBB GetBounds(Vector3 position, Quaternion rotation, Vector3 extrude)
	{
		position += rotation * (base.transform.localRotation * this.bounds.center + base.transform.localPosition);
		return new OBB(position, this.bounds.size + extrude, rotation * base.transform.localRotation);
	}

	// Token: 0x04002788 RID: 10120
	public Bounds bounds = new Bounds(Vector3.zero, Vector3.one);
}
