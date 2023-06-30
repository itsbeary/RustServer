using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Runtime.CompilerServices;
using System.Text;
using Facepunch;
using Newtonsoft.Json;
using Steamworks;
using UnityEngine;

namespace ConVar
{
	// Token: 0x02000ACF RID: 2767
	[ConsoleSystem.Factory("inventory")]
	public class Inventory : ConsoleSystem
	{
		// Token: 0x06004266 RID: 16998 RVA: 0x00188664 File Offset: 0x00186864
		[ServerUserVar]
		public static void lighttoggle(ConsoleSystem.Arg arg)
		{
			BasePlayer basePlayer = arg.Player();
			if (!basePlayer)
			{
				return;
			}
			if (basePlayer.IsDead())
			{
				return;
			}
			if (basePlayer.IsSleeping())
			{
				return;
			}
			if (basePlayer.InGesture)
			{
				return;
			}
			basePlayer.LightToggle(true);
		}

		// Token: 0x06004267 RID: 16999 RVA: 0x001886A4 File Offset: 0x001868A4
		[ServerUserVar]
		public static void endloot(ConsoleSystem.Arg arg)
		{
			BasePlayer basePlayer = arg.Player();
			if (!basePlayer)
			{
				return;
			}
			if (basePlayer.IsDead())
			{
				return;
			}
			if (basePlayer.IsSleeping())
			{
				return;
			}
			basePlayer.inventory.loot.Clear();
		}

		// Token: 0x06004268 RID: 17000 RVA: 0x001886E4 File Offset: 0x001868E4
		[ServerVar]
		public static void give(ConsoleSystem.Arg arg)
		{
			BasePlayer basePlayer = arg.Player();
			if (!basePlayer)
			{
				return;
			}
			Item item = ItemManager.CreateByPartialName(arg.GetString(0, ""), 1, arg.GetULong(3, 0UL));
			if (item == null)
			{
				arg.ReplyWith("Invalid Item!");
				return;
			}
			int @int = arg.GetInt(1, 1);
			item.amount = @int;
			float @float = arg.GetFloat(2, 1f);
			item.conditionNormalized = @float;
			item.OnVirginSpawn();
			if (!basePlayer.inventory.GiveItem(item, null, false))
			{
				item.Remove(0f);
				arg.ReplyWith("Couldn't give item (inventory full?)");
				return;
			}
			basePlayer.Command("note.inv", new object[]
			{
				item.info.itemid,
				@int
			});
			Debug.Log(string.Concat(new object[]
			{
				"giving ",
				basePlayer.displayName,
				" ",
				@int,
				" x ",
				item.info.displayName.english
			}));
			if (basePlayer.IsDeveloper)
			{
				basePlayer.ChatMessage(string.Concat(new object[]
				{
					"you silently gave yourself ",
					@int,
					" x ",
					item.info.displayName.english
				}));
				return;
			}
			Chat.Broadcast(string.Concat(new object[]
			{
				basePlayer.displayName,
				" gave themselves ",
				@int,
				" x ",
				item.info.displayName.english
			}), "SERVER", "#eee", 0UL);
		}

		// Token: 0x06004269 RID: 17001 RVA: 0x00188890 File Offset: 0x00186A90
		[ServerVar]
		public static void resetbp(ConsoleSystem.Arg arg)
		{
			BasePlayer basePlayer = arg.GetPlayer(0);
			if (basePlayer == null)
			{
				if (arg.HasArgs(1))
				{
					arg.ReplyWith("Can't find player");
					return;
				}
				basePlayer = arg.Player();
			}
			basePlayer.blueprints.Reset();
		}

		// Token: 0x0600426A RID: 17002 RVA: 0x001888D8 File Offset: 0x00186AD8
		[ServerVar]
		public static void unlockall(ConsoleSystem.Arg arg)
		{
			BasePlayer basePlayer = arg.GetPlayer(0);
			if (basePlayer == null)
			{
				if (arg.HasArgs(1))
				{
					arg.ReplyWith("Can't find player");
					return;
				}
				basePlayer = arg.Player();
			}
			basePlayer.blueprints.UnlockAll();
		}

