using System;
using UnityEngine;

// Token: 0x02000311 RID: 785
public class ObjectSpam : MonoBehaviour
{
	// Token: 0x06001EEC RID: 7916 RVA: 0x000D25C4 File Offset: 0x000D07C4
	private void Start()
	{
		for (int i = 0; i < this.amount; i++)
		{
			GameObject gameObject = UnityEngine.Object.Instantiate<GameObject>(this.source);
			gameObject.transform.position = base.transform.position + Vector3Ex.Range(-this.radius, this.radius);
			gameObject.hideFlags = HideFlags.HideInHierarchy | HideFlags.HideInInspector;
		}
	}

	// Token: 0x040017D3 RID: 6099
	public GameObject source;

	// Token: 0x040017D4 RID: 6100
	public int amount = 1000;

	// Token: 0x040017D5 RID: 6101
	public float radius;
}
