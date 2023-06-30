using System;

namespace ConVar
{
	// Token: 0x02000AC1 RID: 2753
	[ConsoleSystem.Factory("env")]
	public class Env : ConsoleSystem
	{
		// Token: 0x170005A1 RID: 1441
		// (get) Token: 0x060041D8 RID: 16856 RVA: 0x00186389 File Offset: 0x00184589
		// (set) Token: 0x060041D7 RID: 16855 RVA: 0x00186364 File Offset: 0x00184564
		[ServerVar]
		public static bool progresstime
		{
			get
			{
				return !(TOD_Sky.Instance == null) && TOD_Sky.Instance.Components.Time.ProgressTime;
			}
			set
			{
				if (TOD_Sky.Instance == null)
				{
					return;
				}
				TOD_Sky.Instance.Components.Time.ProgressTime = value;
			}
		}

		// Token: 0x170005A2 RID: 1442
		// (get) Token: 0x060041DA RID: 16858 RVA: 0x001863CE File Offset: 0x001845CE
		// (set) Token: 0x060041D9 RID: 16857 RVA: 0x001863AE File Offset: 0x001845AE
		[ServerVar(ShowInAdminUI = true)]
		public static float time
		{
			get
			{
				if (TOD_Sky.Instance == null)
				{
					return 0f;
				}
				return TOD_Sky.Instance.Cycle.Hour;
			}
			set
			{
				if (TOD_Sky.Instance == null)
				{
					return;
				}
				TOD_Sky.Instance.Cycle.Hour = value;
			}
		}

		// Token: 0x170005A3 RID: 1443
		// (get) Token: 0x060041DC RID: 16860 RVA: 0x00186412 File Offset: 0x00184612
		// (set) Token: 0x060041DB RID: 16859 RVA: 0x001863F2 File Offset: 0x001845F2
		[ServerVar]
		public static int day
		{
			get
			{
				if (TOD_Sky.Instance == null)
				{
					return 0;
				}
				return TOD_Sky.Instance.Cycle.Day;
			}
			set
			{
				if (TOD_Sky.Instance == null)
				{
					return;
				}
				TOD_Sky.Instance.Cycle.Day = value;
			}
		}

		// Token: 0x170005A4 RID: 1444
		// (get) Token: 0x060041DE RID: 16862 RVA: 0x00186452 File Offset: 0x00184652
		// (set) Token: 0x060041DD RID: 16861 RVA: 0x00186432 File Offset: 0x00184632
		[ServerVar]
		public static int month
		{
			get
			{
				if (TOD_Sky.Instance == null)
				{
					return 0;
				}
				return TOD_Sky.Instance.Cycle.Month;
			}
			set
			{
				if (TOD_Sky.Instance == null)
				{
					return;
				}
				TOD_Sky.Instance.Cycle.Month = value;
			}
		}

		// Token: 0x170005A5 RID: 1445
		// (get) Token: 0x060041E0 RID: 16864 RVA: 0x00186492 File Offset: 0x00184692
		// (set) Token: 0x060041DF RID: 16863 RVA: 0x00186472 File Offset: 0x00184672
		[ServerVar]
		public static int year
		{
			get
			{
				if (TOD_Sky.Instance == null)
				{
					return 0;
				}
				return TOD_Sky.Instance.Cycle.Year;
			}
			set
			{
				if (TOD_Sky.Instance == null)
				{
					return;
				}
				TOD_Sky.Instance.Cycle.Year = value;
			}
		}

		// Token: 0x060041E1 RID: 16865 RVA: 0x001864B4 File Offset: 0x001846B4
		[ServerVar]
		public static void addtime(ConsoleSystem.Arg arg)
		{
			if (TOD_Sky.Instance == null)
			{
				return;
			}
			DateTime dateTime = TOD_Sky.Instance.Cycle.DateTime.AddTicks(arg.GetTicks(0, 0L));
			TOD_Sky.Instance.Cycle.DateTime = dateTime;
		}

		// Token: 0x170005A6 RID: 1446
		// (get) Token: 0x060041E2 RID: 16866 RVA: 0x00186500 File Offset: 0x00184700
		// (set) Token: 0x060041E3 RID: 16867 RVA: 0x00186507 File Offset: 0x00184707
		[ReplicatedVar(Default = "0")]
		public static float oceanlevel
		{
			get
			{
				return WaterSystem.OceanLevel;
			}
			set
			{
				WaterSystem.OceanLevel = value;
			}
		}
	}
}
