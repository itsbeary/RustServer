using System;
using UnityEngine;

// Token: 0x0200023D RID: 573
public class ShoutcastStreamer : MonoBehaviour, IClientComponent
{
	// Token: 0x04001495 RID: 5269
	public string Host = "http://listen.57fm.com:80/rcxmas";

	// Token: 0x04001496 RID: 5270
	public AudioSource Source;
}
