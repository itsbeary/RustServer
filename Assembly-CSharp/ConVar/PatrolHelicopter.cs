using System;
using UnityEngine;

namespace ConVar
{
	// Token: 0x02000ACB RID: 2763
	[ConsoleSystem.Factory("heli")]
	public class PatrolHelicopter : ConsoleSystem
	{
		// Token: 0x06004258 RID: 16984 RVA: 0x00187FCC File Offset: 0x001861CC
		[ServerVar]
		public static void drop(ConsoleSystem.Arg arg)
		{
			BasePlayer basePlayer = arg.Player();
			if (!basePlayer)
			{
				return;
			}
			Debug.Log("heli called to : " + basePlayer.transform.position);
			BaseEntity baseEntity = GameManager.server.CreateEntity("assets/prefabs/npc/patrol helicopter/patrolhelicopter.prefab", default(Vector3), default(Quaternion), true);
			if (baseEntity)
			{
				baseEntity.GetComponent<PatrolHelicopterAI>().SetInitialDestination(basePlayer.transform.position + new Vector3(0f, 10f, 0f), 0f);
				baseEntity.Spawn();
			}
		}

		// Token: 0x06004259 RID: 16985 RVA: 0x00188070 File Offset: 0x00186270
		[ServerVar]
		public static void calltome(ConsoleSystem.Arg arg)
		{
			BasePlayer basePlayer = arg.Player();
			if (!basePlayer)
			{
				return;
			}
			Debug.Log("heli called to : " + basePlayer.transform.position);
			BaseEntity baseEntity = GameManager.server.CreateEntity("assets/prefabs/npc/patrol helicopter/patrolhelicopter.prefab", default(Vector3), default(Quaternion), true);
			if (baseEntity)
			{
				baseEntity.GetComponent<PatrolHelicopterAI>().SetInitialDestination(basePlayer.transform.position + new Vector3(0f, 10f, 0f), 0.25f);
				baseEntity.Spawn();
			}
		}

		// Token: 0x0600425A RID: 16986 RVA: 0x00188114 File Offset: 0x00186314
		[ServerVar]
		public static void call(ConsoleSystem.Arg arg)
		{
			if (!arg.Player())
			{
				return;
			}
			Debug.Log("Helicopter inbound");
			BaseEntity baseEntity = GameManager.server.CreateEntity("assets/prefabs/npc/patrol helicopter/patrolhelicopter.prefab", default(Vector3), default(Quaternion), true);
			if (baseEntity)
			{
				baseEntity.Spawn();
			}
		}

		// Token: 0x0600425B RID: 16987 RVA: 0x0018816C File Offset: 0x0018636C
		[ServerVar]
		public static void strafe(ConsoleSystem.Arg arg)
		{
			BasePlayer basePlayer = arg.Player();
			if (!basePlayer)
			{
				return;
			}
			PatrolHelicopterAI heliInstance = PatrolHelicopterAI.heliInstance;
			if (heliInstance == null)
			{
				Debug.Log("no heli instance");
				return;
			}
			RaycastHit raycastHit;
			if (Physics.Raycast(basePlayer.eyes.HeadRay(), out raycastHit, 1000f, 1218652417))
			{
				Debug.Log("strafing :" + raycastHit.point);
				heliInstance.interestZoneOrigin = raycastHit.point;
				heliInstance.ExitCurrentState();
				heliInstance.State_Strafe_Enter(raycastHit.point, false);
				return;
			}
			Debug.Log("strafe ray missed");
		}

		// Token: 0x0600425C RID: 16988 RVA: 0x00188208 File Offset: 0x00186408
		[ServerVar]
		public static void testpuzzle(ConsoleSystem.Arg arg)
		{
			BasePlayer basePlayer = arg.Player();
			if (!basePlayer)
			{
				return;
			}
			bool isDeveloper = basePlayer.IsDeveloper;
		}

		// Token: 0x04003BE4 RID: 15332
		private const string path = "assets/prefabs/npc/patrol helicopter/patrolhelicopter.prefab";

		// Token: 0x04003BE5 RID: 15333
		[ServerVar]
		public static float lifetimeMinutes = 15f;

		// Token: 0x04003BE6 RID: 15334
		[ServerVar]
		public static int guns = 1;

		// Token: 0x04003BE7 RID: 15335
		[ServerVar]
		public static float bulletDamageScale = 1f;

		// Token: 0x04003BE8 RID: 15336
		[ServerVar]
		public static float bulletAccuracy = 2f;
	}
}
