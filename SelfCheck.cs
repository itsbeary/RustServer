using System;
using System.Runtime.InteropServices;
using Facepunch;
using UnityEngine;

// Token: 0x020004EF RID: 1263
public static class SelfCheck
{
	// Token: 0x060028E9 RID: 10473 RVA: 0x000FC4F8 File Offset: 0x000FA6F8
	public static bool Run()
	{
		if (FileSystem.Backend.isError)
		{
			return SelfCheck.Failed("Asset Bundle Error: " + FileSystem.Backend.loadingError);
		}
		if (FileSystem.Load<GameManifest>("Assets/manifest.asset", true) == null)
		{
			return SelfCheck.Failed("Couldn't load game manifest - verify your game content!");
		}
		if (!SelfCheck.TestRustNative())
		{
			return false;
		}
		if (CommandLine.HasSwitch("-force-feature-level-9-3"))
		{
			return SelfCheck.Failed("Invalid command line argument: -force-feature-level-9-3");
		}
		if (CommandLine.HasSwitch("-force-feature-level-10-0"))
		{
			return SelfCheck.Failed("Invalid command line argument: -force-feature-level-10-0");
		}
		return !CommandLine.HasSwitch("-force-feature-level-10-1") || SelfCheck.Failed("Invalid command line argument: -force-feature-level-10-1");
	}

	// Token: 0x060028EA RID: 10474 RVA: 0x000FC598 File Offset: 0x000FA798
	private static bool Failed(string Message)
	{
		if (SingletonComponent<Bootstrap>.Instance)
		{
			SingletonComponent<Bootstrap>.Instance.messageString = "";
			SingletonComponent<Bootstrap>.Instance.ThrowError(Message);
		}
		Debug.LogError("SelfCheck Failed: " + Message);
		return false;
	}

	// Token: 0x060028EB RID: 10475 RVA: 0x000FC5D4 File Offset: 0x000FA7D4
	private static bool TestRustNative()
	{
		try
		{
			if (!SelfCheck.RustNative_VersionCheck(5))
			{
				return SelfCheck.Failed("RustNative is wrong version!");
			}
		}
		catch (DllNotFoundException ex)
		{
			return SelfCheck.Failed("RustNative library couldn't load! " + ex.Message);
		}
		return true;
	}

	// Token: 0x060028EC RID: 10476
	[DllImport("RustNative")]
	private static extern bool RustNative_VersionCheck(int version);
}
