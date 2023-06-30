using System;
using UnityEngine;

// Token: 0x02000998 RID: 2456
public class ExplosionsDeactivateRendererByTime : MonoBehaviour
{
	// Token: 0x06003A4F RID: 14927 RVA: 0x001584BC File Offset: 0x001566BC
	private void Awake()
	{
		this.rend = base.GetComponent<Renderer>();
	}

	// Token: 0x06003A50 RID: 14928 RVA: 0x001584CA File Offset: 0x001566CA
	private void DeactivateRenderer()
	{
		this.rend.enabled = false;
	}

	// Token: 0x06003A51 RID: 14929 RVA: 0x001584D8 File Offset: 0x001566D8
	private void OnEnable()
	{
		this.rend.enabled = true;
		base.Invoke("DeactivateRenderer", this.TimeDelay);
	}

	// Token: 0x040034FC RID: 13564
	public float TimeDelay = 1f;

	// Token: 0x040034FD RID: 13565
	private Renderer rend;
}
