using System.Collections.Concurrent;

namespace KweezR.Hubs
{
    public static class RoomHandler
    {
        public static ConcurrentDictionary<Guid, LinkedList<string>> RoomCapacities = new ConcurrentDictionary<Guid, LinkedList<string>>();
    }
}
