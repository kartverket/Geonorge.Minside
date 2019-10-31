using Geonorge.MinSide.Infrastructure.Context;
using Geonorge.MinSide.Models;
using Microsoft.EntityFrameworkCore;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace Geonorge.MinSide.Services
{
    public interface IMeetingService
    {
        Task<MeetingViewModel> GetAll(string organizationNumber);
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
    }
}
