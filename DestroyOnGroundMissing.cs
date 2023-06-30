using System;
using UnityEngine;

// Token: 0x02000504 RID: 1284
public class DestroyOnGroundMissing : MonoBehaviour, IServerComponent
{
	// Token: 0x06002970 RID: 10608 RVA: 0x000FEC34 File Offset: 0x000FCE34
	private void OnGroundMissing()
	{
		BaseEntity baseEntity = base.gameObject.ToBaseEntity();
		if (baseEntity != null)
		{
			BaseCombatEntity baseCombatEntity = baseEntity as BaseCombatEntity;
			if (baseCombatEntity != null)
			{
				baseCombatEntity.Die(null);
				return;
			}
			baseEntity.Kill(BaseNetworkable.DestroyMode.Gib);
		}
	}
}
