using System;
using UnityEngine;

namespace Rust.Interpolation
{
	// Token: 0x02000B44 RID: 2884
	public struct FloatSnapshot : ISnapshot<FloatSnapshot>
	{
		// Token: 0x1700065B RID: 1627
		// (get) Token: 0x060045C7 RID: 17863 RVA: 0x001971DE File Offset: 0x001953DE
		// (set) Token: 0x060045C8 RID: 17864 RVA: 0x001971E6 File Offset: 0x001953E6
		public float Time { get; set; }

		// Token: 0x060045C9 RID: 17865 RVA: 0x001971EF File Offset: 0x001953EF
		public FloatSnapshot(float time, float value)
		{
			this.Time = time;
			this.value = value;
		}

		// Token: 0x060045CA RID: 17866 RVA: 0x001971FF File Offset: 0x001953FF
		public void MatchValuesTo(FloatSnapshot entry)
		{
			this.value = entry.value;
		}

		// Token: 0x060045CB RID: 17867 RVA: 0x0019720D File Offset: 0x0019540D
		public void Lerp(FloatSnapshot prev, FloatSnapshot next, float delta)
		{
			this.value = Mathf.Lerp(prev.value, next.value, delta);
		}

		// Token: 0x060045CC RID: 17868 RVA: 0x00197228 File Offset: 0x00195428
		public FloatSnapshot GetNew()
		{
			return default(FloatSnapshot);
		}

		// Token: 0x04003EB9 RID: 16057
		public float value;
	}
}