		// Token: 0x0600426B RID: 17003 RVA: 0x00188920 File Offset: 0x00186B20
		[ServerVar]
		public static void giveall(ConsoleSystem.Arg arg)
		{
			Item item = null;
			string text = "SERVER";
			if (arg.Player() != null)
			{
				text = arg.Player().displayName;
			}
			foreach (BasePlayer basePlayer in BasePlayer.activePlayerList)
			{
				item = ItemManager.CreateByPartialName(arg.GetString(0, ""), 1, 0UL);
				if (item == null)
				{
					arg.ReplyWith("Invalid Item!");
					return;
				}
				int @int = arg.GetInt(1, 1);
				item.amount = @int;
				item.OnVirginSpawn();
				if (!basePlayer.inventory.GiveItem(item, null, false))
				{
					item.Remove(0f);
					arg.ReplyWith("Couldn't give item (inventory full?)");
				}
				else
				{
					basePlayer.Command("note.inv", new object[]
					{
						item.info.itemid,
						@int
					});
					Debug.Log(string.Concat(new object[]
					{
						" [ServerVar] giving ",
						basePlayer.displayName,
						" ",
						item.amount,
						" x ",
						item.info.displayName.english
					}));
				}
			}
			if (item != null)
			{
				Chat.Broadcast(string.Concat(new object[]
				{
					text,
					" gave everyone ",
					item.amount,
					" x ",
					item.info.displayName.english
				}), "SERVER", "#eee", 0UL);
			}
		}

		// Token: 0x0600426C RID: 17004 RVA: 0x00188AE0 File Offset: 0x00186CE0
		[ServerVar]
		public static void giveto(ConsoleSystem.Arg arg)
		{
			string text = "SERVER";
			if (arg.Player() != null)
			{
				text = arg.Player().displayName;
			}
			BasePlayer basePlayer = BasePlayer.Find(arg.GetString(0, ""));
			if (basePlayer == null)
			{
				arg.ReplyWith("Couldn't find player!");
				return;
			}
			Item item = ItemManager.CreateByPartialName(arg.GetString(1, ""), 1, arg.GetULong(3, 0UL));
			if (item == null)
			{
				arg.ReplyWith("Invalid Item!");
				return;
			}
			int @int = arg.GetInt(2, 1);
			item.amount = @int;
			item.OnVirginSpawn();
			if (!basePlayer.inventory.GiveItem(item, null, false))
			{
				item.Remove(0f);
				arg.ReplyWith("Couldn't give item (inventory full?)");
				return;
			}
			basePlayer.Command("note.inv", new object[]
			{
				item.info.itemid,
				@int
			});
			Debug.Log(string.Concat(new object[]
			{
				" [ServerVar] giving ",
				basePlayer.displayName,
				" ",
				@int,
				" x ",
				item.info.displayName.english
			}));
			Chat.Broadcast(string.Concat(new object[]
			{
				text,
				" gave ",
				basePlayer.displayName,
				" ",
				@int,
				" x ",
				item.info.displayName.english
			}), "SERVER", "#eee", 0UL);
		}

		// Token: 0x0600426D RID: 17005 RVA: 0x00188C74 File Offset: 0x00186E74
		[ServerVar]
		public static void giveid(ConsoleSystem.Arg arg)
		{
			BasePlayer basePlayer = arg.Player();
			if (!basePlayer)
			{
				return;
			}
			Item item = ItemManager.CreateByItemID(arg.GetInt(0, 0), 1, 0UL);
			if (item == null)
			{
				arg.ReplyWith("Invalid Item!");
				return;
			}
			int @int = arg.GetInt(1, 1);
			item.amount = @int;
			item.OnVirginSpawn();
			if (!basePlayer.inventory.GiveItem(item, null, false))
			{
				item.Remove(0f);
				arg.ReplyWith("Couldn't give item (inventory full?)");
				return;
			}
			basePlayer.Command("note.inv", new object[]
			{
				item.info.itemid,
				@int
			});
			Debug.Log(string.Concat(new object[]
			{
				" [ServerVar] giving ",
				basePlayer.displayName,
				" ",
				@int,
				" x ",
				item.info.displayName.english
			}));
			if (basePlayer.IsDeveloper)
			{
				basePlayer.ChatMessage(string.Concat(new object[]
				{
					"you silently gave yourself ",
					@int,
					" x ",
					item.info.displayName.english
				}));
				return;
			}
			Chat.Broadcast(string.Concat(new object[]
			{
				basePlayer.displayName,
				" gave themselves ",
				@int,
				" x ",
				item.info.displayName.english
			}), "SERVER", "#eee", 0UL);
		}

