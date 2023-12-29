using Microsoft.AspNetCore.Mvc;
using Microsoft.Extensions.Localization;
using System;
using PublicHoliday;
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
        public virtual async Task<IActionResult> Index(IndexViewModel model)
        {
            var users = await _sharedService.Query(model.ToUsersIndexQuery());
            model.SetUsers(users);
            model.CalendarData = GenerateCalendarData(DateTime.Now.Year, DateTime.Now.Month)
                .Select(week => week.Select(day => new CalendarCell { Day = day.Day, Status = day.Status, CssClass = day.CssClass }).ToList())
                .ToList();


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

        private List<List<CalendarDay>> GenerateCalendarData(int year, int month)
        {
            var dal = new DateTime(year, month, 1);
            dal = dal.AddDays(-dal.Day + 1);

            var al = dal.AddMonths(1).AddDays(-1);

            var holidays = new ItalyPublicHoliday().PublicHolidaysInformation(dal.Year);

            var calendarData = new List<List<CalendarDay>>();

            for (int i = 1; i <= DateTime.DaysInMonth(dal.Year, dal.Month); i++)
            {
                var day = new DateTime(dal.Year, dal.Month, i);
                var ferieTrovata = holidays.FirstOrDefault(x => x.HolidayDate.Date == day);

                var calendarDay = new CalendarDay
                {
                    Day = i,
                    Status = GetStatusForDay(day, ferieTrovata),
                    CssClass = GetCssClassForStatus(GetStatusForDay(day, ferieTrovata)),
                    DayOfWeek = day.ToString("dddd") // Assegna il nome del giorno
                };

                // Aggiungi il giorno a CalendarData
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
            // Implementa la logica per determinare la classe CSS in base allo status.
            // Ad esempio, potresti restituire "presente", "assente", "festivo", ecc.
            return status?.ToLower() ?? "lavorativo"; // Restituisce il nome dello status in minuscolo come classe CSS; se status è null, restituisce "lavorativo" come default
        }

    }
}
