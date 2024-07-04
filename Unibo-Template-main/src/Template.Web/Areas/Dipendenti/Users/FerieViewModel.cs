using Template.Web.Infrastructure;
using Microsoft.AspNetCore.Mvc;
using System;
using Template.Services.Shared;
using System.Linq;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;

namespace Template.Web.Areas.Dipendenti.Users
{
    [TypeScriptModule("Dipendenti.Users.Server")]
    public class FerieViewModel 
    {
        public FerieViewModel() { } 
        public Guid? UserId { get; set; }

        public void setUserId(String id)
        {
            UserId = new Guid(id);
        }
        public DateOnly Day { get; set; }
        public DateOnly DayEnd { get; set; }
        public decimal HSmartWork = 0;
        public decimal HHoliday { get; set; }

        public string ToJson()
        {
            return Infrastructure.JsonSerializer.ToJsonCamelCase(this);
        }

        public DateOnly DateTommorow(DateTime date)
        {
            return DateOnly.FromDateTime(date.AddDays(1));
        }

        public void SetUserDay(UserDayDetailDTO userDayDetailDTO)
        {
            if (userDayDetailDTO != null)
            {
                UserId = userDayDetailDTO.Id;
                HHoliday = userDayDetailDTO.HHoliday;
                Day = userDayDetailDTO.Day;
                HSmartWork = userDayDetailDTO.HSmartWork;
            }
        }

        public AddOrUpdateUserDayCommand ToAddOrUpdateUserCommand()
        {
            return new AddOrUpdateUserDayCommand
            {
                Id = (Guid)UserId,
                Day = Day,
                HSmartWork = HSmartWork,
                HHoliday = HHoliday,
                DayEnd = DayEnd
            };
        }
    }
}