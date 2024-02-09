using System.Collections.Generic;

namespace AVR.SDK.Modding
{
    public class SDKModManager
    {
        public static List<SDKMod> Mods { get; private set; } = new();
        
        public static SDKMod GetMod(string id)
        {
            foreach (var mod in Mods)
                if (mod.Id == id)
                    return mod;
            return null;
        }
        
        public static SDKMod[] GetMods() => Mods.ToArray();
        public static void AddMod(SDKMod mod) => Mods.Add(mod);
        public static void RemoveMod(SDKMod mod) => Mods.Remove(mod);
    }
}
