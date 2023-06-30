using System;
using System.Collections.Generic;
using System.Globalization;
using System.Linq;
using System.Text;
using Facepunch;
using Facepunch.Extend;
using Facepunch.Math;
using Network;
using Newtonsoft.Json;
using ProtoBuf;
using Rust;
using UnityEngine;
using UnityEngine.Scripting;

namespace ConVar
{
	// Token: 0x02000AAB RID: 2731
	[ConsoleSystem.Factory("global")]
	public class Admin : ConsoleSystem
	{
		// Token: 0x0600411A RID: 16666 RVA: 0x0017F634 File Offset: 0x0017D834
		[ServerVar(Help = "Print out currently connected clients")]
		public static void status(ConsoleSystem.Arg arg)
		{
			string text = arg.GetString(0, "");
			if (text == "--json")
			{
				text = arg.GetString(1, "");
			}
			bool flag = arg.HasArg("--json");
			string text2 = string.Empty;
			if (!flag && text.Length == 0)
			{
				text2 = text2 + "hostname: " + ConVar.Server.hostname + "\n";
				text2 = text2 + "version : " + 2397.ToString() + " secure (secure mode enabled, connected to Steam3)\n";
				text2 = text2 + "map     : " + ConVar.Server.level + "\n";
				text2 += string.Format("players : {0} ({1} max) ({2} queued) ({3} joining)\n\n", new object[]
				{
					global::BasePlayer.activePlayerList.Count<global::BasePlayer>(),
					ConVar.Server.maxplayers,
					SingletonComponent<ServerMgr>.Instance.connectionQueue.Queued,
					SingletonComponent<ServerMgr>.Instance.connectionQueue.Joining
				});
			}
			TextTable textTable = new TextTable();
			textTable.AddColumn("id");
			textTable.AddColumn("name");
			textTable.AddColumn("ping");
			textTable.AddColumn("connected");
			textTable.AddColumn("addr");
			textTable.AddColumn("owner");
			textTable.AddColumn("violation");
			textTable.AddColumn("kicks");
			foreach (global::BasePlayer basePlayer in global::BasePlayer.activePlayerList)
			{
				try
				{
					if (basePlayer.IsValid())
					{
						string userIDString = basePlayer.UserIDString;
						if (basePlayer.net.connection == null)
						{
							textTable.AddRow(new string[] { userIDString, "NO CONNECTION" });
						}
						else
						{
							string text3 = basePlayer.net.connection.ownerid.ToString();
							string text4 = basePlayer.displayName.QuoteSafe();
							string text5 = Net.sv.GetAveragePing(basePlayer.net.connection).ToString();
							string text6 = basePlayer.net.connection.ipaddress;
							string text7 = basePlayer.violationLevel.ToString("0.0");
							string text8 = basePlayer.GetAntiHackKicks().ToString();
							if (!arg.IsAdmin && !arg.IsRcon)
							{
								text6 = "xx.xxx.xx.xxx";
							}
							string text9 = basePlayer.net.connection.GetSecondsConnected().ToString() + "s";
							if (text.Length <= 0 || text4.Contains(text, CompareOptions.IgnoreCase) || userIDString.Contains(text) || text3.Contains(text) || text6.Contains(text))
							{
								textTable.AddRow(new string[]
								{
									userIDString,
									text4,
									text5,
									text9,
									text6,
									(text3 == userIDString) ? string.Empty : text3,
									text7,
									text8
								});
							}
						}
					}
				}
				catch (Exception ex)
				{
					textTable.AddRow(new string[]
					{
						basePlayer.UserIDString,
						ex.Message.QuoteSafe()
					});
				}
			}
			if (flag)
			{
				arg.ReplyWith(textTable.ToJson());
				return;
			}
			arg.ReplyWith(text2 + textTable.ToString());
		}

		// Token: 0x0600411B RID: 16667 RVA: 0x0017F9C8 File Offset: 0x0017DBC8
		[ServerVar(Help = "Print out stats of currently connected clients")]
		public static void stats(ConsoleSystem.Arg arg)
		{
			TextTable table = new TextTable();
			table.AddColumn("id");
			table.AddColumn("name");
			table.AddColumn("time");
			table.AddColumn("kills");
			table.AddColumn("deaths");
			table.AddColumn("suicides");
			table.AddColumn("player");
			table.AddColumn("building");
			table.AddColumn("entity");
			Action<ulong, string> action = delegate(ulong id, string name)
			{
				ServerStatistics.Storage storage = ServerStatistics.Get(id);
				string text3 = TimeSpan.FromSeconds((double)storage.Get("time")).ToShortString();
				string text4 = storage.Get("kill_player").ToString();
				string text5 = (storage.Get("deaths") - storage.Get("death_suicide")).ToString();
				string text6 = storage.Get("death_suicide").ToString();
				string text7 = storage.Get("hit_player_direct_los").ToString();
				string text8 = storage.Get("hit_player_indirect_los").ToString();
				string text9 = storage.Get("hit_building_direct_los").ToString();
				string text10 = storage.Get("hit_building_indirect_los").ToString();
				string text11 = storage.Get("hit_entity_direct_los").ToString();
				string text12 = storage.Get("hit_entity_indirect_los").ToString();
				table.AddRow(new string[]
				{
					id.ToString(),
					name,
					text3,
					text4,
					text5,
					text6,
					text7 + " / " + text8,
					text9 + " / " + text10,
					text11 + " / " + text12
				});
			};
			ulong @uint = arg.GetUInt64(0, 0UL);
			if (@uint == 0UL)
			{
				string @string = arg.GetString(0, "");
				using (ListHashSet<global::BasePlayer>.Enumerator enumerator = global::BasePlayer.activePlayerList.GetEnumerator())
				{
					while (enumerator.MoveNext())
					{
						global::BasePlayer basePlayer = enumerator.Current;
						try
						{
							if (basePlayer.IsValid())
							{
								string text = basePlayer.displayName.QuoteSafe();
								if (@string.Length <= 0 || text.Contains(@string, CompareOptions.IgnoreCase))
								{
									action(basePlayer.userID, text);
								}
							}
						}
						catch (Exception ex)
						{
							table.AddRow(new string[]
							{
								basePlayer.UserIDString,
								ex.Message.QuoteSafe()
							});
						}
					}
					goto IL_198;
				}
			}
			string text2 = "N/A";
			global::BasePlayer basePlayer2 = global::BasePlayer.FindByID(@uint);
			if (basePlayer2)
			{
				text2 = basePlayer2.displayName.QuoteSafe();
			}
			action(@uint, text2);
			IL_198:
			arg.ReplyWith(arg.HasArg("--json") ? table.ToJson() : table.ToString());
		}

		// Token: 0x0600411C RID: 16668 RVA: 0x0017FBB4 File Offset: 0x0017DDB4
		[ServerVar(Help = "upgrade_radius 'grade' 'radius'")]
		public static void upgrade_radius(ConsoleSystem.Arg arg)
		{
			if (!arg.HasArgs(2))
			{
				arg.ReplyWith("Format is 'upgrade_radius {grade} {radius}'");
				return;
			}
			Admin.SkinRadiusInternal(arg, true);
		}

		// Token: 0x0600411D RID: 16669 RVA: 0x0017FBD2 File Offset: 0x0017DDD2
		[ServerVar(Help = "skin_radius 'skin' 'radius'")]
		public static void skin_radius(ConsoleSystem.Arg arg)
		{
			if (!arg.HasArgs(2))
			{
				arg.ReplyWith("Format is 'skin_radius {skin} {radius}'");
				return;
			}
			Admin.SkinRadiusInternal(arg, false);
		}

		// Token: 0x0600411E RID: 16670 RVA: 0x0017FBF0 File Offset: 0x0017DDF0
		private static void SkinRadiusInternal(ConsoleSystem.Arg arg, bool changeAnyGrade)
		{
			global::BasePlayer basePlayer = arg.Player();
			if (basePlayer == null)
			{
				arg.ReplyWith("This must be called from the client");
				return;
			}
			float @float = arg.GetFloat(1, 0f);
			string @string = arg.GetString(0, "");
			IEnumerable<BuildingGrade> enumerable = from x in PrefabAttribute.server.FindAll<ConstructionGrade>(2194854973U)
				select x.gradeBase;
			uint num = <PrivateImplementationDetails>.ComputeStringHash(@string);
			BuildingGrade buildingGrade;
			if (num > 2377118072U)
			{
				if (num <= 2939597809U)
				{
					if (num <= 2697986656U)
					{
						if (num != 2630706691U)
						{
							if (num != 2697986656U)
							{
								goto IL_437;
							}
							if (!(@string == "armoured"))
							{
								goto IL_437;
							}
							goto IL_364;
						}
						else if (!(@string == "sheetmetal"))
						{
							goto IL_437;
						}
					}
					else if (num != 2709983224U)
					{
						if (num != 2939597809U)
						{
							goto IL_437;
						}
						if (!(@string == "brutal"))
						{
							goto IL_437;
						}
						goto IL_3E5;
					}
					else
					{
						if (!(@string == "twig"))
						{
							goto IL_437;
						}
						buildingGrade = enumerable.FirstOrDefault((BuildingGrade x) => x.name == "twigs");
						goto IL_443;
					}
				}
				else if (num <= 3357104378U)
				{
					if (num != 3051889091U)
					{
						if (num != 3357104378U)
						{
							goto IL_437;
						}
						if (!(@string == "stone"))
						{
							goto IL_437;
						}
						buildingGrade = enumerable.FirstOrDefault((BuildingGrade x) => x.name == "stone");
						goto IL_443;
					}
					else
					{
						if (!(@string == "shipping"))
						{
							goto IL_437;
						}
						goto IL_3BC;
					}
				}
				else if (num != 3612782300U)
				{
					if (num != 4174649966U)
					{
						goto IL_437;
					}
					if (!(@string == "shippingcontainer"))
					{
						goto IL_437;
					}
					goto IL_3BC;
				}
				else if (!(@string == "metal"))
				{
					goto IL_437;
				}
				buildingGrade = enumerable.FirstOrDefault((BuildingGrade x) => x.name == "metal");
				goto IL_443;
			}
			if (num <= 215566298U)
			{
				if (num != 126679824U)
				{
					if (num != 194419885U)
					{
						if (num != 215566298U)
						{
							goto IL_437;
						}
						if (!(@string == "brick"))
						{
							goto IL_437;
						}
						buildingGrade = enumerable.FirstOrDefault((BuildingGrade x) => x.name == "brick");
						goto IL_443;
					}
					else if (!(@string == "hqm"))
					{
						goto IL_437;
					}
				}
				else
				{
					if (!(@string == "adobe"))
					{
						goto IL_437;
					}
					buildingGrade = enumerable.FirstOrDefault((BuildingGrade x) => x.name == "adobe");
					goto IL_443;
				}
			}
			else if (num <= 2226448744U)
			{
				if (num != 1388707653U)
				{
					if (num != 2226448744U)
					{
						goto IL_437;
					}
					if (!(@string == "wood"))
					{
						goto IL_437;
					}
					buildingGrade = enumerable.FirstOrDefault((BuildingGrade x) => x.name == "wood");
					goto IL_443;
				}
				else if (!(@string == "armored"))
				{
					goto IL_437;
				}
			}
			else if (num != 2231973519U)
			{
				if (num != 2377118072U)
				{
					goto IL_437;
				}
				if (!(@string == "container"))
				{
					goto IL_437;
				}
				goto IL_3BC;
			}
			else
			{
				if (!(@string == "brutalist"))
				{
					goto IL_437;
				}
				goto IL_3E5;
			}
			IL_364:
			buildingGrade = enumerable.FirstOrDefault((BuildingGrade x) => x.name == "toptier");
			goto IL_443;
			IL_3BC:
			buildingGrade = enumerable.FirstOrDefault((BuildingGrade x) => x.name == "shipping_container");
			goto IL_443;
			IL_3E5:
			buildingGrade = enumerable.FirstOrDefault((BuildingGrade x) => x.name == "brutalist");
			goto IL_443;
			IL_437:
			arg.ReplyWith("Valid skins are: twig, wood, stone, metal, hqm, adobe, shipping, brutalist, brick");
			return;
			IL_443:
			if (buildingGrade == null)
			{
				arg.ReplyWith("Unable to find skin object for " + @string);
				return;
			}
			if (!buildingGrade.enabledInStandalone)
			{
				arg.ReplyWith("Skin " + @string + " is not enabled in standalone yet");
				return;
			}
			List<global::BuildingBlock> list = new List<global::BuildingBlock>();
			Vis.Entities<global::BuildingBlock>(basePlayer.transform.position, @float, list, 2097152, QueryTriggerInteraction.Collide);
			foreach (global::BuildingBlock buildingBlock in list)
			{
				if (buildingBlock.grade == buildingGrade.type || changeAnyGrade)
				{
					buildingBlock.ChangeGradeAndSkin(buildingGrade.type, buildingGrade.skin, false, true);
				}
			}
		}

