using System;
using UnityEngine;

// Token: 0x020005F3 RID: 1523
public class ItemModDeployable : MonoBehaviour
{
	// Token: 0x06002DA5 RID: 11685 RVA: 0x00113324 File Offset: 0x00111524
	public Deployable GetDeployable(BaseEntity entity)
	{
		if (entity.gameManager.FindPrefab(this.entityPrefab.resourcePath) == null)
		{
			return null;
		}
		return entity.prefabAttribute.Find<Deployable>(this.entityPrefab.resourceID);
	}

	// Token: 0x06002DA6 RID: 11686 RVA: 0x0011335C File Offset: 0x0011155C
	internal void OnDeployed(BaseEntity ent, BasePlayer player)
	{
		if (player.IsValid() && !string.IsNullOrEmpty(this.UnlockAchievement))
		{
			player.GiveAchievement(this.UnlockAchievement);
		}
		BuildingPrivlidge buildingPrivlidge;
		if ((buildingPrivlidge = ent as BuildingPrivlidge) != null)
		{
			buildingPrivlidge.AddPlayer(player);
		}
	}

	// Token: 0x0400254E RID: 9550
	public GameObjectRef entityPrefab = new GameObjectRef();

	// Token: 0x0400254F RID: 9551
	[Header("Tooltips")]
	public bool showCrosshair;

	// Token: 0x04002550 RID: 9552
	public string UnlockAchievement;
}
