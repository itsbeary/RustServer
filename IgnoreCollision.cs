using System;
using UnityEngine;

// Token: 0x02000902 RID: 2306
public class IgnoreCollision : MonoBehaviour
{
	// Token: 0x060037DA RID: 14298 RVA: 0x0014D94A File Offset: 0x0014BB4A
	protected void OnTriggerEnter(Collider other)
	{
		Debug.Log("IgnoreCollision: " + this.collider.gameObject.name + " + " + other.gameObject.name);
		Physics.IgnoreCollision(other, this.collider, true);
	}

	// Token: 0x04003334 RID: 13108
	public Collider collider;
}