		// Token: 0x0600411F RID: 16671 RVA: 0x00180100 File Offset: 0x0017E300
		[ServerVar]
		public static void killplayer(ConsoleSystem.Arg arg)
		{
			global::BasePlayer basePlayer = arg.GetPlayerOrSleeper(0);
			if (!basePlayer)
			{
				basePlayer = global::BasePlayer.FindBotClosestMatch(arg.GetString(0, ""));
			}
			if (!basePlayer)
			{
				arg.ReplyWith("Player not found");
				return;
			}
			basePlayer.Hurt(1000f, DamageType.Suicide, basePlayer, false);
		}

		// Token: 0x06004120 RID: 16672 RVA: 0x00180154 File Offset: 0x0017E354
		[ServerVar]
		public static void injureplayer(ConsoleSystem.Arg arg)
		{
			global::BasePlayer basePlayer = arg.GetPlayerOrSleeper(0);
			if (!basePlayer)
			{
				basePlayer = global::BasePlayer.FindBotClosestMatch(arg.GetString(0, ""));
			}
			if (!basePlayer)
			{
				arg.ReplyWith("Player not found");
				return;
			}
			Global.InjurePlayer(basePlayer);
		}

		// Token: 0x06004121 RID: 16673 RVA: 0x001801A0 File Offset: 0x0017E3A0
		[ServerVar]
		public static void recoverplayer(ConsoleSystem.Arg arg)
		{
			global::BasePlayer basePlayer = arg.GetPlayerOrSleeper(0);
			if (!basePlayer)
			{
				basePlayer = global::BasePlayer.FindBotClosestMatch(arg.GetString(0, ""));
			}
			if (!basePlayer)
			{
				arg.ReplyWith("Player not found");
				return;
			}
			Global.RecoverPlayer(basePlayer);
		}

		// Token: 0x06004122 RID: 16674 RVA: 0x001801EC File Offset: 0x0017E3EC
		[ServerVar]
		public static void kick(ConsoleSystem.Arg arg)
		{
			global::BasePlayer player = arg.GetPlayer(0);
			if (!player || player.net == null || player.net.connection == null)
			{
				arg.ReplyWith("Player not found");
				return;
			}
			string @string = arg.GetString(1, "no reason given");
			arg.ReplyWith("Kicked: " + player.displayName);
			Chat.Broadcast(string.Concat(new string[] { "Kicking ", player.displayName, " (", @string, ")" }), "SERVER", "#eee", 0UL);
			player.Kick("Kicked: " + arg.GetString(1, "No Reason Given"));
		}

		// Token: 0x06004123 RID: 16675 RVA: 0x001802AC File Offset: 0x0017E4AC
		[ServerVar]
		public static void kickall(ConsoleSystem.Arg arg)
		{
			global::BasePlayer[] array = global::BasePlayer.activePlayerList.ToArray<global::BasePlayer>();
			for (int i = 0; i < array.Length; i++)
			{
				array[i].Kick("Kicked: " + arg.GetString(1, "No Reason Given"));
			}
		}

		// Token: 0x06004124 RID: 16676 RVA: 0x001802F0 File Offset: 0x0017E4F0
		[ServerVar(Help = "ban <player> <reason> [optional duration]")]
		public static void ban(ConsoleSystem.Arg arg)
		{
			global::BasePlayer player = arg.GetPlayer(0);
			if (!player || player.net == null || player.net.connection == null)
			{
				arg.ReplyWith("Player not found");
				return;
			}
			ServerUsers.User user = ServerUsers.Get(player.userID);
			if (user != null && user.group == ServerUsers.UserGroup.Banned)
			{
				arg.ReplyWith(string.Format("User {0} is already banned", player.userID));
				return;
			}
			string @string = arg.GetString(1, "No Reason Given");
			long num;
			string text;
			if (!Admin.TryGetBanExpiry(arg, 2, out num, out text))
			{
				return;
			}
			ServerUsers.Set(player.userID, ServerUsers.UserGroup.Banned, player.displayName, @string, num);
			string text2 = "";
			if (player.IsConnected && player.net.connection.ownerid != 0UL && player.net.connection.ownerid != player.net.connection.userid)
			{
				text2 += string.Format(" and also banned ownerid {0}", player.net.connection.ownerid);
				ServerUsers.Set(player.net.connection.ownerid, ServerUsers.UserGroup.Banned, player.displayName, arg.GetString(1, string.Format("Family share owner of {0}", player.net.connection.userid)), -1L);
			}
			ServerUsers.Save();
			arg.ReplyWith(string.Format("Kickbanned User{0}: {1} - {2}{3}", new object[] { text, player.userID, player.displayName, text2 }));
			Chat.Broadcast(string.Concat(new string[] { "Kickbanning ", player.displayName, text, " (", @string, ")" }), "SERVER", "#eee", 0UL);
			Net.sv.Kick(player.net.connection, "Banned" + text + ": " + @string, false);
		}

		// Token: 0x06004125 RID: 16677 RVA: 0x001804F0 File Offset: 0x0017E6F0
		[ServerVar]
		public static void moderatorid(ConsoleSystem.Arg arg)
		{
			ulong @uint = arg.GetUInt64(0, 0UL);
			string @string = arg.GetString(1, "unnamed");
			string string2 = arg.GetString(2, "no reason");
			if (@uint < 70000000000000000UL)
			{
				arg.ReplyWith("This doesn't appear to be a 64bit steamid: " + @uint);
				return;
			}
			ServerUsers.User user = ServerUsers.Get(@uint);
			if (user != null && user.group == ServerUsers.UserGroup.Moderator)
			{
				arg.ReplyWith("User " + @uint + " is already a Moderator");
				return;
			}
			ServerUsers.Set(@uint, ServerUsers.UserGroup.Moderator, @string, string2, -1L);
			global::BasePlayer basePlayer = global::BasePlayer.FindByID(@uint);
			if (basePlayer != null)
			{
				basePlayer.SetPlayerFlag(global::BasePlayer.PlayerFlags.IsAdmin, true);
				basePlayer.SendNetworkUpdate(global::BasePlayer.NetworkQueue.Update);
			}
			arg.ReplyWith(string.Concat(new object[] { "Added moderator ", @string, ", steamid ", @uint }));
		}

		// Token: 0x06004126 RID: 16678 RVA: 0x001805D0 File Offset: 0x0017E7D0
		[ServerVar]
		public static void ownerid(ConsoleSystem.Arg arg)
		{
			ulong @uint = arg.GetUInt64(0, 0UL);
			string @string = arg.GetString(1, "unnamed");
			string string2 = arg.GetString(2, "no reason");
			if (@uint < 70000000000000000UL)
			{
				arg.ReplyWith("This doesn't appear to be a 64bit steamid: " + @uint);
				return;
			}
			if (arg.Connection != null && arg.Connection.authLevel < 2U)
			{
				arg.ReplyWith("Moderators cannot run ownerid");
				return;
			}
			ServerUsers.User user = ServerUsers.Get(@uint);
			if (user != null && user.group == ServerUsers.UserGroup.Owner)
			{
				arg.ReplyWith("User " + @uint + " is already an Owner");
				return;
			}
			ServerUsers.Set(@uint, ServerUsers.UserGroup.Owner, @string, string2, -1L);
			global::BasePlayer basePlayer = global::BasePlayer.FindByID(@uint);
			if (basePlayer != null)
			{
				basePlayer.SetPlayerFlag(global::BasePlayer.PlayerFlags.IsAdmin, true);
				basePlayer.SendNetworkUpdate(global::BasePlayer.NetworkQueue.Update);
			}
			arg.ReplyWith(string.Concat(new object[] { "Added owner ", @string, ", steamid ", @uint }));
		}

