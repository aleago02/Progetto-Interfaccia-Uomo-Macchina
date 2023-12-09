using Template.Web.Infrastructure;
using Microsoft.AspNetCore.Mvc;

namespace Template.Web.Areas.Dipendenti.Users
{
    public class IndexDipendentiViewModel : PagingViewModel
    {

        public override IActionResult GetRoute() => MVC.Dipendenti.Users.Index(this).GetAwaiter().GetResult();

        public string ToJson()
        {
            return JsonSerializer.ToJsonCamelCase(this);
        }
    }
}