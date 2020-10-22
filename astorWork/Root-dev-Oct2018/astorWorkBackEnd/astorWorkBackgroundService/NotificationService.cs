using astorWorkBackgroundService.EmailHelper;
using astorWorkDAO;
using astorWorkShared.MultiTenancy;
using astorWorkShared.Services;
using astorWorkShared.Utilities;
using DinkToPdf.Contracts;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Caching.Distributed;
using Microsoft.Extensions.Logging;
using Newtonsoft.Json;
using System;
using System.Collections.Concurrent;
using System.Collections.Generic;
using System.Linq;
using System.Threading;
using System.Threading.Tasks;

namespace astorWorkBackgroundService
{
    public class NotificationService : BackgroundService
    {
        private readonly ILogger<NotificationService> _logger;
        private IDistributedCache _cache;
        static List<TenantInfo> RunningTeneant = new List<TenantInfo>();
        private readonly IAstorWorkEmail _emailService;
        private ConcurrentDictionary<int, NotificationTimer> lstTimers = new ConcurrentDictionary<int, NotificationTimer>();
        private IConverter _converter;

        public NotificationService(ILogger<NotificationService> logger, IDistributedCache cache, IAstorWorkEmail emailService, IConverter converter)
        {
            _logger = logger;
            _cache = cache;
            _emailService = emailService;
            _converter = converter;
        }

        protected override async Task ExecuteAsync(CancellationToken stoppingToken)
        {
            _logger.LogInformation($"[{DateTime.UtcNow}] SchedulerService is starting.");
            stoppingToken.Register(() => _logger.LogDebug($"[{DateTime.UtcNow}] SchedulerService is stopping."));

            var dictTenantTasks = new Dictionary<string, CancellationTokenSource>();
            while (!stoppingToken.IsCancellationRequested)
            {
                //_logger.LogInformation($"Main loop is running");

                var listTenants = await GetTenantInfosAsync();
                if (listTenants != null)
                {
                    foreach (var tenant in listTenants)
                    {
                        lock (dictTenantTasks)
                        {
                            if (!dictTenantTasks.ContainsKey(tenant.ConnectionString))
                            {

                                var tokenSource = new CancellationTokenSource();
                                //dictTenantTasks.Add(tenant.ConnectionString, tokenSource);
                                Task.Factory.StartNew(() => CheckNotificationAudit(tenant, tokenSource.Token), tokenSource.Token);
                                Task.Factory.StartNew(() => CheckSchedulerTimer(tenant, stoppingToken));
                                dictTenantTasks.Add(tenant.ConnectionString, tokenSource);
                            }
                        }
                    }
                }
                await Task.Delay(30000, stoppingToken);
            }
            _logger.LogInformation($"[{DateTime.UtcNow}] SchedulerService is stopping.");
        }

