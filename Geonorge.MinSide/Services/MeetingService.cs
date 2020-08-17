using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Threading.Tasks;
using Geonorge.MinSide.Infrastructure.Context;
using Geonorge.MinSide.Models;
using Geonorge.MinSide.Utils;
using Microsoft.AspNetCore.Http;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Diagnostics;
using MailKit.Net.Smtp;
using MimeKit;
using System.Text;
using Microsoft.Extensions.Logging;
using Kartverket.Geonorge.Utilities.LogEntry;
using Markdig;

namespace Geonorge.MinSide.Services
{
    public interface IMeetingService
    {
        Task<MeetingViewModel> GetAll(string organizationNumber, string status = null);
        Task<Meeting> Create(Meeting meeting);
        Task<ToDo> CreateToDo(ToDo todo, Notification notification);
        Task<Meeting> Get(int meetingId, string status = null);
        Task Update(Meeting updatedMeeting, int meetingId, List<IFormFile> files);
        Task Delete(int meetingId);
        Task DeleteFile(int id);
        Task<List<ToDo>> GetAllTodo(string organizationNumber, string[] statuses, int? meetingId);
        Task<ToDo> GetToDo(int? id);
        Task UpdateToDo(ToDo toDo, Notification notification);
        Task DeleteToDo(int id, Notification notification);
        Task UpdateToDoList(int meetingId, List<ToDo> toDo, Notification notification);
    }

    public class MeetingService : IMeetingService
    {
        private readonly OrganizationContext _context;
        ApplicationSettings _applicationSettings;
        ILogEntryService _logEntryService;
        private readonly ILogger _logger;

        public MeetingService(OrganizationContext context, ApplicationSettings applicationSettings, ILogEntryService logEntryService)
        {
            _context = context;
            _applicationSettings = applicationSettings;
            _logEntryService = logEntryService;
        }

        public async Task<Meeting> Create(Meeting meeting)
        {
            _context.Meetings.Add(meeting);
            await SaveChanges();

            return meeting;
        }

        public async Task Delete(int meetingId)
        {
            var meeting = await _context.Meetings.Where(m => m.Id == meetingId)
                                            .Include(d => d.Documents)
                                            .SingleOrDefaultAsync();

            foreach (var file in meeting.Documents)
            {
                string fileToRemove = _applicationSettings.FilePath + "\\" + file.FileName;
                if (File.Exists(fileToRemove))
                    File.Delete(fileToRemove);
            }

            _context.Meetings.Remove(meeting);
            await SaveChanges();
        }

        public async Task DeleteFile(int id)
        {
            var document = await _context.Documents.Where(m => m.Id == id)
                                            .SingleOrDefaultAsync();

            string fileToRemove = _applicationSettings.FilePath + "\\" + document.FileName;
            if (File.Exists(fileToRemove))
                File.Delete(fileToRemove);

            _context.Documents.Remove(document);
            await SaveChanges();

        }

        private async Task SaveChanges()
        {
            await _context.SaveChangesAsync();
        }

        public async Task<Meeting> Get(int meetingId, string status = null)
        {
            var meeting = await _context.Meetings
                          .Include(d => d.Documents)
                          .Include(d => d.ToDo)
                          .FirstOrDefaultAsync(d => d.Id == meetingId);

            if (!string.IsNullOrEmpty(status))
                meeting.ToDo = meeting.ToDo.Where(s => s.Status == status).ToList();

            return meeting;
        }

        public async Task<MeetingViewModel> GetAll(string organizationNumber,string status = null)
        {
            MeetingViewModel meetingViewModel = new MeetingViewModel();
            var last  =          await _context.Meetings
                                    .Include(d=> d.Documents)
                                    .Include(d => d.ToDo)
                                    .Where(o => o.OrganizationNumber == organizationNumber)
                                    .OrderByDescending(o => o.Date).FirstOrDefaultAsync();

            if(!string.IsNullOrEmpty(status))
                last.ToDo = last.ToDo.Where(s => s.Status == status).ToList();

            meetingViewModel.Last = last;

            meetingViewModel.Archive = await _context.Meetings
                                    .Where(o => o.OrganizationNumber == organizationNumber)
                                    .OrderByDescending(o => o.Date).Skip(1).ToListAsync();

            return meetingViewModel;
        }

