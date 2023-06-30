using System;
using System.Net;
using ConVar;
using Facepunch;
using Network;
using Rust;
using Rust.Platform.Common;

// Token: 0x0200073E RID: 1854
public class RustPlatformHooks : IPlatformHooks
{
	// Token: 0x1700043A RID: 1082
	// (get) Token: 0x06003371 RID: 13169 RVA: 0x0013B3DF File Offset: 0x001395DF
	public uint SteamAppId
	{
		get
		{
			return Rust.Defines.appID;
		}
	}

	// Token: 0x06003372 RID: 13170 RVA: 0x0013B3E6 File Offset: 0x001395E6
	public void Abort()
	{
		Rust.Application.Quit();
	}

	// Token: 0x06003373 RID: 13171 RVA: 0x0013B3ED File Offset: 0x001395ED
	public void OnItemDefinitionsChanged()
	{
		ItemManager.InvalidateWorkshopSkinCache();
	}

	// Token: 0x1700043B RID: 1083
	// (get) Token: 0x06003374 RID: 13172 RVA: 0x0013B3F4 File Offset: 0x001395F4
	public ServerParameters? ServerParameters
	{
		get
		{
			if (Network.Net.sv == null)
			{
				return null;
			}
			IPAddress ipaddress = null;
			if (!string.IsNullOrEmpty(ConVar.Server.ip))
			{
				ipaddress = IPAddress.Parse(ConVar.Server.ip);
			}
			if (ConVar.Server.queryport <= 0 || ConVar.Server.queryport == ConVar.Server.port)
			{
				ConVar.Server.queryport = Math.Max(ConVar.Server.port, RCon.Port) + 1;
			}
			return new ServerParameters?(new ServerParameters("rust", "Rust", 2397.ToString(), ConVar.Server.secure, CommandLine.HasSwitch("-sdrnet"), ipaddress, (ushort)Network.Net.sv.port, (ushort)ConVar.Server.queryport));
		}
	}

	// Token: 0x06003375 RID: 13173 RVA: 0x0013B497 File Offset: 0x00139697
	public void AuthSessionValidated(ulong userId, ulong ownerUserId, AuthResponse response)
	{
		SingletonComponent<ServerMgr>.Instance.OnValidateAuthTicketResponse(userId, ownerUserId, response);
	}

	// Token: 0x04002A52 RID: 10834
	public static readonly RustPlatformHooks Instance = new RustPlatformHooks();
}
