using System;
using UnityEngine;

// Token: 0x020004ED RID: 1261
public class TriggeredEventPrefab : TriggeredEvent
{
	// Token: 0x060028D5 RID: 10453 RVA: 0x000FC134 File Offset: 0x000FA334
	private void RunEvent()
	{
		Debug.Log("[event] " + this.targetPrefab.resourcePath);
		BaseEntity baseEntity = GameManager.server.CreateEntity(this.targetPrefab.resourcePath, default(Vector3), default(Quaternion), true);
		if (baseEntity)
		{
			baseEntity.SendMessage("TriggeredEventSpawn", SendMessageOptions.DontRequireReceiver);
			baseEntity.Spawn();
			if (this.shouldBroadcastSpawn)
			{
				foreach (BasePlayer basePlayer in BasePlayer.activePlayerList)
				{
					if (basePlayer && basePlayer.IsConnected)
					{
						basePlayer.ShowToast(GameTip.Styles.Server_Event, this.spawnPhrase, Array.Empty<string>());
					}
				}
			}
		}
	}

	// Token: 0x04002114 RID: 8468
	public GameObjectRef targetPrefab;

	// Token: 0x04002115 RID: 8469
	public bool shouldBroadcastSpawn;

	// Token: 0x04002116 RID: 8470
	public Translate.Phrase spawnPhrase;
}
