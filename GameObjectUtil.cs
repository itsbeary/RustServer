using System;
using UnityEngine;

// Token: 0x0200093B RID: 2363
public static class GameObjectUtil
{
	// Token: 0x0600389A RID: 14490 RVA: 0x00150C34 File Offset: 0x0014EE34
	public static void GlobalBroadcast(string messageName, object param = null)
	{
		Transform[] rootObjects = TransformUtil.GetRootObjects();
		for (int i = 0; i < rootObjects.Length; i++)
		{
			rootObjects[i].BroadcastMessage(messageName, param, SendMessageOptions.DontRequireReceiver);
		}
	}
}
