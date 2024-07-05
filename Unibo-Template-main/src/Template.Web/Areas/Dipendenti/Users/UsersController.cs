using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore.Metadata.Internal;
using Microsoft.Extensions.Localization;
using System;
using System.Threading.Tasks;
using Template.Infrastructure.AspNetCore;
using Template.Services.Shared;
using Template.Web.Infrastructure;
using Template.Web.SignalR;
using Template.Web.SignalR.Hubs.Events;
using static Microsoft.EntityFrameworkCore.DbLoggerCategory;

namespace Template.Web.Areas.Dipendenti.Users
{
    [Area("Dipendenti")]
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

            ModelUnbinderHelpers.ModelUnbinders.Add(typeof(IndexDipendentiViewModel), new SimplePropertyModelUnbinder());
        }

        [HttpGet]
        public virtual async Task<IActionResult> Index(IndexDipendentiViewModel model)
        {
            model.setCurrentId(this.Identita.IdCorrente);
            var users = await _sharedService.QueryDaysDipendenti(model.ToUsersIndexQuery());
            model.SetUsers(users);
            return View(model);
        }

        [HttpGet]
        public virtual IActionResult Ferie()
        {
            var model = new FerieViewModel();
            return View(model);
        }

        [HttpPost]
        public virtual async Task<IActionResult> Ferie(FerieViewModel model)
        {
            if ( model.Day > DateOnly.FromDateTime(DateTime.Now) && model.Day.DayOfWeek != DayOfWeek.Sunday && model.Day.DayOfWeek != DayOfWeek.Saturday && model.Day <= model.DayEnd)
            {
                try
                {
                    string g = await _sharedService.HandleDay(model.ToAddOrUpdateUserCommand());

                    if (g.Equals("error"))
                    {
                        Alerts.AddError(this, "Errore : Data già utilizzata");
                        return RedirectToAction(Actions.Ferie(model));
                    }

                    if (g.Equals("errorPeriod"))
                    {
                        Alerts.AddError(this, "Errore : Tra il periodo utilizzato c'è una data già utilizzata");
                        return RedirectToAction(Actions.Ferie(model));
                    }

                    model.UserId = Guid.Parse(g);

                    Alerts.AddSuccess(this, "Informazioni aggiornate");

                    // Esempio lancio di un evento SignalR
                    await _publisher.Publish(new NewMessageEvent
                    {
                        IdGroup = model.UserId.Value,
                        IdUser = model.UserId.Value,
                        IdMessage = Guid.NewGuid()
                    });
                }
                catch (Exception e)
                {
                    ModelState.AddModelError(string.Empty, e.Message);
                }
            } else if (model.Day.DayOfWeek == DayOfWeek.Sunday || model.Day.DayOfWeek == DayOfWeek.Saturday)
            {
                Alerts.AddError(this, "Errore : Selezionato un girono festivo");
            }else if (model.Day == DateOnly.FromDateTime(DateTime.Now))
            {
                Alerts.AddError(this, "Errore : Non si può selezionato il giorno corrente");
            }
            else
            {
                Alerts.AddError(this, "Errore nella data");
            }

            return RedirectToAction(Actions.Ferie(model));
        }

        [HttpGet]
        public virtual IActionResult SmartWorking()
        {
            var model = new SmartWorkingViewModel();
            return View(model);
        }

        [HttpPost]
        public virtual async Task<IActionResult> SmartWorking(SmartWorkingViewModel model)
        {
            if (ModelState.IsValid && model.Day > DateOnly.FromDateTime(DateTime.Now) && model.Day.DayOfWeek != DayOfWeek.Sunday && model.Day.DayOfWeek != DayOfWeek.Saturday)
            {
                try
                {
                    string g = await _sharedService.HandleDay(model.ToAddOrUpdateUserCommand());

                    if (g.Equals("error"))
                    {
                        Alerts.AddError(this, "Errore : Data già utilizzata");
                        return RedirectToAction(Actions.SmartWorking(model));
                    }

                    model.UserId = Guid.Parse(g);

                    Alerts.AddSuccess(this, "Informazioni aggiornate");

                    // Esempio lancio di un evento SignalR
                    await _publisher.Publish(new NewMessageEvent
                    {
                        IdGroup = model.UserId.Value,
                        IdUser = model.UserId.Value,
                        IdMessage = Guid.NewGuid()
                    });
                }
                catch (Exception e)
                {
                    ModelState.AddModelError(string.Empty, e.Message);
                }
            }
            else if (model.Day == DateOnly.FromDateTime(DateTime.Now))
            {
                Alerts.AddError(this, "Errore : Non si può selezionato il giorno corrente");
            }
            else
            { 
                Alerts.AddError(this, "Errore nella data");
            }

            return RedirectToAction(Actions.SmartWorking(model));
        }

        [HttpPost]
        public virtual async Task<IActionResult> Delete(int day)
        {
            try
            {
                await _sharedService.DeleteDay(day);

                Alerts.AddSuccess(this, "Richiesta cancellata");
            } catch (Exception e)
            {
                Alerts.AddSuccess(this, "Errore nella cancellazione");
            }


            return RedirectToAction(Actions.Index());
        }
    }
}
