﻿using System;
using Terraria;
using System.Threading;
using System.Reflection;
using TerrariaApi.Server;
using System.Threading.Tasks;

namespace AntiWOF
{
	[ApiVersion(1, 26)]
	public class Plugin : TerrariaPlugin
	{
		public override Version Version
		{
			get { return Assembly.GetExecutingAssembly().GetName().Version; }
		}

		public override string Name
		{
			get { return "AntiWOF-2"; }
		}

		public override string Author
		{
			get { return "Simon311"; }
		}

		public override string Description
		{
			get { return "Removes WOF boxes."; }
		}

		public Plugin(Main game)
			: base(game)
		{
			Order = 0;
		}

		public const int DefaultOrder = 1;

		public override void Initialize()
		{
			ServerApi.Hooks.NpcLootDrop.Register(this, LootHook, DefaultOrder);
		}

		private void LootHook(NpcLootDropEventArgs e)
		{
			if (e == null || e.ItemId != 367) return;
			Task.Factory.StartNew(() => UndoBox((int)e.Position.X, (int)e.Position.Y, e.Width, e.Height)).LogExceptions();
		}

		private void UndoBox(int X, int Y, int W, int H)
		{
			Thread.Sleep(5000); // Give the player 5 seconds to pick up the loot.
			/* The code below is partially a copy-paste of Terraria code */
			int num22 = (X + (W / 2)) / 16;
			int num23 = (Y + (H / 2)) / 16;
			int num24 = W / 32 + 1;
			for (int k = num22 - num24; k <= num22 + num24; k++)
			{
				for (int l = num23 - num24; l <= num23 + num24; l++)
				{
					var tile = Main.tile[k, l];
					if ((k == num22 - num24 || k == num22 + num24 || l == num23 - num24 || l == num23 + num24) && tile.active() && tile.type == (WorldGen.crimson ? 347 : 140))
					{
						tile.type = 0;
						tile.active(false);
					}

					try
					{
						NetMessage.SendTileSquare(-1, k, l, 1);
					}
					catch { }
				}
			}

		}

		protected override void Dispose(bool disposing)
		{
			if (disposing)
				ServerApi.Hooks.NpcLootDrop.Deregister(this, LootHook);

			base.Dispose(disposing);
		}
	}
}