		// Token: 0x0600426E RID: 17006 RVA: 0x00188E00 File Offset: 0x00187000
		[ServerVar]
		public static void givearm(ConsoleSystem.Arg arg)
		{
			BasePlayer basePlayer = arg.Player();
			if (!basePlayer)
			{
				return;
			}
			Item item = ItemManager.CreateByItemID(arg.GetInt(0, 0), 1, 0UL);
			if (item == null)
			{
				arg.ReplyWith("Invalid Item!");
				return;
			}
			int @int = arg.GetInt(1, 1);
			item.amount = @int;
			item.OnVirginSpawn();
			if (!basePlayer.inventory.GiveItem(item, basePlayer.inventory.containerBelt, false))
			{
				item.Remove(0f);
				arg.ReplyWith("Couldn't give item (inventory full?)");
				return;
			}
			basePlayer.Command("note.inv", new object[]
			{
				item.info.itemid,
				@int
			});
			Debug.Log(string.Concat(new object[]
			{
				" [ServerVar] giving ",
				basePlayer.displayName,
				" ",
				item.amount,
				" x ",
				item.info.displayName.english
			}));
			if (basePlayer.IsDeveloper)
			{
				basePlayer.ChatMessage(string.Concat(new object[]
				{
					"you silently gave yourself ",
					item.amount,
					" x ",
					item.info.displayName.english
				}));
				return;
			}
			Chat.Broadcast(string.Concat(new object[]
			{
				basePlayer.displayName,
				" gave themselves ",
				item.amount,
				" x ",
				item.info.displayName.english
			}), "SERVER", "#eee", 0UL);
		}

		// Token: 0x0600426F RID: 17007 RVA: 0x00188FA4 File Offset: 0x001871A4
		[ServerVar(Help = "Copies the players inventory to the player in front of them")]
		public static void copyTo(ConsoleSystem.Arg arg)
		{
			BasePlayer basePlayer = arg.Player();
			if (!basePlayer.IsAdmin && !basePlayer.IsDeveloper && !Server.cinematic)
			{
				return;
			}
			if (basePlayer == null)
			{
				return;
			}
			BasePlayer basePlayer2 = null;
			if (arg.HasArgs(1) && arg.GetString(0, "").ToLower() != "true")
			{
				basePlayer2 = arg.GetPlayer(0);
				if (basePlayer2 == null)
				{
					uint @uint = arg.GetUInt(0, 0U);
					basePlayer2 = BasePlayer.FindByID((ulong)@uint);
					if (basePlayer2 == null)
					{
						basePlayer2 = BasePlayer.FindBot((ulong)@uint);
					}
				}
			}
			else
			{
				basePlayer2 = RelationshipManager.GetLookingAtPlayer(basePlayer);
			}
			if (basePlayer2 == null)
			{
				return;
			}
			basePlayer2.inventory.containerBelt.Clear();
			basePlayer2.inventory.containerWear.Clear();
			int num = 0;
			foreach (Item item in basePlayer.inventory.containerBelt.itemList)
			{
				basePlayer2.inventory.containerBelt.AddItem(item.info, item.amount, item.skin, ItemContainer.LimitStack.Existing);
				if (item.contents != null)
				{
					Item item2 = basePlayer2.inventory.containerBelt.itemList[num];
					foreach (Item item3 in item.contents.itemList)
					{
						item2.contents.AddItem(item3.info, item3.amount, item3.skin, ItemContainer.LimitStack.Existing);
					}
				}
				num++;
			}
			foreach (Item item4 in basePlayer.inventory.containerWear.itemList)
			{
				basePlayer2.inventory.containerWear.AddItem(item4.info, item4.amount, item4.skin, ItemContainer.LimitStack.Existing);
			}
			if (basePlayer.IsDeveloper)
			{
				basePlayer.ChatMessage("you silently copied items to " + basePlayer2.displayName);
				return;
			}
			Chat.Broadcast(basePlayer.displayName + " copied their inventory to " + basePlayer2.displayName, "SERVER", "#eee", 0UL);
		}

