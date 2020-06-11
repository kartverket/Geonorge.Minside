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
using Ical.Net.CalendarComponents;
using Ical.Net.DataTypes;
using Ical.Net;
using Ical.Net.Serialization;
using System.Text;

namespace Geonorge.MinSide.Services
{
    public interface IMeetingService
    {
        Task<MeetingViewModel> GetAll(string organizationNumber, string status = null);
        Task<Meeting> Create(Meeting meeting);
        Task<ToDo> CreateToDo(ToDo todo);
        Task<Meeting> Get(int meetingId, string status = null);
        Task Update(Meeting updatedMeeting, int meetingId, List<IFormFile> files);
        Task Delete(int meetingId);
        Task DeleteFile(int id);
        Task<List<ToDo>> GetAllTodo(string organizationNumber, string[] statuses, int? meetingId);
        Task<ToDo> GetToDo(int? id);
        Task UpdateToDo(ToDo toDo);
        Task DeleteToDo(int id);
        Task UpdateToDoList(int meetingId, List<ToDo> toDo);
    }

    public class MeetingService : IMeetingService
    {
        private readonly OrganizationContext _context;
        ApplicationSettings _applicationSettings;

        public MeetingService(OrganizationContext context, ApplicationSettings applicationSettings)
        {
            _context = context;
            _applicationSettings = applicationSettings;
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

        public async Task<ToDo> CreateToDo(ToDo todo)
        {
            todo.Number = await GetNextNumber(todo.OrganizationNumber);
            todo.Status = CodeList.ToDoStatus.First().Key;
            _context.Todo.Add(todo);
            await SaveChanges();
            await SendNotification(todo);
            return todo;
        }

        private Task SendNotification(ToDo todo)
        {
            MimeMessage message = new MimeMessage();

            MailboxAddress from = new MailboxAddress("Admin",
            "dev@arkitektum.no");
            message.From.Add(from);

            MailboxAddress to = new MailboxAddress("Dag",
            "dagolav@arkitektum.no");
            message.To.Add(to);

            message.Subject = "Oppfølgingspunkt: " + todo.Description;

            BodyBuilder bodyBuilder = new BodyBuilder();
            bodyBuilder.HtmlBody = todo.Comment;
            bodyBuilder.TextBody = todo.Comment;



            var attendee = new Ical.Net.DataTypes.Attendee()
            {
                CommonName = "Dag",
                ParticipationStatus = "REQ-PARTICIPANT",
                Rsvp = true,
                Value = new Uri($"mailto:dev@arkitektum.no")
            };

            List<Attendee> attendees = new List<Attendee>();
            attendees.Add(attendee);

            var e = new CalendarEvent
            {
                Summary = todo.Description,
                IsAllDay = true,
                Organizer = new Organizer()
                {
                    CommonName = "Geonorge MinSide",
                    Value = new Uri($"mailto:post@kartverket.no")
                },
                Attendees = attendees,
                Start = new CalDateTime(todo.Deadline),
                Transparency = TransparencyType.Transparent,
                Location = "Teams",
                Description = todo.Description,
                Uid = todo.Id.ToString()
            };

            var calendar = new Calendar();
            calendar.Events.Add(e);

            var serializer = new CalendarSerializer();
            var serializedCalendar = serializer.SerializeToString(calendar);

            var bytesCalendar = Encoding.ASCII.GetBytes(serializedCalendar);
            MemoryStream ms = new MemoryStream(bytesCalendar);
            using (ms)
            {
                ms.Position = 0;

                var fileName = "oppfølging.ics";

                bodyBuilder.Attachments.Add(fileName, ms); 
            }

            message.Body = bodyBuilder.ToMessageBody();

            SmtpClient client = new SmtpClient();
            client.Connect(_applicationSettings.SmtpHost);

            client.Send(message);
            client.Disconnect(true);
            client.Dispose();

            return Task.CompletedTask;
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

        public async Task UpdateToDo(ToDo toDo)
        {
            _context.Update(toDo);
            await SaveChanges();
        }

        public async Task DeleteToDo(int id)
        {
            var toDo = await _context.Todo.FindAsync(id);
            _context.Todo.Remove(toDo);
            await SaveChanges();
        }

        public async Task UpdateToDoList(int meetingId, List<ToDo> toDoes)
        {
            foreach (var todo in toDoes)
            {
                var updatedTodo = await GetToDo(todo.Id);

                if(updatedTodo == null && !string.IsNullOrEmpty(todo.Description))
                {
                    todo.MeetingId = meetingId;
                    todo.Number = await GetNextNumber(updatedTodo.OrganizationNumber);
                    _context.Todo.Add(todo);
                }
                else if(updatedTodo != null) { 
                updatedTodo.Done = todo?.Done;
                updatedTodo.Comment = todo?.Comment;
                updatedTodo.Status = todo?.Status;

                _context.Todo.Update(updatedTodo);
                }

                await SaveChanges();
            }
        }
    }
}
