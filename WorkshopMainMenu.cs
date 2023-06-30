using System;

// Token: 0x0200089D RID: 2205
public class WorkshopMainMenu : SingletonComponent<WorkshopMainMenu>
{
	// Token: 0x040031B0 RID: 12720
	public static Translate.Phrase loading_workshop = new TokenisedPhrase("loading.workshop", "Loading Workshop");

	// Token: 0x040031B1 RID: 12721
	public static Translate.Phrase loading_workshop_setup = new TokenisedPhrase("loading.workshop.initializing", "Setting Up Scene");

	// Token: 0x040031B2 RID: 12722
	public static Translate.Phrase loading_workshop_skinnables = new TokenisedPhrase("loading.workshop.skinnables", "Getting Skinnables");

	// Token: 0x040031B3 RID: 12723
	public static Translate.Phrase loading_workshop_item = new TokenisedPhrase("loading.workshop.item", "Loading Item Data");
}
