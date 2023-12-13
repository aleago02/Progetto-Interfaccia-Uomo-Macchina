using Microsoft.EntityFrameworkCore;
using System;
using System.ComponentModel.DataAnnotations.Schema;
using System.ComponentModel.DataAnnotations;
using System.Linq;
using System.Threading.Tasks;

namespace Template.Services.Shared
{
    public class AddOrUpdateDaysCommand
    {
        public Guid Id_User { get; set; }
        public DateTime Day { get; set; }
        public float HSmartWorking { get; set; }
        public float HHoliday { get; set; }
    }

    public partial class SharedService
    {
        public async Task<DateTime> Handle(AddOrUpdateDaysCommand cmd)
        {
            var day = await _dbContext.Days
                .Where(x => x.Day.Equals(cmd.Day))
                .FirstOrDefaultAsync();

            if (day == null)
            {
                day = new Days
                {
                    Day = cmd.Day,
                };
                _dbContext.Days.Add(day);
            }

            day.HHoliday = cmd.HHoliday;
            day.Id_User = cmd.Id_User;
            day.HSmartWorking = cmd.HSmartWorking;

            await _dbContext.SaveChangesAsync();

            return day.Day;
        }
    }
}