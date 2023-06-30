using System;
using UnityEngine;

namespace ConVar
{
	// Token: 0x02000AED RID: 2797
	[ConsoleSystem.Factory("vehicle")]
	public class vehicle : ConsoleSystem
	{
		// Token: 0x0600433C RID: 17212 RVA: 0x0018D010 File Offset: 0x0018B210
		[ServerUserVar]
		public static void swapseats(ConsoleSystem.Arg arg)
		{
			int num = 0;
			BasePlayer basePlayer = arg.Player();
			if (basePlayer == null)
			{
				return;
			}
			if (basePlayer.SwapSeatCooldown())
			{
				return;
			}
			BaseMountable mounted = basePlayer.GetMounted();
			if (mounted == null)
			{
				return;
			}
			BaseVehicle baseVehicle = mounted.GetComponent<BaseVehicle>();
			if (baseVehicle == null)
			{
				baseVehicle = mounted.VehicleParent();
			}
			if (baseVehicle == null)
			{
				return;
			}
			baseVehicle.SwapSeats(basePlayer, num);
		}

		// Token: 0x0600433D RID: 17213 RVA: 0x0018D074 File Offset: 0x0018B274
		[ServerVar]
		public static void fixcars(ConsoleSystem.Arg arg)
		{
			BasePlayer basePlayer = arg.Player();
			if (basePlayer == null)
			{
				arg.ReplyWith("Null player.");
				return;
			}
			if (!basePlayer.IsAdmin)
			{
				arg.ReplyWith("Must be an admin to use fixcars.");
				return;
			}
			int num = arg.GetInt(0, 2);
			num = Mathf.Clamp(num, 1, 3);
			BaseVehicle[] array = UnityEngine.Object.FindObjectsOfType<BaseVehicle>();
			int num2 = 0;
			foreach (BaseVehicle baseVehicle in array)
			{
				if (baseVehicle.isServer && Vector3.Distance(baseVehicle.transform.position, basePlayer.transform.position) <= 10f && baseVehicle.AdminFixUp(num))
				{
					num2++;
				}
			}
			foreach (MLRS mlrs in UnityEngine.Object.FindObjectsOfType<MLRS>())
			{
				if (mlrs.isServer && Vector3.Distance(mlrs.transform.position, basePlayer.transform.position) <= 10f && mlrs.AdminFixUp())
				{
					num2++;
				}
			}
			arg.ReplyWith(string.Format("Fixed up {0} vehicles.", num2));
		}

		// Token: 0x0600433E RID: 17214 RVA: 0x0018D190 File Offset: 0x0018B390
		[ServerVar]
		public static void stop_all_trains(ConsoleSystem.Arg arg)
		{
			TrainEngine[] array = UnityEngine.Object.FindObjectsOfType<TrainEngine>();
			for (int i = 0; i < array.Length; i++)
			{
				array[i].StopEngine();
			}
			arg.ReplyWith("All trains stopped.");
		}

		// Token: 0x0600433F RID: 17215 RVA: 0x0018D1C4 File Offset: 0x0018B3C4
		[ServerVar]
		public static void killcars(ConsoleSystem.Arg args)
		{
			ModularCar[] array = BaseEntity.Util.FindAll<ModularCar>();
			for (int i = 0; i < array.Length; i++)
			{
				array[i].Kill(BaseNetworkable.DestroyMode.None);
			}
		}

		// Token: 0x06004340 RID: 17216 RVA: 0x0018D1F0 File Offset: 0x0018B3F0
		[ServerVar]
		public static void killminis(ConsoleSystem.Arg args)
		{
			foreach (MiniCopter miniCopter in BaseEntity.Util.FindAll<MiniCopter>())
			{
				if (miniCopter.name.ToLower().Contains("minicopter"))
				{
					miniCopter.Kill(BaseNetworkable.DestroyMode.None);
				}
			}
		}

		// Token: 0x06004341 RID: 17217 RVA: 0x0018D234 File Offset: 0x0018B434
		[ServerVar]
		public static void killscraphelis(ConsoleSystem.Arg args)
		{
			ScrapTransportHelicopter[] array = BaseEntity.Util.FindAll<ScrapTransportHelicopter>();
			for (int i = 0; i < array.Length; i++)
			{
				array[i].Kill(BaseNetworkable.DestroyMode.None);
			}
		}

		// Token: 0x06004342 RID: 17218 RVA: 0x0018D260 File Offset: 0x0018B460
		[ServerVar]
		public static void killtrains(ConsoleSystem.Arg args)
		{
			TrainCar[] array = BaseEntity.Util.FindAll<TrainCar>();
			for (int i = 0; i < array.Length; i++)
			{
				array[i].Kill(BaseNetworkable.DestroyMode.None);
			}
		}

		// Token: 0x06004343 RID: 17219 RVA: 0x0018D28C File Offset: 0x0018B48C
		[ServerVar]
		public static void killboats(ConsoleSystem.Arg args)
		{
			BaseBoat[] array = BaseEntity.Util.FindAll<BaseBoat>();
			for (int i = 0; i < array.Length; i++)
			{
				array[i].Kill(BaseNetworkable.DestroyMode.None);
			}
		}

		// Token: 0x06004344 RID: 17220 RVA: 0x0018D2B8 File Offset: 0x0018B4B8
		[ServerVar]
		public static void killdrones(ConsoleSystem.Arg args)
		{
			foreach (Drone drone in BaseEntity.Util.FindAll<Drone>())
			{
				if (!(drone is DeliveryDrone))
				{
					drone.Kill(BaseNetworkable.DestroyMode.None);
				}
			}
		}

		// Token: 0x04003C9F RID: 15519
		[ServerVar]
		[Help("how long until boat corpses despawn (excluding tugboat - use tugboat_corpse_seconds)")]
		public static float boat_corpse_seconds = 300f;

		// Token: 0x04003CA0 RID: 15520
		[ServerVar(Help = "If true, trains always explode when destroyed, and hitting a barrier always destroys the train immediately. Default: false")]
		public static bool cinematictrains = false;

		// Token: 0x04003CA1 RID: 15521
		[ServerVar(Help = "Determines whether trains stop automatically when there's no-one on them. Default: false")]
		public static bool trainskeeprunning = false;

		// Token: 0x04003CA2 RID: 15522
		[ServerVar(Help = "Determines whether modular cars turn into wrecks when destroyed, or just immediately gib. Default: true")]
		public static bool carwrecks = true;

		// Token: 0x04003CA3 RID: 15523
		[ServerVar(Help = "Determines whether vehicles drop storage items when destroyed. Default: true")]
		public static bool vehiclesdroploot = true;
	}
}
