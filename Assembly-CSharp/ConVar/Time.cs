using System;
using UnityEngine;

namespace ConVar
{
	// Token: 0x02000AEB RID: 2795
	[ConsoleSystem.Factory("time")]
	public class Time : ConsoleSystem
	{
		// Token: 0x170005DC RID: 1500
		// (get) Token: 0x06004332 RID: 17202 RVA: 0x0018CFD8 File Offset: 0x0018B1D8
		// (set) Token: 0x06004333 RID: 17203 RVA: 0x0018CFDF File Offset: 0x0018B1DF
		[ServerVar]
		[Help("Fixed delta time in seconds")]
		public static float fixeddelta
		{
			get
			{
				return Time.fixedDeltaTime;
			}
			set
			{
				Time.fixedDeltaTime = value;
			}
		}

		// Token: 0x170005DD RID: 1501
		// (get) Token: 0x06004334 RID: 17204 RVA: 0x0018CFE7 File Offset: 0x0018B1E7
		// (set) Token: 0x06004335 RID: 17205 RVA: 0x0018CFEE File Offset: 0x0018B1EE
		[ServerVar]
		[Help("The minimum amount of times to tick per frame")]
		public static float maxdelta
		{
			get
			{
				return Time.maximumDeltaTime;
			}
			set
			{
				Time.maximumDeltaTime = value;
			}
		}

		// Token: 0x170005DE RID: 1502
		// (get) Token: 0x06004336 RID: 17206 RVA: 0x0018CFF6 File Offset: 0x0018B1F6
		// (set) Token: 0x06004337 RID: 17207 RVA: 0x0018CFFD File Offset: 0x0018B1FD
		[ServerVar]
		[Help("The time scale")]
		public static float timescale
		{
			get
			{
				return Time.timeScale;
			}
			set
			{
				Time.timeScale = value;
			}
		}

		// Token: 0x04003C9D RID: 15517
		[ServerVar]
		[Help("Pause time while loading")]
		public static bool pausewhileloading = true;
	}
}
