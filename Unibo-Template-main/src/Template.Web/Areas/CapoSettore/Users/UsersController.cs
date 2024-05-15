using Microsoft.AspNetCore.Mvc;
using Microsoft.Extensions.Localization;
using System;
using System.Collections.Generic;
using System.Globalization;
using System.Linq;
using System.Threading.Tasks;
using Template.Infrastructure.AspNetCore;
using Template.Services.Shared;
using Template.Web.Areas.Example.Users;
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
            int endYear = DateTime.Now.Year;
            int endMonth = DateTime.Now.Month;

            if (startDate.HasValue && endDate.HasValue)
            {
                model.CalendarData = GenerateCalendarData(startDate.Value.Day, startDate.Value.Month, startDate.Value.Month, endDate.Value.Day, endDate.Value.Month, endDate.Value.Year)
                    .Select(week => week.Select(day => new CalendarCell { Day = day.Day, Status = day.Status, CssClass = day.CssClass, Date = day.Date }).ToList())
                    .ToList();
            }
            else
            {
                model.CalendarData = GenerateCalendarData(DateTime.Now.Year, DateTime.Now.Month, endYear, endMonth)
                    .Select(week => week.Select(day => new CalendarCell { Day = day.Day, Status = day.Status, CssClass = day.CssClass, Date = day.Date}).ToList())
                    .ToList();
            }
            var users = await _sharedService.Query(model.ToUsersIndexQuery());
            model.SetUsers(users);
            var days = await _sharedService.QueryDays(model.ToDaysIndexQuery());
            model.SetDays(days);
            return View(model);
        }

        [HttpGet]
        public virtual IActionResult New()
        {
            return RedirectToAction(Actions.Edit());
        }

        [HttpGet]
        public virtual async Task<IActionResult> Edit(Guid? id)
        {
            var model = new EditViewModel();

            if (id.HasValue)
            {
                model.SetUser(await _sharedService.Query(new UserDetailQuery
                {
                    Id = id.Value,
                }));
            }

            return View(model);
        }

        [HttpPost]
        public virtual async Task<IActionResult> Edit(EditViewModel model)
        {
            if (ModelState.IsValid)
            {
                try
                {
                    model.Id = await _sharedService.Handle(model.ToAddOrUpdateUserCommand());

                    Alerts.AddSuccess(this, "Informazioni aggiornate");

                    // Esempio lancio di un evento SignalR
                    await _publisher.Publish(new NewMessageEvent
                    {
                        IdGroup = model.Id.Value,
                        IdUser = model.Id.Value,
                        IdMessage = Guid.NewGuid()
                    });
                }
                catch (Exception e)
                {
                    ModelState.AddModelError(string.Empty, e.Message);
                }
            }

            if (ModelState.IsValid == false)
            {
                Alerts.AddError(this, "Errore in aggiornamento");
            }

            return RedirectToAction(Actions.Edit(model.Id));
        }

        [HttpPost]
        public virtual async Task<IActionResult> Delete(Guid id)
        {
            // Query to delete user

            Alerts.AddSuccess(this, "Utente cancellato");

            return RedirectToAction(Actions.Index());
        }

        [HttpPost]
        public virtual IActionResult Update(IndexViewModel model)
        {
            return RedirectToAction(Actions.Index());
        }

        private List<List<CalendarDay>> GenerateCalendarData(int startDay, int startMonth, int startYear, int endDay, int endMonth, int endYear)
        {
            var calendarData = new List<List<CalendarDay>>();

            var startDate = new DateTime(startYear, startMonth, startDay);
            var endDate = new DateTime(endYear, endMonth, endDay);

            for (var date = startDate; date <= endDate; date = date.AddDays(1))
            {
                string status = "";
                string cssClass = "";

                if (date.DayOfWeek == DayOfWeek.Sunday)
                {
                    status = "Domenica";
                    cssClass = date.Date == DateTime.Now.Date ? "current-day" : GetCssClassForStatus(status);
                }
                else if (date.DayOfWeek == DayOfWeek.Saturday)
                {
                    status = "Sabato";
                    cssClass = date.Date == DateTime.Now.Date ? "current-day" : GetCssClassForStatus(status);
                }
                else
                {
                    status = IsFestivity(date) ? "Festività" : "Lavorativo";
                    cssClass = date.Date == DateTime.Now.Date ? "current-day" : GetCssClassForStatus(status);
                }

                var calendarDay = new CalendarDay
                {
                    Day = date.Day,
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

            return calendarData;
        }



        private bool IsFestivity(DateTime date)
        {
            var holidays = new ItalyPublicHoliday().PublicHolidaysInformation(date.Year);

            var isFestivity = holidays.Any(holiday => holiday.HolidayDate.Date == date.Date);

            return isFestivity;
        }

        private string GetStatusForDay(DateTime day, PublicHoliday.Holiday ferieTrovata)
        {
            if (day.DayOfWeek == DayOfWeek.Sunday)
            {
                return "Domenica";
            }
            else if (day.DayOfWeek == DayOfWeek.Saturday)
            {
                return "Sabato";
            }
            else if (ferieTrovata != null)
            {
                return ferieTrovata.GetName();
            }
            else
            {
                return "Lavorativo";
            }
        }
        private string GetCssClassForStatus(string status)
        {
            return status?.ToLower() ?? "lavorativo"; 
        }
    }
}
