using McWeaponsPlus.Configuration;
using McWeaponsPlus.Template;
using p3rpc.classconstructor.Interfaces;
using p3rpc.commonmodutils;
using Reloaded.Hooks.ReloadedII.Interfaces;
using Reloaded.Memory;
using Reloaded.Memory.Sigscan.Definitions;
using Reloaded.Memory.SigScan.ReloadedII.Interfaces;
using Reloaded.Mod.Interfaces;
using SharedScans.Interfaces;
using System.Diagnostics;


namespace McWeaponsPlus
{
    /// <summary>
    /// Your mod logic goes here.
    /// </summary>
    public class Mod : ModBase // <= Do not Remove.
    {
        /// <summary>
        /// Provides access to the mod loader API.
        /// </summary>
        private readonly IModLoader _modLoader;

        /// <summary>
        /// Provides access to the Reloaded.Hooks API.
        /// </summary>
        /// <remarks>This is null if you remove dependency on Reloaded.SharedLib.Hooks in your mod.</remarks>
        private readonly IReloadedHooks? _hooks;

        /// <summary>
        /// Provides access to the Reloaded logger.
        /// </summary>
        private readonly ILogger _logger;

        /// <summary>
        /// Entry point into the mod, instance that created this class.
        /// </summary>
        private readonly IMod _owner;

        /// <summary>
        /// Provides access to this mod's configuration.
        /// </summary>
        private Config _configuration;

        /// <summary>
        /// The configuration of the currently executing mod.
        /// </summary>
        private readonly IModConfig _modConfig;

        private WeaponContext _context;
        private ModuleRuntime<WeaponContext> _modRuntime;

        public Mod(ModContext context)
        {
            _modLoader = context.ModLoader;
            _hooks = context.Hooks;
            _logger = context.Logger;
            _owner = context.Owner;
            _configuration = context.Configuration;
            _modConfig = context.ModConfig;

            var process = Process.GetCurrentProcess();
            if (process.MainModule == null) throw new Exception($"[{_modConfig.ModName}] Could not get main module (this should never happen)");
            var baseAddress = process.MainModule.BaseAddress;
            var scannerFactory = GetDependency<IScannerFactory>("Scanner Factory");
            var startupScanner = GetDependency<IStartupScanner>("Reloaded Startup Scanner");
            var sharedScans = GetDependency<ISharedScans>("Shared Scans");
            if (_hooks == null) throw new Exception($"[{_modConfig.ModName}] Could not get controller for Reloaded hooks");
            Utils utils = new(startupScanner, _logger, _hooks, baseAddress, "McWeaponsPlus", System.Drawing.Color.Thistle, LogLevel.Information);
            var memory = new Memory();
            var classMethods = GetDependency<IClassMethods>("Class Constructor (Class Methods)");
            var objectMethods = GetDependency<IObjectMethods>("Class Constructor (Object Methods)");

            _context = new(baseAddress, _configuration, _logger, startupScanner, _hooks, _modLoader.GetDirectoryForModId(_modConfig.ModId), utils, memory, sharedScans, classMethods, objectMethods);
            _modRuntime = new(_context);

            _modRuntime.AddModule<McWeaponsPlus>();

            // For more information about this template, please see
            // https://reloaded-project.github.io/Reloaded-II/ModTemplate/

            // If you want to implement e.g. unload support in your mod,
            // and some other neat features, override the methods in ModBase.

            // TODO: Implement some mod logic
        }

        private IControllerType GetDependency<IControllerType>(string modName) where IControllerType : class
        {
            var controller = _modLoader.GetController<IControllerType>();
            if (controller == null || !controller.TryGetTarget(out var target))
                throw new Exception($"[{_modConfig.ModName}] Could not get controller for \"{modName}\". This depedency is likely missing.");
            return target;

        }

        #region Standard Overrides
        public override void ConfigurationUpdated(Config configuration)
        {
            // Apply settings from configuration.
            // ... your code here.
            _configuration = configuration;
            _logger.WriteLine($"[{_modConfig.ModId}] Config Updated: Applying");
        }
        #endregion

        #region For Exports, Serialization etc.
#pragma warning disable CS8618 // Non-nullable field must contain a non-null value when exiting constructor. Consider declaring as nullable.
        public Mod() { }
#pragma warning restore CS8618
        #endregion
    }
}