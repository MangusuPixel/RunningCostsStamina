using GenericModConfigMenu;
using StardewModdingAPI;
using StardewModdingAPI.Events;
using StardewValley;

namespace RunningCostsStamina
{
    internal sealed class ModEntry : Mod
    {
        /*********
        ** Public properties
        *********/

        public static ModConfig Config { get; private set; }

        /******
         * Public methods
         ******/

        public override void Entry(IModHelper helper)
        {
            Config = helper.ReadConfig<ModConfig>();

            helper.Events.GameLoop.GameLaunched += OnGameLaunched;
            helper.Events.GameLoop.UpdateTicked += OnUpdateTicked;
        }

        /******
         * Private methods
         ******/

        private void OnGameLaunched(object? sender, GameLaunchedEventArgs e)
        {
            LoadConfigOptions();
        }

        private void OnUpdateTicked(object? sender, UpdateTickedEventArgs e)
        {
            var farmer = Game1.player;

            if (farmer.movementDirections.Count > 0 && farmer.running && !farmer.isRidingHorse() && !farmer.UsingTool &&
                (!Config.OnlyOutdoors || farmer.currentLocation.IsOutdoors))
                Game1.player.Stamina -= Config.DrainMult * Game1.currentGameTime.ElapsedGameTime.Milliseconds / 1000f;
        }

        private void LoadConfigOptions()
        {
            var configMenu = Helper.ModRegistry.GetApi<IGenericModConfigMenuApi>("spacechase0.GenericModConfigMenu");
            if (configMenu is null)
                return;

            configMenu.Register(
                mod: ModManifest,
                reset: () => Config = new ModConfig(),
                save: () => Helper.WriteConfig(Config)
            );

            configMenu.AddNumberOption(
                mod: ModManifest,
                getValue: () => Config.DrainMult,
                setValue: (value) => Config.DrainMult = value,
                name: () => "Multiplier",
                tooltip: () => "Stamina drain multiplier. By default (1), running for 1 second costs 1 stamina. 0 essentially disables the mod."
                );

            configMenu.AddBoolOption(
                mod: ModManifest,
                getValue: () => Config.OnlyOutdoors,
                setValue: (value) => Config.OnlyOutdoors = value,
                name: () => "Only outdoors",
                tooltip: () => "Stamina drain will only happen outdoors and won't drain indoors if true."
            );
        }
    }
}