		// Token: 0x06004127 RID: 16679 RVA: 0x001806D4 File Offset: 0x0017E8D4
		[ServerVar]
		public static void removemoderator(ConsoleSystem.Arg arg)
		{
			ulong @uint = arg.GetUInt64(0, 0UL);
			if (@uint < 70000000000000000UL)
			{
				arg.ReplyWith("This doesn't appear to be a 64bit steamid: " + @uint);
				return;
			}
			ServerUsers.User user = ServerUsers.Get(@uint);
			if (user == null || user.group != ServerUsers.UserGroup.Moderator)
			{
				arg.ReplyWith("User " + @uint + " isn't a moderator");
				return;
			}
			ServerUsers.Remove(@uint);
			global::BasePlayer basePlayer = global::BasePlayer.FindByID(@uint);
			if (basePlayer != null)
			{
				basePlayer.SetPlayerFlag(global::BasePlayer.PlayerFlags.IsAdmin, false);
				basePlayer.SendNetworkUpdate(global::BasePlayer.NetworkQueue.Update);
			}
			arg.ReplyWith("Removed Moderator: " + @uint);
		}

		// Token: 0x06004128 RID: 16680 RVA: 0x00180778 File Offset: 0x0017E978
		[ServerVar]
		public static void removeowner(ConsoleSystem.Arg arg)
		{
			ulong @uint = arg.GetUInt64(0, 0UL);
			if (@uint < 70000000000000000UL)
			{
				arg.ReplyWith("This doesn't appear to be a 64bit steamid: " + @uint);
				return;
			}
			ServerUsers.User user = ServerUsers.Get(@uint);
			if (user == null || user.group != ServerUsers.UserGroup.Owner)
			{
				arg.ReplyWith("User " + @uint + " isn't an owner");
				return;
			}
			ServerUsers.Remove(@uint);
			global::BasePlayer basePlayer = global::BasePlayer.FindByID(@uint);
			if (basePlayer != null)
			{
				basePlayer.SetPlayerFlag(global::BasePlayer.PlayerFlags.IsAdmin, false);
				basePlayer.SendNetworkUpdate(global::BasePlayer.NetworkQueue.Update);
			}
			arg.ReplyWith("Removed Owner: " + @uint);
		}

		// Token: 0x06004129 RID: 16681 RVA: 0x0018081C File Offset: 0x0017EA1C
		[ServerVar(Help = "banid <steamid> <username> <reason> [optional duration]")]
		public static void banid(ConsoleSystem.Arg arg)
		{
			ulong @uint = arg.GetUInt64(0, 0UL);
			string text = arg.GetString(1, "unnamed");
			string @string = arg.GetString(2, "no reason");
			if (@uint < 70000000000000000UL)
			{
				arg.ReplyWith("This doesn't appear to be a 64bit steamid: " + @uint);
				return;
			}
			ServerUsers.User user = ServerUsers.Get(@uint);
			if (user != null && user.group == ServerUsers.UserGroup.Banned)
			{
				arg.ReplyWith("User " + @uint + " is already banned");
				return;
			}
			long num;
			string text2;
			if (!Admin.TryGetBanExpiry(arg, 3, out num, out text2))
			{
				return;
			}
			string text3 = "";
			global::BasePlayer basePlayer = global::BasePlayer.FindByID(@uint);
			if (basePlayer != null && basePlayer.IsConnected)
			{
				text = basePlayer.displayName;
				if (basePlayer.IsConnected && basePlayer.net.connection.ownerid != 0UL && basePlayer.net.connection.ownerid != basePlayer.net.connection.userid)
				{
					text3 += string.Format(" and also banned ownerid {0}", basePlayer.net.connection.ownerid);
					ServerUsers.Set(basePlayer.net.connection.ownerid, ServerUsers.UserGroup.Banned, basePlayer.displayName, arg.GetString(1, string.Format("Family share owner of {0}", basePlayer.net.connection.userid)), num);
				}
				Chat.Broadcast(string.Concat(new string[] { "Kickbanning ", basePlayer.displayName, text2, " (", @string, ")" }), "SERVER", "#eee", 0UL);
				Net.sv.Kick(basePlayer.net.connection, "Banned" + text2 + ": " + @string, false);
			}
			ServerUsers.Set(@uint, ServerUsers.UserGroup.Banned, text, @string, num);
			arg.ReplyWith(string.Format("Banned User{0}: {1} - \"{2}\" for \"{3}\"{4}", new object[] { text2, @uint, text, @string, text3 }));
		}

		// Token: 0x0600412A RID: 16682 RVA: 0x00180A40 File Offset: 0x0017EC40
		private static bool TryGetBanExpiry(ConsoleSystem.Arg arg, int n, out long expiry, out string durationSuffix)
		{
			expiry = arg.GetTimestamp(n, -1L);
			durationSuffix = null;
			int num = Epoch.Current;
			if (expiry > 0L && expiry <= (long)num)
			{
				arg.ReplyWith("Expiry time is in the past");
				return false;
			}
			durationSuffix = ((expiry > 0L) ? (" for " + (expiry - (long)num).FormatSecondsLong()) : "");
			return true;
		}

		// Token: 0x0600412B RID: 16683 RVA: 0x00180AA0 File Offset: 0x0017ECA0
		[ServerVar]
		public static void unban(ConsoleSystem.Arg arg)
		{
			ulong @uint = arg.GetUInt64(0, 0UL);
			if (@uint < 70000000000000000UL)
			{
				arg.ReplyWith(string.Format("This doesn't appear to be a 64bit steamid: {0}", @uint));
				return;
			}
			ServerUsers.User user = ServerUsers.Get(@uint);
			if (user == null || user.group != ServerUsers.UserGroup.Banned)
			{
				arg.ReplyWith(string.Format("User {0} isn't banned", @uint));
				return;
			}
			ServerUsers.Remove(@uint);
			arg.ReplyWith("Unbanned User: " + @uint);
		}

		// Token: 0x0600412C RID: 16684 RVA: 0x00180B20 File Offset: 0x0017ED20
		[ServerVar]
		public static void skipqueue(ConsoleSystem.Arg arg)
		{
			ulong @uint = arg.GetUInt64(0, 0UL);
			if (@uint < 70000000000000000UL)
			{
				arg.ReplyWith("This doesn't appear to be a 64bit steamid: " + @uint);
				return;
			}
			SingletonComponent<ServerMgr>.Instance.connectionQueue.SkipQueue(@uint);
		}

		// Token: 0x0600412D RID: 16685 RVA: 0x00180B6C File Offset: 0x0017ED6C
		[ServerVar(Help = "Adds skip queue permissions to a SteamID")]
		public static void skipqueueid(ConsoleSystem.Arg arg)
		{
			ulong @uint = arg.GetUInt64(0, 0UL);
			string @string = arg.GetString(1, "unnamed");
			string string2 = arg.GetString(2, "no reason");
			if (@uint < 70000000000000000UL)
			{
				arg.ReplyWith("This doesn't appear to be a 64bit steamid: " + @uint);
				return;
			}
			ServerUsers.User user = ServerUsers.Get(@uint);
			if (user != null && (user.group == ServerUsers.UserGroup.Owner || user.group == ServerUsers.UserGroup.Moderator || user.group == ServerUsers.UserGroup.SkipQueue))
			{
				arg.ReplyWith(string.Format("User {0} will already skip the queue ({1})", @uint, user.group));
				return;
			}
			if (user != null && user.group == ServerUsers.UserGroup.Banned)
			{
				arg.ReplyWith(string.Format("User {0} is banned", @uint));
				return;
			}
			ServerUsers.Set(@uint, ServerUsers.UserGroup.SkipQueue, @string, string2, -1L);
			arg.ReplyWith(string.Format("Added skip queue permission for {0} ({1})", @string, @uint));
		}

		// Token: 0x0600412E RID: 16686 RVA: 0x00180C4C File Offset: 0x0017EE4C
		[ServerVar(Help = "Removes skip queue permission from a SteamID")]
		public static void removeskipqueue(ConsoleSystem.Arg arg)
		{
			ulong @uint = arg.GetUInt64(0, 0UL);
			if (@uint < 70000000000000000UL)
			{
				arg.ReplyWith("This doesn't appear to be a 64bit steamid: " + @uint);
				return;
			}
			ServerUsers.User user = ServerUsers.Get(@uint);
			if (user != null && (user.group == ServerUsers.UserGroup.Owner || user.group == ServerUsers.UserGroup.Moderator))
			{
				arg.ReplyWith(string.Format("User is a {0}, cannot remove skip queue permission with this command", user.group));
				return;
			}
			if (user == null || user.group != ServerUsers.UserGroup.SkipQueue)
			{
				arg.ReplyWith("User does not have skip queue permission");
				return;
			}
			ServerUsers.Remove(@uint);
			arg.ReplyWith("Removed skip queue permission: " + @uint);
		}

		// Token: 0x0600412F RID: 16687 RVA: 0x00180CF4 File Offset: 0x0017EEF4
		[ServerVar(Help = "Print out currently connected clients etc")]
		public static void players(ConsoleSystem.Arg arg)
		{
			TextTable textTable = new TextTable();
			textTable.AddColumn("id");
			textTable.AddColumn("name");
			textTable.AddColumn("ping");
			textTable.AddColumn("snap");
			textTable.AddColumn("updt");
			textTable.AddColumn("posi");
			textTable.AddColumn("dist");
			foreach (global::BasePlayer basePlayer in global::BasePlayer.activePlayerList)
			{
				string userIDString = basePlayer.UserIDString;
				string text = basePlayer.displayName.ToString();
				if (text.Length >= 14)
				{
					text = text.Substring(0, 14) + "..";
				}
				string text2 = text;
				string text3 = Net.sv.GetAveragePing(basePlayer.net.connection).ToString();
				string text4 = basePlayer.GetQueuedUpdateCount(global::BasePlayer.NetworkQueue.Update).ToString();
				string text5 = basePlayer.GetQueuedUpdateCount(global::BasePlayer.NetworkQueue.UpdateDistance).ToString();
				textTable.AddRow(new string[]
				{
					userIDString,
					text2,
					text3,
					string.Empty,
					text4,
					string.Empty,
					text5
				});
			}
			arg.ReplyWith(arg.HasArg("--json") ? textTable.ToJson() : textTable.ToString());
		}

		// Token: 0x06004130 RID: 16688 RVA: 0x00180E6C File Offset: 0x0017F06C
		[ServerVar(Help = "Sends a message in chat")]
		public static void say(ConsoleSystem.Arg arg)
		{
			Chat.Broadcast(arg.FullString, "SERVER", "#eee", 0UL);
		}

