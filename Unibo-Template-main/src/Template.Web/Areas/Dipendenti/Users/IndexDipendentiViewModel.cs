using Template.Web.Infrastructure;
using Microsoft.AspNetCore.Mvc;
using System;
using Template.Services.Shared;
using System.Linq;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;

namespace Template.Web.Areas.Dipendenti.Users
{
    public class IndexDipendentiViewModel : PagingViewModel
    {
        public IndexDipendentiViewModel()
        {
            Users = Array.Empty<UserDaysIndexViewModel>();
        }

        public override IActionResult GetRoute() => MVC.Dipendenti.Users.Index(this).GetAwaiter().GetResult();

        public string ToJson()
        {
            return JsonSerializer.ToJsonCamelCase(this);
        }

        public IEnumerable<UserDaysIndexViewModel> Users { get; set; }
        public Guid CurrentId { get; set; }

        internal void SetUsers(DaysIndexDTO usersIndexDTO)
        {
            Users = usersIndexDTO.Users.Select(x => new UserDaysIndexViewModel(x)).ToArray();
            TotalItems = usersIndexDTO.Count;
        }

        public decimal LessWork(decimal HSmartWork, decimal HHoliday)
        {
            return 8 - HSmartWork - HHoliday;
        }

        public Guid setCurrentId(String CurrentId)
        {
            return this.CurrentId = new Guid(CurrentId);
        }

        public DaysSelectQuery ToUsersIndexQuery()
        {
            return new DaysSelectQuery
            {
                IdCurrentUser = this.CurrentId
            };
        }

    }

    public class UserDaysIndexViewModel
    {
        public UserDaysIndexViewModel(DaysIndexDTO.User userWorkIndexDTO)
        {
            this.Id = userWorkIndexDTO.Id;
            this.Day = userWorkIndexDTO.Day;
            this.HSmartWork = userWorkIndexDTO.HSmartWork;
            this.HHoliday = userWorkIndexDTO.HHoliday;
            this.Request = userWorkIndexDTO.Request;
        }
        public Guid Id { get; set; }
        public DateOnly Day { get; set; }
        public decimal HSmartWork { get; set; }
        public decimal HHoliday { get; set; }
        public Boolean Request { get; set; }
    }
}