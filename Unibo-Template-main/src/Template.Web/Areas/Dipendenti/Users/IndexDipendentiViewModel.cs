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

        internal void SetUsers(UsersDaysIndexDTO usersIndexDTO)
        {
            Users = usersIndexDTO.Users.Select(x => new UserDaysIndexViewModel(x)).ToArray();
            TotalItems = usersIndexDTO.Count;
        }

        public float LessWork(float HSmartWork, float HHoliday)
        {
            return 8 - HSmartWork - HHoliday;
        }

        public UsersSelectQuery ToUsersIndexQuery()
        {
            return new UsersSelectQuery
            {
                //qui passargli Id User corrente
            };
        }

    }

    public class UserDaysIndexViewModel
    {
        public UserDaysIndexViewModel(UsersDaysIndexDTO.User userWorkIndexDTO)
        {
            this.Day = userWorkIndexDTO.Day;
            this.HSmartWork = userWorkIndexDTO.HSmartWork;
            this.HHoliday = userWorkIndexDTO.HHoliday;
        }

        public DataType Day { get; set; }
        public float HSmartWork { get; set; }
        public float HHoliday { get; set; }
    }
}