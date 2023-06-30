using System;

// Token: 0x020005B8 RID: 1464
public abstract class WeatherEffectSting : BaseMonoBehaviour, IClientComponent
{
	// Token: 0x0400240C RID: 9228
	public float frequency = 600f;

	// Token: 0x0400240D RID: 9229
	public float variance = 300f;

	// Token: 0x0400240E RID: 9230
	public GameObjectRef[] effects;
}
