using System;
using UnityEngine;

namespace Rust.Ai
{
	// Token: 0x02000B49 RID: 2889
	[DefaultExecutionOrder(-102)]
	public class AiManagedAgent : FacepunchBehaviour, IServerComponent
	{
		// Token: 0x060045F1 RID: 17905 RVA: 0x00197B73 File Offset: 0x00195D73
		private void OnEnable()
		{
			this.isRegistered = false;
			if (SingletonComponent<AiManager>.Instance == null || !SingletonComponent<AiManager>.Instance.enabled || AiManager.nav_disable)
			{
				base.enabled = false;
				return;
			}
		}

		// Token: 0x060045F2 RID: 17906 RVA: 0x00197BA4 File Offset: 0x00195DA4
		private void DelayedRegistration()
		{
			if (!this.isRegistered)
			{
				this.isRegistered = true;
			}
		}

		// Token: 0x060045F3 RID: 17907 RVA: 0x00197BB5 File Offset: 0x00195DB5
		private void OnDisable()
		{
			if (Application.isQuitting)
			{
				return;
			}
			if (!(SingletonComponent<AiManager>.Instance == null) && SingletonComponent<AiManager>.Instance.enabled)
			{
				bool flag = this.isRegistered;
			}
		}

		// Token: 0x04003ED4 RID: 16084
		[Tooltip("TODO: Replace with actual agent type id on the NavMeshAgent when we upgrade to 5.6.1 or above.")]
		public int AgentTypeIndex;

		// Token: 0x04003ED5 RID: 16085
		[ReadOnly]
		[NonSerialized]
		public Vector2i NavmeshGridCoord;

		// Token: 0x04003ED6 RID: 16086
		private bool isRegistered;
	}
}
