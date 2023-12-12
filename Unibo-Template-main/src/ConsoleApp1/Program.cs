using PublicHoliday;
using System.Runtime.CompilerServices;

namespace ConsoleApp1
{
    internal class Program
    {
        static void Main(string[] args)
        {
            Console.WriteLine("Hello, World!");


            //var dal = DateTime.Now.Date;

            var dal = new DateTime(2024,12,1);

            dal = dal.AddDays(-dal.Day+1);

            var al = dal.AddMonths(1).AddDays(-1);

            var holidays = new ItalyPublicHoliday().PublicHolidaysInformation(dal.Year);


            for (var data = dal; data <= al; data = data.AddDays(1))
            {
                var ferieTrovata = holidays.Where(x => x.HolidayDate.Date == data).FirstOrDefault();


                var giornoNonLavorativo = (data.DayOfWeek == DayOfWeek.Sunday)?"Domenica": (data.DayOfWeek == DayOfWeek.Saturday)?"Sabato": (ferieTrovata!= null) ? ferieTrovata.GetName() : "";

                Console.WriteLine($"{data.Day} -- {giornoNonLavorativo}");
            }



        }
    }
}