		// Token: 0x06004270 RID: 17008 RVA: 0x00189220 File Offset: 0x00187420
		[ServerVar(Help = "Deploys a loadout to players in a radius eg. inventory.deployLoadoutInRange testloadout 30")]
		public static void deployLoadoutInRange(ConsoleSystem.Arg arg)
		{
			BasePlayer basePlayer = arg.Player();
			if (!basePlayer.IsAdmin && !basePlayer.IsDeveloper && !Server.cinematic)
			{
				return;
			}
			if (basePlayer == null)
			{
				return;
			}
			string @string = arg.GetString(0, "");
			Inventory.SavedLoadout savedLoadout;
			if (!Inventory.LoadLoadout(@string, out savedLoadout))
			{
				arg.ReplyWith("Can't find loadout: " + @string);
				return;
			}
			float @float = arg.GetFloat(1, 0f);
			List<BasePlayer> list = Pool.GetList<BasePlayer>();
			Vis.Entities<BasePlayer>(basePlayer.transform.position, @float, list, 131072, QueryTriggerInteraction.Collide);
			int num = 0;
			foreach (BasePlayer basePlayer2 in list)
			{
				if (!(basePlayer2 == basePlayer) && !basePlayer2.isClient)
				{
					savedLoadout.LoadItemsOnTo(basePlayer2);
					num++;
				}
			}
			arg.ReplyWith(string.Format("Applied loadout {0} to {1} players", @string, num));
			Pool.FreeList<BasePlayer>(ref list);
		}

		// Token: 0x06004271 RID: 17009 RVA: 0x0018932C File Offset: 0x0018752C
		[ServerVar(Help = "Deploys the given loadout to a target player. eg. inventory.deployLoadout testloadout jim")]
		public static void deployLoadout(ConsoleSystem.Arg arg)
		{
			BasePlayer basePlayer = arg.Player();
			if (basePlayer == null)
			{
				return;
			}
			if (!basePlayer.IsAdmin && !basePlayer.IsDeveloper && !Server.cinematic)
			{
				return;
			}
			string @string = arg.GetString(0, "");
			BasePlayer basePlayer2 = (string.IsNullOrEmpty(arg.GetString(1, "")) ? null : arg.GetPlayerOrSleeperOrBot(1));
			if (basePlayer2 == null)
			{
				basePlayer2 = basePlayer;
			}
			if (basePlayer2 == null)
			{
				arg.ReplyWith("Could not find player " + arg.GetString(1, "") + " and no local player available");
				return;
			}
			Inventory.SavedLoadout savedLoadout;
			if (Inventory.LoadLoadout(@string, out savedLoadout))
			{
				savedLoadout.LoadItemsOnTo(basePlayer2);
				arg.ReplyWith("Deployed loadout " + @string + " to " + basePlayer2.displayName);
				return;
			}
			arg.ReplyWith("Could not find loadout " + @string);
		}

		// Token: 0x06004272 RID: 17010 RVA: 0x00189404 File Offset: 0x00187604
		[ServerVar(Help = "Clears the inventory of a target player. eg. inventory.clearInventory jim")]
		public static void clearInventory(ConsoleSystem.Arg arg)
		{
			BasePlayer basePlayer = arg.Player();
			if (basePlayer == null)
			{
				return;
			}
			if (!basePlayer.IsAdmin && !basePlayer.IsDeveloper && !Server.cinematic)
			{
				return;
			}
			BasePlayer basePlayer2 = (string.IsNullOrEmpty(arg.GetString(1, "")) ? null : arg.GetPlayerOrSleeperOrBot(1));
			if (basePlayer2 == null)
			{
				basePlayer2 = basePlayer;
			}
			if (basePlayer2 == null)
			{
				arg.ReplyWith("Could not find player " + arg.GetString(1, "") + " and no local player available");
				return;
			}
			basePlayer2.inventory.containerBelt.Clear();
			basePlayer2.inventory.containerWear.Clear();
			basePlayer2.inventory.containerMain.Clear();
		}

