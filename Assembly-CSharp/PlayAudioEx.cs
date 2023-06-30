using System;
using UnityEngine;

// Token: 0x020002D9 RID: 729
public class PlayAudioEx : MonoBehaviour
{
	// Token: 0x06001DF5 RID: 7669 RVA: 0x000063A5 File Offset: 0x000045A5
	private void Start()
	{
	}

	// Token: 0x06001DF6 RID: 7670 RVA: 0x000CCF38 File Offset: 0x000CB138
	private void OnEnable()
	{
		AudioSource component = base.GetComponent<AudioSource>();
		if (component)
		{
			component.PlayDelayed(this.delay);
		}
	}

	// Token: 0x040016F2 RID: 5874
	public float delay;
}
