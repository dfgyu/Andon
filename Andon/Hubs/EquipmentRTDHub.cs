using Microsoft.AspNetCore.SignalR;



namespace Andon.Hubs
{
    public class EquipmentRTDHub : Hub
    {

        public async Task JoinEquipmentRTDGroup(string equipmentId)
        {
            await Groups.AddToGroupAsync(Context.ConnectionId, equipmentId);
        }
    }
}
