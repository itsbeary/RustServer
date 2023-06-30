using System;
using ProtoBuf;

namespace CompanionServer
{
	// Token: 0x020009ED RID: 2541
	public interface IConnection
	{
		// Token: 0x170004EF RID: 1263
		// (get) Token: 0x06003CA2 RID: 15522
		long ConnectionId { get; }

		// Token: 0x06003CA3 RID: 15523
		void Send(AppResponse response);

		// Token: 0x06003CA4 RID: 15524
		void Subscribe(PlayerTarget target);

		// Token: 0x06003CA5 RID: 15525
		void Subscribe(EntityTarget target);

		// Token: 0x170004F0 RID: 1264
		// (get) Token: 0x06003CA6 RID: 15526
		IRemoteControllable CurrentCamera { get; }

		// Token: 0x170004F1 RID: 1265
		// (get) Token: 0x06003CA7 RID: 15527
		bool IsControllingCamera { get; }

		// Token: 0x170004F2 RID: 1266
		// (get) Token: 0x06003CA8 RID: 15528
		ulong ControllingSteamId { get; }

		// Token: 0x170004F3 RID: 1267
		// (get) Token: 0x06003CA9 RID: 15529
		// (set) Token: 0x06003CAA RID: 15530
		InputState InputState { get; set; }

		// Token: 0x06003CAB RID: 15531
		bool BeginViewing(IRemoteControllable camera);

		// Token: 0x06003CAC RID: 15532
		void EndViewing();
	}
}
