using Andon.Enums;
using Andon.Models;
using Microsoft.EntityFrameworkCore;

namespace Andon.Helpers
{
    public static class AlarmHelper
    {
        /// <summary>
        /// 统一报警判断逻辑（全系统唯一入口）
        /// </summary>
        public static bool IsAlarm(EquipmentStatus runStatus, ErrorTypes errorTypes)
        {
            return runStatus is EquipmentStatus.故障 or EquipmentStatus.报警中
                || errorTypes != ErrorTypes.正常;
        }



        /// <summary>
        /// 统一获取报警描述
        /// </summary>
        public static string GetAlarmDesc(EquipmentStatus runStatus, ErrorTypes errorTypes)
        {
            if (errorTypes == ErrorTypes.大杂输送螺旋过载) return "大杂输送螺旋过载";
            if (errorTypes == ErrorTypes.上层筛网堵料) return "上层筛网堵料";
            if (errorTypes == ErrorTypes.风量失压) return "风量失压";
            if (errorTypes == ErrorTypes.胶辊磨损严重) return "胶辊磨损严重";
            if (errorTypes == ErrorTypes.胶辊高温) return "胶辊高温";
            if (errorTypes == ErrorTypes.筛孔堵塞) return "筛孔堵塞";
            if (errorTypes == ErrorTypes.碾白室过载) return "碾白室过载";
            if (errorTypes == ErrorTypes.碎米率飙升) return "碎米率飙升";
            if (errorTypes == ErrorTypes.电机过载) return "电机过载";
            if (errorTypes == ErrorTypes.压缩空气低压) return "压缩空气低压";
            if (errorTypes == ErrorTypes.筛网破损) return "筛网破损";
            if (errorTypes == ErrorTypes.动平衡失效) return "动平衡失效";
            if (errorTypes == ErrorTypes.封口温度异常) return "封口温度异常";
            if (errorTypes == ErrorTypes.皮带跑偏撕裂) return "皮带跑偏撕裂";
            if (errorTypes == ErrorTypes.成品重量异常) return "成品重量异常";
            if (errorTypes == ErrorTypes.卡袋停机) return "卡袋停机";

            if (runStatus == EquipmentStatus.故障) return "设备故障停机";
            return "设备异常";
        }

        /// <summary>
        /// 统一报警恢复（自动 + 手动 共用这一个方法）
        /// </summary>
        public static async Task AutoRecoverAlarm(AppDbContext context, int equipmentId)
        {
            var alarms = await context.WarningAlarms
                .Where(a => a.EquipmentId == equipmentId && a.EndTime == null)
                .ToListAsync();

            var eq = await context.BizEquipments
                .FindAsync(equipmentId);
            eq.Status = EquipmentStatus.正常;

            if (!alarms.Any()) return;

            DateTime now = DateTime.Now;


            foreach (var alarm in alarms)
            {
                alarm.EndTime = now;

                alarm.Status = 2;

                if (alarm.StartTime.HasValue && alarm.EndTime.HasValue)
                {
                    alarm.DurationMin = (int)(alarm.EndTime.Value - alarm.StartTime.Value).TotalMinutes;
                }
            }

            await context.SaveChangesAsync();
        }
    }
}