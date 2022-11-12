using Microsoft.AspNetCore.SignalR;
using Newtonsoft.Json;
using PlanningPoker.Services;

namespace PlanningPoker.Hubs;

public class PlanningHub : Hub
{
    private readonly IPlanningPokerService _planningPokerService;

    public PlanningHub(IPlanningPokerService planningPokerService)
    {
        _planningPokerService = planningPokerService;
    }

    public async Task SendMessage(string user, string message)
    {
        await Clients.All.SendAsync("ReceiveMessage", user, message);
    }

    public async Task JoinRoomAsync(string groupName, string userName, bool isSpectator)
    {
        await Groups.AddToGroupAsync(Context.ConnectionId, groupName);
        _planningPokerService.JoinRoom(groupName, userName, isSpectator, Context.ConnectionId);
        await Clients.All.SendAsync("ReceiveRooms", _planningPokerService.GetRooms().ToList());
        await Clients.Group(groupName).SendAsync("ReceiveUsers", _planningPokerService.GetUsersFromRoom(groupName));
    }

    public async Task LeaveRoomAsync(string groupName, string userName)
    {
        if (string.IsNullOrEmpty(groupName) || string.IsNullOrEmpty(userName))
            return;

        await Groups.RemoveFromGroupAsync(Context.ConnectionId, groupName);
        _planningPokerService.LeaveRoom(groupName, userName);
        await Clients.Group(groupName).SendAsync("ReceiveUsers", _planningPokerService.GetUsersFromRoom(groupName));
    }

    public async Task BroadcastToRoomAsync(string groupName, string message)
    {
        await Clients.Group(groupName).SendAsync("OnMessage", message);
    }

    public IEnumerable<string> GetRooms()
    {
        return _planningPokerService.GetRooms();
    }

    public async Task SetCardValue(string groupName, string userName, double cardValue)
    {
        _planningPokerService.SetCardValue(groupName, userName, cardValue);
        await Clients.Group(groupName).SendAsync("ReceiveUserVote", userName, cardValue);
    }

    public Dictionary<string, double> GetCardValues(string groupName)
    {
        return _planningPokerService.GetCardValues(groupName);
    }

    public async Task ClearCardValues(string groupName)
    {
        _planningPokerService.ClearCardValues(groupName);
        await Clients.Group(groupName).SendAsync("ReceiveUsers", _planningPokerService.GetUsersFromRoom(groupName));
    }

    public override async Task OnDisconnectedAsync(Exception? exception)
    {
        await base.OnDisconnectedAsync(exception);
        var groupName = _planningPokerService.GetRoomByConnectionId(Context.ConnectionId);
        if (string.IsNullOrEmpty(groupName))
            return;
        await Groups.RemoveFromGroupAsync(Context.ConnectionId, groupName);
        _planningPokerService.RemoveUser(Context.ConnectionId);
        await Clients.Group(groupName).SendAsync("ReceiveUsers", _planningPokerService.GetUsersFromRoom(groupName));
    }

    public async Task RevealCards(string groupName)
    {
        await Clients.Group(groupName).SendAsync("RevealCards");
    }

    public async Task StartNewGame(string groupName)
    {
        await Clients.Group(groupName).SendAsync("StartNewGame");
        await ClearCardValues(groupName);
    }

    public async Task CloseRoom(string groupName)
    {
        _planningPokerService.RemoveRoom(groupName);
        await Clients.All.SendAsync("ReceiveRooms", _planningPokerService.GetRooms().ToList());
    }

    public async Task KickUser(string groupName, string userName)
    {
        var userConnectionId = _planningPokerService.GetUserConnectionIdByNameAndGroup(groupName, userName);
        if (string.IsNullOrEmpty(groupName))
            return;
        await Groups.RemoveFromGroupAsync(userConnectionId, groupName);
        _planningPokerService.RemoveUser(userConnectionId);
        await Clients.Group(groupName).SendAsync("ReceiveUsers", _planningPokerService.GetUsersFromRoom(groupName));
    }

}