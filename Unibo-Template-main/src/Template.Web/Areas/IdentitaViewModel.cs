using Template.Services.Shared;
using Template.Web.Infrastructure;
using System.Threading.Tasks;
using Template.Infrastructure;
using Microsoft.AspNetCore.Mvc;
using System;

namespace Template.Web.Areas
{
    public class IdentitaViewModel
    {
        public static string VIEWDATA_IDENTITACORRENTE_KEY = "IdentitaUtenteCorrente";

        public String IdCorrente { get; set; }
        public string EmailUtenteCorrente { get; set; }
        public string NomeUtenteCorrente { get; set; }
        public string CognomeUtenteCorrente { get; set; }

        public string NomeCognome
        {
          get
            {
                return NomeUtenteCorrente + " " + CognomeUtenteCorrente;
            }
        }

        public string GravatarUrl
        {
            get
            {
                return EmailUtenteCorrente.ToGravatarUrl(ToGravatarUrlExtension.DefaultGravatar.Identicon, null);
            }
        }
    }
}
