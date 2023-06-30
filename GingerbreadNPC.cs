using System;
using System.Runtime.CompilerServices;
using ConVar;
using ProtoBuf;
using UnityEngine;

// Token: 0x020001F2 RID: 498
public class GingerbreadNPC : HumanNPC, IClientBrainStateListener
{
	// Token: 0x06001A28 RID: 6696 RVA: 0x000BDB87 File Offset: 0x000BBD87
	public override void OnAttacked(HitInfo info)
	{
		base.OnAttacked(info);
		info.HitMaterial = Global.GingerbreadMaterialID();
	}

	// Token: 0x06001A29 RID: 6697 RVA: 0x000BDB9B File Offset: 0x000BBD9B
	public override string Categorize()
	{
		return "Gingerbread";
	}

	// Token: 0x06001A2A RID: 6698 RVA: 0x00007A44 File Offset: 0x00005C44
	public override bool ShouldDropActiveItem()
	{
		return false;
	}

	// Token: 0x06001A2B RID: 6699 RVA: 0x000BDBA4 File Offset: 0x000BBDA4
	public override BaseCorpse CreateCorpse()
	{
		BaseCorpse baseCorpse;
		using (TimeWarning.New("Create corpse", 0))
		{
			string corpseResourcePath = this.CorpseResourcePath;
			NPCPlayerCorpse npcplayerCorpse = base.DropCorpse(corpseResourcePath) as NPCPlayerCorpse;
			if (npcplayerCorpse)
			{
				npcplayerCorpse.transform.position = npcplayerCorpse.transform.position + Vector3.down * this.NavAgent.baseOffset;
				npcplayerCorpse.SetLootableIn(2f);
				npcplayerCorpse.SetFlag(global::BaseEntity.Flags.Reserved5, base.HasPlayerFlag(global::BasePlayer.PlayerFlags.DisplaySash), false, true);
				npcplayerCorpse.SetFlag(global::BaseEntity.Flags.Reserved2, true, false, true);
				npcplayerCorpse.TakeFrom(new global::ItemContainer[] { this.inventory.containerMain });
				npcplayerCorpse.playerName = "Gingerbread";
				npcplayerCorpse.playerSteamID = this.userID;
				npcplayerCorpse.Spawn();
				global::ItemContainer[] containers = npcplayerCorpse.containers;
				for (int i = 0; i < containers.Length; i++)
				{
					containers[i].Clear();
				}
				if (this.LootSpawnSlots.Length != 0)
				{
					foreach (LootContainer.LootSpawnSlot lootSpawnSlot in this.LootSpawnSlots)
					{
						for (int j = 0; j < lootSpawnSlot.numberToSpawn; j++)
						{
							if (UnityEngine.Random.Range(0f, 1f) <= lootSpawnSlot.probability)
							{
								lootSpawnSlot.definition.SpawnIntoContainer(npcplayerCorpse.containers[0]);
							}
						}
					}
				}
			}
			baseCorpse = npcplayerCorpse;
		}
		return baseCorpse;
	}

	// Token: 0x06001A2C RID: 6700 RVA: 0x000BDD34 File Offset: 0x000BBF34
	public override void AttackerInfo(PlayerLifeStory.DeathInfo info)
	{
		base.AttackerInfo(info);
		info.inflictorName = this.inventory.containerBelt.GetSlot(0).info.shortname;
		info.attackerName = base.ShortPrefabName;
	}

	// Token: 0x17000232 RID: 562
	// (get) Token: 0x06001A2D RID: 6701 RVA: 0x000BDD6C File Offset: 0x000BBF6C
	protected string CorpseResourcePath
	{
		get
		{
			bool flag = GingerbreadNPC.<get_CorpseResourcePath>g__GetFloatBasedOnUserID|10_0(this.userID, 4332UL) > 0.5f;
			if (this.OverrideCorpseMale.isValid && !flag)
			{
				return this.OverrideCorpseMale.resourcePath;
			}
			if (this.OverrideCorpseFemale.isValid && flag)
			{
				return this.OverrideCorpseFemale.resourcePath;
			}
			return "assets/prefabs/npc/murderer/murderer_corpse.prefab";
		}
	}

	// Token: 0x06001A2E RID: 6702 RVA: 0x000063A5 File Offset: 0x000045A5
	public void OnClientStateChanged(AIState state)
	{
	}

	// Token: 0x06001A30 RID: 6704 RVA: 0x000BDDDC File Offset: 0x000BBFDC
	[CompilerGenerated]
	internal static float <get_CorpseResourcePath>g__GetFloatBasedOnUserID|10_0(ulong steamid, ulong seed)
	{
		UnityEngine.Random.State state = UnityEngine.Random.state;
		UnityEngine.Random.InitState((int)(seed + steamid));
		float num = UnityEngine.Random.Range(0f, 1f);
		UnityEngine.Random.state = state;
		return num;
	}

	// Token: 0x040012B1 RID: 4785
	public GameObjectRef OverrideCorpseMale;

	// Token: 0x040012B2 RID: 4786
	public GameObjectRef OverrideCorpseFemale;

	// Token: 0x040012B3 RID: 4787
	public PhysicMaterial HitMaterial;

	// Token: 0x040012B4 RID: 4788
	public bool RoamAroundHomePoint;
}
