using PlanningPoker.Data;
using PlanningPoker.Hubs;

namespace PlanningPoker.Services;

public interface IPlanningPokerService
{
    void JoinRoom(string groupName, string userName, bool isSpectator, string connectionId);
    IEnumerable<string> GetRooms();
    Dictionary<string, double> GetCardValues(string groupName);
    void SetCardValue(string groupName, string userName, double cardValue);
    void ClearCardValues(string groupName);
    List<PlanningUserPage> GetUsersFromRoom(string groupName);
    void RemoveUser(string connectionId);
    void LeaveRoom(string groupName, string userName);
    string GetRoomByConnectionId(string contextConnectionId);
    void RemoveRoom(string groupName);
    string GetUserConnectionIdByNameAndGroup(string groupName, string userName);
}

public class PlanningPokerService : IPlanningPokerService
{
    private Dictionary<string, List<PlanningUserHub>> Rooms { get; } = new();

    public void JoinRoom(string groupName, string userName, bool isSpectator, string connectionId)
    {
        Rooms.TryAdd(groupName, new List<PlanningUserHub>());
        if (!Rooms[groupName].Any(pu => pu.Name.Equals(userName)))
        {
            Rooms[groupName].Add(new PlanningUserHub
            {
                Name = userName,
                ConnectionId = connectionId,
                IsSpectator = isSpectator
            });
        }
    }

    public void ClearCardValues(string groupName)
    {
        Rooms[groupName].ForEach(u => u.CardValue = 0);
    }
    
    public IEnumerable<string> GetRooms()
    {
        return Rooms.Select(r => r.Key).ToList();
    }

    public Dictionary<string, double> GetCardValues(string groupName)
    {
        return new Dictionary<string, double>(
            Rooms[groupName]
                .Select(x => new KeyValuePair<string, double>(x.Name, x.CardValue))
                .ToList()
        );
    }

    public List<PlanningUserPage> GetUsersFromRoom(string groupName)
    {
        return !Rooms.ContainsKey(groupName)
            ? new List<PlanningUserPage>()
            : Rooms[groupName].Select(x => new PlanningUserPage
            {
                Name = x.Name,
                IsSpectator = x.IsSpectator
            }).ToList();
    }
    
    public string GetRoomByConnectionId(string contextConnectionId)
    {
        foreach (var (roomKey, usersList) in Rooms)
        {
            if (usersList.Any(r => r.ConnectionId == contextConnectionId))
                return roomKey;
        }
        return string.Empty;
    }
    
    public string GetUserConnectionIdByNameAndGroup(string groupName, string userName)
    {
        var userConnectionId = string.Empty;
        if (Rooms.ContainsKey(groupName))
        {
            var user = Rooms[groupName].SingleOrDefault(r => r.Name.Equals(userName));
            userConnectionId = user?.ConnectionId ?? string.Empty;
        }
        return userConnectionId;
    }
    
    public void SetCardValue(string groupName, string userName, double cardValue)
    {
        Rooms[groupName].FirstOrDefault(pu => pu.Name.Equals(userName))!.CardValue = cardValue;
    }

    public void LeaveRoom(string groupName, string userName)
    {
        if (!Rooms.ContainsKey(groupName))
            return;
        
        Rooms[groupName] = Rooms[groupName].Where(pu => pu.ConnectionId != userName).ToList();
    }
    
    public void RemoveUser(string connectionId)
    {
        var groupKey = GetRoomByConnectionId(connectionId);
        if (string.IsNullOrEmpty(groupKey))
            return;
        
        Rooms[groupKey] = Rooms[groupKey].Where(pu => pu.ConnectionId != connectionId).ToList();
    }

    public void RemoveRoom(string groupName)
    {
        Rooms.Remove(groupName);
    }
}