        private async Task CheckNotificationAudit(TenantInfo tenant, CancellationToken stoppingToken)
        {
            //if (tenant.DBName != "tenant1")
            //    return; 
            _logger.LogInformation($"[{DateTime.UtcNow}] task doing background {tenant.DBName} started.");
            while (!stoppingToken.IsCancellationRequested)
            {
                try
                {
                    using (var _astorWorkDbContext = new astorWorkDbContext(tenant))
                    {
                        var lstNotificationAudit = _astorWorkDbContext.NotificationAudit
                            .Include(n => n.NotificationTimer).ThenInclude(p => p.Project)
                            .Include(n => n.NotificationTimer).ThenInclude(s => s.Site)
                            .Include(i => i.UserNotificationAssociation).ThenInclude(una => una.Receipient)
                            .Where(p => p.ProcessedDate == null).ToList();

                        EmailHandler emailHandler = new EmailHandler(_emailService, _astorWorkDbContext, tenant);
                        foreach (var notification in lstNotificationAudit)
                        {
                            /*
                            _logger.LogInformation($" handling notification {notification.ID} for {tenant.ConnectionString}.");
                            */
                            try
                            {
                                switch (notification.Code)
                                {
                                    case (int)Enums.NotificationCode.CreateMRF:
                                        await emailHandler.SendNewMRFEmail(notification);
                                        break;
                                    case (int)Enums.NotificationCode.QCFailed:
                                        await emailHandler.SendQCFailEmail(notification);
                                        break;
                                    case (int)Enums.NotificationCode.QCRectified:
                                        await emailHandler.SendQCFailEmail(notification);
                                        break;
                                    case (int)Enums.NotificationCode.BIMSync:
                                        await emailHandler.SendNewBIMSyncEmail(notification);
                                        break;
                                    case (int)Enums.NotificationCode.CloseMRF:
                                        await emailHandler.SendCloseMRFEmail(notification);
                                        break;
                                    case (int)Enums.NotificationCode.DelayInDelivery:
                                        if (notification.NotificationTimer != null)
                                            await emailHandler.SendDeliveryMaterialsEmail(notification, tenant.DBName, _converter);
                                        else
                                            _logger.LogError($"[{DateTime.UtcNow}] Delayed Delivery email for {tenant.DBName} not processed. Error: notificationtimer column should not null for delayed delivery notification.");
                                        break;
                                    case (int)Enums.NotificationCode.TodayExpectedDelivery:
                                        if (notification.NotificationTimer != null)
                                            await emailHandler.SendDeliveryMaterialsEmail(notification, tenant.DBName, _converter);
                                        else
                                            _logger.LogError($"[{DateTime.UtcNow}] Expected Delivery email for {tenant.DBName} not processed. Error: notificationtimer column should not null for expected delivery notification.");
                                        break;
                                    default:
                                        break;
                                }
                            }
                            catch (Exception ex)
                            {
                                _logger.LogError($"[{DateTime.UtcNow}] email for {tenant.DBName} Exception {ex.Message}.");

                                var emailHealth = _astorWorkDbContext.SystemHealthMaster
                                .Where(sm => sm.Type == 1).FirstOrDefault();
                                if (emailHealth == null)
                                {
                                    _astorWorkDbContext.SystemHealthMaster.Add(
                                        new SystemHealthMaster()
                                        {
                                            Type = 1,
                                            Status = 1,
                                            Reference = notification.ID.ToString(),
                                            Message = ex.Message,
                                            LastUpdated = DateTime.UtcNow
                                        });
                                }
                                else
                                {
                                    emailHealth.Status = 1;
                                    emailHealth.Reference = notification.ID.ToString();
                                    emailHealth.Message = ex.Message;
                                    emailHealth.LastUpdated = DateTime.UtcNow;
                                }
                            }
                            notification.ProcessedDate = DateTime.UtcNow;
                            _astorWorkDbContext.Entry(notification).State = EntityState.Modified;
                        }
                        HealthPole(_astorWorkDbContext, tenant);
                        await _astorWorkDbContext.SaveChangesAsync();

                        await Task.Delay(TimeSpan.FromSeconds(3), stoppingToken);
                    }
                }
                catch (Exception exc)
                {
                    var details = ExceptionUtility.GetExceptionDetails(exc);
                    _logger.LogError($"[{DateTime.UtcNow}] task doing background {tenant.DBName} error");
                    _logger.LogError($"{details}");
                }
            }
            _logger.LogInformation($"[{DateTime.UtcNow}] task doing background {tenant.DBName} cancelled.");
        }