        public async Task Update(Meeting updatedMeeting, int meetingId, List<IFormFile> files)
        {
            var currentMeeting = await Get(meetingId);

            if (currentMeeting.Documents == null)
                currentMeeting.Documents = new List<Document>();

            foreach (var file in files)
            {
                string organizationName = CodeList.Organizations[updatedMeeting.OrganizationNumber].ToString();

                Document meetingDocument = new Document();
                meetingDocument.Name = Path.GetFileNameWithoutExtension(file.FileName);
                meetingDocument.Date = DateTime.Today;
                meetingDocument.OrganizationNumber = updatedMeeting.OrganizationNumber;

                meetingDocument.FileName = Helper.CreateFileName(Helper.GetFileExtension(file.FileName), meetingDocument.Name, meetingDocument.Date, organizationName);
                currentMeeting.Documents.Add(meetingDocument);

                await SaveFile(meetingDocument, file);
            }

            _context.Entry(currentMeeting).CurrentValues.SetValues(updatedMeeting);
            await SaveChanges();
        }

        private async Task SaveFile(Document document, IFormFile file)
        {
            using (var fileStream = new FileStream(_applicationSettings.FilePath + "\\" + document.FileName, FileMode.Create))
            {
                await file.CopyToAsync(fileStream);
            }
        }

        public async Task<ToDo> CreateToDo(ToDo todo, Notification notification)
        {
            todo.Number = await GetNextNumber(todo.OrganizationNumber);
            todo.Status = CodeList.ToDoStatus.First().Key;
            _context.Todo.Add(todo);
            await SaveChanges();
            await SendNotificationAdded(todo, notification);
            await Task.Run(() => _logEntryService.AddLogEntry(new LogEntry { ElementId = todo.Id.ToString(), Operation = Operation.Added, User = notification.UserNameCurrentUser, Description = "Nytt oppfølgingspunkt: " + todo.Number + " " + todo.Subject }));
            return todo;
        }

        private Task SendNotificationAdded(ToDo todo, Notification notification)
        {
            if (notification.Send)
            {
                var emails = GetEmailsToNotify(todo, notification);

                foreach (var email in emails)
                {
                    try
                    { 
                        MimeMessage message = new MimeMessage();
                        MailboxAddress from = MailboxAddress.Parse(_applicationSettings.WebmasterEmail);
                        message.From.Add(from);

                        MailboxAddress to = MailboxAddress.Parse(email);
                        message.To.Add(to);

                        message.Subject = "Nytt oppfølgingspunkt Geonorge min side: "+ todo.Number + " " + todo.Subject;

                        BodyBuilder bodyBuilder = new BodyBuilder();
                        bodyBuilder.HtmlBody = todo.Description + "<br>Frist: " + todo.Deadline.ToShortDateString(); 
                        bodyBuilder.TextBody = todo.Description + Environment.NewLine + "Frist: " + todo.Deadline.ToShortDateString();

                        message.Body = bodyBuilder.ToMessageBody();

                        SmtpClient client = new SmtpClient();
                        client.Connect(_applicationSettings.SmtpHost);

                        client.Send(message);
                        client.Disconnect(true);
                        client.Dispose();
                    }

                    catch(Exception ex)
                    {
                        _logger.LogError(ex, ex.Message);
                    }
                }
            }

            return Task.CompletedTask;
        }

        private Task SendNotificationDeleted(Notification notification, string subject, string body, List<string> emails)
        {
            if (notification.Send)
            {
                foreach (var email in emails)
                {
                    try
                    {
                        MimeMessage message = new MimeMessage();
                        MailboxAddress from = MailboxAddress.Parse(_applicationSettings.WebmasterEmail);
                        message.From.Add(from);

                        MailboxAddress to = MailboxAddress.Parse(email);
                        message.To.Add(to);

                        message.Subject = subject;

                        BodyBuilder bodyBuilder = new BodyBuilder();
                        bodyBuilder.HtmlBody = body;
                        bodyBuilder.TextBody = body;

                        message.Body = bodyBuilder.ToMessageBody();

                        SmtpClient client = new SmtpClient();
                        client.Connect(_applicationSettings.SmtpHost);

                        client.Send(message);
                        client.Disconnect(true);
                        client.Dispose();
                    }

                    catch (Exception ex)
                    {
                        _logger.LogError(ex, ex.Message);
                    }
                }
            }

            return Task.CompletedTask;
        }

