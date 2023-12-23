using Microsoft.EntityFrameworkCore;
using System;
using System.ComponentModel;
using System.Linq;
using System.Numerics;
using System.Threading.Tasks;

namespace Template.Services.Shared
{
    public class AddOrUpdateUserCommand
    {
        public Guid? Id { get; set; }
        public string Email { get; set; }
        public string FirstName { get; set; }
        public string LastName { get; set; }
        public string NickName { get; set; }
    }

    public class AddOrUpdateUserDayCommand
    {
        public Guid Id { get; set; }
        public DateOnly Day { get; set; }
        public decimal HSmartWork { get; set; }
        public decimal HHoliday { get; set; }
    }

    public partial class SharedService
    {
        public async Task<Guid> Handle(AddOrUpdateUserCommand cmd)
        {
            var user = await _dbContext.Users
                .Where(x => x.Id == cmd.Id)
                .FirstOrDefaultAsync();

            if (user == null)
            {
                user = new User
                {
                    Email = cmd.Email,
                };
                _dbContext.Users.Add(user);
            }

            user.FirstName = cmd.FirstName;
            user.LastName = cmd.LastName;
            user.NickName = cmd.NickName;

            await _dbContext.SaveChangesAsync();

            return user.Id;
        }

        public async Task<Guid> HandleDay(AddOrUpdateUserDayCommand cmd)
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
            day.UserId = cmd.Id;
            day.HSmartWorking = cmd.HSmartWork;

            await _dbContext.SaveChangesAsync();

            return cmd.Id;
        }

    }
}