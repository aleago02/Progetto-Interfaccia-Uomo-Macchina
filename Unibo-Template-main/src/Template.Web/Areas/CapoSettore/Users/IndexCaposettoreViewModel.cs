using Template.Web.Infrastructure;
using Microsoft.AspNetCore.Mvc;

namespace Template.Web.Areas.CapoSettore.Users
{
    public class IndexCaposettoreViewModel : PagingViewModel
    {

        public override IActionResult GetRoute() => MVC.CapoSettore.Users.Index(this).GetAwaiter().GetResult();

        public string ToJson()
        {
            return JsonSerializer.ToJsonCamelCase(this);
        }
    }
}