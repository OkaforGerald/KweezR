using System.Collections.Concurrent;

namespace KweezR.Hubs
{
    public static class RoomHandler
    {
        public static ConcurrentDictionary<string, List<string>> RoomCapacities = new ConcurrentDictionary<string, List<string>>();
    }
}
