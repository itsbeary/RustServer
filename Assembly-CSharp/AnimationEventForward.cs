using System;
using UnityEngine;

// Token: 0x02000295 RID: 661
public class AnimationEventForward : MonoBehaviour
{
	// Token: 0x06001D55 RID: 7509 RVA: 0x000CA480 File Offset: 0x000C8680
	public void Event(string type)
	{
		this.targetObject.SendMessage(type);
	}

	// Token: 0x040015FE RID: 5630
	public GameObject targetObject;
}
