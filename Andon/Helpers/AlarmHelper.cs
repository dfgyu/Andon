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
            if (errorTypes == ErrorTypes.大杂输送螺旋过载) return "电流 > 额定 115% 持续 3s 报警；>130% 立即停机";
            if (errorTypes == ErrorTypes.上层筛网堵料) return "振动幅值↓30%\r\n电流↑10%\r\n视觉：堆积高度 > 2/3 且覆盖面积 > 80% 持续 5s\r\n";
            if (errorTypes == ErrorTypes.风量失压) return "风机电流↓20%\r\n振动频率偏离 ±10%\r\n视觉：无明显分层且重质物料 < 5%\r\n";
            if (errorTypes == ErrorTypes.胶辊磨损严重) return "电流波动 > 15%\r\n视觉：胶辊直径 < 标准值 85%\r\n";
            if (errorTypes == ErrorTypes.胶辊高温) return "表面温度 > 70℃预警，>85℃立即停机；\r\n轴承温度 > 65℃预警\r\n";
            if (errorTypes == ErrorTypes.筛孔堵塞) return "振动幅值↓40%\r\n电流↑15%\r\n视觉：局部堆积面积>30%持续8s\r\n";
            if (errorTypes == ErrorTypes.碾白室过载) return "电流 > 额定 120% 持续 2s 报警；>140% 立即停机";
            if (errorTypes == ErrorTypes.碎米率飙升) return "视觉：碎米率 > 15%\r\n电流波动 > 20%\r\n碾白室温度 > 60℃\r\n";
            if (errorTypes == ErrorTypes.电机过载) return "电流>额定115%持续3s报警；>130%立即停机\r\n轴承温度 > 70℃预警\r\n";
            if (errorTypes == ErrorTypes.压缩空气低压) return "气阀电流↓30%\r\n视觉：剔除率↓10%且喷吹准确率<85%\r\n";
            if (errorTypes == ErrorTypes.筛网破损) return "视觉：破洞面积 > 5mm²\r\n振动出现异常频率\r\n成品大颗粒占比 > 10%\r\n";
            if (errorTypes == ErrorTypes.动平衡失效) return "振动幅值 > 标准值 2 倍";
            if (errorTypes == ErrorTypes.封口温度异常) return "温度 <120℃或> 180℃报警\r\n电流波动 > 20% 预警\r\n";
            if (errorTypes == ErrorTypes.皮带跑偏撕裂) return "视觉：偏移 > 5cm 报警\r\n出现 250Hz 撕裂特征峰\r\n电流↓20% 立即停机\r\n";
            if (errorTypes == ErrorTypes.成品重量异常) return "视觉：包装袋高度超出 ±5% + 下料流量异常";
            if (errorTypes == ErrorTypes.卡袋停机) return "视觉：包装袋卡在封口位置 + 输送带电流↑30% + 封口机无动作电流持续 2";

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