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

namespace Geonorge.MinSide.Services
{
    public interface IMeetingService
    {
        Task<MeetingViewModel> GetAll(string organizationNumber);
        Task<Meeting> Create(Meeting meeting);
        Task<ToDo> CreateToDo(ToDo todo);
        Task<Meeting> Get(int meetingId);
        Task Update(Meeting updatedMeeting, int meetingId, List<IFormFile> files);
        Task Delete(int meetingId);
        Task<List<ToDo>> GetAllTodo(int meetingId);
        Task<ToDo> GetToDo(int? id);
        void UpdateToDo(ToDo toDo);
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

        private async Task SaveChanges()
        {
            await _context.SaveChangesAsync();
        }

        public async Task<Meeting> Get(int meetingId)
        {
            return await _context.Meetings
                          .Include(d => d.Documents)
                          .Include(d => d.ToDo)
                          .FirstOrDefaultAsync(d => d.Id == meetingId);
        }

        public async Task<MeetingViewModel> GetAll(string organizationNumber)
        {
            MeetingViewModel meetingViewModel = new MeetingViewModel();
            meetingViewModel.Last =  await _context.Meetings
                                    .Include(d=> d.Documents)
                                    .Include(d => d.ToDo)
                                    .Where(o => o.OrganizationNumber == organizationNumber)
                                    .OrderByDescending(o => o.Date).FirstOrDefaultAsync();

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
            todo.Number = await GetNextNumber(todo.MeetingId);
            todo.Status = CodeList.ToDoStatus.First().Key;
            _context.Todo.Add(todo);
            await SaveChanges();
            return todo;
        }

        private async Task<int> GetNextNumber(int meetingId)
        {
            var query = _context.Todo.Where(m => m.MeetingId == meetingId);

            var itemsExist = await query.AnyAsync();
            int maxNumberIndex = 0;

            if (itemsExist)
            {
                maxNumberIndex = await query.MaxAsync(x => x.Number);
            }

            return maxNumberIndex + 1;
        }

        public async Task<List<ToDo>> GetAllTodo(int meetingId)
        {
            return await _context.Todo.Where(m => m.MeetingId == meetingId).ToListAsync();
        }

        public async Task<ToDo> GetToDo(int? id)
        {
            return await _context.Todo.FindAsync(id);
        }

        public async void UpdateToDo(ToDo toDo)
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

                updatedTodo.Done = todo?.Done;
                updatedTodo.Comment = todo?.Comment;
                updatedTodo.Status = todo?.Status;

                _context.Todo.Update(updatedTodo);
                await SaveChanges();
            }
        }
    }
}
