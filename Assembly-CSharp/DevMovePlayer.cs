using System;
using UnityEngine;

// Token: 0x02000323 RID: 803
public class DevMovePlayer : BaseMonoBehaviour
{
	// Token: 0x06001F1B RID: 7963 RVA: 0x000D36F8 File Offset: 0x000D18F8
	public void Awake()
	{
		this.randRun = UnityEngine.Random.Range(5f, 10f);
		this.player = base.GetComponent<BasePlayer>();
		if (this.Waypoints.Length != 0)
		{
			this.destination = this.Waypoints[0].position;
		}
		else
		{
			this.destination = base.transform.position;
		}
		if (this.player.isClient)
		{
			return;
		}
		if (this.player.eyes == null)
		{
			this.player.eyes = this.player.GetComponent<PlayerEyes>();
		}
		base.Invoke(new Action(this.LateSpawn), 1f);
	}

	// Token: 0x06001F1C RID: 7964 RVA: 0x000D37A4 File Offset: 0x000D19A4
	public void LateSpawn()
	{
		Item item = ItemManager.CreateByName("rifle.semiauto", 1, 0UL);
		this.player.inventory.GiveItem(item, this.player.inventory.containerBelt, false);
		this.player.UpdateActiveItem(item.uid);
		this.player.health = 100f;
	}

	// Token: 0x06001F1D RID: 7965 RVA: 0x000D3803 File Offset: 0x000D1A03
	public void SetWaypoints(Transform[] wps)
	{
		this.Waypoints = wps;
		this.destination = wps[0].position;
	}

	// Token: 0x06001F1E RID: 7966 RVA: 0x000D381C File Offset: 0x000D1A1C
	public void Update()
	{
		if (this.player.isClient)
		{
			return;
		}
		if (!this.player.IsAlive() || this.player.IsWounded())
		{
			return;
		}
		if (Vector3.Distance(this.destination, base.transform.position) < 0.25f)
		{
			if (this.moveRandomly)
			{
				this.waypointIndex = UnityEngine.Random.Range(0, this.Waypoints.Length);
			}
			else
			{
				this.waypointIndex++;
			}
			if (this.waypointIndex >= this.Waypoints.Length)
			{
				this.waypointIndex = 0;
			}
		}
		if (this.Waypoints.Length <= this.waypointIndex)
		{
			return;
		}
		this.destination = this.Waypoints[this.waypointIndex].position;
		Vector3 normalized = (this.destination - base.transform.position).normalized;
		float num = Mathf.Sin(Time.time + this.randRun);
		float speed = this.player.GetSpeed(num, 0f, 0f);
		Vector3 vector = base.transform.position;
		float num2 = 1f;
		LayerMask layerMask = 1537286401;
		RaycastHit raycastHit;
		if (TransformUtil.GetGroundInfo(base.transform.position + normalized * speed * Time.deltaTime, out raycastHit, num2, layerMask, this.player.transform))
		{
			vector = raycastHit.point;
		}
		base.transform.position = vector;
		Vector3 normalized2 = (new Vector3(this.destination.x, 0f, this.destination.z) - new Vector3(this.player.transform.position.x, 0f, this.player.transform.position.z)).normalized;
		this.player.SendNetworkUpdate(BasePlayer.NetworkQueue.Update);
	}

	// Token: 0x0400180A RID: 6154
	public BasePlayer player;

	// Token: 0x0400180B RID: 6155
	public Transform[] Waypoints;

	// Token: 0x0400180C RID: 6156
	public bool moveRandomly;

	// Token: 0x0400180D RID: 6157
	public Vector3 destination = Vector3.zero;

	// Token: 0x0400180E RID: 6158
	public Vector3 lookPoint = Vector3.zero;

	// Token: 0x0400180F RID: 6159
	private int waypointIndex;

	// Token: 0x04001810 RID: 6160
	private float randRun;
}
