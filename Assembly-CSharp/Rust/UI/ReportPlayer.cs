using System;
using UnityEngine;

namespace Rust.UI
{
	// Token: 0x02000B18 RID: 2840
	public class ReportPlayer : UIDialog
	{
		// Token: 0x04003DA7 RID: 15783
		public GameObject FindPlayer;

		// Token: 0x04003DA8 RID: 15784
		public GameObject GetInformation;

		// Token: 0x04003DA9 RID: 15785
		public GameObject Finished;

		// Token: 0x04003DAA RID: 15786
		public GameObject RecentlyReported;

		// Token: 0x04003DAB RID: 15787
		public Dropdown ReasonDropdown;

		// Token: 0x04003DAC RID: 15788
		public RustInput Subject;

		// Token: 0x04003DAD RID: 15789
		public RustInput Message;

		// Token: 0x04003DAE RID: 15790
		public RustButton ReportButton;

		// Token: 0x04003DAF RID: 15791
		public SteamUserButton SteamUserButton;

		// Token: 0x04003DB0 RID: 15792
		public RustIcon ProgressIcon;

		// Token: 0x04003DB1 RID: 15793
		public RustText ProgressText;

		// Token: 0x04003DB2 RID: 15794
		public static Option[] ReportReasons = new Option[]
		{
			new Option(new Translate.Phrase("report.reason.none", "Select an option"), "none", false, Icons.Bars),
			new Option(new Translate.Phrase("report.reason.abuse", "Racism/Sexism/Abusive"), "abusive", false, Icons.Angry),
			new Option(new Translate.Phrase("report.reason.cheat", "Cheating"), "cheat", false, Icons.Crosshairs),
			new Option(new Translate.Phrase("report.reason.spam", "Spamming"), "spam", false, Icons.Bullhorn),
			new Option(new Translate.Phrase("report.reason.name", "Offensive Name"), "name", false, Icons.Radiation)
		};
	}
}