		// Token: 0x06004131 RID: 16689 RVA: 0x00180E88 File Offset: 0x0017F088
		[ServerVar(Help = "Show user info for players on server.")]
		public static void users(ConsoleSystem.Arg arg)
		{
			string text = "<slot:userid:\"name\">\n";
			int num = 0;
			foreach (global::BasePlayer basePlayer in global::BasePlayer.activePlayerList)
			{
				text = string.Concat(new object[] { text, basePlayer.userID, ":\"", basePlayer.displayName, "\"\n" });
				num++;
			}
			text = text + num.ToString() + "users\n";
			arg.ReplyWith(text);
		}

		// Token: 0x06004132 RID: 16690 RVA: 0x00180F30 File Offset: 0x0017F130
		[ServerVar(Help = "Show user info for players on server.")]
		public static void sleepingusers(ConsoleSystem.Arg arg)
		{
			string text = "<slot:userid:\"name\">\n";
			int num = 0;
			foreach (global::BasePlayer basePlayer in global::BasePlayer.sleepingPlayerList)
			{
				text += string.Format("{0}:{1}\n", basePlayer.userID, basePlayer.displayName);
				num++;
			}
			text += string.Format("{0} sleeping users\n", num);
			arg.ReplyWith(text);
		}

		// Token: 0x06004133 RID: 16691 RVA: 0x00180FC8 File Offset: 0x0017F1C8
		[ServerVar(Help = "Show user info for sleeping players on server in range of the player.")]
		public static void sleepingusersinrange(ConsoleSystem.Arg arg)
		{
			global::BasePlayer fromPlayer = arg.Player();
			if (fromPlayer == null)
			{
				return;
			}
			float range = arg.GetFloat(0, 0f);
			string text = "<slot:userid:\"name\">\n";
			int num = 0;
			List<global::BasePlayer> list = Pool.GetList<global::BasePlayer>();
			foreach (global::BasePlayer basePlayer3 in global::BasePlayer.sleepingPlayerList)
			{
				list.Add(basePlayer3);
			}
			list.RemoveAll((global::BasePlayer p) => p.Distance2D(fromPlayer) > range);
			list.Sort(delegate(global::BasePlayer player, global::BasePlayer basePlayer)
			{
				if (player.Distance2D(fromPlayer) >= basePlayer.Distance2D(fromPlayer))
				{
					return 1;
				}
				return -1;
			});
			foreach (global::BasePlayer basePlayer2 in list)
			{
				text += string.Format("{0}:{1}:{2}m\n", basePlayer2.userID, basePlayer2.displayName, basePlayer2.Distance2D(fromPlayer));
				num++;
			}
			Pool.FreeList<global::BasePlayer>(ref list);
			text += string.Format("{0} sleeping users within {1}m\n", num, range);
			arg.ReplyWith(text);
		}

		// Token: 0x06004134 RID: 16692 RVA: 0x00181128 File Offset: 0x0017F328
		[ServerVar(Help = "Show user info for players on server in range of the player.")]
		public static void usersinrange(ConsoleSystem.Arg arg)
		{
			global::BasePlayer fromPlayer = arg.Player();
			if (fromPlayer == null)
			{
				return;
			}
			float range = arg.GetFloat(0, 0f);
			string text = "<slot:userid:\"name\">\n";
			int num = 0;
			List<global::BasePlayer> list = Pool.GetList<global::BasePlayer>();
			foreach (global::BasePlayer basePlayer3 in global::BasePlayer.activePlayerList)
			{
				list.Add(basePlayer3);
			}
			list.RemoveAll((global::BasePlayer p) => p.Distance2D(fromPlayer) > range);
			list.Sort(delegate(global::BasePlayer player, global::BasePlayer basePlayer)
			{
				if (player.Distance2D(fromPlayer) >= basePlayer.Distance2D(fromPlayer))
				{
					return 1;
				}
				return -1;
			});
			foreach (global::BasePlayer basePlayer2 in list)
			{
				text += string.Format("{0}:{1}:{2}m\n", basePlayer2.userID, basePlayer2.displayName, basePlayer2.Distance2D(fromPlayer));
				num++;
			}
			Pool.FreeList<global::BasePlayer>(ref list);
			text += string.Format("{0} users within {1}m\n", num, range);
			arg.ReplyWith(text);
		}

		// Token: 0x06004135 RID: 16693 RVA: 0x00181288 File Offset: 0x0017F488
		[ServerVar(Help = "Show user info for players on server in range of the supplied player (eg. Jim 50)")]
		public static void usersinrangeofplayer(ConsoleSystem.Arg arg)
		{
			global::BasePlayer targetPlayer = arg.GetPlayerOrSleeper(0);
			if (targetPlayer == null)
			{
				return;
			}
			float range = arg.GetFloat(1, 0f);
			string text = "<slot:userid:\"name\">\n";
			int num = 0;
			List<global::BasePlayer> list = Pool.GetList<global::BasePlayer>();
			foreach (global::BasePlayer basePlayer3 in global::BasePlayer.activePlayerList)
			{
				list.Add(basePlayer3);
			}
			list.RemoveAll((global::BasePlayer p) => p.Distance2D(targetPlayer) > range);
			list.Sort(delegate(global::BasePlayer player, global::BasePlayer basePlayer)
			{
				if (player.Distance2D(targetPlayer) >= basePlayer.Distance2D(targetPlayer))
				{
					return 1;
				}
				return -1;
			});
			foreach (global::BasePlayer basePlayer2 in list)
			{
				text += string.Format("{0}:{1}:{2}m\n", basePlayer2.userID, basePlayer2.displayName, basePlayer2.Distance2D(targetPlayer));
				num++;
			}
			Pool.FreeList<global::BasePlayer>(ref list);
			text += string.Format("{0} users within {1}m of {2}\n", num, range, targetPlayer.displayName);
			arg.ReplyWith(text);
		}

		// Token: 0x06004136 RID: 16694 RVA: 0x001813F4 File Offset: 0x0017F5F4
		[ServerVar(Help = "List of banned users (sourceds compat)")]
		public static void banlist(ConsoleSystem.Arg arg)
		{
			arg.ReplyWith(ServerUsers.BanListString(false));
		}

		// Token: 0x06004137 RID: 16695 RVA: 0x00181402 File Offset: 0x0017F602
		[ServerVar(Help = "List of banned users - shows reasons and usernames")]
		public static void banlistex(ConsoleSystem.Arg arg)
		{
			arg.ReplyWith(ServerUsers.BanListStringEx());
		}

		// Token: 0x06004138 RID: 16696 RVA: 0x0018140F File Offset: 0x0017F60F
		[ServerVar(Help = "List of banned users, by ID (sourceds compat)")]
		public static void listid(ConsoleSystem.Arg arg)
		{
			arg.ReplyWith(ServerUsers.BanListString(true));
		}

		// Token: 0x06004139 RID: 16697 RVA: 0x00181420 File Offset: 0x0017F620
		[ServerVar]
		public static void mute(ConsoleSystem.Arg arg)
		{
			global::BasePlayer playerOrSleeper = arg.GetPlayerOrSleeper(0);
			if (!playerOrSleeper || playerOrSleeper.net == null || playerOrSleeper.net.connection == null)
			{
				arg.ReplyWith("Player not found");
				return;
			}
			playerOrSleeper.SetPlayerFlag(global::BasePlayer.PlayerFlags.ChatMute, true);
		}

		// Token: 0x0600413A RID: 16698 RVA: 0x0018146C File Offset: 0x0017F66C
		[ServerVar]
		public static void unmute(ConsoleSystem.Arg arg)
		{
			global::BasePlayer playerOrSleeper = arg.GetPlayerOrSleeper(0);
			if (!playerOrSleeper || playerOrSleeper.net == null || playerOrSleeper.net.connection == null)
			{
				arg.ReplyWith("Player not found");
				return;
			}
			playerOrSleeper.SetPlayerFlag(global::BasePlayer.PlayerFlags.ChatMute, false);
		}

		// Token: 0x0600413B RID: 16699 RVA: 0x001814B8 File Offset: 0x0017F6B8
		[ServerVar(Help = "Print a list of currently muted players")]
		public static void mutelist(ConsoleSystem.Arg arg)
		{
			var enumerable = from x in global::BasePlayer.allPlayerList
				where x.HasPlayerFlag(global::BasePlayer.PlayerFlags.ChatMute)
				select new
				{
					SteamId = x.UserIDString,
					Name = x.displayName
				};
			arg.ReplyWith(enumerable);
		}

		// Token: 0x0600413C RID: 16700 RVA: 0x0018151C File Offset: 0x0017F71C
		[ServerVar]
		public static void clientperf(ConsoleSystem.Arg arg)
		{
			string @string = arg.GetString(0, "legacy");
			int @int = arg.GetInt(1, UnityEngine.Random.Range(int.MinValue, int.MaxValue));
			foreach (global::BasePlayer basePlayer in global::BasePlayer.activePlayerList)
			{
				basePlayer.ClientRPCPlayer<string, int>(null, basePlayer, "GetPerformanceReport", @string, @int);
			}
		}

		// Token: 0x0600413D RID: 16701 RVA: 0x0018159C File Offset: 0x0017F79C
		[ServerVar]
		public static void clientperf_frametime(ConsoleSystem.Arg arg)
		{
			ClientFrametimeRequest clientFrametimeRequest = new ClientFrametimeRequest
			{
				request_id = arg.GetInt(0, UnityEngine.Random.Range(int.MinValue, int.MaxValue)),
				start_frame = arg.GetInt(1, 0),
				max_frames = arg.GetInt(2, 1000)
			};
			foreach (global::BasePlayer basePlayer in global::BasePlayer.activePlayerList)
			{
				basePlayer.ClientRPCPlayer<string>(null, basePlayer, "GetPerformanceReport_Frametime", JsonConvert.SerializeObject(clientFrametimeRequest));
			}
		}