        private Task SendNotificationUpdated(ToDo todo, ToDo todoOld, Notification notification)
        {
            StringBuilder changes = new StringBuilder();


            if (todo.Number != todoOld.Number)
                changes.Append("Nummer: " + todo.Number + "<br>");

            if (todo.Subject != todoOld.Subject)
                changes.Append("Emne: " + todo.Subject + "<br>");

            if (todo.Description != todoOld.Description)
                changes.Append("Beskrivelse: " + todo.Description + "<br>");

            if (todo.ResponsibleOrganization != todoOld.ResponsibleOrganization)
                changes.Append("Ansvarlig: " + todo.ResponsibleOrganization + "<br>");

            if (todo.Deadline != todoOld.Deadline)
                changes.Append("Frist: " + todo.Deadline.ToShortDateString() + "<br>");

            if (todo.Status != todoOld.Status)
                changes.Append("Status: " + todo.Status + "<br>");

            if (todo.Comment != todoOld.Comment)
                changes.Append("Kommentar: " + Markdown.ToHtml(todo.Comment) + "<br>");

            if (todo.Done.HasValue && todo.Done != todoOld.Done)
                changes.Append("Utført: " + todo.Done.Value.ToShortDateString() + "<br>");

            if (changes.Length > 0)
            {
                changes.Insert(0, "Oppfølgingspunkt " + todo.Number + " " + todo.Subject + " endret:<br>");
            }

            if (notification.Send)
            {
                var emails = GetEmailsToNotify(todo, notification);

                foreach (var email in emails)
                {
                    try
                    {
                        MimeMessage message = new MimeMessage();
                        MailboxAddress from = MailboxAddress.Parse(_applicationSettings.WebmasterEmail);
                        message.From.Add(from);

                        MailboxAddress to = MailboxAddress.Parse(email);
                        message.To.Add(to);

                        message.Subject = "Endret oppfølgingspunkt Geonorge min side: " + todo.Number + " " + todo.Subject;

                        BodyBuilder bodyBuilder = new BodyBuilder();
                        bodyBuilder.HtmlBody = changes.ToString();

                        message.Body = bodyBuilder.ToMessageBody();

                        SmtpClient client = new SmtpClient();
                        client.Connect(_applicationSettings.SmtpHost);

                        client.Send(message);
                        client.Disconnect(true);
                        client.Dispose();
                    }

                    catch (Exception ex)
                    {
                        _logger.LogError(ex, ex.Message);
                    }
                }
            }

            if (changes.Length > 0)
                Task.Run(() => _logEntryService.AddLogEntry(new LogEntry { ElementId = todo.Id.ToString(), Operation = Operation.Modified, User = notification.UserNameCurrentUser, Description = changes.ToString() }));

            return Task.CompletedTask;
        }

        private List<string> GetEmailsToNotify(ToDo todo, Notification notification)
        {
            var emails = _context.UserSettings.Where(s => s.TodoNotification == true && s.Organization == todo.ResponsibleOrganization && s.Email != notification.EmailCurrentUser).Select(u => u.Email).Distinct().ToList();

            return emails;
        }

        private async Task<int> GetNextNumber(string organizationNumber)
        {
            var query = _context.Todo.Where(m => m.OrganizationNumber == organizationNumber);

            var itemsExist = await query.AnyAsync();
            int maxNumberIndex = 0;

            if (itemsExist)
            {
                maxNumberIndex = await query.MaxAsync(x => x.Number);
            }

            return maxNumberIndex + 1;
        }

        public async Task<List<ToDo>> GetAllTodo(string organizationNumber, string[] statuses, int? meetingId)
        {
            if(statuses == null || statuses.Length == 0) { 
                statuses = CodeList.DefaultStatus;
            }

            if (meetingId.HasValue)
                return await _context.Todo.Where(m => m.MeetingId == meetingId).ToListAsync();
            else
                return await _context.Todo.Where(m => m.OrganizationNumber == organizationNumber && statuses.Contains(m.Status)).OrderBy(o => o.Deadline).ToListAsync();
        }

        public async Task<ToDo> GetToDo(int? id)
        {
            return await _context.Todo.FindAsync(id);
        }

