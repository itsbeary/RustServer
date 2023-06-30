using System;
using UnityEngine;

namespace ConVar
{
	// Token: 0x02000ADB RID: 2779
	[ConsoleSystem.Factory("physics")]
	public class Physics : ConsoleSystem
	{
		// Token: 0x170005BD RID: 1469
		// (get) Token: 0x06004294 RID: 17044 RVA: 0x00189C68 File Offset: 0x00187E68
		// (set) Token: 0x06004295 RID: 17045 RVA: 0x00189C6F File Offset: 0x00187E6F
		[ServerVar]
		public static float bouncethreshold
		{
			get
			{
				return Physics.bounceThreshold;
			}
			set
			{
				Physics.bounceThreshold = value;
			}
		}

		// Token: 0x170005BE RID: 1470
		// (get) Token: 0x06004296 RID: 17046 RVA: 0x00189C77 File Offset: 0x00187E77
		// (set) Token: 0x06004297 RID: 17047 RVA: 0x00189C7E File Offset: 0x00187E7E
		[ServerVar]
		public static float sleepthreshold
		{
			get
			{
				return Physics.sleepThreshold;
			}
			set
			{
				Physics.sleepThreshold = value;
			}
		}

		// Token: 0x170005BF RID: 1471
		// (get) Token: 0x06004298 RID: 17048 RVA: 0x00189C86 File Offset: 0x00187E86
		// (set) Token: 0x06004299 RID: 17049 RVA: 0x00189C8D File Offset: 0x00187E8D
		[ServerVar(Help = "The default solver iteration count permitted for any rigid bodies (default 7). Must be positive")]
		public static int solveriterationcount
		{
			get
			{
				return Physics.defaultSolverIterations;
			}
			set
			{
				Physics.defaultSolverIterations = value;
			}
		}

		// Token: 0x170005C0 RID: 1472
		// (get) Token: 0x0600429A RID: 17050 RVA: 0x00189C95 File Offset: 0x00187E95
		// (set) Token: 0x0600429B RID: 17051 RVA: 0x00189CA7 File Offset: 0x00187EA7
		[ServerVar(Help = "Gravity multiplier")]
		public static float gravity
		{
			get
			{
				return Physics.gravity.y / -9.81f;
			}
			set
			{
				Physics.gravity = new Vector3(0f, value * -9.81f, 0f);
			}
		}

		// Token: 0x0600429C RID: 17052 RVA: 0x00189CC4 File Offset: 0x00187EC4
		internal static void ApplyDropped(Rigidbody rigidBody)
		{
			if (Physics.droppedmode <= 0)
			{
				rigidBody.collisionDetectionMode = CollisionDetectionMode.Discrete;
			}
			if (Physics.droppedmode == 1)
			{
				rigidBody.collisionDetectionMode = CollisionDetectionMode.Continuous;
			}
			if (Physics.droppedmode == 2)
			{
				rigidBody.collisionDetectionMode = CollisionDetectionMode.ContinuousDynamic;
			}
			if (Physics.droppedmode >= 3)
			{
				rigidBody.collisionDetectionMode = CollisionDetectionMode.ContinuousSpeculative;
			}
		}

		// Token: 0x170005C1 RID: 1473
		// (get) Token: 0x0600429D RID: 17053 RVA: 0x00189D02 File Offset: 0x00187F02
		// (set) Token: 0x0600429E RID: 17054 RVA: 0x00189D0F File Offset: 0x00187F0F
		[ClientVar(ClientAdmin = true)]
		[ServerVar(Help = "The amount of physics steps per second")]
		public static float steps
		{
			get
			{
				return 1f / Time.fixedDeltaTime;
			}
			set
			{
				if (value < 10f)
				{
					value = 10f;
				}
				if (value > 60f)
				{
					value = 60f;
				}
				Time.fixedDeltaTime = 1f / value;
			}
		}

		// Token: 0x170005C2 RID: 1474
		// (get) Token: 0x0600429F RID: 17055 RVA: 0x00189D3B File Offset: 0x00187F3B
		// (set) Token: 0x060042A0 RID: 17056 RVA: 0x00189D48 File Offset: 0x00187F48
		[ClientVar(ClientAdmin = true)]
		[ServerVar(Help = "The slowest physics steps will operate")]
		public static float minsteps
		{
			get
			{
				return 1f / Time.maximumDeltaTime;
			}
			set
			{
				if (value < 1f)
				{
					value = 1f;
				}
				if (value > 60f)
				{
					value = 60f;
				}
				Time.maximumDeltaTime = 1f / value;
			}
		}

		// Token: 0x170005C3 RID: 1475
		// (get) Token: 0x060042A1 RID: 17057 RVA: 0x00189D74 File Offset: 0x00187F74
		// (set) Token: 0x060042A2 RID: 17058 RVA: 0x00189D7B File Offset: 0x00187F7B
		[ClientVar]
		[ServerVar]
		public static bool autosynctransforms
		{
			get
			{
				return Physics.autoSyncTransforms;
			}
			set
			{
				Physics.autoSyncTransforms = value;
			}
		}

		// Token: 0x04003BF4 RID: 15348
		private const float baseGravity = -9.81f;

		// Token: 0x04003BF5 RID: 15349
		[ServerVar(Help = "The collision detection mode that dropped items and corpses should use")]
		public static int droppedmode = 2;

		// Token: 0x04003BF6 RID: 15350
		[ServerVar(Help = "Send effects to clients when physics objects collide")]
		public static bool sendeffects = true;

		// Token: 0x04003BF7 RID: 15351
		[ServerVar]
		public static bool groundwatchdebug = false;

		// Token: 0x04003BF8 RID: 15352
		[ServerVar]
		public static int groundwatchfails = 1;

		// Token: 0x04003BF9 RID: 15353
		[ServerVar]
		public static float groundwatchdelay = 0.1f;

		// Token: 0x04003BFA RID: 15354
		[ClientVar]
		[ServerVar]
		public static bool batchsynctransforms = true;
	}
}