		// Token: 0x0600413E RID: 16702 RVA: 0x0018163C File Offset: 0x0017F83C
		[ServerVar(Help = "Get information about all the cars in the world")]
		public static void carstats(ConsoleSystem.Arg arg)
		{
			HashSet<global::ModularCar> allCarsList = global::ModularCar.allCarsList;
			TextTable textTable = new TextTable();
			textTable.AddColumn("id");
			textTable.AddColumn("sockets");
			textTable.AddColumn("modules");
			textTable.AddColumn("complete");
			textTable.AddColumn("engine");
			textTable.AddColumn("health");
			textTable.AddColumn("location");
			int count = allCarsList.Count;
			int num = 0;
			int num2 = 0;
			int num3 = 0;
			foreach (global::ModularCar modularCar in allCarsList)
			{
				string text = modularCar.net.ID.ToString();
				string text2 = modularCar.TotalSockets.ToString();
				string text3 = modularCar.NumAttachedModules.ToString();
				string text4;
				if (modularCar.IsComplete())
				{
					text4 = "Complete";
					num++;
				}
				else
				{
					text4 = "Partial";
				}
				string text5;
				if (modularCar.HasAnyWorkingEngines())
				{
					text5 = "Working";
					num2++;
				}
				else
				{
					text5 = "Broken";
				}
				string text6;
				if (modularCar.TotalMaxHealth() == 0f)
				{
					text6 = "0";
				}
				else
				{
					text6 = string.Format("{0:0%}", modularCar.TotalHealth() / modularCar.TotalMaxHealth());
				}
				string text7;
				if (modularCar.IsOutside())
				{
					text7 = "Outside";
				}
				else
				{
					text7 = "Inside";
					num3++;
				}
				textTable.AddRow(new string[] { text, text2, text3, text4, text5, text6, text7 });
			}
			string text8 = "";
			if (count == 1)
			{
				text8 += "\nThe world contains 1 modular car.";
			}
			else
			{
				text8 += string.Format("\nThe world contains {0} modular cars.", count);
			}
			if (num == 1)
			{
				text8 += string.Format("\n1 ({0:0%}) is in a completed state.", 1f / (float)count);
			}
			else
			{
				text8 += string.Format("\n{0} ({1:0%}) are in a completed state.", num, (float)num / (float)count);
			}
			if (num2 == 1)
			{
				text8 += string.Format("\n1 ({0:0%}) is driveable.", 1f / (float)count);
			}
			else
			{
				text8 += string.Format("\n{0} ({1:0%}) are driveable.", num2, (float)num2 / (float)count);
			}
			if (num3 == 1)
			{
				text8 += string.Format("\n1 ({0:0%}) is sheltered indoors.", 1f / (float)count);
			}
			else
			{
				text8 += string.Format("\n{0} ({1:0%}) are sheltered indoors.", num3, (float)num3 / (float)count);
			}
			arg.ReplyWith(textTable.ToString() + text8);
		}

		// Token: 0x0600413F RID: 16703 RVA: 0x00181920 File Offset: 0x0017FB20
		[ServerVar]
		public static string teaminfo(ConsoleSystem.Arg arg)
		{
			ulong num = arg.GetUInt64(0, 0UL);
			if (num == 0UL)
			{
				global::BasePlayer player = arg.GetPlayer(0);
				if (player == null)
				{
					return "Player not found";
				}
				num = player.userID;
			}
			global::RelationshipManager.PlayerTeam playerTeam = global::RelationshipManager.ServerInstance.FindPlayersTeam(num);
			if (playerTeam == null)
			{
				return "Player is not in a team";
			}
			TextTable textTable = new TextTable();
			textTable.AddColumn("steamID");
			textTable.AddColumn("username");
			textTable.AddColumn("online");
			textTable.AddColumn("leader");
			using (List<ulong>.Enumerator enumerator = playerTeam.members.GetEnumerator())
			{
				while (enumerator.MoveNext())
				{
					ulong memberId = enumerator.Current;
					bool flag = Net.sv.connections.FirstOrDefault((Connection c) => c.connected && c.userid == memberId) != null;
					textTable.AddRow(new string[]
					{
						memberId.ToString(),
						Admin.GetPlayerName(memberId),
						flag ? "x" : "",
						(memberId == playerTeam.teamLeader) ? "x" : ""
					});
				}
			}
			if (!arg.HasArg("--json"))
			{
				return textTable.ToString();
			}
			return textTable.ToJson();
		}

		// Token: 0x06004140 RID: 16704 RVA: 0x00181A88 File Offset: 0x0017FC88
		[ServerVar]
		public static void authradius(ConsoleSystem.Arg arg)
		{
			float @float = arg.GetFloat(0, -1f);
			if (@float < 0f)
			{
				arg.ReplyWith("Format is 'authradius {radius} [user]'");
				return;
			}
			Admin.SetAuthInRadius(arg.GetPlayer(1) ?? arg.Player(), @float, true);
		}

		// Token: 0x06004141 RID: 16705 RVA: 0x00181AD0 File Offset: 0x0017FCD0
		[ServerVar]
		public static void deauthradius(ConsoleSystem.Arg arg)
		{
			float @float = arg.GetFloat(0, -1f);
			if (@float < 0f)
			{
				arg.ReplyWith("Format is 'deauthradius {radius} [user]'");
				return;
			}
			Admin.SetAuthInRadius(arg.GetPlayer(1) ?? arg.Player(), @float, false);
		}

		// Token: 0x06004142 RID: 16706 RVA: 0x00181B18 File Offset: 0x0017FD18
		private static void SetAuthInRadius(global::BasePlayer player, float radius, bool auth)
		{
			List<global::BaseEntity> list = new List<global::BaseEntity>();
			Vis.Entities<global::BaseEntity>(player.transform.position, radius, list, -1, QueryTriggerInteraction.Collide);
			foreach (global::BaseEntity baseEntity in list)
			{
				if (baseEntity.isServer && !Admin.SetUserAuthorized(baseEntity, player.userID, auth))
				{
					Admin.SetUserAuthorized(baseEntity.GetSlot(global::BaseEntity.Slot.Lock), player.userID, auth);
				}
			}
		}

		// Token: 0x06004143 RID: 16707 RVA: 0x00181BA4 File Offset: 0x0017FDA4
		private static bool SetUserAuthorized(global::BaseEntity entity, ulong userId, bool state)
		{
			if (entity == null)
			{
				return false;
			}
			global::CodeLock codeLock;
			global::AutoTurret autoTurret;
			if ((codeLock = entity as global::CodeLock) != null)
			{
				if (state)
				{
					codeLock.whitelistPlayers.Add(userId);
				}
				else
				{
					codeLock.whitelistPlayers.Remove(userId);
					codeLock.guestPlayers.Remove(userId);
				}
				codeLock.SendNetworkUpdate(global::BasePlayer.NetworkQueue.Update);
			}
			else if ((autoTurret = entity as global::AutoTurret) != null)
			{
				if (state)
				{
					autoTurret.authorizedPlayers.Add(new PlayerNameID
					{
						ShouldPool = false,
						userid = userId,
						username = ""
					});
				}
				else
				{
					autoTurret.authorizedPlayers.RemoveAll((PlayerNameID x) => x.userid == userId);
				}
				autoTurret.SendNetworkUpdate(global::BasePlayer.NetworkQueue.Update);
			}
			else
			{
				BuildingPrivlidge buildingPrivlidge;
				if ((buildingPrivlidge = entity as BuildingPrivlidge) == null)
				{
					return false;
				}
				if (state)
				{
					buildingPrivlidge.authorizedPlayers.Add(new PlayerNameID
					{
						ShouldPool = false,
						userid = userId,
						username = ""
					});
				}
				else
				{
					buildingPrivlidge.authorizedPlayers.RemoveAll((PlayerNameID x) => x.userid == userId);
				}
				buildingPrivlidge.SendNetworkUpdate(global::BasePlayer.NetworkQueue.Update);
			}
			return true;
		}

		// Token: 0x06004144 RID: 16708 RVA: 0x00181CD8 File Offset: 0x0017FED8
		[ServerVar]
		public static void entid(ConsoleSystem.Arg arg)
		{
			global::BaseEntity baseEntity = global::BaseNetworkable.serverEntities.Find(arg.GetEntityID(1, default(NetworkableId))) as global::BaseEntity;
			if (baseEntity == null)
			{
				return;
			}
			if (baseEntity is global::BasePlayer)
			{
				return;
			}
			string @string = arg.GetString(0, "");
			if (arg.Player() != null)
			{
				Debug.Log(string.Concat(new object[]
				{
					"[ENTCMD] ",
					arg.Player().displayName,
					"/",
					arg.Player().userID,
					" used *",
					@string,
					"* on ent: ",
					baseEntity.name
				}));
			}
			uint num = <PrivateImplementationDetails>.ComputeStringHash(@string);
			if (num <= 2152183181U)
			{
				if (num <= 720644751U)
				{
					if (num != 693242804U)
					{
						if (num == 720644751U)
						{
							if (@string == "who")
							{
								arg.ReplyWith(baseEntity.Admin_Who());
								return;
							}
						}
					}
					else if (@string == "repair")
					{
						Admin.RunInRadius<BaseCombatEntity>(arg.GetFloat(2, 0f), baseEntity, delegate(BaseCombatEntity entity)
						{
							if (entity.repair.enabled)
							{
								entity.SetHealth(entity.MaxHealth());
							}
						}, null);
					}
				}
				else if (num != 1449533269U)
				{
					if (num != 1483009432U)
					{
						if (num == 2152183181U)
						{
							if (@string == "undebug")
							{
								baseEntity.SetFlag(global::BaseEntity.Flags.Debugging, false, false, true);
								return;
							}
						}
					}
					else if (@string == "debug")
					{
						baseEntity.SetFlag(global::BaseEntity.Flags.Debugging, true, false, true);
						return;
					}
				}
				else if (@string == "unlock")
				{
					baseEntity.SetFlag(global::BaseEntity.Flags.Locked, false, false, true);
					return;
				}
			}
			else if (num <= 3306112409U)
			{
				if (num != 2382367150U)
				{
					if (num != 2503977039U)
					{
						if (num == 3306112409U)
						{
							if (@string == "kill")
							{
								baseEntity.AdminKill();
								return;
							}
						}
					}
					else if (@string == "auth")
					{
						arg.ReplyWith(Admin.AuthList(baseEntity));
						return;
					}
				}
				else if (@string == "setgrade")
				{
					arg.ReplyWith(Admin.ChangeGrade(baseEntity, 0, 0, (BuildingGrade.Enum)arg.GetInt(2, 0), arg.GetFloat(3, 0f)));
					return;
				}
			}
			else if (num != 3700935799U)
			{
				if (num != 3846680516U)
				{
					if (num == 4010637378U)
					{
						if (@string == "lock")
						{
							baseEntity.SetFlag(global::BaseEntity.Flags.Locked, true, false, true);
							return;
						}
					}
				}
				else if (@string == "downgrade")
				{
					arg.ReplyWith(Admin.ChangeGrade(baseEntity, 0, arg.GetInt(2, 1), BuildingGrade.Enum.None, arg.GetFloat(3, 0f)));
					return;
				}
			}
			else if (@string == "upgrade")
			{
				arg.ReplyWith(Admin.ChangeGrade(baseEntity, arg.GetInt(2, 1), 0, BuildingGrade.Enum.None, arg.GetFloat(3, 0f)));
				return;
			}
			arg.ReplyWith("Unknown command");
		}

