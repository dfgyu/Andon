using Microsoft.AspNetCore.SignalR;

namespace Andon.Hubs
{
    public class LineMonitorHub : Hub
    {
        // 前端加入对应产线组（按LineId推送）
        public async Task JoinLineGroup(string lineId)
        {
            await Groups.AddToGroupAsync(Context.ConnectionId, lineId);
        }

        // 离开产线组
        public async Task LeaveLineGroup(string lineId)
        {
            await Groups.RemoveFromGroupAsync(Context.ConnectionId, lineId);
        }
    }
}