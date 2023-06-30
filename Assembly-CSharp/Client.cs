using System;

// Token: 0x020002A9 RID: 681
public class Client : SingletonComponent<Client>
{
	// Token: 0x0400163C RID: 5692
	public static Translate.Phrase loading_loading = new Translate.Phrase("loading.loading", "Loading");

	// Token: 0x0400163D RID: 5693
	public static Translate.Phrase loading_connecting = new Translate.Phrase("loading.connecting", "Connecting");

	// Token: 0x0400163E RID: 5694
	public static Translate.Phrase loading_connectionaccepted = new Translate.Phrase("loading.connectionaccepted", "Connection Accepted");

	// Token: 0x0400163F RID: 5695
	public static Translate.Phrase loading_connecting_negotiate = new Translate.Phrase("loading.connecting.negotiate", "Negotiating Connection");

	// Token: 0x04001640 RID: 5696
	public static Translate.Phrase loading_level = new Translate.Phrase("loading.loadinglevel", "Loading Level");

	// Token: 0x04001641 RID: 5697
	public static Translate.Phrase loading_skinnablewarmup = new Translate.Phrase("loading.skinnablewarmup", "Skinnable Warmup");

	// Token: 0x04001642 RID: 5698
	public static Translate.Phrase loading_preloadcomplete = new Translate.Phrase("loading.preloadcomplete", "Preload Complete");

	// Token: 0x04001643 RID: 5699
	public static Translate.Phrase loading_openingscene = new Translate.Phrase("loading.openingscene", "Opening Scene");

	// Token: 0x04001644 RID: 5700
	public static Translate.Phrase loading_clientready = new Translate.Phrase("loading.clientready", "Client Ready");

	// Token: 0x04001645 RID: 5701
	public static Translate.Phrase loading_prefabwarmup = new Translate.Phrase("loading.prefabwarmup", "Warming Prefabs [{0}/{1}]");
}