		// Token: 0x06004145 RID: 16709 RVA: 0x00182018 File Offset: 0x00180218
		private static string AuthList(global::BaseEntity ent)
		{
			if (ent != null)
			{
				BuildingPrivlidge buildingPrivlidge;
				List<PlayerNameID> list;
				if ((buildingPrivlidge = ent as BuildingPrivlidge) == null)
				{
					global::AutoTurret autoTurret;
					if ((autoTurret = ent as global::AutoTurret) == null)
					{
						global::CodeLock codeLock;
						if ((codeLock = ent as global::CodeLock) != null)
						{
							return Admin.CodeLockAuthList(codeLock);
						}
						BaseVehicleModule baseVehicleModule;
						if ((baseVehicleModule = ent as BaseVehicleModule) != null)
						{
							return Admin.CodeLockAuthList(baseVehicleModule);
						}
						goto IL_55;
					}
					else
					{
						list = autoTurret.authorizedPlayers;
					}
				}
				else
				{
					list = buildingPrivlidge.authorizedPlayers;
				}
				if (list == null || list.Count == 0)
				{
					return "Nobody is authed to this entity";
				}
				TextTable textTable = new TextTable();
				textTable.AddColumn("steamID");
				textTable.AddColumn("username");
				foreach (PlayerNameID playerNameID in list)
				{
					textTable.AddRow(new string[]
					{
						playerNameID.userid.ToString(),
						Admin.GetPlayerName(playerNameID.userid)
					});
				}
				return textTable.ToString();
			}
			IL_55:
			return "Entity has no auth list";
		}

		// Token: 0x06004146 RID: 16710 RVA: 0x0018211C File Offset: 0x0018031C
		private static string CodeLockAuthList(global::CodeLock codeLock)
		{
			if (codeLock.whitelistPlayers.Count == 0 && codeLock.guestPlayers.Count == 0)
			{
				return "Nobody is authed to this entity";
			}
			TextTable textTable = new TextTable();
			textTable.AddColumn("steamID");
			textTable.AddColumn("username");
			textTable.AddColumn("isGuest");
			foreach (ulong num in codeLock.whitelistPlayers)
			{
				textTable.AddRow(new string[]
				{
					num.ToString(),
					Admin.GetPlayerName(num),
					""
				});
			}
			foreach (ulong num2 in codeLock.guestPlayers)
			{
				textTable.AddRow(new string[]
				{
					num2.ToString(),
					Admin.GetPlayerName(num2),
					"x"
				});
			}
			return textTable.ToString();
		}

		// Token: 0x06004147 RID: 16711 RVA: 0x00182240 File Offset: 0x00180440
		private static string CodeLockAuthList(BaseVehicleModule vehicleModule)
		{
			if (!vehicleModule.IsOnAVehicle)
			{
				return "Nobody is authed to this entity";
			}
			global::ModularCar modularCar = vehicleModule.Vehicle as global::ModularCar;
			if (modularCar == null || !modularCar.IsLockable || modularCar.CarLock.WhitelistPlayers.Count == 0)
			{
				return "Nobody is authed to this entity";
			}
			TextTable textTable = new TextTable();
			textTable.AddColumn("steamID");
			textTable.AddColumn("username");
			foreach (ulong num in modularCar.CarLock.WhitelistPlayers)
			{
				textTable.AddRow(new string[]
				{
					num.ToString(),
					Admin.GetPlayerName(num)
				});
			}
			return textTable.ToString();
		}

		// Token: 0x06004148 RID: 16712 RVA: 0x00182318 File Offset: 0x00180518
		public static string GetPlayerName(ulong steamId)
		{
			global::BasePlayer basePlayer = global::BasePlayer.allPlayerList.FirstOrDefault((global::BasePlayer p) => p.userID == steamId);
			string text;
			if (!(basePlayer != null))
			{
				if ((text = SingletonComponent<ServerMgr>.Instance.persistance.GetPlayerName(steamId)) == null)
				{
					return "[unknown]";
				}
			}
			else
			{
				text = basePlayer.displayName;
			}
			return text;
		}

		// Token: 0x06004149 RID: 16713 RVA: 0x00182378 File Offset: 0x00180578
		public static string ChangeGrade(global::BaseEntity entity, int increaseBy = 0, int decreaseBy = 0, BuildingGrade.Enum targetGrade = BuildingGrade.Enum.None, float radius = 0f)
		{
			if (entity as global::BuildingBlock == null)
			{
				return string.Format("'{0}' is not a building block", entity);
			}
			Admin.RunInRadius<global::BuildingBlock>(radius, entity, delegate(global::BuildingBlock block)
			{
				BuildingGrade.Enum @enum = block.grade;
				if (targetGrade > BuildingGrade.Enum.None && targetGrade < BuildingGrade.Enum.Count)
				{
					@enum = targetGrade;
				}
				else
				{
					@enum = (BuildingGrade.Enum)Mathf.Min((int)(@enum + increaseBy), 4);
					@enum = (BuildingGrade.Enum)Mathf.Max(@enum - (BuildingGrade.Enum)decreaseBy, 0);
				}
				if (@enum != block.grade)
				{
					block.ChangeGrade(@enum, false, true);
				}
			}, null);
			int count = Pool.GetList<global::BuildingBlock>().Count;
			return string.Format("Upgraded/downgraded '{0}' building block(s)", count);
		}

		// Token: 0x0600414A RID: 16714 RVA: 0x001823EC File Offset: 0x001805EC
		private static bool RunInRadius<T>(float radius, global::BaseEntity initial, Action<T> callback, Func<T, bool> filter = null) where T : global::BaseEntity
		{
			List<T> list = Pool.GetList<T>();
			radius = Mathf.Clamp(radius, 0f, 200f);
			T t;
			if (radius > 0f)
			{
				Vis.Entities<T>(initial.transform.position, radius, list, 2097152, QueryTriggerInteraction.Collide);
			}
			else if ((t = initial as T) != null)
			{
				list.Add(t);
			}
			foreach (T t2 in list)
			{
				try
				{
					callback(t2);
				}
				catch (Exception ex)
				{
					Debug.LogError(string.Format("Exception while running callback in radius: {0}", ex));
					return false;
				}
			}
			return true;
		}

		// Token: 0x0600414B RID: 16715 RVA: 0x001824BC File Offset: 0x001806BC
		[ServerVar(Help = "Get a list of players")]
		public static Admin.PlayerInfo[] playerlist()
		{
			return global::BasePlayer.activePlayerList.Select((global::BasePlayer x) => new Admin.PlayerInfo
			{
				SteamID = x.UserIDString,
				OwnerSteamID = x.OwnerID.ToString(),
				DisplayName = x.displayName,
				Ping = Net.sv.GetAveragePing(x.net.connection),
				Address = x.net.connection.ipaddress,
				ConnectedSeconds = (int)x.net.connection.GetSecondsConnected(),
				VoiationLevel = x.violationLevel,
				Health = x.Health()
			}).ToArray<Admin.PlayerInfo>();
		}

		// Token: 0x0600414C RID: 16716 RVA: 0x001824EC File Offset: 0x001806EC
		[ServerVar(Help = "List of banned users")]
		public static ServerUsers.User[] Bans()
		{
			return ServerUsers.GetAll(ServerUsers.UserGroup.Banned).ToArray<ServerUsers.User>();
		}

		// Token: 0x0600414D RID: 16717 RVA: 0x001824FC File Offset: 0x001806FC
		[ServerVar(Help = "Get a list of information about the server")]
		public static Admin.ServerInfoOutput ServerInfo()
		{
			return new Admin.ServerInfoOutput
			{
				Hostname = ConVar.Server.hostname,
				MaxPlayers = ConVar.Server.maxplayers,
				Players = global::BasePlayer.activePlayerList.Count,
				Queued = SingletonComponent<ServerMgr>.Instance.connectionQueue.Queued,
				Joining = SingletonComponent<ServerMgr>.Instance.connectionQueue.Joining,
				EntityCount = global::BaseNetworkable.serverEntities.Count,
				GameTime = ((TOD_Sky.Instance != null) ? TOD_Sky.Instance.Cycle.DateTime.ToString() : DateTime.UtcNow.ToString()),
				Uptime = (int)Time.realtimeSinceStartup,
				Map = ConVar.Server.level,
				Framerate = (float)global::Performance.report.frameRate,
				Memory = (int)global::Performance.report.memoryAllocations,
				MemoryUsageSystem = (int)global::Performance.report.memoryUsageSystem,
				Collections = (int)global::Performance.report.memoryCollections,
				NetworkIn = ((Net.sv == null) ? 0 : ((int)Net.sv.GetStat(null, BaseNetwork.StatTypeLong.BytesReceived_LastSecond))),
				NetworkOut = ((Net.sv == null) ? 0 : ((int)Net.sv.GetStat(null, BaseNetwork.StatTypeLong.BytesSent_LastSecond))),
				Restarting = SingletonComponent<ServerMgr>.Instance.Restarting,
				SaveCreatedTime = SaveRestore.SaveCreatedTime.ToString(),
				Version = 2397,
				Protocol = Protocol.printable
			};
		}

		// Token: 0x0600414E RID: 16718 RVA: 0x00182689 File Offset: 0x00180889
		[ServerVar(Help = "Get information about this build")]
		public static BuildInfo BuildInfo()
		{
			return Facepunch.BuildInfo.Current;
		}

		// Token: 0x0600414F RID: 16719 RVA: 0x00182690 File Offset: 0x00180890
		[ServerVar]
		public static void AdminUI_FullRefresh(ConsoleSystem.Arg arg)
		{
			Admin.AdminUI_RequestPlayerList(arg);
			Admin.AdminUI_RequestServerInfo(arg);
			Admin.AdminUI_RequestServerConvars(arg);
			Admin.AdminUI_RequestUGCList(arg);
		}

