using System;
using UnityEngine;

namespace ConVar
{
	// Token: 0x02000AE7 RID: 2791
	[ConsoleSystem.Factory("supply")]
	public class Supply : ConsoleSystem
	{
		// Token: 0x06004326 RID: 17190 RVA: 0x0018C9E4 File Offset: 0x0018ABE4
		[ServerVar]
		public static void drop(ConsoleSystem.Arg arg)
		{
			BasePlayer basePlayer = arg.Player();
			if (!basePlayer)
			{
				return;
			}
			Debug.Log("Supply Drop Inbound");
			BaseEntity baseEntity = GameManager.server.CreateEntity("assets/prefabs/npc/cargo plane/cargo_plane.prefab", default(Vector3), default(Quaternion), true);
			if (baseEntity)
			{
				baseEntity.GetComponent<CargoPlane>().InitDropPosition(basePlayer.transform.position + new Vector3(0f, 10f, 0f));
				baseEntity.Spawn();
			}
		}

		// Token: 0x06004327 RID: 17191 RVA: 0x0018CA6C File Offset: 0x0018AC6C
		[ServerVar]
		public static void call(ConsoleSystem.Arg arg)
		{
			if (!arg.Player())
			{
				return;
			}
			Debug.Log("Supply Drop Inbound");
			BaseEntity baseEntity = GameManager.server.CreateEntity("assets/prefabs/npc/cargo plane/cargo_plane.prefab", default(Vector3), default(Quaternion), true);
			if (baseEntity)
			{
				baseEntity.Spawn();
			}
		}

		// Token: 0x04003C9A RID: 15514
		private const string path = "assets/prefabs/npc/cargo plane/cargo_plane.prefab";
	}
}
