using Template.Services.Shared;
using System;
using System.Linq;
using Template.Services;

namespace Template.Infrastructure
{
    public class DataGenerator
    {
        public static void InitializeUsers(TemplateDbContext context)
        {
            if (context.Users.Any())
            {
                return;   // Data was already seeded
            }

            context.Users.AddRange(
                new User
                {
                    Id = Guid.Parse("3de6883f-9a0b-4667-aa53-0fbc52c4d300"), // Forced to specific Guid for tests
                    Email = "email1@test.it",
                    Password = "M0Cuk9OsrcS/rTLGf5SY6DUPqU2rGc1wwV2IL88GVGo=", // SHA-256 of text "Prova"
                    FirstName = "Nome1",
                    LastName = "Cognome1",
                    NickName = "Nickname1"
                },
                new User
                {
                    Id = Guid.Parse("a030ee81-31c7-47d0-9309-408cb5ac0ac7"), // Forced to specific Guid for tests
                    Email = "amministratore@test.it",
                    Password = "Uy6qvZV0iA2/drm4zACDLCCm7BE9aCKZVQ16bg80XiU=", // SHA-256 of text "Test"
                    FirstName = "Nome2",
                    LastName = "Cognome2",
                    NickName = "Nickname2"
                },
                new User
                {
                    Id = Guid.Parse("bfdef48b-c7ea-4227-8333-c635af267354"), // Forced to specific Guid for tests
                    Email = "aa",
                    Password = "Uy6qvZV0iA2/drm4zACDLCCm7BE9aCKZVQ16bg80XiU=", // SHA-256 of text "Test"
                    FirstName = "Lorenzo",
                    LastName = "Needle",
                    NickName = "Asdrubale"
                });

            context.UsersDayDetails.AddRange(
                new UserDayDetail
                {
                    UserId = Guid.Parse("3de6883f-9a0b-4667-aa53-0fbc52c4d300"), 
                    Day = new DateOnly(2024, 05, 24),
                    HSmartWorking = 8, 
                    HHoliday = 0,
                },
                new UserDayDetail
                {
                    UserId = Guid.Parse("a030ee81-31c7-47d0-9309-408cb5ac0ac7"), 
                    Day = new DateOnly(2024, 05, 20),
                    HSmartWorking = 5, 
                    HHoliday = 0,
                },
                new UserDayDetail
                {
                    UserId = Guid.Parse("bfdef48b-c7ea-4227-8333-c635af267354"),
                    Day = new DateOnly(2024, 05, 16),
                    HSmartWorking = 0,
                    HHoliday = 8,
                },
                new UserDayDetail
                {
                    UserId = Guid.Parse("bfdef48b-c7ea-4227-8333-c635af267354"), 
                    Day = new DateOnly(2024, 05, 05),
                    HSmartWorking = 0,
                    HHoliday = 8,
                });

            context.SaveChanges();
        }



    }
}
