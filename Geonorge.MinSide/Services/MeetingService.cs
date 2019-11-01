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
        Task<Meeting> Get(int meetingId);
        Task Update(Meeting updatedMeeting, int meetingId);
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
            await _context.SaveChangesAsync();

            return meeting;
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

        public async Task Update(Meeting updatedMeeting, int meetingId)
        {
            var currentMeeting = await Get(meetingId);
            _context.Entry(currentMeeting).CurrentValues.SetValues(updatedMeeting);
            await _context.SaveChangesAsync();
        }

    }
}