		// Token: 0x06004273 RID: 17011 RVA: 0x001894BE File Offset: 0x001876BE
		private static string GetLoadoutPath(string loadoutName)
		{
			return Server.GetServerFolder("loadouts") + "/" + loadoutName + ".ldt";
		}

		// Token: 0x06004274 RID: 17012 RVA: 0x001894DC File Offset: 0x001876DC
		[ServerVar(Help = "Saves the current equipped loadout of the calling player. eg. inventory.saveLoadout loaduoutname")]
		public static void saveloadout(ConsoleSystem.Arg arg)
		{
			BasePlayer basePlayer = arg.Player();
			if (basePlayer == null)
			{
				return;
			}
			if (!basePlayer.IsAdmin && !basePlayer.IsDeveloper && !Server.cinematic)
			{
				return;
			}
			string @string = arg.GetString(0, "");
			string text = JsonConvert.SerializeObject(new Inventory.SavedLoadout(basePlayer), Formatting.Indented);
			string loadoutPath = Inventory.GetLoadoutPath(@string);
			File.WriteAllText(loadoutPath, text);
			arg.ReplyWith("Saved loadout to " + loadoutPath);
		}

		// Token: 0x06004275 RID: 17013 RVA: 0x0018954C File Offset: 0x0018774C
		public static bool LoadLoadout(string name, out Inventory.SavedLoadout so)
		{
			PlayerInventoryProperties inventoryConfig = PlayerInventoryProperties.GetInventoryConfig(name);
			if (inventoryConfig != null)
			{
				Debug.Log("Found builtin config!");
				so = new Inventory.SavedLoadout(inventoryConfig);
				return true;
			}
			so = new Inventory.SavedLoadout();
			string loadoutPath = Inventory.GetLoadoutPath(name);
			if (!File.Exists(loadoutPath))
			{
				return false;
			}
			so = JsonConvert.DeserializeObject<Inventory.SavedLoadout>(File.ReadAllText(loadoutPath));
			return so != null;
		}

		// Token: 0x06004276 RID: 17014 RVA: 0x001895AC File Offset: 0x001877AC
		[ServerVar(Help = "Prints all saved inventory loadouts")]
		public static void listloadouts(ConsoleSystem.Arg arg)
		{
			BasePlayer basePlayer = arg.Player();
			if (basePlayer == null)
			{
				return;
			}
			if (!basePlayer.IsAdmin && !basePlayer.IsDeveloper && !Server.cinematic)
			{
				return;
			}
			string serverFolder = Server.GetServerFolder("loadouts");
			StringBuilder stringBuilder = new StringBuilder();
			foreach (string text in Directory.EnumerateFiles(serverFolder))
			{
				stringBuilder.AppendLine(text);
			}
			arg.ReplyWith(stringBuilder.ToString());
		}

		// Token: 0x06004277 RID: 17015 RVA: 0x00189640 File Offset: 0x00187840
		[ClientVar]
		[ServerVar]
		public static void defs(ConsoleSystem.Arg arg)
		{
			if (Steamworks.SteamInventory.Definitions == null)
			{
				arg.ReplyWith("no definitions");
				return;
			}
			if (Steamworks.SteamInventory.Definitions.Length == 0)
			{
				arg.ReplyWith("0 definitions");
				return;
			}
			string[] array = Steamworks.SteamInventory.Definitions.Select((InventoryDef x) => x.Name).ToArray<string>();
			arg.ReplyWith(array);
		}

		// Token: 0x06004278 RID: 17016 RVA: 0x001896AA File Offset: 0x001878AA
		[ClientVar]
		[ServerVar]
		public static void reloaddefs(ConsoleSystem.Arg arg)
		{
			Steamworks.SteamInventory.LoadItemDefinitions();
		}

