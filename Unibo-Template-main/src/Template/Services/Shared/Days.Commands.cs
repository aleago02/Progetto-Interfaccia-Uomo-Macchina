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
        public DateOnly Day { get; set; }
        public decimal HSmartWorking { get; set; }
        public decimal HHoliday { get; set; }
    }

    public partial class SharedService
    {
        public async Task<DateOnly> Handle(AddOrUpdateDaysCommand cmd)
        {
            var day = await _dbContext.UsersDayDetails
                .Where(x => x.Day.Equals(cmd.Day))
                .FirstOrDefaultAsync();

            if (day == null)
            {
                day = new UserDayDetail
                {
                    Day = cmd.Day,
                };
                _dbContext.UsersDayDetails.Add(day);
            }

            day.HHoliday = cmd.HHoliday;
            day.UserId = cmd.Id_User;
            day.HSmartWorking = cmd.HSmartWorking;

            await _dbContext.SaveChangesAsync();

            return day.Day;
        }
    }
}