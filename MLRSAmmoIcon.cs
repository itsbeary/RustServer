using System;
using UnityEngine;

// Token: 0x0200048B RID: 1163
public class MLRSAmmoIcon : MonoBehaviour
{
	// Token: 0x0600267E RID: 9854 RVA: 0x000F2E30 File Offset: 0x000F1030
	protected void Awake()
	{
		this.SetState(false);
	}

	// Token: 0x0600267F RID: 9855 RVA: 0x000F2E39 File Offset: 0x000F1039
	public void SetState(bool filled)
	{
		this.fill.SetActive(filled);
	}

	// Token: 0x04001ECC RID: 7884
	[SerializeField]
	private GameObject fill;
}