		// Token: 0x06004279 RID: 17017 RVA: 0x001896B4 File Offset: 0x001878B4
		[ServerVar]
		public static void equipslottarget(ConsoleSystem.Arg arg)
		{
			BasePlayer basePlayer = arg.Player();
			if (!basePlayer.IsAdmin && !basePlayer.IsDeveloper && !Server.cinematic)
			{
				return;
			}
			if (basePlayer == null)
			{
				return;
			}
			BasePlayer lookingAtPlayer = RelationshipManager.GetLookingAtPlayer(basePlayer);
			if (lookingAtPlayer == null)
			{
				return;
			}
			int @int = arg.GetInt(0, 0);
			Inventory.EquipItemInSlot(lookingAtPlayer, @int);
			arg.ReplyWith(string.Format("Equipped slot {0} on player {1}", @int, lookingAtPlayer.displayName));
		}

		// Token: 0x0600427A RID: 17018 RVA: 0x00189728 File Offset: 0x00187928
		[ServerVar]
		public static void equipslot(ConsoleSystem.Arg arg)
		{
			BasePlayer basePlayer = arg.Player();
			if (!basePlayer.IsAdmin && !basePlayer.IsDeveloper && !Server.cinematic)
			{
				return;
			}
			if (basePlayer == null)
			{
				return;
			}
			BasePlayer basePlayer2 = null;
			if (arg.HasArgs(2))
			{
				basePlayer2 = arg.GetPlayer(1);
				if (basePlayer2 == null)
				{
					uint @uint = arg.GetUInt(1, 0U);
					basePlayer2 = BasePlayer.FindByID((ulong)@uint);
					if (basePlayer2 == null)
					{
						basePlayer2 = BasePlayer.FindBot((ulong)@uint);
					}
				}
			}
			if (basePlayer2 == null)
			{
				return;
			}
			int @int = arg.GetInt(0, 0);
			Inventory.EquipItemInSlot(basePlayer2, @int);
			Debug.Log(string.Format("Equipped slot {0} on player {1}", @int, basePlayer2.displayName));
		}

		// Token: 0x0600427B RID: 17019 RVA: 0x001897D4 File Offset: 0x001879D4
		private static void EquipItemInSlot(BasePlayer player, int slot)
		{
			ItemId itemId = default(ItemId);
			for (int i = 0; i < player.inventory.containerBelt.itemList.Count; i++)
			{
				if (player.inventory.containerBelt.itemList[i] != null && i == slot)
				{
					itemId = player.inventory.containerBelt.itemList[i].uid;
					break;
				}
			}
			player.UpdateActiveItem(itemId);
		}

		// Token: 0x0600427C RID: 17020 RVA: 0x0018984C File Offset: 0x00187A4C
		private static int GetSlotIndex(BasePlayer player)
		{
			if (player.GetActiveItem() == null)
			{
				return -1;
			}
			ItemId uid = player.GetActiveItem().uid;
			for (int i = 0; i < player.inventory.containerBelt.itemList.Count; i++)
			{
				if (player.inventory.containerBelt.itemList[i] != null && player.inventory.containerBelt.itemList[i].uid == uid)
				{
					return i;
				}
			}
			return -1;
		}

