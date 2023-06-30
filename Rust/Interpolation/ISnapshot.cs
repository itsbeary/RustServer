using System;

namespace Rust.Interpolation
{
	// Token: 0x02000B41 RID: 2881
	public interface ISnapshot<T>
	{
		// Token: 0x17000659 RID: 1625
		// (get) Token: 0x060045B7 RID: 17847
		// (set) Token: 0x060045B8 RID: 17848
		float Time { get; set; }

		// Token: 0x060045B9 RID: 17849
		void MatchValuesTo(T entry);

		// Token: 0x060045BA RID: 17850
		void Lerp(T prev, T next, float delta);

		// Token: 0x060045BB RID: 17851
		T GetNew();
	}
}
