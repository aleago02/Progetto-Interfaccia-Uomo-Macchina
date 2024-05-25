using Microsoft.EntityFrameworkCore;
using Microsoft.VisualBasic;
using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.ComponentModel.Design;
using System.Linq;
using System.Numerics;
using System.Threading.Tasks;
using static System.Runtime.InteropServices.JavaScript.JSType;

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
        public Guid UserId { get; set; }
        public DateOnly Day { get; set; }
        public decimal HSmartWork { get; set; }
        public decimal HHoliday { get; set; }
        public DateOnly DayEnd { get; set; }
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
                .Where(x => x.UserId.Equals(cmd.Id) && x.Day.Equals(cmd.Day))
                .FirstOrDefaultAsync();

            if (cmd.DayEnd.Equals(new DateOnly(0001, 01, 01))) {
                cmd.DayEnd = cmd.Day;
            }


            if (day == null)
            {
                day = new UserDayDetail
                {
                    Day = cmd.Day,
                    DayEnd = cmd.DayEnd,
                    UserId = cmd.Id
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
                   .Where(x => x.UserDayDetailId == day.Id)
                   .FirstOrDefaultAsync();

                if (request == null)
                {
                    request = new Request
                    {
                        request = true,
                        UserDayDetailId = day.Id
                    };
                    _dbContext.Requests.Add(request);
                }
                else
                {
                    request.request = true;
                }
                day.Requests.Add(request);
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
            if (day.DayEnd != day.Day)
            {
                int giorni = (day.DayEnd.DayNumber - day.Day.DayNumber) + 1;
                day.HHoliday = giorni * 8;
            }
            day.UserId = cmd.Id;

            await _dbContext.SaveChangesAsync();
            var q = _dbContext.Requests;
            System.Diagnostics.Debug.WriteLine(q.ToString());

            return cmd.Id;
        }

        public async Task DeleteDay(int day)
        {
            var dato = await _dbContext.UsersDayDetails
                .Where(x => x.Id.Equals(day))
                .FirstOrDefaultAsync();

            _dbContext.UsersDayDetails.Remove(dato);

            await _dbContext.SaveChangesAsync();
        }

        public async Task AcceptRequest(int Id)
        {
            var userRequest = _dbContext.Requests;

            foreach(var request in userRequest)
            {
                if (request != null) 
                { 
                    if (request.Id == Id)
                    {
                        request.request = true;

                        await _dbContext.SaveChangesAsync();
                    }
                }
            }

        }
    }
}