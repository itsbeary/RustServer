using System;
using UnityEngine;

// Token: 0x020002C6 RID: 710
public class FollowCamera : MonoBehaviour, IClientComponent
{
	// Token: 0x06001DB7 RID: 7607 RVA: 0x000CC434 File Offset: 0x000CA634
	private void LateUpdate()
	{
		if (MainCamera.mainCamera == null)
		{
			return;
		}
		base.transform.position = MainCamera.position;
	}
}
