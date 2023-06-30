using System;
using ProtoBuf;

namespace CompanionServer.Handlers
{
	// Token: 0x02000A08 RID: 2568
	public class PromoteToLeader : BaseHandler<AppPromoteToLeader>
	{
		// Token: 0x06003D3D RID: 15677 RVA: 0x00166C4C File Offset: 0x00164E4C
		public override void Execute()
		{
			global::RelationshipManager.PlayerTeam playerTeam = global::RelationshipManager.ServerInstance.FindPlayersTeam(base.UserId);
			if (playerTeam == null)
			{
				base.SendError("no_team");
				return;
			}
			if (playerTeam.teamLeader != base.UserId)
			{
				base.SendError("access_denied");
				return;
			}
			if (playerTeam.teamLeader == base.Proto.steamId)
			{
				base.SendSuccess();
				return;
			}
			if (!playerTeam.members.Contains(base.Proto.steamId))
			{
				base.SendError("not_found");
				return;
			}
			playerTeam.SetTeamLeader(base.Proto.steamId);
			base.SendSuccess();
		}
	}
}
