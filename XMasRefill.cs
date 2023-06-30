using System;
using System.Collections.Generic;
using ConVar;
using Network;
using UnityEngine;

// Token: 0x020000F5 RID: 245
public class XMasRefill : BaseEntity
{
	// Token: 0x0600156E RID: 5486 RVA: 0x000A9254 File Offset: 0x000A7454
	public override bool OnRpcMessage(BasePlayer player, uint rpc, Message msg)
	{
		using (TimeWarning.New("XMasRefill.OnRpcMessage", 0))
		{
		}
		return base.OnRpcMessage(player, rpc, msg);
	}

	// Token: 0x0600156F RID: 5487 RVA: 0x000A9294 File Offset: 0x000A7494
	public float GiftRadius()
	{
		return XMas.spawnRange;
	}

	// Token: 0x06001570 RID: 5488 RVA: 0x000A929B File Offset: 0x000A749B
	public int GiftsPerPlayer()
	{
		return XMas.giftsPerPlayer;
	}

	// Token: 0x06001571 RID: 5489 RVA: 0x000A92A2 File Offset: 0x000A74A2
	public int GiftSpawnAttempts()
	{
		return XMas.giftsPerPlayer * XMas.spawnAttempts;
	}

	// Token: 0x06001572 RID: 5490 RVA: 0x000A92B0 File Offset: 0x000A74B0
	public override void ServerInit()
	{
		base.ServerInit();
		if (!XMas.enabled)
		{
			base.Invoke(new Action(this.RemoveMe), 0.1f);
			return;
		}
		this.goodKids = ((BasePlayer.activePlayerList != null) ? new List<BasePlayer>(BasePlayer.activePlayerList) : new List<BasePlayer>());
		this.stockings = ((Stocking.stockings != null) ? new List<Stocking>(Stocking.stockings.Values) : new List<Stocking>());
		base.Invoke(new Action(this.RemoveMe), 60f);
		base.InvokeRepeating(new Action(this.DistributeLoot), 3f, 0.02f);
		base.Invoke(new Action(this.SendBells), 0.5f);
	}

	// Token: 0x06001573 RID: 5491 RVA: 0x000A936D File Offset: 0x000A756D
	public void SendBells()
	{
		base.ClientRPC(null, "PlayBells");
	}

	// Token: 0x06001574 RID: 5492 RVA: 0x000A937B File Offset: 0x000A757B
	public void RemoveMe()
	{
		if (this.goodKids.Count == 0 && this.stockings.Count == 0)
		{
			base.Kill(BaseNetworkable.DestroyMode.None);
			return;
		}
		base.Invoke(new Action(this.RemoveMe), 60f);
	}

	// Token: 0x06001575 RID: 5493 RVA: 0x000A93B8 File Offset: 0x000A75B8
	public void DistributeLoot()
	{
		if (this.goodKids.Count > 0)
		{
			BasePlayer basePlayer = null;
			foreach (BasePlayer basePlayer2 in this.goodKids)
			{
				if (!basePlayer2.IsSleeping() && !basePlayer2.IsWounded() && basePlayer2.IsAlive())
				{
					basePlayer = basePlayer2;
					break;
				}
			}
			if (basePlayer)
			{
				this.DistributeGiftsForPlayer(basePlayer);
				this.goodKids.Remove(basePlayer);
			}
		}
		if (this.stockings.Count > 0)
		{
			Stocking stocking = this.stockings[0];
			if (stocking != null)
			{
				stocking.SpawnLoot();
			}
			this.stockings.RemoveAt(0);
		}
	}

	// Token: 0x06001576 RID: 5494 RVA: 0x000A9484 File Offset: 0x000A7684
	protected bool DropToGround(ref Vector3 pos)
	{
		int num = 1235288065;
		int num2 = 8454144;
		if (TerrainMeta.TopologyMap && (TerrainMeta.TopologyMap.GetTopology(pos) & 82048) != 0)
		{
			return false;
		}
		if (TerrainMeta.HeightMap && TerrainMeta.Collision && !TerrainMeta.Collision.GetIgnore(pos, 0.01f))
		{
			float height = TerrainMeta.HeightMap.GetHeight(pos);
			pos.y = Mathf.Max(pos.y, height);
		}
		RaycastHit raycastHit;
		if (!TransformUtil.GetGroundInfo(pos, out raycastHit, 80f, num, null))
		{
			return false;
		}
		if (((1 << raycastHit.transform.gameObject.layer) & num2) == 0)
		{
			return false;
		}
		pos = raycastHit.point;
		return true;
	}

	// Token: 0x06001577 RID: 5495 RVA: 0x000A955C File Offset: 0x000A775C
	public bool DistributeGiftsForPlayer(BasePlayer player)
	{
		int num = this.GiftsPerPlayer();
		int num2 = this.GiftSpawnAttempts();
		int num3 = 0;
		while (num3 < num2 && num > 0)
		{
			Vector2 vector = UnityEngine.Random.insideUnitCircle * this.GiftRadius();
			Vector3 vector2 = player.transform.position + new Vector3(vector.x, 10f, vector.y);
			Quaternion quaternion = Quaternion.Euler(0f, UnityEngine.Random.Range(0f, 360f), 0f);
			if (this.DropToGround(ref vector2))
			{
				string resourcePath = this.giftPrefabs[UnityEngine.Random.Range(0, this.giftPrefabs.Length)].resourcePath;
				BaseEntity baseEntity = GameManager.server.CreateEntity(resourcePath, vector2, quaternion, true);
				if (baseEntity)
				{
					baseEntity.Spawn();
					num--;
				}
			}
			num3++;
		}
		return true;
	}

	// Token: 0x04000D93 RID: 3475
	public GameObjectRef[] giftPrefabs;

	// Token: 0x04000D94 RID: 3476
	public List<BasePlayer> goodKids;

	// Token: 0x04000D95 RID: 3477
	public List<Stocking> stockings;

	// Token: 0x04000D96 RID: 3478
	public AudioSource bells;
}
