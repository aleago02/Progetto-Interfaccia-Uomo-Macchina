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
                model.CalendarData = GenerateCalendarData(startDate.Value.Year, startDate.Value.Month, endDate.Value.Year, endDate.Value.Month)
                    .Select(week => week.Select(day => new CalendarCell { Day = day.Day, Status = day.Status, CssClass = day.CssClass }).ToList())
                    .ToList();
            }
            else
            {
                model.CalendarData = GenerateCalendarData(DateTime.Now.Year, DateTime.Now.Month, endYear, endMonth)
                    .Select(week => week.Select(day => new CalendarCell { Day = day.Day, Status = day.Status, CssClass = day.CssClass }).ToList())
                    .ToList();
            }
            var users = await _sharedService.Query(model.ToUsersIndexQuery());
            model.SetUsers(users);
            var days = await _sharedService.QueryDaysArray(model.Id());
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

        private List<List<CalendarDay>> GenerateCalendarData(int startYear, int startMonth, int endYear, int endMonth)
        {
            var calendarData = new List<List<CalendarDay>>();

            for (var year = startYear; year <= endYear; year++)
            {
                for (var month = startMonth; month <= (year == endYear ? endMonth : 12); month++)
                {
                    var daysInMonth = DateTime.DaysInMonth(year, month);
                    for (var day = 1; day <= daysInMonth; day++)
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
                            if (date.Date.AddDays(1)  == DateTime.Now.Date.AddDays(1))
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
                            DayOfWeek = date.ToString("dddd")
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
