using System;
using Rust.Localization;
using UnityEngine;

// Token: 0x020008E7 RID: 2279
public class LocalizeText : MonoBehaviour, IClientComponent, ILocalize
{
	// Token: 0x17000465 RID: 1125
	// (get) Token: 0x06003793 RID: 14227 RVA: 0x0014CD7C File Offset: 0x0014AF7C
	// (set) Token: 0x06003794 RID: 14228 RVA: 0x0014CD84 File Offset: 0x0014AF84
	public string LanguageToken
	{
		get
		{
			return this.token;
		}
		set
		{
			this.token = value;
		}
	}

	// Token: 0x17000466 RID: 1126
	// (get) Token: 0x06003795 RID: 14229 RVA: 0x0014CD8D File Offset: 0x0014AF8D
	// (set) Token: 0x06003796 RID: 14230 RVA: 0x0014CD95 File Offset: 0x0014AF95
	public string LanguageEnglish
	{
		get
		{
			return this.english;
		}
		set
		{
			this.english = value;
		}
	}

	// Token: 0x040032F6 RID: 13046
	public string token;

	// Token: 0x040032F7 RID: 13047
	[TextArea]
	public string english;

	// Token: 0x040032F8 RID: 13048
	public string append;

	// Token: 0x040032F9 RID: 13049
	public LocalizeText.SpecialMode specialMode;

	// Token: 0x02000EBD RID: 3773
	public enum SpecialMode
	{
		// Token: 0x04004D27 RID: 19751
		None,
		// Token: 0x04004D28 RID: 19752
		AllUppercase,
		// Token: 0x04004D29 RID: 19753
		AllLowercase
	}
}
