using System;
using UnityEngine;

// Token: 0x02000715 RID: 1813
[ExecuteInEditMode]
[RequireComponent(typeof(Camera))]
public class AtmosphereVolumeRenderer : MonoBehaviour
{
	// Token: 0x17000424 RID: 1060
	// (get) Token: 0x060032F9 RID: 13049 RVA: 0x001391D9 File Offset: 0x001373D9
	private static bool isSupported
	{
		get
		{
			return Application.platform != RuntimePlatform.OSXEditor && Application.platform != RuntimePlatform.OSXPlayer;
		}
	}

	// Token: 0x040029B9 RID: 10681
	public FogMode Mode = FogMode.ExponentialSquared;

	// Token: 0x040029BA RID: 10682
	public bool DistanceFog = true;

	// Token: 0x040029BB RID: 10683
	public bool HeightFog = true;

	// Token: 0x040029BC RID: 10684
	public AtmosphereVolume Volume;
}
