using Template.Web.Infrastructure;
using Microsoft.AspNetCore.Mvc;
using System;
using Template.Services.Shared;
using System.Linq;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace Template.Web.Areas.Dipendenti.Users
{
    [TypeScriptModule("Dipendenti.Users.Server")]
    public class SmartWorkingViewModel 
    {
        public SmartWorkingViewModel()
        {
        }

        public Guid? UserId { get; set; }

        public void setUserId(String id)
        {
            UserId = new Guid(id);
        }
        public DateOnly Day { get; set; }
        public decimal HHoliday = 0;
        public decimal HSmartWork { get; set; }

        public string ToJson()
        {
            return Infrastructure.JsonSerializer.ToJsonCamelCase(this);
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
                HHoliday = HHoliday
            };
        }
    }
}