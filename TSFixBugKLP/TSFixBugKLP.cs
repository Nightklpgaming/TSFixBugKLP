using Mysqlx.Crud;
using Terraria;
using TerrariaApi.Server;
using TShockAPI.Hooks;
using TShockAPI;
using Terraria.Localization;
using Terraria.GameContent.Tile_Entities;
using Terraria.DataStructures;

namespace TSFixBugKLP
{
    [ApiVersion(2, 1)]
    public class TSFixBugKLP : TerrariaPlugin
    {
        #region =[ Plugin Info ]=
        public override string Author => "NightKLP";

        public override string Description => "fix a certain tshock simple bug";

        public override string Name => "TSFixBugKLP";

        public override Version Version => new System.Version(1, 0);

        #endregion

        public static Config Config = Config.Read(); //CONFIG

        public TSFixBugKLP(Main game) : base(game)
        {
            //amogus
        }

        #region [ Initialize ]
        public override void Initialize()
        {
            ServerApi.Hooks.NetGetData.Register(this, OnGetData);

            GeneralHooks.ReloadEvent += OnReload;
        }
        #endregion

        #region [ Dispose ]
        protected override void Dispose(bool disposing)
        {
            if (disposing)
            {
                ServerApi.Hooks.NetGetData.Deregister(this, OnGetData);

                GeneralHooks.ReloadEvent -= OnReload;
            }
            base.Dispose(disposing);
        }
        #endregion

        #region [ Reload ]

        private void OnReload(ReloadEventArgs args)
        {
            #region code
            Config = Config.Read();
            args.Player.SendInfoMessage("TSFixBugKLP Config reloaded!");
            #endregion
        }

        #endregion

        private async void OnGetData(GetDataEventArgs args)
        {
            if (args.Handled)
                return;

            #region { Place Item Frame }
            if (args.MsgID == PacketTypes.PlaceItemFrame && (bool)Config.Main.Enable_ItemFrameFix)
            {
                TSPlayer player = TShock.Players[args.Msg.whoAmI];
                using (var stream = new MemoryStream(args.Msg.readBuffer, args.Index, args.Length))
                {
                    using (var reader = new BinaryReader(stream))
                    {
                        short X = reader.ReadInt16();
                        short Y = reader.ReadInt16();
                        short ItemID = reader.ReadInt16();
                        byte Prefix = reader.ReadByte();
                        short Stack = reader.ReadInt16();

                        if (!player.HasBuildPermission(X, Y))
                        {
                            int num = Item.NewItem(null, (X * 16) + 8, (Y * 16) + 8, player.TPlayer.width, player.TPlayer.height, ItemID, Stack, noBroadcast: true, Prefix, noGrabDelay: true);
                            Main.item[num].playerIndexTheItemIsReservedFor = player.Index;
                            NetMessage.SendData((int)PacketTypes.ItemDrop, player.Index, -1, NetworkText.Empty, num, 1f);
                            NetMessage.SendData((int)PacketTypes.ItemOwner, player.Index, -1, NetworkText.Empty, num);
                        }
                    }
                }
                return;
            }
            #endregion

            #region { Weapons Rack Try Placing }
            if (args.MsgID == PacketTypes.WeaponsRackTryPlacing && (bool)Config.Main.Enable_WeaponRackFix)
            {
                TSPlayer player = TShock.Players[args.Msg.whoAmI];
                using (var stream = new MemoryStream(args.Msg.readBuffer, args.Index, args.Length))
                {
                    using (var reader = new BinaryReader(stream))
                    {
                        short X = reader.ReadInt16();
                        short Y = reader.ReadInt16();
                        short ItemID = reader.ReadInt16();
                        byte Prefix = reader.ReadByte();
                        short Stack = reader.ReadInt16();
                        TEWeaponsRack WeaponRack = (TEWeaponsRack)TileEntity.ByID[TEWeaponsRack.Find(X, Y)];

                        if (!player.HasBuildPermission(X, Y))
                        {
                            int num = Item.NewItem(null, (X * 16) + 8, (Y * 16) + 8, player.TPlayer.width, player.TPlayer.height, ItemID, Stack, noBroadcast: true, Prefix, noGrabDelay: true);
                            Main.item[num].playerIndexTheItemIsReservedFor = player.Index;
                            NetMessage.SendData((int)PacketTypes.ItemDrop, player.Index, -1, NetworkText.Empty, num, 1f);
                            NetMessage.SendData((int)PacketTypes.ItemOwner, player.Index, -1, NetworkText.Empty, num);

                            NetMessage.SendData((int)PacketTypes.UpdateTileEntity, -1, -1, NetworkText.Empty, WeaponRack.ID, 0, 1);
                            args.Handled = true;
                        }
                    }
                }
                return;
            }
            #endregion

            #region { Player Spawn }
            if (args.MsgID == PacketTypes.PlayerSpawn && (bool)Config.Main.Enable_FixCPlayerG)
            {
                using (var stream = new MemoryStream(args.Msg.readBuffer, args.Index, args.Length))
                {
                    using (var reader = new BinaryReader(stream))
                    {
                        byte playerid = reader.ReadByte();
                        short spawnX = reader.ReadInt16();
                        short spawnY = reader.ReadInt16();
                        int respawnTimer = reader.ReadInt32();
                        short numberOfDeathsPVE = reader.ReadInt16();
                        short numberOfDeathsPVP = reader.ReadInt16();
                        PlayerSpawnContext context = (PlayerSpawnContext)reader.ReadByte();

                        if (context == PlayerSpawnContext.ReviveFromDeath)
                        {
                            await fixp();
                            async Task fixp()
                            {
                                await Task.Delay(800);
                                NetMessage.SendData((int)PacketTypes.PlayerSpawn, -1, -1, NetworkText.Empty, playerid);
                            }
                        }

                    }
                }
                return;
            }
            #endregion
        }
    }
}
