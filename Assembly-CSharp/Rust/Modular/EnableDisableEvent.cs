using System;
using UnityEngine;
using UnityEngine.Events;

namespace Rust.Modular
{
	// Token: 0x02000B33 RID: 2867
	public class EnableDisableEvent : MonoBehaviour
	{
		// Token: 0x0600455F RID: 17759 RVA: 0x00195A95 File Offset: 0x00193C95
		protected void OnEnable()
		{
			if (this.enableEvent != null)
			{
				this.enableEvent.Invoke();
			}
		}

		// Token: 0x06004560 RID: 17760 RVA: 0x00195AAA File Offset: 0x00193CAA
		protected void OnDisable()
		{
			if (this.disableEvent != null)
			{
				this.disableEvent.Invoke();
			}
		}

		// Token: 0x04003E6A RID: 15978
		[SerializeField]
		private UnityEvent enableEvent;

		// Token: 0x04003E6B RID: 15979
		[SerializeField]
		private UnityEvent disableEvent;
	}
}
