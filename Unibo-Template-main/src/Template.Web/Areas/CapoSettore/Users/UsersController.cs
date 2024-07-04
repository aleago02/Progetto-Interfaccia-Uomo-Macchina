using Microsoft.AspNetCore.Mvc;
using Microsoft.Extensions.Localization;
using System;
using System.Collections.Generic;
using System.Globalization;
using System.Linq;
using System.Threading.Tasks;
using Template.Infrastructure.AspNetCore;
using Template.Services.Shared;
using Template.Web.Infrastructure;
using Template.Web.SignalR;
using Template.Web.SignalR.Hubs.Events;
using PublicHoliday;
using static Template.Web.Areas.CapoSettore.Users.IndexViewModel;

namespace Template.Web.Areas.CapoSettore.Users
{
    [Area("CapoSettore")]
    public partial class UsersController : AuthenticatedBaseController
    {
        private readonly SharedService _sharedService;
        private readonly IPublishDomainEvents _publisher;
        private readonly IStringLocalizer<SharedResource> _sharedLocalizer;

        public UsersController(SharedService sharedService, IPublishDomainEvents publisher, IStringLocalizer<SharedResource> sharedLocalizer)
        {
            _sharedService = sharedService;
            _publisher = publisher;
            _sharedLocalizer = sharedLocalizer;

            ModelUnbinderHelpers.ModelUnbinders.Add(typeof(IndexViewModel), new SimplePropertyModelUnbinder());
        }

        [HttpGet]
        public virtual async Task<IActionResult> Index(IndexViewModel model, DateTime? startDate = null, DateTime? endDate = null)
        {
            model.CurrentDate = DateTime.Now;
            int startDay = DateTime.Now.Day + 1 - DateTime.Now.Day;
            int endYear = DateTime.Now.Year;
            int endMonth = DateTime.Now.Month;
            int endDay = DateTime.DaysInMonth(endYear, endMonth);

            if (startDate.HasValue && endDate.HasValue)
            {
                model.CalendarData = GenerateCalendarData(startDate.Value.Day, startDate.Value.Month, startDate.Value.Year, endDate.Value.Day, endDate.Value.Month, endDate.Value.Year)
                    .Select(week => week.Select(day => new CalendarCell { Day = day.Day, Status = day.Status, CssClass = day.CssClass, Date = day.Date }).ToList())
                    .ToList();
            }
            else
            {
                model.CalendarData = GenerateCalendarData(startDay, DateTime.Now.Month, DateTime.Now.Year, endDay, endMonth, endYear)
                    .Select(week => week.Select(day => new CalendarCell { Day = day.Day, Status = day.Status, CssClass = day.CssClass, Date = day.Date }).ToList())
                    .ToList();
            }
            var users = await _sharedService.Query(model.ToUsersIndexQuery());
            model.SetUsers(users);
            var days = await _sharedService.QueryDays(model.ToDaysIndexQuery());
            model.SetDays(days);
            return View(model);
        }

        [HttpGet]
        public virtual async Task<IActionResult> ManageRequest()
        {
            var model = new ManageRequestViewModel();
            var users = await _sharedService.Query(model.ToUsersIndexQuery());
            model.SetUsers(users);
            var days = await _sharedService.QueryDays(model.ToDaysIndexQuery());
            model.SetDays(days);
            return View(model);
        }

        [HttpPost]
        public virtual async Task<IActionResult> Accept(int Id)
        {
            try
            {
                await _sharedService.AcceptRequest(Id);
                Alerts.AddSuccess(this, "Richiesta Accettata");
            }
            catch
            {
                Alerts.AddSuccess(this, "Errore");
            }
            return RedirectToAction(nameof(ManageRequest));
        }

        [HttpPost]
        public virtual async Task<IActionResult> Reject(int Id)
        {
            try
            {
                await _sharedService.DeleteDay(Id);
                Alerts.AddSuccess(this, "Richiesta Rifiutata");
            }
            catch
            {
                Alerts.AddSuccess(this, "Errore");
            }
            return RedirectToAction(nameof(ManageRequest));
        }
        private static List<List<CalendarDay>> GenerateCalendarData(int startDay, int startMonth, int startYear, int endDay, int endMonth, int endYear)
        {
            var calendarData = new List<List<CalendarDay>>();

            for (var year = startYear; year <= endYear; year++)
            {
                var startMonthLoop = (year == startYear) ? startMonth : 1;
                var endMonthLoop = (year == endYear) ? endMonth : 12;

                for (var month = startMonthLoop; month <= endMonthLoop; month++)
                {
                    var daysInMonth = DateTime.DaysInMonth(year, month);
                    var startDayLoop = (year == startYear && month == startMonth) ? startDay : 1;
                    var endDayLoop = (year == endYear && month == endMonth) ? endDay : daysInMonth;

                    for (var day = startDayLoop; day <= endDayLoop; day++)
                    {
                        var date = new DateTime(year, month, day);

                        string status = "";
                        string cssClass = "";

                        if (date.DayOfWeek == DayOfWeek.Sunday)
                        {
                            if (date.Date == DateTime.Now.Date)
                            {
                                status = "Domenica";
                                cssClass = "current-day";
                            }
                            else
                            {
                                status = "Domenica";
                                cssClass = GetCssClassForStatus(status);
                            }
                        }
                        else if (date.DayOfWeek == DayOfWeek.Saturday)
                        {
                            if (date.Date == DateTime.Now.Date)
                            {
                                status = "Sabato";
                                cssClass = "current-day";
                            }
                            else
                            {
                                status = "Sabato";
                                cssClass = GetCssClassForStatus(status);
                            }
                        }
                        else
                        {
                            if (date.Date.AddDays(1) == DateTime.Now.Date.AddDays(1))
                            {
                                status = IsFestivity(date) ? "Festività" : "Lavorativo";
                                cssClass = "current-day";
                            }
                            else
                            {
                                status = IsFestivity(date) ? "Festività" : "Lavorativo";
                                cssClass = GetCssClassForStatus(status);
                            }
                        }

                        var calendarDay = new CalendarDay
                        {
                            Day = day,
                            Status = status,
                            CssClass = cssClass,
                            DayOfWeek = date.ToString("dddd"),
                            Date = DateOnly.FromDateTime(date)
                        };

                        if (calendarData.Count == 0 || calendarData.Last().Count == 7)
                        {
                            calendarData.Add(new List<CalendarDay> { calendarDay });
                        }
                        else
                        {
                            calendarData.Last().Add(calendarDay);
                        }
                    }
                }
            }
            return calendarData;
        }


        private static bool IsFestivity(DateTime date)
        {
            var holidays = new ItalyPublicHoliday().PublicHolidaysInformation(date.Year);

            var isFestivity = holidays.Any(holiday => holiday.HolidayDate.Date == date.Date);

            return isFestivity;
        }

        private static string GetCssClassForStatus(string status)
        {
            return status?.ToLower() ?? "lavorativo";
        }
    }
}