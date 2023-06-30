using System;
using UnityEngine;

// Token: 0x0200050F RID: 1295
public class EnvironmentVolumeTrigger : MonoBehaviour
{
	// Token: 0x17000382 RID: 898
	// (get) Token: 0x06002993 RID: 10643 RVA: 0x000FF39A File Offset: 0x000FD59A
	// (set) Token: 0x06002994 RID: 10644 RVA: 0x000FF3A2 File Offset: 0x000FD5A2
	public EnvironmentVolume volume { get; private set; }

	// Token: 0x06002995 RID: 10645 RVA: 0x000FF3AC File Offset: 0x000FD5AC
	protected void Awake()
	{
		this.volume = base.gameObject.GetComponent<EnvironmentVolume>();
		if (this.volume == null)
		{
			this.volume = base.gameObject.AddComponent<EnvironmentVolume>();
			this.volume.Center = this.Center;
			this.volume.Size = this.Size;
			this.volume.UpdateTrigger();
		}
	}

	// Token: 0x040021A1 RID: 8609
	[HideInInspector]
	public Vector3 Center = Vector3.zero;

	// Token: 0x040021A2 RID: 8610
	[HideInInspector]
	public Vector3 Size = Vector3.one;
}
