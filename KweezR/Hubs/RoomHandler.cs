using System.Collections.Concurrent;

namespace KweezR.Hubs
{
    public static class RoomHandler
    {
        public static ConcurrentDictionary<Guid, List<string>> RoomCapacities = new ConcurrentDictionary<Guid, List<string>>();
    }
}
