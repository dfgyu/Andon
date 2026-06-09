using Andon.Dtos;
using Andon.Hubs;
using Andon.Models;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.SignalR;
using Microsoft.EntityFrameworkCore;

namespace Andon.Controllers
{
    [ApiController]
    [Route("api/equipmentRTD")]
    public class EquipmentRTDController : ControllerBase
    {
            private readonly AppDbContext _context;
            private readonly IHubContext<EquipmentRTDHub> _hubContext;
    
            public EquipmentRTDController(AppDbContext context, IHubContext<EquipmentRTDHub> hubContext)
            {
                _context = context;
                _hubContext = hubContext;
            }

        [HttpGet("latest/{equipmentId}")]
        public async Task<IActionResult> GetLatestByEquipmentId(int equipmentId)
        {

            var data = await _context.BizEquipmentRTDs
                .Where(r => r.EquipmentId == equipmentId)
                .OrderByDescending(r => r.CreateAt)
                .FirstOrDefaultAsync();


            return Ok(data);
        }


        [HttpGet("latest/all")]
        public async Task<IActionResult> GetAllLatest()
        {
            var list = await _context.BizEquipmentRTDs
                .GroupBy(r => r.EquipmentId)
                .Select(g => g.OrderByDescending(r => r.CreateAt).First())
                .ToListAsync();

            return Ok(list);
        }

        /// <summary>
        /// 获取设备历史数据
        /// </summary>
        [HttpGet("history/{equipmentId}")]
        public async Task<IActionResult> GetHistory(int equipmentId, int page = 1, int limit = 100)
        {
            var query = _context.BizEquipmentRTDs
                .Where(r => r.EquipmentId == equipmentId)
                .OrderByDescending(r => r.CreateAt);

            var total = await query.CountAsync();
            var items = await query
                .Skip((page - 1) * limit)
                .Take(limit)
                .ToListAsync();

            return Ok(new { total, items });
        }
    }
 }

