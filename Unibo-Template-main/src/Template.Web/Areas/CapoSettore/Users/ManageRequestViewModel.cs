using Microsoft.AspNetCore.Mvc;
using System;
using System.Collections.Generic;
using System.Linq;
using Template.Services.Shared;
using Template.Web.Areas.Dipendenti.Users;
using Template.Web.Infrastructure;
using static Template.Web.Areas.CapoSettore.Users.IndexViewModel;

namespace Template.Web.Areas.CapoSettore.Users
{
    public class ManageRequestViewModel
    {
        public ManageRequestViewModel() 
        {
            Users = Array.Empty<UserRequestViewModel>();
            Days = Array.Empty<DaysRequestViewModel>();
        }

        public Guid? UserId { get; set; }
        public Guid CurrentId { get; set; }
        public IEnumerable<UserRequestViewModel> Users { get; set; }
        public IEnumerable<DaysRequestViewModel> Days { get; set; }

        internal void SetUsers(UsersIndexDTO usersIndexDTO)
        {
            Users = usersIndexDTO.Users.Select(x => new UserRequestViewModel(x)).ToArray();
        }

        internal void SetDays(DaysIndexDTO dayIndexDTO)
        {
            Days = dayIndexDTO.Users.Select(x => new DaysRequestViewModel(x)).ToArray();
        }
        public Guid SetCurrentId(String CurrentId)
        {
            return this.CurrentId = new Guid(CurrentId);
        }
        public UsersIndexQuery ToUsersIndexQuery()
        {
            return new UsersIndexQuery
            {
                IdCurrentUser = this.CurrentId
            };
        }

        public DaysSelectQuery ToDaysIndexQuery()
        {
            return new DaysSelectQuery
            {
                IdCurrentUser = this.CurrentId
            };
        }


    }

    public class UserRequestViewModel
    {
        public UserRequestViewModel(UsersIndexDTO.User userRequestIndexDTO)
        {
            this.Id = userRequestIndexDTO.Id;
            this.Email = userRequestIndexDTO.Email;
            this.FirstName = userRequestIndexDTO.FirstName;
            this.LastName = userRequestIndexDTO.LastName;
        }

        public Guid Id { get; set; }
        public string Email { get; set; }
        public string FirstName { get; set; }
        public string LastName { get; set; }
        public List<List<CalendarCell>> CalendarData { get; set; }
    }

    public class DaysRequestViewModel
    {
        public DaysRequestViewModel(DaysIndexDTO.User userWorkIndexDTO)
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
