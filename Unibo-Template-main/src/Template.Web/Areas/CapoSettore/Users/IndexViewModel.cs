using Template.Web.Infrastructure;
using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Linq;
using Template.Services.Shared;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using static Template.Web.Areas.CapoSettore.Users.IndexViewModel;
using Microsoft.EntityFrameworkCore.Diagnostics;
using Template.Web.Areas.Dipendenti.Users;
using System.Collections;

namespace Template.Web.Areas.CapoSettore.Users
{
    public class IndexViewModel : PagingViewModel
    {
        public IndexViewModel()
        {
            OrderBy = nameof(UserIndexViewModel.Email);
            OrderByDescending = false;
            Users = Array.Empty<UserIndexViewModel>();  
            Days = Array.Empty<DaysIndexViewModel>();
        }

        public override IActionResult GetRoute() => MVC.CapoSettore.Users.Index(this).GetAwaiter().GetResult();

        public DateTime CurrentDate { get; set; }

        public class CalendarCell
        {
            public int? Day { get; set; }
            public string Status { get; set; }
            public string CssClass { get; set; }
            public string DayOfWeek { get; set; }
        }

        public List<List<CalendarCell>> CalendarData { get; set; }

        [Display(Name = "Cerca")]
        public string Filter { get; set; }

        public IEnumerable<UserIndexViewModel> Users { get; set; }

        public Array Id() => Users.Select(x => x.Id).ToArray();

        public IEnumerable<DaysIndexViewModel> Days { get; set; }

        public Guid CurrentId { get ; set; }

        internal void SetUsers(UsersIndexDTO usersIndexDTO)
        {
            Users = usersIndexDTO.Users.Select(x => new UserIndexViewModel(x)).ToArray();
            TotalItems = usersIndexDTO.Count;
        }

        internal void SetDays(ArrayList days)
        {
            Days = (IEnumerable<DaysIndexViewModel>)days;
            TotalItems = 0;
        }

        public Guid setCurrentId(String CurrentId)
        {
            return this.CurrentId = new Guid(CurrentId);
        }

        public UsersIndexQuery ToUsersIndexQuery()
        {
            return new UsersIndexQuery
            {
                Filter = Filter,
                Paging = new Template.Infrastructure.Paging
                {
                    OrderBy = OrderBy,
                    OrderByDescending = OrderByDescending,
                    Page = Page,
                    PageSize = PageSize
                }
            };
        }

        public UsersSelectQuery ToDaysIndexQuery()
        {
            return new UsersSelectQuery
            {
                IdCurrentUser = this.CurrentId
            };
        }


        public string OrderbyUrl<TProperty>(IUrlHelper url, System.Linq.Expressions.Expression<Func<UserIndexViewModel, TProperty>> expression) => base.OrderbyUrl(url, expression);

        public string OrderbyCss<TProperty>(HttpContext context, System.Linq.Expressions.Expression<Func<UserIndexViewModel, TProperty>> expression) => base.OrderbyCss(context, expression);

        public string ToJson()
        {
            return JsonSerializer.ToJsonCamelCase(this);
        }

        public class UpdateViewModel
        {
            public DateTime StartDate { get; set; }
            public DateTime EndDate { get; set; }
        }
    }

    public class UserIndexViewModel
    {
        public UserIndexViewModel(UsersIndexDTO.User userIndexDTO)
        {
            this.Id = userIndexDTO.Id;
            this.Email = userIndexDTO.Email;
            this.FirstName = userIndexDTO.FirstName;
            this.LastName = userIndexDTO.LastName;
        }

        public Guid Id { get; set; }
        public string Email { get; set; }
        public string FirstName { get; set; }
        public string LastName { get; set; }
        public List<List<CalendarCell>> CalendarData { get; set; }
    }

    public class CalendarDay
    {
        public int Day { get; set; }
        public string Status { get; set; }
        public string CssClass { get; set; }
        public string DayOfWeek { get; set; }
    }

    public class DaysIndexViewModel
    {
        public DaysIndexViewModel(UsersDaysIndexDTO.User userWorkIndexDTO)
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