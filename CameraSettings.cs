using System;
using ConVar;
using UnityEngine;

// Token: 0x020002A8 RID: 680
public class CameraSettings : MonoBehaviour, IClientComponent
{
	// Token: 0x06001D76 RID: 7542 RVA: 0x000CACFB File Offset: 0x000C8EFB
	private void OnEnable()
	{
		this.cam = base.GetComponent<Camera>();
	}

	// Token: 0x06001D77 RID: 7543 RVA: 0x000CAD09 File Offset: 0x000C8F09
	private void Update()
	{
		this.cam.farClipPlane = Mathf.Clamp(ConVar.Graphics.drawdistance, 500f, 2500f);
	}

	// Token: 0x0400163B RID: 5691
	private Camera cam;
}