        public async Task UpdateToDo(ToDo toDo, Notification notification)
        {
            ToDo oldTodo = _context.Todo.AsNoTracking().Where(i => i.Id == toDo.Id).FirstOrDefault();
            _context.Update(toDo);
            await SaveChanges();
            await SendNotificationUpdated(toDo, oldTodo, notification);
        }

        public async Task DeleteToDo(int id, Notification notification)
        {

            ToDo toDo = await _context.Todo.FindAsync(id);
            var emails = GetEmailsToNotify(toDo, notification);
            var subject = "Oppfølgingspunkt Geonorge min side er slettet: " + toDo.Number + " " + toDo.Subject;
            var body = toDo.Number + " " + toDo.Subject + " er slettet";
            _context.Todo.Remove(toDo);
            await SaveChanges();
            await SendNotificationDeleted(notification, subject, body, emails);
            await Task.Run(() => _logEntryService.AddLogEntry(new LogEntry { ElementId = id.ToString(), Operation = Operation.Deleted, User = notification.UserNameCurrentUser, Description = subject }));
        }

        public async Task UpdateToDoList(int meetingId, List<ToDo> toDoes, Notification notification)
        {
            foreach (var todo in toDoes)
            {
                var updatedTodo = await GetToDo(todo.Id);

                if(updatedTodo == null && !string.IsNullOrEmpty(todo.Subject))
                {
                    todo.MeetingId = meetingId;
                    todo.Number = await GetNextNumber(todo.OrganizationNumber);
                    _context.Todo.Add(todo);
                }
                else if(updatedTodo != null) {

                DateTime? doneOld = updatedTodo.Done;
                string commentOld = updatedTodo.Comment;
                string statusOld = updatedTodo.Status;

                updatedTodo.Done = todo?.Done;
                updatedTodo.Comment = todo?.Comment;
                updatedTodo.Status = todo?.Status;

                _context.Todo.Update(updatedTodo);
                await SendNotificationUpdatedToDoList(updatedTodo,notification, doneOld, commentOld, statusOld);
                }

                await SaveChanges();
            }
        }

        private Task SendNotificationUpdatedToDoList(ToDo todo, Notification notification, DateTime? doneOld, string commentOld, string statusOld)
        {
            StringBuilder changes = new StringBuilder();

            if (todo.Comment != commentOld)
                changes.Append("Kommentar: " + Markdown.ToHtml(todo.Comment) + "<br>");

            if (todo.Done.HasValue && !doneOld.HasValue)
                changes.Append("Utført: " + todo.Done.Value.ToShortDateString() + "<br>");

            if (todo.Status != statusOld)
                changes.Append("Status: " + todo.Status + "<br>");

            if (changes.Length > 0)
            {
                changes.Insert(0, "Oppfølgingspunkt " + todo.Number + " " + todo.Subject + " endret:<br>");
            }

            if (changes.Length > 0 && notification.Send)
            {
                var emails = GetEmailsToNotify(todo, notification);

                foreach (var email in emails)
                {
                    try
                    {
                        MimeMessage message = new MimeMessage();
                        MailboxAddress from = MailboxAddress.Parse(_applicationSettings.WebmasterEmail);
                        message.From.Add(from);

                        MailboxAddress to = MailboxAddress.Parse(email);
                        message.To.Add(to);

                        message.Subject = "Endret oppfølgingspunkt Geonorge min side: " + todo.Number + " " + todo.Subject;


                        BodyBuilder bodyBuilder = new BodyBuilder();
                        bodyBuilder.HtmlBody = changes.ToString();

                        message.Body = bodyBuilder.ToMessageBody();

                        SmtpClient client = new SmtpClient();
                        client.Connect(_applicationSettings.SmtpHost);

                        client.Send(message);
                        client.Disconnect(true);
                        client.Dispose();
                    }

                    catch (Exception ex)
                    {
                        _logger.LogError(ex, ex.Message);
                    }
                }
            }

            if (changes.Length > 0)
                Task.Run(() => _logEntryService.AddLogEntry(new LogEntry { ElementId = todo.Id.ToString(), Operation = Operation.Modified, User = notification.UserNameCurrentUser, Description = changes.ToString() }));

            return Task.CompletedTask;
        }
    }
}