		// Token: 0x0600427D RID: 17021 RVA: 0x001898D0 File Offset: 0x00187AD0
		[ServerVar]
		public static void giveBp(ConsoleSystem.Arg arg)
		{
			BasePlayer basePlayer = arg.Player();
			if (!basePlayer)
			{
				return;
			}
			ItemDefinition itemDefinition = ItemManager.FindDefinitionByPartialName(arg.GetString(0, ""), 1, 0UL);
			if (itemDefinition == null)
			{
				arg.ReplyWith("Could not find item: " + arg.GetString(0, ""));
				return;
			}
			if (itemDefinition.Blueprint == null)
			{
				arg.ReplyWith(itemDefinition.shortname + " has no blueprint!");
				return;
			}
			Item item = ItemManager.Create(ItemManager.blueprintBaseDef, 1, 0UL);
			item.blueprintTarget = itemDefinition.itemid;
			item.OnVirginSpawn();
			if (!basePlayer.inventory.GiveItem(item, null, false))
			{
				item.Remove(0f);
				arg.ReplyWith("Couldn't give item (inventory full?)");
				return;
			}
			basePlayer.Command("note.inv", new object[]
			{
				item.info.itemid,
				1
			});
			Debug.Log(string.Concat(new string[]
			{
				"giving ",
				basePlayer.displayName,
				" 1 x ",
				item.blueprintTargetDef.shortname,
				" blueprint"
			}));
			if (basePlayer.IsDeveloper)
			{
				basePlayer.ChatMessage("you silently gave yourself 1 x " + item.blueprintTargetDef.shortname + " blueprint");
				return;
			}
			Chat.Broadcast(basePlayer.displayName + " gave themselves 1 x " + item.blueprintTargetDef.shortname + " blueprint", "SERVER", "#eee", 0UL);
		}

		// Token: 0x04003BEB RID: 15339
		private const string LoadoutDirectory = "loadouts";

		// Token: 0x04003BEC RID: 15340
		[ServerVar(Help = "Disables all attire limitations, so NPC clothing and invalid overlaps can be equipped")]
		public static bool disableAttireLimitations;

		// Token: 0x02000F75 RID: 3957
		public class SavedLoadout
		{
			// Token: 0x060054D6 RID: 21718 RVA: 0x00008777 File Offset: 0x00006977
			public SavedLoadout()
			{
			}

			// Token: 0x060054D7 RID: 21719 RVA: 0x001B6140 File Offset: 0x001B4340
			public SavedLoadout(BasePlayer player)
			{
				this.belt = Inventory.SavedLoadout.SaveItems(player.inventory.containerBelt);
				this.wear = Inventory.SavedLoadout.SaveItems(player.inventory.containerWear);
				this.main = Inventory.SavedLoadout.SaveItems(player.inventory.containerMain);
				this.heldItemIndex = Inventory.GetSlotIndex(player);
			}

			// Token: 0x060054D8 RID: 21720 RVA: 0x001B61A4 File Offset: 0x001B43A4
			public SavedLoadout(PlayerInventoryProperties properties)
			{
				this.belt = Inventory.SavedLoadout.SaveItems(properties.belt);
				this.wear = Inventory.SavedLoadout.SaveItems(properties.wear);
				this.main = Inventory.SavedLoadout.SaveItems(properties.main);
				this.heldItemIndex = 0;
			}

			// Token: 0x060054D9 RID: 21721 RVA: 0x001B61F4 File Offset: 0x001B43F4
			private static Inventory.SavedLoadout.SavedItem[] SaveItems(ItemContainer itemContainer)
			{
				List<Inventory.SavedLoadout.SavedItem> list = new List<Inventory.SavedLoadout.SavedItem>();
				for (int i = 0; i < itemContainer.capacity; i++)
				{
					Item slot = itemContainer.GetSlot(i);
					if (slot != null)
					{
						Inventory.SavedLoadout.SavedItem savedItem = new Inventory.SavedLoadout.SavedItem
						{
							id = slot.info.itemid,
							amount = slot.amount,
							skin = slot.skin,
							blueprintTarget = slot.blueprintTarget
						};
						if (slot.contents != null && slot.contents.itemList != null)
						{
							List<int> list2 = new List<int>();
							foreach (Item item in slot.contents.itemList)
							{
								list2.Add(item.info.itemid);
							}
							savedItem.containedItems = list2.ToArray();
						}
						list.Add(savedItem);
					}
				}
				return list.ToArray();
			}

