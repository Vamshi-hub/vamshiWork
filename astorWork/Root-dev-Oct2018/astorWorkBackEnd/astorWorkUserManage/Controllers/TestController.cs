using System;
using System.Threading.Tasks;
using astorWorkDAO;
using astorWorkDAO.Data;
using astorWorkShared.GlobalResponse;
using astorWorkShared.MultiTenancy;
using astorWorkShared.Services;
using astorWorkShared.Utilities;
using astorWorkUserManage.Models;
using Microsoft.AspNetCore.Mvc;

namespace astorWorkUserManage.Controllers
{
    [Produces("application/json")]
    [Route("api/test")]
    public class TestController : Controller
    {
        private IAstorWorkEmail _emailService;
        private IAstorWorkTableStorage _tableStorageService;
        private IAstorWorkBlobStorage _blobStorageService;
        private astorWorkDbContext _context;

        public TestController(astorWorkDbContext context, IAstorWorkEmail emailService, IAstorWorkTableStorage tableStorageService, IAstorWorkBlobStorage blobStorageService)
        {
            _context = context;
            _emailService = emailService;
            _tableStorageService = tableStorageService;
            _blobStorageService = blobStorageService;
        }

        // GET: api/user/5
        [HttpPost("send-email")]
        public async Task<IActionResult> SendEmail([FromBody] EmailCompose compose)
        {
            if (!ModelState.IsValid)
            {
                return BadRequest(ModelState);
            }

            var success = await _emailService.SendSingle(compose.RecipientAddress, compose.RecipientName, compose.Subject, compose.Body);

            if (success)
                return Ok();
            else
                return BadRequest("Unable to send Email");
        }
        [HttpPost("send-bulk-email")]
        public async Task<IActionResult> SendBulkEmail([FromBody] EmailCompose compose)
        {
            if (!ModelState.IsValid)
            {
                return BadRequest(ModelState);
            }

            var success = await _emailService.SendBulk(compose.RecipientAddresses, compose.RecipientNames, compose.Subject, notificationCode: 1, notificationParams: new string[]{
                DateTimeOffset.Now.ToString(), "4", "6", @"http://tenant1.astorworkqa.com/login" });

            if (success)
                return Ok();
            else
                return BadRequest("Unable to send Email");
        }
        // GET: api/test/test_table_storage
        [HttpPost("table_storage_insert")]
        public async Task<IActionResult> TestTableStorageInsert([FromBody] TenantInfo tenantInfo)
        {
            if (!ModelState.IsValid)
            {
                return BadRequest(ModelState);
            }

            var success = await _tableStorageService.AddOrUpdateEntity("TenantConfig", tenantInfo);
            if (success)
                return Ok();
            else
                return BadRequest("Unable to add entity");
        }
        // GET: api/test/test_table_storage
        [HttpGet("table_storage_get")]
        public async Task<IActionResult> TestTableStorageGEt()
        {
            if (!ModelState.IsValid)
            {
                return BadRequest(ModelState);
            }

            var result = await _tableStorageService.ListTenants();
            return Ok(result);
        }

        [HttpPost("upload-tenant-image")]
        public async Task<APIResponse> UploadTenantImage([FromQuery] string fileExt)
        {
            var containerName = "tenant-dev";
            var fileName = string.Format("{0}.{1}", Guid.NewGuid(), fileExt);
            var success = await _blobStorageService.UploadFile(containerName, fileName, Request.Body);
            if (success)
            {
                return new APIResponse
                {
                    Status = 0,
                    Data = fileName
                };
            }
            else
            {
                return new APIResponse
                {
                    Status = 1,
                    Message = _blobStorageService.ErrorMessage()
                };
            }
        }

        [HttpPost("create-tenant-info")]
        public async Task<APIResponse> CreateTenantInfo(TenantInfo tenantInfo)
        {
            tenantInfo.PartitionKey = "TenantConfig";
            tenantInfo.RowKey = tenantInfo.TenantName;
            if (await _tableStorageService.AddOrUpdateEntity(AppConfiguration.GetTenantTableName(), tenantInfo))
            {
                return new APIResponse
                {
                    Status = 0,
                };
            }
            else
            {
                return new APIResponse
                {
                    Status = 1,
                    Message = _tableStorageService.ErrorMessage()
                };
            }
        }

        [HttpGet("init_sql_db")]
        public APIResponse InitSQLDB()
        {
            var error = DbInitializer.Initialize(_context);
            if (string.IsNullOrEmpty(error))
                return new APIResponse
                {
                    Status = 0,
                };
            else
                return new APIResponse
                {
                    Status = 1,
                    Message = error
                };
        }

        [HttpGet("get_sql_db")]
        public IActionResult GetSQLDB()
        {
            if (_context == null)
                return NotFound();
            else
                return Ok(_context.GetConnectionString());
        }
    }
}