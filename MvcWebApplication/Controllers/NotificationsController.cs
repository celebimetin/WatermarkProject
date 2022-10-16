using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.SignalR;
using MvcWebApplication.Hubs;

namespace MvcWebApplication.Controllers
{
    [Route("api/[controller]/[action]")]
    [ApiController]
    public class NotificationsController : ControllerBase
    {
        private readonly IHubContext<NotificationHub> _hubContext;

        public NotificationsController(IHubContext<NotificationHub> hubContext)
        {
            _hubContext = hubContext;
        }

        [HttpGet("connectionId")]
        public IActionResult CompleteWatermarkProcess(string connectionId)
        {
            _hubContext.Clients.Client(connectionId).SendAsync("NotifyCompleteWatermarkProcess");
            return Ok();
        }
    }
}