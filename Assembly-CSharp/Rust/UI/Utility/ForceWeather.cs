using System;
using UnityEngine;
using UnityEngine.UI;

namespace Rust.UI.Utility
{
	// Token: 0x02000B2B RID: 2859
	[RequireComponent(typeof(Toggle))]
	internal class ForceWeather : MonoBehaviour
	{
		// Token: 0x06004538 RID: 17720 RVA: 0x00194C9D File Offset: 0x00192E9D
		public void OnEnable()
		{
			this.component = base.GetComponent<Toggle>();
		}

		// Token: 0x06004539 RID: 17721 RVA: 0x00194CAC File Offset: 0x00192EAC
		public void Update()
		{
			if (SingletonComponent<Climate>.Instance == null)
			{
				return;
			}
			if (this.Rain)
			{
				SingletonComponent<Climate>.Instance.Overrides.Rain = Mathf.MoveTowards(SingletonComponent<Climate>.Instance.Overrides.Rain, (float)(this.component.isOn ? 1 : 0), Time.deltaTime / 2f);
			}
			if (this.Fog)
			{
				SingletonComponent<Climate>.Instance.Overrides.Fog = Mathf.MoveTowards(SingletonComponent<Climate>.Instance.Overrides.Fog, (float)(this.component.isOn ? 1 : 0), Time.deltaTime / 2f);
			}
			if (this.Wind)
			{
				SingletonComponent<Climate>.Instance.Overrides.Wind = Mathf.MoveTowards(SingletonComponent<Climate>.Instance.Overrides.Wind, (float)(this.component.isOn ? 1 : 0), Time.deltaTime / 2f);
			}
			if (this.Clouds)
			{
				SingletonComponent<Climate>.Instance.Overrides.Clouds = Mathf.MoveTowards(SingletonComponent<Climate>.Instance.Overrides.Clouds, (float)(this.component.isOn ? 1 : 0), Time.deltaTime / 2f);
			}
		}

		// Token: 0x04003E11 RID: 15889
		private Toggle component;

		// Token: 0x04003E12 RID: 15890
		public bool Rain;

		// Token: 0x04003E13 RID: 15891
		public bool Fog;

		// Token: 0x04003E14 RID: 15892
		public bool Wind;

		// Token: 0x04003E15 RID: 15893
		public bool Clouds;
	}
}
