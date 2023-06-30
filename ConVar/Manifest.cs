using System;
using Facepunch;

namespace ConVar
{
	// Token: 0x02000AD3 RID: 2771
	public class Manifest
	{
		// Token: 0x06004283 RID: 17027 RVA: 0x00189A5A File Offset: 0x00187C5A
		[ClientVar]
		[ServerVar]
		public static object PrintManifest()
		{
			return Application.Manifest;
		}

		// Token: 0x06004284 RID: 17028 RVA: 0x00189A61 File Offset: 0x00187C61
		[ClientVar]
		[ServerVar]
		public static object PrintManifestRaw()
		{
			return Manifest.Contents;
		}
	}
}