		// Token: 0x06004150 RID: 16720 RVA: 0x001826AA File Offset: 0x001808AA
		[ServerVar]
		public static void AdminUI_RequestPlayerList(ConsoleSystem.Arg arg)
		{
			if (!Admin.allowAdminUI)
			{
				return;
			}
			ConsoleNetwork.SendClientCommand(arg.Connection, "AdminUI_ReceivePlayerList", new object[] { JsonConvert.SerializeObject(Admin.playerlist()) });
		}

		// Token: 0x06004151 RID: 16721 RVA: 0x001826D7 File Offset: 0x001808D7
		[ServerVar]
		public static void AdminUI_RequestServerInfo(ConsoleSystem.Arg arg)
		{
			if (!Admin.allowAdminUI)
			{
				return;
			}
			ConsoleNetwork.SendClientCommand(arg.Connection, "AdminUI_ReceiveServerInfo", new object[] { JsonConvert.SerializeObject(Admin.ServerInfo()) });
		}

		// Token: 0x06004152 RID: 16722 RVA: 0x0018270C File Offset: 0x0018090C
		[ServerVar]
		public static void AdminUI_RequestServerConvars(ConsoleSystem.Arg arg)
		{
			if (!Admin.allowAdminUI)
			{
				return;
			}
			List<Admin.ServerConvarInfo> list = Pool.GetList<Admin.ServerConvarInfo>();
			foreach (ConsoleSystem.Command command in ConsoleSystem.Index.All)
			{
				if (command.Server && command.Variable && command.ServerAdmin && command.ShowInAdminUI)
				{
					List<Admin.ServerConvarInfo> list2 = list;
					Admin.ServerConvarInfo serverConvarInfo = default(Admin.ServerConvarInfo);
					serverConvarInfo.FullName = command.FullName;
					Func<string> getOveride = command.GetOveride;
					serverConvarInfo.Value = ((getOveride != null) ? getOveride() : null);
					serverConvarInfo.Help = command.Description;
					list2.Add(serverConvarInfo);
				}
			}
			ConsoleNetwork.SendClientCommand(arg.Connection, "AdminUI_ReceiveCommands", new object[] { JsonConvert.SerializeObject(list) });
			Pool.FreeList<Admin.ServerConvarInfo>(ref list);
		}

		// Token: 0x06004153 RID: 16723 RVA: 0x001827C8 File Offset: 0x001809C8
		[ServerVar]
		public static void AdminUI_RequestUGCList(ConsoleSystem.Arg arg)
		{
			if (!Admin.allowAdminUI)
			{
				return;
			}
			List<Admin.ServerUGCInfo> list = Pool.GetList<Admin.ServerUGCInfo>();
			foreach (global::BaseNetworkable baseNetworkable in global::BaseNetworkable.serverEntities)
			{
				uint[] array = null;
				ulong[] array2 = null;
				UGCType ugctype = UGCType.ImageJpg;
				IUGCBrowserEntity iugcbrowserEntity;
				if (baseNetworkable.TryGetComponent<IUGCBrowserEntity>(out iugcbrowserEntity))
				{
					array = iugcbrowserEntity.GetContentCRCs;
					array2 = iugcbrowserEntity.EditingHistory.ToArray();
					ugctype = iugcbrowserEntity.ContentType;
				}
				if (array != null && array.Length != 0)
				{
					bool flag = false;
					uint[] array3 = array;
					for (int i = 0; i < array3.Length; i++)
					{
						if (array3[i] != 0U)
						{
							flag = true;
							break;
						}
					}
					if (ugctype == UGCType.PatternBoomer)
					{
						flag = true;
					}
					if (flag)
					{
						list.Add(new Admin.ServerUGCInfo
						{
							entityId = baseNetworkable.net.ID,
							crcs = array,
							contentType = ugctype,
							entityPrefabID = baseNetworkable.prefabID,
							shortPrefabName = baseNetworkable.ShortPrefabName,
							playerIds = array2
						});
					}
				}
			}
			ConsoleNetwork.SendClientCommand(arg.Connection, "AdminUI_ReceiveUGCList", new object[] { JsonConvert.SerializeObject(list) });
			Pool.FreeList<Admin.ServerUGCInfo>(ref list);
		}

		// Token: 0x06004154 RID: 16724 RVA: 0x00182914 File Offset: 0x00180B14
		[ServerVar]
		public static void AdminUI_RequestUGCContent(ConsoleSystem.Arg arg)
		{
			if (!Admin.allowAdminUI || arg.Player() == null)
			{
				return;
			}
			uint @uint = arg.GetUInt(0, 0U);
			NetworkableId entityID = arg.GetEntityID(1, default(NetworkableId));
			FileStorage.Type @int = (FileStorage.Type)arg.GetInt(2, 0);
			uint uint2 = arg.GetUInt(3, 0U);
			byte[] array = FileStorage.server.Get(@uint, @int, entityID, uint2);
			if (array == null)
			{
				return;
			}
			SendInfo sendInfo = new SendInfo(arg.Connection)
			{
				channel = 2,
				method = SendMethod.Reliable
			};
			arg.Player().ClientRPCEx<uint, uint, byte[], uint, byte>(sendInfo, null, "AdminReceivedUGC", @uint, (uint)array.Length, array, uint2, (byte)@int);
		}

		// Token: 0x06004155 RID: 16725 RVA: 0x001829B8 File Offset: 0x00180BB8
		[ServerVar]
		public static void AdminUI_DeleteUGCContent(ConsoleSystem.Arg arg)
		{
			if (!Admin.allowAdminUI)
			{
				return;
			}
			NetworkableId entityID = arg.GetEntityID(0, default(NetworkableId));
			global::BaseNetworkable baseNetworkable = global::BaseNetworkable.serverEntities.Find(entityID);
			if (baseNetworkable != null)
			{
				FileStorage.server.RemoveAllByEntity(entityID);
				IUGCBrowserEntity iugcbrowserEntity;
				if (baseNetworkable.TryGetComponent<IUGCBrowserEntity>(out iugcbrowserEntity))
				{
					iugcbrowserEntity.ClearContent();
				}
			}
		}

		// Token: 0x06004156 RID: 16726 RVA: 0x00182A10 File Offset: 0x00180C10
		[ServerVar]
		public static void AdminUI_RequestFireworkPattern(ConsoleSystem.Arg arg)
		{
			if (!Admin.allowAdminUI)
			{
				return;
			}
			NetworkableId entityID = arg.GetEntityID(0, default(NetworkableId));
			global::BaseNetworkable baseNetworkable = global::BaseNetworkable.serverEntities.Find(entityID);
			global::PatternFirework patternFirework;
			if (baseNetworkable != null && (patternFirework = baseNetworkable as global::PatternFirework) != null)
			{
				SendInfo sendInfo = new SendInfo(arg.Connection)
				{
					channel = 2,
					method = SendMethod.Reliable
				};
				arg.Player().ClientRPCEx<NetworkableId, byte[]>(sendInfo, null, "AdminReceivedPatternFirework", entityID, patternFirework.Design.ToProtoBytes());
			}
		}

		// Token: 0x06004157 RID: 16727 RVA: 0x00182A98 File Offset: 0x00180C98
		[ServerVar]
		public static void clearugcentity(ConsoleSystem.Arg arg)
		{
			NetworkableId entityID = arg.GetEntityID(0, default(NetworkableId));
			global::BaseNetworkable baseNetworkable = global::BaseNetworkable.serverEntities.Find(entityID);
			IUGCBrowserEntity iugcbrowserEntity;
			if (baseNetworkable != null && baseNetworkable.TryGetComponent<IUGCBrowserEntity>(out iugcbrowserEntity))
			{
				iugcbrowserEntity.ClearContent();
				arg.ReplyWith(string.Format("Cleared content on {0}/{1}", baseNetworkable.ShortPrefabName, entityID));
				return;
			}
			arg.ReplyWith(string.Format("Could not find UGC entity with id {0}", entityID));
		}

		// Token: 0x06004158 RID: 16728 RVA: 0x00182B10 File Offset: 0x00180D10
		[ServerVar]
		public static void clearugcentitiesinrange(ConsoleSystem.Arg arg)
		{
			Vector3 vector = arg.GetVector3(0, default(Vector3));
			float @float = arg.GetFloat(1, 0f);
			int num = 0;
			foreach (global::BaseNetworkable baseNetworkable in global::BaseNetworkable.serverEntities)
			{
				IUGCBrowserEntity iugcbrowserEntity;
				if (baseNetworkable.TryGetComponent<IUGCBrowserEntity>(out iugcbrowserEntity) && Vector3.Distance(baseNetworkable.transform.position, vector) <= @float)
				{
					iugcbrowserEntity.ClearContent();
					num++;
				}
			}
			arg.ReplyWith(string.Format("Cleared {0} UGC entities within {1}m of {2}", num, @float, vector));
		}

		// Token: 0x06004159 RID: 16729 RVA: 0x00182BC8 File Offset: 0x00180DC8
		[ServerVar]
		public static void getugcinfo(ConsoleSystem.Arg arg)
		{
			NetworkableId entityID = arg.GetEntityID(0, default(NetworkableId));
			global::BaseNetworkable baseNetworkable = global::BaseNetworkable.serverEntities.Find(entityID);
			IUGCBrowserEntity iugcbrowserEntity;
			if (baseNetworkable != null && baseNetworkable.TryGetComponent<IUGCBrowserEntity>(out iugcbrowserEntity))
			{
				Admin.ServerUGCInfo serverUGCInfo = new Admin.ServerUGCInfo(iugcbrowserEntity);
				arg.ReplyWith(JsonConvert.SerializeObject(serverUGCInfo));
				return;
			}
			arg.ReplyWith(string.Format("Invalid entity id: {0}", entityID));
		}

