using System;
using UnityEngine;

namespace Sonar
{
	// Token: 0x02000A20 RID: 2592
	public class SonarObject : MonoBehaviour, IClientComponent
	{
		// Token: 0x040037BA RID: 14266
		[SerializeField]
		private SonarObject.SType sonarType;

		// Token: 0x02000F1B RID: 3867
		public enum SType
		{
			// Token: 0x04004EC4 RID: 20164
			MoonPool,
			// Token: 0x04004EC5 RID: 20165
			Sub
		}
	}
}
