using System;
using ConVar;
using Facepunch.Extend;
using ProtoBuf;

namespace CompanionServer.Handlers
{
	// Token: 0x02000A09 RID: 2569
	public class SendTeamChat : BaseHandler<AppSendMessage>
	{
		// Token: 0x17000507 RID: 1287
		// (get) Token: 0x06003D3F RID: 15679 RVA: 0x00166CF0 File Offset: 0x00164EF0
		protected override double TokenCost
		{
			get
			{
				return 2.0;
			}
		}

		// Token: 0x06003D40 RID: 15680 RVA: 0x00166CFC File Offset: 0x00164EFC
		public override void Execute()
		{
			string message = base.Proto.message;
			string text = ((message != null) ? message.Trim() : null);
			if (string.IsNullOrWhiteSpace(text))
			{
				base.SendSuccess();
				return;
			}
			text = text.Truncate(256, "…");
			global::BasePlayer player = base.Player;
			string text2;
			if ((text2 = ((player != null) ? player.displayName : null)) == null)
			{
				text2 = SingletonComponent<ServerMgr>.Instance.persistance.GetPlayerName(base.UserId) ?? "[unknown]";
			}
			string text3 = text2;
			if (Chat.sayAs(Chat.ChatChannel.Team, base.UserId, text3, text, base.Player))
			{
				base.SendSuccess();
				return;
			}
			base.SendError("message_not_sent");
		}
	}
}
