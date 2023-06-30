using System;
using ConVar;
using UnityEngine;

// Token: 0x020002ED RID: 749
public class SunSettings : MonoBehaviour, IClientComponent
{
	// Token: 0x06001E43 RID: 7747 RVA: 0x000CE31F File Offset: 0x000CC51F
	private void OnEnable()
	{
		this.light = base.GetComponent<Light>();
	}

	// Token: 0x06001E44 RID: 7748 RVA: 0x000CE330 File Offset: 0x000CC530
	private void Update()
	{
		LightShadows lightShadows = (LightShadows)Mathf.Clamp(ConVar.Graphics.shadowmode, 1, 2);
		if (this.light.shadows != lightShadows)
		{
			this.light.shadows = lightShadows;
		}
	}

	// Token: 0x0400177F RID: 6015
	private Light light;
}