		// Token: 0x0600415A RID: 16730 RVA: 0x00182C38 File Offset: 0x00180E38
		[ServerVar(Help = "Returns all entities that the provided player is authed to (TC's, locks, etc), supports --json")]
		public static void authcount(ConsoleSystem.Arg arg)
		{
			global::BasePlayer playerOrSleeper = arg.GetPlayerOrSleeper(0);
			if (playerOrSleeper == null)
			{
				arg.ReplyWith("Please provide a valid player, unable to find '" + arg.GetString(0, "") + "'");
				return;
			}
			string text = arg.GetString(1, "");
			if (text == "--json")
			{
				text = string.Empty;
			}
			List<Admin.EntityAssociation> list = Pool.GetList<Admin.EntityAssociation>();
			Admin.FindEntityAssociationsForPlayer(playerOrSleeper, false, true, text, list);
			TextTable textTable = new TextTable();
			textTable.AddColumns(new string[] { "Prefab name", "Position", "ID", "Type" });
			foreach (Admin.EntityAssociation entityAssociation in list)
			{
				textTable.AddRow(new string[]
				{
					entityAssociation.TargetEntity.ShortPrefabName,
					entityAssociation.TargetEntity.transform.position.ToString(),
					entityAssociation.TargetEntity.net.ID.ToString(),
					entityAssociation.AssociationType.ToString()
				});
			}
			Pool.FreeList<Admin.EntityAssociation>(ref list);
			if (arg.HasArg("--json"))
			{
				arg.ReplyWith(textTable.ToJson());
				return;
			}
			StringBuilder stringBuilder = new StringBuilder();
			stringBuilder.AppendLine("Found entities " + playerOrSleeper.displayName + " is authed to");
			stringBuilder.AppendLine(textTable.ToString());
			arg.ReplyWith(stringBuilder.ToString());
		}

		// Token: 0x0600415B RID: 16731 RVA: 0x00182DEC File Offset: 0x00180FEC
		[ServerVar(Help = "Returns all entities that the provided player has placed, supports --json")]
		public static void entcount(ConsoleSystem.Arg arg)
		{
			global::BasePlayer playerOrSleeper = arg.GetPlayerOrSleeper(0);
			if (playerOrSleeper == null)
			{
				arg.ReplyWith("Please provide a valid player, unable to find '" + arg.GetString(0, "") + "'");
				return;
			}
			string text = arg.GetString(1, "");
			if (text == "--json")
			{
				text = string.Empty;
			}
			List<Admin.EntityAssociation> list = Pool.GetList<Admin.EntityAssociation>();
			Admin.FindEntityAssociationsForPlayer(playerOrSleeper, true, false, text, list);
			TextTable textTable = new TextTable();
			textTable.AddColumns(new string[] { "Prefab name", "Position", "ID" });
			foreach (Admin.EntityAssociation entityAssociation in list)
			{
				textTable.AddRow(new string[]
				{
					entityAssociation.TargetEntity.ShortPrefabName,
					entityAssociation.TargetEntity.transform.position.ToString(),
					entityAssociation.TargetEntity.net.ID.ToString()
				});
			}
			Pool.FreeList<Admin.EntityAssociation>(ref list);
			if (arg.HasArg("--json"))
			{
				arg.ReplyWith(textTable.ToJson());
				return;
			}
			StringBuilder stringBuilder = new StringBuilder();
			stringBuilder.AppendLine("Found entities associated with " + playerOrSleeper.displayName);
			stringBuilder.AppendLine(textTable.ToString());
			arg.ReplyWith(stringBuilder.ToString());
		}

		// Token: 0x0600415C RID: 16732 RVA: 0x00182F7C File Offset: 0x0018117C
		private static void FindEntityAssociationsForPlayer(global::BasePlayer ply, bool useOwnerId, bool useAuth, string filter, List<Admin.EntityAssociation> results)
		{
			results.Clear();
			foreach (global::BaseNetworkable baseNetworkable in global::BaseNetworkable.serverEntities)
			{
				Admin.EntityAssociationType entityAssociationType = Admin.EntityAssociationType.Owner;
				global::BaseEntity baseEntity;
				if ((baseEntity = baseNetworkable as global::BaseEntity) != null)
				{
					bool flag = false;
					if (useOwnerId && baseEntity.OwnerID == ply.userID)
					{
						flag = true;
					}
					if (useAuth && !flag)
					{
						BuildingPrivlidge buildingPrivlidge;
						if (!flag && (buildingPrivlidge = baseEntity as BuildingPrivlidge) != null && buildingPrivlidge.IsAuthed(ply.userID))
						{
							flag = true;
						}
						global::KeyLock keyLock;
						global::CodeLock codeLock;
						if (!flag && (keyLock = baseEntity as global::KeyLock) != null && keyLock.HasLockPermission(ply))
						{
							flag = true;
						}
						else if ((codeLock = baseEntity as global::CodeLock) != null)
						{
							if (codeLock.whitelistPlayers.Contains(ply.userID))
							{
								flag = true;
							}
							else if (codeLock.guestPlayers.Contains(ply.userID))
							{
								flag = true;
								entityAssociationType = Admin.EntityAssociationType.LockGuest;
							}
						}
						global::ModularCar modularCar;
						if (!flag && (modularCar = baseEntity as global::ModularCar) != null && modularCar.IsLockable && modularCar.CarLock.HasLockPermission(ply))
						{
							flag = true;
						}
						if (flag && entityAssociationType == Admin.EntityAssociationType.Owner)
						{
							entityAssociationType = Admin.EntityAssociationType.Auth;
						}
					}
					if (flag && !string.IsNullOrEmpty(filter) && !baseNetworkable.ShortPrefabName.Contains(filter, CompareOptions.IgnoreCase))
					{
						flag = false;
					}
					if (flag)
					{
						results.Add(new Admin.EntityAssociation
						{
							TargetEntity = baseEntity,
							AssociationType = entityAssociationType
						});
					}
				}
			}
		}

		// Token: 0x04003ABD RID: 15037
		[ReplicatedVar(Help = "Controls whether the in-game admin UI is displayed to admins")]
		public static bool allowAdminUI = true;

		// Token: 0x02000F4C RID: 3916
		private enum ChangeGradeMode
		{
			// Token: 0x04004FAA RID: 20394
			Upgrade,
			// Token: 0x04004FAB RID: 20395
			Downgrade
		}

		// Token: 0x02000F4D RID: 3917
		[Preserve]
		public struct PlayerInfo
		{
			// Token: 0x04004FAC RID: 20396
			public string SteamID;

			// Token: 0x04004FAD RID: 20397
			public string OwnerSteamID;

			// Token: 0x04004FAE RID: 20398
			public string DisplayName;

			// Token: 0x04004FAF RID: 20399
			public int Ping;

			// Token: 0x04004FB0 RID: 20400
			public string Address;

			// Token: 0x04004FB1 RID: 20401
			public int ConnectedSeconds;

			// Token: 0x04004FB2 RID: 20402
			public float VoiationLevel;

			// Token: 0x04004FB3 RID: 20403
			public float CurrentLevel;

			// Token: 0x04004FB4 RID: 20404
			public float UnspentXp;

			// Token: 0x04004FB5 RID: 20405
			public float Health;
		}

		// Token: 0x02000F4E RID: 3918
		[Preserve]
		public struct ServerInfoOutput
		{
			// Token: 0x04004FB6 RID: 20406
			public string Hostname;

			// Token: 0x04004FB7 RID: 20407
			public int MaxPlayers;

			// Token: 0x04004FB8 RID: 20408
			public int Players;

			// Token: 0x04004FB9 RID: 20409
			public int Queued;

			// Token: 0x04004FBA RID: 20410
			public int Joining;

			// Token: 0x04004FBB RID: 20411
			public int EntityCount;

			// Token: 0x04004FBC RID: 20412
			public string GameTime;

			// Token: 0x04004FBD RID: 20413
			public int Uptime;

			// Token: 0x04004FBE RID: 20414
			public string Map;

			// Token: 0x04004FBF RID: 20415
			public float Framerate;

			// Token: 0x04004FC0 RID: 20416
			public int Memory;

			// Token: 0x04004FC1 RID: 20417
			public int MemoryUsageSystem;

			// Token: 0x04004FC2 RID: 20418
			public int Collections;

			// Token: 0x04004FC3 RID: 20419
			public int NetworkIn;

			// Token: 0x04004FC4 RID: 20420
			public int NetworkOut;

			// Token: 0x04004FC5 RID: 20421
			public bool Restarting;

			// Token: 0x04004FC6 RID: 20422
			public string SaveCreatedTime;

			// Token: 0x04004FC7 RID: 20423
			public int Version;

			// Token: 0x04004FC8 RID: 20424
			public string Protocol;
		}

		// Token: 0x02000F4F RID: 3919
		[Preserve]
		public struct ServerConvarInfo
		{
			// Token: 0x04004FC9 RID: 20425
			public string FullName;

			// Token: 0x04004FCA RID: 20426
			public string Value;

			// Token: 0x04004FCB RID: 20427
			public string Help;
		}

		// Token: 0x02000F50 RID: 3920
		[Preserve]
		public struct ServerUGCInfo
		{
			// Token: 0x0600546B RID: 21611 RVA: 0x001B56E8 File Offset: 0x001B38E8
			public ServerUGCInfo(IUGCBrowserEntity fromEntity)
			{
				this.entityId = fromEntity.UgcEntity.net.ID;
				this.crcs = fromEntity.GetContentCRCs;
				this.contentType = fromEntity.ContentType;
				this.entityPrefabID = fromEntity.UgcEntity.prefabID;
				this.shortPrefabName = fromEntity.UgcEntity.ShortPrefabName;
				this.playerIds = fromEntity.EditingHistory.ToArray();
			}

			// Token: 0x04004FCC RID: 20428
			public NetworkableId entityId;

			// Token: 0x04004FCD RID: 20429
			public uint[] crcs;

			// Token: 0x04004FCE RID: 20430
			public UGCType contentType;

			// Token: 0x04004FCF RID: 20431
			public uint entityPrefabID;

			// Token: 0x04004FD0 RID: 20432
			public string shortPrefabName;

			// Token: 0x04004FD1 RID: 20433
			public ulong[] playerIds;
		}

		// Token: 0x02000F51 RID: 3921
		private struct EntityAssociation
		{
			// Token: 0x04004FD2 RID: 20434
			public global::BaseEntity TargetEntity;

			// Token: 0x04004FD3 RID: 20435
			public Admin.EntityAssociationType AssociationType;
		}

		// Token: 0x02000F52 RID: 3922
		private enum EntityAssociationType
		{
			// Token: 0x04004FD5 RID: 20437
			Owner,
			// Token: 0x04004FD6 RID: 20438
			Auth,
			// Token: 0x04004FD7 RID: 20439
			LockGuest
		}
	}
}
