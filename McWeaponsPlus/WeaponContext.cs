using McWeaponsPlus.Configuration;
using p3rpc.classconstructor.Interfaces;
using p3rpc.commonmodutils;
using Reloaded.Hooks.Definitions;
using Reloaded.Memory;
using Reloaded.Memory.SigScan.ReloadedII.Interfaces;
using Reloaded.Mod.Interfaces;
using SharedScans.Interfaces;

namespace McWeaponsPlus
{
    public class WeaponContext : UnrealContext
    {
        public new Config _config { get; set; }

        public WeaponContext(long baseAddress, IConfigurable config, ILogger logger, IStartupScanner startupScanner, IReloadedHooks hooks, string modLocation,
            Utils utils, Memory memory, ISharedScans sharedScans, IClassMethods classMethods, IObjectMethods objectMethods)
            : base(baseAddress, config, logger, startupScanner, hooks, modLocation, utils, memory, sharedScans, classMethods, objectMethods)
        {
            _config = (Config)config;
        }

        public override void OnConfigUpdated(IConfigurable newConfig) => _config = (Config)newConfig;
    }
}
