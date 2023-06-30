using System;
using Rust.UI;
using UnityEngine;

// Token: 0x02000870 RID: 2160
public class CompanionSetupScreen : SingletonComponent<CompanionSetupScreen>
{
	// Token: 0x040030E3 RID: 12515
	public const string PairedKey = "companionPaired";

	// Token: 0x040030E4 RID: 12516
	public GameObject instructionsBody;

	// Token: 0x040030E5 RID: 12517
	public GameObject detailsPanel;

	// Token: 0x040030E6 RID: 12518
	public GameObject loadingMessage;

	// Token: 0x040030E7 RID: 12519
	public GameObject errorMessage;

	// Token: 0x040030E8 RID: 12520
	public GameObject notSupportedMessage;

	// Token: 0x040030E9 RID: 12521
	public GameObject disabledMessage;

	// Token: 0x040030EA RID: 12522
	public GameObject enabledMessage;

	// Token: 0x040030EB RID: 12523
	public GameObject refreshButton;

	// Token: 0x040030EC RID: 12524
	public GameObject enableButton;

	// Token: 0x040030ED RID: 12525
	public GameObject disableButton;

	// Token: 0x040030EE RID: 12526
	public GameObject pairButton;

	// Token: 0x040030EF RID: 12527
	public RustText serverName;

	// Token: 0x040030F0 RID: 12528
	public RustButton helpButton;

	// Token: 0x02000EA2 RID: 3746
	public enum ScreenState
	{
		// Token: 0x04004CC5 RID: 19653
		Loading,
		// Token: 0x04004CC6 RID: 19654
		Error,
		// Token: 0x04004CC7 RID: 19655
		NoServer,
		// Token: 0x04004CC8 RID: 19656
		NotSupported,
		// Token: 0x04004CC9 RID: 19657
		NotInstalled,
		// Token: 0x04004CCA RID: 19658
		Disabled,
		// Token: 0x04004CCB RID: 19659
		Enabled,
		// Token: 0x04004CCC RID: 19660
		ShowHelp
	}
}
