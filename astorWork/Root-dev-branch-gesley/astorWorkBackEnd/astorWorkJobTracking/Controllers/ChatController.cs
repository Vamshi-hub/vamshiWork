using astorWorkDAO;
using astorWorkJobTracking.Models;
using astorWorkShared.GlobalExceptions;
using astorWorkShared.GlobalResponse;
using astorWorkShared.Models;
using astorWorkShared.Services;
using astorWorkShared.Utilities;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Threading.Tasks;
using static astorWorkShared.Utilities.Enums;

namespace astorWorkJobTracking.Controllers
{
    [Produces("application/json")]
    [Route("chat")]
    public class ChatController : Controller
    {
        protected astorWorkDbContext _context;
        protected IAstorWorkBlobStorage _blobStorage;
        public ChatController(astorWorkDbContext context, IAstorWorkBlobStorage blobStorage)
        {
            _context = context;
            _blobStorage = blobStorage;
        }
        /// <summary>
        /// to get the chat list
        /// </summary>
        /// <refefrences>
        /// Used in Mobile chatclient service (mobile)
        /// </refefrences>
        /// <param name="tenant_name"></param>
        /// <returns>list of MessageData object </returns>
        [HttpGet]
        public async Task<List<MessageData>> GetChatList([FromQuery] string tenant_name)
        {
            List<MessageData> chats = new List<MessageData>();
            try
            {
                chats = await _context.ChatData.Include(u => u.User)
                    .Include(c => c.Checklist)
                    .Include(c => c.ChecklistItem)
                    .Include(j => j.JobSchedule)
                    .ThenInclude(jm => jm.Material)
                    .Include(j => j.JobSchedule.Trade)
                    .Include(m => m.MaterialStageAudit)
                    .ThenInclude(mm => mm.Material)
                    .Include(cu => cu.SeenBy)
                    .Select(c => new MessageData()
                    {
                        TenantName = tenant_name,
                        UserName = c.User.UserName,
                        Message = c.Message,
                        MaterialID = c.MaterialStageAudit != null ? c.MaterialStageAudit.MaterialID : 0,
                        MarkingNo = c.MaterialStageAudit != null ? c.MaterialStageAudit.Material.MarkingNo : string.Empty,
                        ModuleName = c.MaterialStageAudit != null ? $"{c.MaterialStageAudit.Material.Block}_{c.MaterialStageAudit.Material.Level}_{c.MaterialStageAudit.Material.Zone}"
                    : $"{c.JobSchedule.Material.Block}_{c.JobSchedule.Material.Level}_{c.JobSchedule.Material.Zone}",
                        JobID = c.JobSchedule != null ? c.JobScheduleID : 0,
                        JobName = c.JobSchedule != null ? c.JobSchedule.Trade.Name : string.Empty,
                        IsSystem = c.IsSystem,
                        HasImage = c.HasAttachment,
                        ThumbnailUrl = c.HasAttachment ? GetImageUrl(c.ThumbnailUrl) : string.Empty,
                        ThumbnailImagebase64 = string.Empty,
                        OriginalImagebase64 = string.Empty,
                        OriginalAttachmentUrl = c.HasAttachment && !string.IsNullOrEmpty(c.OriginalAttachmentUrl) ? GetImageUrl(c.OriginalAttachmentUrl) : string.Empty,
                        ChecklistID = c.ChecklistID,
                        ChecklistName = c.Checklist.Name,
                        ChecklistItemID = c.ChecklistItem != null ? c.ChecklistItemID : null,
                        ChecklistItemName = c.ChecklistItem != null ? c.ChecklistItem.Name : string.Empty,
                        Timestamp = c.TimeStamp,
                        SeenUsers = c.SeenBy.Select(u => u.UserMaster.UserName).ToList()

                    }).ToListAsync();
            }
            catch (Exception ex)
            {
                throw ex;
            }
            return chats;
        }
        /// <summary>
        /// inserts new chat records into db and save thumbnail image in blob
        /// </summary>
        /// <references>
        /// Used in Mobile chatclient service (mobile)
        /// </references>
        /// <param name="user_name"></param>
        /// <param name="messageData"></param>
        /// <returns> returns the inserted records chat id  (int) </returns>
        [HttpPost("save-chat")]
        public async Task<int> SaveChat([FromQuery] string user_name, [FromBody] MessageData messageData)
        {
            ChatData chat = new ChatData();
            int chatId = 0;
            try
            {
                JobSchedule jobSchedule = null;
                MaterialStageAudit materialStageAudit = null;
                ChecklistMaster checklist = null;
                ChecklistItemMaster checklistItem = null;
                UserMaster user = _context.UserMaster.Where(u => u.UserName == messageData.UserName).FirstOrDefault();
                if (messageData != null)
                {
                    if (messageData.MaterialID != null && messageData.MaterialID > 0)
                        materialStageAudit = _context.MaterialStageAudit.Where(msa => msa.MaterialID == messageData.MaterialID).OrderByDescending(msa => msa.CreatedDate).FirstOrDefault();
                    
                    if (messageData.JobID != null && messageData.JobID > 0)
                        jobSchedule = _context.JobSchedule.Where(js => js.ID == messageData.JobID).FirstOrDefault();
                    
                    if (messageData.ChecklistID != null && messageData.ChecklistID > 0)
                        checklist = _context.ChecklistMaster.Where(cm => cm.ID == messageData.ChecklistID).FirstOrDefault();
                    
                    if (messageData.ChecklistItemID != null && messageData.ChecklistItemID > 0)
                        checklistItem = _context.ChecklistItemMaster.Where(chi => chi.ID == messageData.ChecklistItemID).FirstOrDefault();

                    chat.HasAttachment = messageData.HasImage;
                    if (messageData.HasImage)
                        chat.ThumbnailUrl = await _blobStorage.UploadSignature(messageData.ThumbnailImagebase64);

                    chat.User = user;
                    chat.Message = messageData.Message;
                    chat.HasAttachment = messageData.HasImage;
                    chat.IsSystem = messageData.IsSystem;
                    chat.MaterialStageAudit = materialStageAudit;
                    chat.JobSchedule = jobSchedule;
                    chat.Checklist = checklist;
                    chat.ChecklistItem = checklistItem;
                    chat.TimeStamp = messageData.Timestamp;
                    ChatData v = _context.Add(chat).Entity;
                    ChatUserAssociation chatUserAssociation = new ChatUserAssociation();
                    chatUserAssociation.Chat = v;
                    chatUserAssociation.UserMaster = user;
                    chatUserAssociation.SeenDateTime = DateTime.Now;
                    _context.Add(chatUserAssociation);
                    await _context.SaveChangesAsync();
                    chatId = chat.ID;
                }
            }
            catch (Exception ex)
            {
                throw ex;
            }
            return chatId;
        }
        /// <summary>
        /// Uploads the orginal photo to the blob and updates the uploaded photo URL in db
        /// </summary>
        /// <references>
        /// Used in Mobile chatclient service (mobile)
        /// </references>
        /// <param name="file"></param>
        /// <param name="chat_id"></param>
        /// <returns> transaction status (bool)</returns>
        [HttpPut("upload-photo/{chat_id}")]
        public async Task UploadPhoto([FromBody] string file, [FromRoute] int chat_id)
        {
            try
            {
                string url = await _blobStorage.UploadSignature(file);
                ChatData chatData = await _context.ChatData.Where(cd => cd.ID == chat_id).FirstOrDefaultAsync();
                if (chatData != null)
                    chatData.OriginalAttachmentUrl = url;
                //_context.Entry(chatData).State = EntityState.Modified;
                _context.Update(chatData);
                await _context.SaveChangesAsync();
            }
            catch (Exception)
            {
                throw new Exception("Failed to upload the image");
            }
        }

