using System;
using Rust;
using UnityEngine;
using UnityEngine.AI;

// Token: 0x020003D5 RID: 981
public class Barricade : DecayEntity
{
	// Token: 0x06002200 RID: 8704 RVA: 0x000DD39C File Offset: 0x000DB59C
	public override void ServerInit()
	{
		base.ServerInit();
		if (Barricade.nonWalkableArea < 0)
		{
			Barricade.nonWalkableArea = NavMesh.GetAreaFromName("Not Walkable");
		}
		if (Barricade.animalAgentTypeId < 0)
		{
			Barricade.animalAgentTypeId = NavMesh.GetSettingsByIndex(1).agentTypeID;
		}
		if (this.NavMeshVolumeAnimals == null)
		{
			this.NavMeshVolumeAnimals = base.gameObject.AddComponent<NavMeshModifierVolume>();
			this.NavMeshVolumeAnimals.area = Barricade.nonWalkableArea;
			this.NavMeshVolumeAnimals.AddAgentType(Barricade.animalAgentTypeId);
			this.NavMeshVolumeAnimals.center = Vector3.zero;
			this.NavMeshVolumeAnimals.size = Vector3.one;
		}
		if (!this.canNpcSmash)
		{
			if (Barricade.humanoidAgentTypeId < 0)
			{
				Barricade.humanoidAgentTypeId = NavMesh.GetSettingsByIndex(0).agentTypeID;
			}
			if (this.NavMeshVolumeHumanoids == null)
			{
				this.NavMeshVolumeHumanoids = base.gameObject.AddComponent<NavMeshModifierVolume>();
				this.NavMeshVolumeHumanoids.area = Barricade.nonWalkableArea;
				this.NavMeshVolumeHumanoids.AddAgentType(Barricade.humanoidAgentTypeId);
				this.NavMeshVolumeHumanoids.center = Vector3.zero;
				this.NavMeshVolumeHumanoids.size = Vector3.one;
				return;
			}
		}
		else if (this.NpcTriggerBox == null)
		{
			this.NpcTriggerBox = new GameObject("NpcTriggerBox").AddComponent<NPCBarricadeTriggerBox>();
			this.NpcTriggerBox.Setup(this);
		}
	}

	// Token: 0x06002201 RID: 8705 RVA: 0x000DD4F8 File Offset: 0x000DB6F8
	public override void OnAttacked(HitInfo info)
	{
		if (base.isServer && info.WeaponPrefab is BaseMelee && !info.IsProjectile())
		{
			BasePlayer basePlayer = info.Initiator as BasePlayer;
			if (basePlayer && this.reflectDamage > 0f)
			{
				basePlayer.Hurt(this.reflectDamage * UnityEngine.Random.Range(0.75f, 1.25f), DamageType.Stab, this, true);
				if (this.reflectEffect.isValid)
				{
					Effect.server.Run(this.reflectEffect.resourcePath, basePlayer, StringPool.closest, base.transform.position, Vector3.up, null, false);
				}
			}
		}
		base.OnAttacked(info);
	}

	// Token: 0x04001A58 RID: 6744
	public float reflectDamage = 5f;

	// Token: 0x04001A59 RID: 6745
	public GameObjectRef reflectEffect;

	// Token: 0x04001A5A RID: 6746
	public bool canNpcSmash = true;

	// Token: 0x04001A5B RID: 6747
	public NavMeshModifierVolume NavMeshVolumeAnimals;

	// Token: 0x04001A5C RID: 6748
	public NavMeshModifierVolume NavMeshVolumeHumanoids;

	// Token: 0x04001A5D RID: 6749
	[NonSerialized]
	public NPCBarricadeTriggerBox NpcTriggerBox;

	// Token: 0x04001A5E RID: 6750
	private static int nonWalkableArea = -1;

	// Token: 0x04001A5F RID: 6751
	private static int animalAgentTypeId = -1;

	// Token: 0x04001A60 RID: 6752
	private static int humanoidAgentTypeId = -1;
}