        private async Task CheckSchedulerTimer(TenantInfo tenant, CancellationToken stoppingToken)
        {
            try
            {
                //using (var _astorWorkDbContext = new astorWorkDbContext(tenant))
                //{
                _logger.LogInformation($"[{DateTime.UtcNow}] SchedulerTimer doing background {tenant.DBName} started.");
                while (!stoppingToken.IsCancellationRequested)
                {
                    using (var _astorWorkDbContext = new astorWorkDbContext(tenant))
                    {
                        var timerlist = _astorWorkDbContext.NotificationTimerMaster.Include(p => p.Project).Include(s => s.Site).ToList();
                        foreach (var nt in timerlist)
                        {
                            var TimeZoneOffset = nt.Project == null ? nt.Site.TimeZoneOffset : nt.Project.TimeZoneOffset;
                            var triggerTime = DateTime.UtcNow.AddMinutes(TimeZoneOffset).AddMinutes(nt.TriggerTime).ToUniversalTime();
                            if (lstTimers.ContainsKey(nt.ID) && nt.UpdateRequired)
                            {
                                var timer = lstTimers[nt.ID];

                                lstTimers.Remove(nt.ID, out timer);
                                timer.StopTimer();
                            }
                            if (!lstTimers.ContainsKey(nt.ID))
                            {
                                switch (nt.Code)
                                {
                                    case (int)Enums.NotificationCode.DelayInDelivery:

                                        //var DelayedTimer = new NotificationTimer(_logger, tenant,
                                        //      triggerTime, TimeSpan.FromDays(1),
                                        //        Enums.NotificationCode.DelayInDelivery, nt.ID, true);
                                        //DelayedTimer.StartTimer();
                                        var DelayedTimer = new NotificationTimer(_logger, tenant,
                                   DateTime.Now.AddSeconds(60), TimeSpan.FromMinutes(5),
                                   Enums.NotificationCode.DelayInDelivery, nt.ID, true);
                                        DelayedTimer.StartTimer();
                                        lstTimers.TryAdd(nt.ID, DelayedTimer);
                                        break;
                                    case (int)Enums.NotificationCode.TodayExpectedDelivery:
                                        var expectedTimer = new NotificationTimer(_logger, tenant,
                                          triggerTime, TimeSpan.FromDays(1),
                                            Enums.NotificationCode.TodayExpectedDelivery, nt.ID, true);
                                        expectedTimer.StartTimer();
                                        lstTimers.TryAdd(nt.ID, expectedTimer);
                                        break;
                                    default:
                                        _logger.LogError($"[{DateTime.UtcNow}] invalid notification code");
                                        break;
                                }

                            }
                            nt.UpdateRequired = false;
                            _astorWorkDbContext.NotificationTimerMaster.Update(nt);
                        }

                        if (timerlist.Count > 0)
                            _astorWorkDbContext.SaveChanges();


                        await Task.Delay(TimeSpan.FromMinutes(5), stoppingToken);
                    }
                }
            }
            catch (Exception exc)
            {
                _logger.LogInformation($"[{DateTime.UtcNow}] SchedulerTimer doing background {tenant.DBName} error: {exc.Message}.");
            }
        }



        public void HealthPole(astorWorkDbContext _astorWorkDbContext, TenantInfo tenantInfo)
        {
            var schedulerHealth = _astorWorkDbContext.SystemHealthMaster
                .Where(sm => sm.Type == 0).FirstOrDefault();

            if (schedulerHealth == null)
            {
                _astorWorkDbContext.SystemHealthMaster.Add(new SystemHealthMaster()
                {
                    Type = 0,
                    Status = 0,
                    LastUpdated = DateTimeOffset.UtcNow
                });
            }
            else
            {
                schedulerHealth.Status = 0;
                schedulerHealth.LastUpdated = DateTimeOffset.UtcNow;
            }
        }

        public async Task<List<TenantInfo>> GetTenantInfosAsync()
        {
            List<TenantInfo> lsttenantInfo = null;
            string tenantInfoStr = string.Empty;

            try
            {
                tenantInfoStr = _cache.GetString("TenantList");
            }
            catch (Exception ex)
            {
                _logger.LogError($"Redis cache failed:  {ex.Message}");
            }

            try
            {
                if (string.IsNullOrEmpty(tenantInfoStr))
                {
                    var tableStorage = new AstorWorkTableStorage();
                    lsttenantInfo = await tableStorage.ListTenants();
                    var tenantInfoSer = JsonConvert.SerializeObject(lsttenantInfo);
                    var tenantInfoExpiry = TimeSpan.FromHours(1);
                    var cacheEntryOptions = new DistributedCacheEntryOptions().SetSlidingExpiration(tenantInfoExpiry);
                    _cache.SetString("TenantList", tenantInfoSer, cacheEntryOptions);
                    return lsttenantInfo;
                }
                else
                {
                    lsttenantInfo = JsonConvert.DeserializeObject<List<TenantInfo>>(tenantInfoStr);
                }
            }
            catch (Exception ex)
            {
                _logger.LogError($"Error Occurred While Loading Tenants!  {ex.Message}");
            }
            return lsttenantInfo;
        }

        public override async Task StopAsync(CancellationToken stoppingToken)
        {
            // Run your graceful clean-up actions
            _logger.LogDebug($"Stopped.");
            await Task.Delay(1000);
        }
    }

}