        /// <summary>
        /// update the seenby user to db for a specific chat group
        /// </summary>
        /// <references>
        /// used in chatVM (mobile)
        /// </references>
        /// <param name="user_name"></param>
        /// <param name="messageDatas"></param>
        /// <returns></returns>
        [HttpPut("update-seenby")]
        public async Task<bool> UpdateSeenBy([FromQuery] string user_name, [FromBody] List<MessageData> messageDatas)
        {
            bool status = false;
            try
            {
                var messageData = messageDatas.FirstOrDefault();
                JobSchedule jobSchedule = null;
                MaterialStageAudit materialStageAudit = null;
                ChecklistMaster checklist = null;
                ChecklistItemMaster checklistItem = null;
                List<ChatData> msgs = null;
                UserMaster user = _context.UserMaster.Where(u => u.UserName == user_name).FirstOrDefault();
                if (messageData != null)
                {
                    if (messageData.MaterialID != null && messageData.MaterialID > 0)
                    {
                        materialStageAudit = _context.MaterialStageAudit.Where(msa => msa.MaterialID == messageData.MaterialID).OrderByDescending(msa => msa.CreatedDate).FirstOrDefault();
                    }
                    if (messageData.JobID != null && messageData.JobID > 0)
                    {
                        jobSchedule = _context.JobSchedule.Where(js => js.ID == messageData.JobID).FirstOrDefault();
                    }
                    if (messageData.ChecklistID != null && messageData.ChecklistID > 0)
                    {
                        checklist = _context.ChecklistMaster.Where(cm => cm.ID == messageData.ChecklistID).FirstOrDefault();
                    }
                    if (messageData.ChecklistItemID != null && messageData.ChecklistItemID > 0)
                    {
                        checklistItem = _context.ChecklistItemMaster.Where(chi => chi.ID == messageData.ChecklistItemID).FirstOrDefault();
                    }
                    if (materialStageAudit != null)
                    {
                        if (checklistItem == null)
                            msgs = _context.ChatData.Include(c => c.ChecklistItem).Include(c => c.SeenBy).Where(c => c.MaterialStageAuditID == materialStageAudit.ID && c.ChecklistID == checklist.ID).ToList();
                        else
                        {
                            msgs = _context.ChatData.Include(c => c.ChecklistItem).Include(c => c.SeenBy).Where(c => c.MaterialStageAuditID == materialStageAudit.ID && c.ChecklistID == checklist.ID && c.ChecklistItemID == checklistItem.ID).ToList();
                        }
                    }
                    else
                    {
                        if (checklistItem == null)
                        {
                            msgs = _context.ChatData.Include(c => c.ChecklistItem).Include(c => c.SeenBy).Where(c => c.JobScheduleID == jobSchedule.ID && c.ChecklistID == checklist.ID).ToList();
                        }
                        else
                        {
                            msgs = _context.ChatData.Include(c => c.ChecklistItem).Include(c => c.SeenBy).Where(c => c.JobScheduleID == jobSchedule.ID && c.ChecklistID == checklist.ID && c.ChecklistItemID == checklistItem.ID).ToList();
                        }
                    }
                    if (msgs != null && msgs.Count > 0)
                    {
                        var seenmsgs = msgs.Where(m => m.SeenBy != null && m.SeenBy.Select(s => s.UserMaster).Contains(user)).ToList();
                        var unSeenmsgs = msgs.Except(seenmsgs).ToList();
                        if (unSeenmsgs != null && unSeenmsgs.Count > 0)
                        {
                            foreach (var msg in unSeenmsgs)
                            {
                                ChatUserAssociation chatUserAssociation = new ChatUserAssociation();
                                chatUserAssociation.Chat = msg;
                                chatUserAssociation.UserMaster = user;
                                chatUserAssociation.SeenDateTime = DateTime.Now;

                                await _context.AddAsync(chatUserAssociation);
                            }

                            await _context.SaveChangesAsync();
                            status = true;
                        }
                    }
                }

            }
            catch (Exception ex)
            {
                throw ex;
            }
            return status;
        }
        private string GetImageUrl(string url)
        {
            string token = _blobStorage.GetContainerAccessToken();
            return url + token;
        }
    }
}