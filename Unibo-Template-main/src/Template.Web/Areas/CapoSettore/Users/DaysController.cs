using Microsoft.AspNetCore.Mvc;
using Microsoft.Extensions.Localization;
using System.Threading.Tasks;
using Template.Infrastructure.AspNetCore;
using Template.Services.Shared;
using Template.Web.Areas.CapoSettore.Users;
using Template.Web.Areas.Dipendenti.Users;
using Template.Web.SignalR;

namespace Template.Web.Areas.CapoSettore.Users
{
    [Area("CapoSettore")]
    public partial class DaysController : AuthenticatedBaseController
    {
        private readonly SharedService _sharedService;
        private readonly IPublishDomainEvents _publisher;
        private readonly IStringLocalizer<SharedResource> _sharedLocalizer;

        public DaysController(SharedService sharedService, IPublishDomainEvents publisher, IStringLocalizer<SharedResource> sharedLocalizer)
        {
            _sharedService = sharedService;
            _publisher = publisher;
            _sharedLocalizer = sharedLocalizer;

            ModelUnbinderHelpers.ModelUnbinders.Add(typeof(IndexViewModel), new SimplePropertyModelUnbinder());

        }

        [HttpGet]
        public virtual async Task<IActionResult> Index(IndexViewModel model)
        {
            model.setCurrentId(this.Identita.IdCorrente);
            var days = await _sharedService.QueryDays(model.ToDaysIndexQuery());
            model.SetDays(days);
            return View(model);
        }
    }
}
