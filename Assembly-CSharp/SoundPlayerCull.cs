using System;
using UnityEngine;

// Token: 0x0200024B RID: 587
public class SoundPlayerCull : MonoBehaviour, IClientComponent, ILOD
{
	// Token: 0x040014ED RID: 5357
	public SoundPlayer soundPlayer;

	// Token: 0x040014EE RID: 5358
	public float cullDistance = 100f;
}
