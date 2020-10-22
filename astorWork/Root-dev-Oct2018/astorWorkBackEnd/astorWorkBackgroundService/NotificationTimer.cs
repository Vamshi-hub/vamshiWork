using astorWorkDAO;
using astorWorkShared.MultiTenancy;
using astorWorkShared.Utilities;
using Microsoft.Extensions.Logging;
using System;

namespace astorWorkBackgroundService
{
    public class NotificationTimer
    {
        private TenantInfo _tenant;
        private DateTimeOffset _targetTime;
        private TimeSpan _interval;
        private Enums.NotificationCode _notificationCode;
        private ILogger _logger;
        private bool _repeating;
        private int _timerID;
        private System.Timers.Timer _timer;

        public NotificationTimer(ILogger logger, TenantInfo tenant, DateTimeOffset targetTime, TimeSpan interval, Enums.NotificationCode notificationCode, int timerID, bool repeating = false)
        {
            _logger = logger;
            _tenant = tenant;
            _targetTime = targetTime;
            _interval = interval;
            _notificationCode = notificationCode;
            _repeating = repeating;
            _timerID = timerID;

            if (_logger != null && _tenant != null && _targetTime != null &&
                _targetTime > DateTimeOffset.Now && _interval > TimeSpan.FromSeconds(1))
            {
                _timer = new System.Timers.Timer
                {
                    AutoReset = false,
                    Interval = (targetTime - DateTimeOffset.Now).TotalMilliseconds,
                };
                _logger.LogInformation($"[{DateTime.UtcNow}] Timer targeted at: {targetTime}, notification code {_notificationCode}");
                _timer.Elapsed += _timer_Elapsed;
            }
            else
            {
                _logger.LogError($"[{DateTime.UtcNow}] Fail to create timer");

            }
        }

        public bool StartTimer()
        {
            bool result = false;
            if (_timer != null)
            {
                try
                {
                    _timer.Start();
                    result = true;
                }
                catch (Exception exc)
                {
                    _logger.LogError(exc.Message);
                }
            }
            return result;
        }
        public bool StopTimer()
        {
            bool result = false;
            if (_timer != null)
            {
                try
                {
                    _timer.Stop();
                    result = true;
                }
                catch (Exception exc)
                {
                    _logger.LogError(exc.Message);
                }
            }
            return result;
        }
        private void _timer_Elapsed(object sender, System.Timers.ElapsedEventArgs e)
        {
            //if (_tenant.RowKey == "localhost")
            //{
            astorWorkDbContext _astorWorkDbContext = new astorWorkDbContext(_tenant);

            //using (var astorWorkDbContext = new astorWorkDbContext(_tenant))
            //{
            /*
            _logger.LogInformation($"Timer triggered at {DateTime.Now} with notification code: {_notificationCode}");
            */
            UpdateNotificationAuditService updateNotificationAuditService = new UpdateNotificationAuditService(_astorWorkDbContext);
            switch (_notificationCode)
            {
                case Enums.NotificationCode.DelayInDelivery:
                    updateNotificationAuditService.UpdateDeliveryNotificationAudit(_astorWorkDbContext, _timerID, Convert.ToInt32(Enums.NotificationCode.DelayInDelivery));
                    break;
                case Enums.NotificationCode.TodayExpectedDelivery:
                    updateNotificationAuditService.UpdateDeliveryNotificationAudit(_astorWorkDbContext, _timerID, Convert.ToInt32(Enums.NotificationCode.TodayExpectedDelivery));
                    //updateTodayExpectedMaterialNotificationAuditService.UpdateTodayExpectedDeliveryNotificationAudit(_astorWorkDbContext, _timerID);
                    break;
                default:
                    break;
            }
            
            if (!_timer.AutoReset && _repeating)
            {
                _timer.Interval = _interval.TotalMilliseconds;
                _timer.AutoReset = true;
                StartTimer();
            }
        }
    }
}
