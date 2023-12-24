using Microsoft.EntityFrameworkCore;
using System;
using System.ComponentModel;
using System.ComponentModel.Design;
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

            if (cmd.HHoliday != 0)
            {
                if (cmd.HHoliday + day.HSmartWorking > 8)
                {
                    day.HHoliday = 8 - day.HSmartWorking;
                } 
                else
                {
                    day.HHoliday = cmd.HHoliday;
                }
                var request = await _dbContext.Requests
                    .Where(x => x.Id == day.Id)
                    .FirstOrDefaultAsync();

                if (request == null)
                {
                    request = new Requests
                    {
                        Id = day.Id,
                    };
                    _dbContext.Requests.Add(request);
                }
                request.request = true;
            }
            if (cmd.HSmartWork != 0)
            {
                if (cmd.HSmartWork + day.HHoliday > 8)
                {
                    day.HSmartWorking = 8 - day.HHoliday;
                }
                else
                {
                    day.HSmartWorking = cmd.HSmartWork;
                }
                
            }
            day.UserId = cmd.Id;

            await _dbContext.SaveChangesAsync();

            return cmd.Id;
        }

    }
}