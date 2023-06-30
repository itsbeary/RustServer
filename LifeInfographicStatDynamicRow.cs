using System;
using Rust.UI;

// Token: 0x02000868 RID: 2152
public class LifeInfographicStatDynamicRow : LifeInfographicStat
{
	// Token: 0x0600363D RID: 13885 RVA: 0x00148611 File Offset: 0x00146811
	public void SetStatName(Translate.Phrase phrase)
	{
		this.StatName.SetPhrase(phrase);
	}

	// Token: 0x04003098 RID: 12440
	public RustText StatName;
}