			// Token: 0x060054DA RID: 21722 RVA: 0x001B6300 File Offset: 0x001B4500
			private static Inventory.SavedLoadout.SavedItem[] SaveItems(List<PlayerInventoryProperties.ItemAmountSkinned> items)
			{
				List<Inventory.SavedLoadout.SavedItem> list = new List<Inventory.SavedLoadout.SavedItem>();
				foreach (PlayerInventoryProperties.ItemAmountSkinned itemAmountSkinned in items)
				{
					Inventory.SavedLoadout.SavedItem savedItem = new Inventory.SavedLoadout.SavedItem
					{
						id = itemAmountSkinned.itemid,
						amount = (int)itemAmountSkinned.amount,
						skin = itemAmountSkinned.skinOverride
					};
					if (itemAmountSkinned.blueprint)
					{
						savedItem.blueprintTarget = savedItem.id;
						savedItem.id = ItemManager.blueprintBaseDef.itemid;
					}
					list.Add(savedItem);
				}
				return list.ToArray();
			}

			// Token: 0x060054DB RID: 21723 RVA: 0x001B63B4 File Offset: 0x001B45B4
			public void LoadItemsOnTo(BasePlayer player)
			{
				Inventory.SavedLoadout.<>c__DisplayClass10_0 CS$<>8__locals1;
				CS$<>8__locals1.player = player;
				CS$<>8__locals1.<>4__this = this;
				CS$<>8__locals1.player.inventory.containerMain.Clear();
				CS$<>8__locals1.player.inventory.containerBelt.Clear();
				CS$<>8__locals1.player.inventory.containerWear.Clear();
				ItemManager.DoRemoves();
				this.<LoadItemsOnTo>g__LoadItems|10_0(this.belt, CS$<>8__locals1.player.inventory.containerBelt, ref CS$<>8__locals1);
				this.<LoadItemsOnTo>g__LoadItems|10_0(this.wear, CS$<>8__locals1.player.inventory.containerWear, ref CS$<>8__locals1);
				this.<LoadItemsOnTo>g__LoadItems|10_0(this.main, CS$<>8__locals1.player.inventory.containerMain, ref CS$<>8__locals1);
				Inventory.EquipItemInSlot(CS$<>8__locals1.player, this.heldItemIndex);
				CS$<>8__locals1.player.inventory.SendSnapshot();
			}

			// Token: 0x060054DC RID: 21724 RVA: 0x001B6490 File Offset: 0x001B4690
			private Item LoadItem(Inventory.SavedLoadout.SavedItem item)
			{
				Item item2 = ItemManager.CreateByItemID(item.id, item.amount, item.skin);
				if (item.blueprintTarget != 0)
				{
					item2.blueprintTarget = item.blueprintTarget;
				}
				if (item.containedItems != null && item.containedItems.Length != 0)
				{
					foreach (int num in item.containedItems)
					{
						item2.contents.AddItem(ItemManager.FindItemDefinition(num), 1, 0UL, ItemContainer.LimitStack.Existing);
					}
				}
				return item2;
			}

			// Token: 0x060054DD RID: 21725 RVA: 0x001B650C File Offset: 0x001B470C
			[CompilerGenerated]
			private void <LoadItemsOnTo>g__LoadItems|10_0(Inventory.SavedLoadout.SavedItem[] items, ItemContainer container, ref Inventory.SavedLoadout.<>c__DisplayClass10_0 A_3)
			{
				foreach (Inventory.SavedLoadout.SavedItem savedItem in items)
				{
					A_3.player.inventory.GiveItem(this.LoadItem(savedItem), container, false);
				}
			}

			// Token: 0x04005027 RID: 20519
			public Inventory.SavedLoadout.SavedItem[] belt;

			// Token: 0x04005028 RID: 20520
			public Inventory.SavedLoadout.SavedItem[] wear;

			// Token: 0x04005029 RID: 20521
			public Inventory.SavedLoadout.SavedItem[] main;

			// Token: 0x0400502A RID: 20522
			public int heldItemIndex;

			// Token: 0x02000FED RID: 4077
			public struct SavedItem
			{
				// Token: 0x040051BB RID: 20923
				public int id;

				// Token: 0x040051BC RID: 20924
				public int amount;

				// Token: 0x040051BD RID: 20925
				public ulong skin;

				// Token: 0x040051BE RID: 20926
				public int[] containedItems;

				// Token: 0x040051BF RID: 20927
				public int blueprintTarget;
			}
		}
	}
}
