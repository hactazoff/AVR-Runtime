using Cysharp.Threading.Tasks;

namespace AVR.Worlds
{
    public class WorldManager : Base.Manager<World>
    {
        public delegate void OnWorldUpdateEvent(World world);
        public static event OnWorldUpdateEvent OnWorldUpdate;
        public static void OnUpdate(World world) => OnWorldUpdate?.Invoke(world);

        public delegate void OnWorldRemoveEvent(World world);
        public static event OnWorldRemoveEvent OnWorldRemove;
        public static void OnRemove(World world) => OnWorldRemove?.Invoke(world);

        public delegate void OnWorldAddEvent(World world);
        public static event OnWorldAddEvent OnWorldAdd;
        public static void OnAdd(World world) => OnWorldAdd?.Invoke(world);

        public static World GetWorld(string search)
        {
            var wp = WorldPatern.Parser(search);
            if (wp == null)
                return null;
            foreach (var world in Cache)
                if (world.id == wp.id && world.server == wp.server)
                    return world;
            return null;
        }

        public static void SetWorld(World world)
        {
            if (GetWorld(world.id) != null)
            {
                Cache.Add(world);
                OnUpdate(world);
            }
            else
            {
                Cache.Add(world);
                OnAdd(world);
            }
        }

        public static void RemoveWorld(World world)
        {
            Cache.Remove(world);
            OnRemove(world);
        }

        public static async UniTask<World> GetOrFetchWorld(string search)
        {
            var world = GetWorld(search);
            if (world != null)
                return world;
            var wp = WorldPatern.Parser(search);
            if (wp == null)
                return null;
            world = new World() { id = wp.id, server = wp.server };
            return await world.Fetch();
        }
    }
}