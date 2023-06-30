using System;
using Facepunch;
using ProtoBuf;

namespace CompanionServer.Handlers
{
	// Token: 0x02000A0D RID: 2573
	public class TeamInfo : BaseHandler<AppEmpty>
	{
		// Token: 0x06003D48 RID: 15688 RVA: 0x00166F64 File Offset: 0x00165164
		public override void Execute()
		{
			global::RelationshipManager.PlayerTeam playerTeam = global::RelationshipManager.ServerInstance.FindPlayersTeam(base.UserId);
			AppTeamInfo appTeamInfo;
			if (playerTeam != null)
			{
				appTeamInfo = playerTeam.GetAppTeamInfo(base.UserId);
			}
			else
			{
				appTeamInfo = base.Player.GetAppTeamInfo(base.UserId);
			}
			AppResponse appResponse = Pool.Get<AppResponse>();
			appResponse.teamInfo = appTeamInfo;
			base.Send(appResponse);
		}
	}
}
