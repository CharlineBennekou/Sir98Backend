using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Builder;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.DependencyInjection;
using Sir98Backend.Models;
// using Sir98Backend.Data; // if your DbContext lives here

namespace Sir98Backend.Data // <- change if your project uses a different namespace
{
    public static class DbSeeder
    {
        public static async Task SeedAsync(IApplicationBuilder app)
        {
            using var scope = app.ApplicationServices.CreateScope();

            
            var context = scope.ServiceProvider.GetRequiredService<AppDbContext>();

            
            await context.Database.MigrateAsync();

            // ---------------------------
            // Seed Instructors (5 total)
            // ---------------------------
            if (!context.Instructors.Any())
            {
                var instructors = new List<Instructor>
                {
                    new Instructor
                    {
                        Email = "larsboh@roskilde.dk",
                        Number = "+4524629361",
                        FirstName = "Lars",
                        Image = "/images/instructors/Lars.jpg",
                        Activities = new List<Activity>()
                    },
                    new Instructor
                    {
                        Email = "kassereren@if-sir98.dk",
                        Number = "+4530263764",
                        FirstName = "Margit",
                        Image = "/images/instructors/Margit.jpg",
                        Activities = new List<Activity>()
                    },
                    new Instructor
                    {
                        Email = "Sarah",
                        Number = "+45000000",
                        FirstName = "Sarah",
                        Image = "/images/instructors/Sarah.jpg",
                        Activities = new List<Activity>()
                    },
                    new Instructor
                    {
                        Email = "Hellecla@roskilde.dk",
                        Number = "+4561246783",
                        FirstName = "Helle",
                        Image = "/images/instructors/Helle.jpg",
                        Activities = new List<Activity>()
                    },
                    new Instructor
                    {
                        Email = "lillianv@roskilde.dk",
                        Number = "+4530841920",
                        FirstName = "Lillian",
                        Image = "/images/instructors/Lillian.jpg",
                        Activities = new List<Activity>()
                    },
                    new Instructor
                    {
                        Email = "Pietherlh@roskilde.dk",
                        Number = "+4561246799",
                        FirstName = "Piether",
                        Image = "/images/instructors/Piether.jpg",
                        Activities = new List<Activity>()
                    },
                    new Instructor
                    {
                        Email = "Egon.Christensen@outlook.dk",
                        Number = "+4522295133",
                        FirstName = "Egon",
                        Image = "/images/instructors/Egon.jpg",
                        Activities = new List<Activity>()
                    },
                    new Instructor
                    {
                        Email = "Kmnyholm@gmail.com",
                        Number = "+45000000",
                        FirstName = "Kim",
                        Image = "/images/instructors/Kim.jpg",
                        Activities = new List<Activity>()
                    },


                };

                context.Instructors.AddRange(instructors);
                await context.SaveChangesAsync();
            }

            // ---------------------------
            // Seed Activities
            // All times in UTC
            // Week starting Monday 2025-12-15
            // ---------------------------

            // 🔹 Mandag (Monday)

            // Yoga - Mandag 10:00–11:00
            if (!context.Activities.Any(a => a.Title == "Yoga"))
            {
                context.Activities.Add(new Activity
                {
                    Title = "Yoga",
                    // 2025-12-15 10:00 local → 09:00 UTC
                    StartUtc = new DateTimeOffset(2025, 12, 15, 9, 0, 0, TimeSpan.Zero),
                    // 11:00 local → 10:00 UTC
                    EndUtc = new DateTimeOffset(2025, 12, 15, 10, 0, 0, TimeSpan.Zero),
                    Address = "Makers Corner, Penselstrøget 66, i lokalet \"Pulsen\"",
                    Cancelled = false,
                    Description = "Rolig yogatræning med fokus på smidighed og balance.",
                    IsRecurring = true,
                    Tag = "Træning",
                    Rrule = "FREQ=WEEKLY;BYDAY=MO",
                    Link = "https://if-sir98.dk/#yoga",
                    Image = "yoga.png"
                });
            }

            // Spinning - Mandag 14:30–15:00
            if (!context.Activities.Any(a => a.Title == "Spinning"))
            {
                context.Activities.Add(new Activity
                {
                    Title = "Spinning",
                    // 2025-12-15 14:30 local → 13:30 UTC
                    StartUtc = new DateTimeOffset(2025, 12, 15, 13, 30, 0, TimeSpan.Zero),
                    // 15:00 local → 14:00 UTC
                    EndUtc = new DateTimeOffset(2025, 12, 15, 14, 0, 0, TimeSpan.Zero),
                    Address = "RMI, Store Møllevej 5, 4000 Roskilde",
                    Cancelled = false,
                    Description = "Intens cykeltræning med høj puls.",
                    IsRecurring = true,
                    Tag = "Træning",
                    Rrule = "FREQ=WEEKLY;BYDAY=MO",
                    Link = "https://if-sir98.dk/#spinning",
                    Image = "spinning.png"
                });
            }

            // Styrke og sundhedstræning - Mandag 14:30–16:00
            if (!context.Activities.Any(a => a.Title == "Styrke og sundhedstræning" && a.Rrule == "FREQ=WEEKLY;BYDAY=MO"))
            {
                context.Activities.Add(new Activity
                {
                    Title = "Styrke og sundhedstræning",
                    // 2025-12-15 14:30 local → 13:30 UTC
                    StartUtc = new DateTimeOffset(2025, 12, 15, 13, 30, 0, TimeSpan.Zero),
                    // 16:00 local → 15:00 UTC
                    EndUtc = new DateTimeOffset(2025, 12, 15, 15, 0, 0, TimeSpan.Zero),
                    Address = "RMI, Store Møllevej 5, 4000 Roskilde",
                    Cancelled = false,
                    Description = "Styrke- og konditionstræning med fokus på sundhed.",
                    IsRecurring = true,
                    Tag = "Træning",
                    Rrule = "FREQ=WEEKLY;BYDAY=MO",
                    Link = "https://if-sir98.dk/#styrke-sundhed",
                    Image = "styrke-sundhed.png"
                });
            }

            // Styrketræning (Lindelunden) - Mandag, Onsdag, Fredag 14:00–17:00
            if (!context.Activities.Any(a => a.Title == "Styrketræning (Lindelunden)" && a.Rrule == "FREQ=WEEKLY;BYDAY=MO,WE,FR"))
            {
                context.Activities.Add(new Activity
                {
                    Title = "Styrketræning (Lindelunden)",
                    // Base occurrence: Monday 2025-12-15 14:00 local → 13:00 UTC
                    StartUtc = new DateTimeOffset(2025, 12, 15, 13, 0, 0, TimeSpan.Zero),
                    // 17:00 local → 16:00 UTC
                    EndUtc = new DateTimeOffset(2025, 12, 15, 16, 0, 0, TimeSpan.Zero),
                    Address = "Lindelunden 1, Trekroner",
                    Cancelled = false,
                    Description = "Åbent styrketræningslokale med mulighed for egen træning.",
                    IsRecurring = true,
                    Tag = "Træning",
                    Rrule = "FREQ=WEEKLY;BYDAY=MO,WE,FR",
                    Link = "https://if-sir98.dk/#styrketraening",
                    Image = "styrketraening.png"
                });
            }


            // 🔹 Tirsdag (Tuesday)

            // Pilates - Tirsdag 10:00–10:45
            if (!context.Activities.Any(a => a.Title == "Pilates"))
            {
                context.Activities.Add(new Activity
                {
                    Title = "Pilates",
                    // 2025-12-16 10:00 local → 09:00 UTC
                    StartUtc = new DateTimeOffset(2025, 12, 16, 9, 0, 0, TimeSpan.Zero),
                    // 10:45 local → 09:45 UTC
                    EndUtc = new DateTimeOffset(2025, 12, 16, 9, 45, 0, TimeSpan.Zero),
                    Address = "Bevægelsesrum, Pulsen på Makers Corner, Penselstrøget 66, Roskilde",
                    Cancelled = false,
                    Description = "Skånsom træning med fokus på core og kropskontrol.",
                    IsRecurring = true,
                    Tag = "Træning",
                    Rrule = "FREQ=WEEKLY;BYDAY=TU",
                    Link = "https://if-sir98.dk/#pilates",
                    Image = "pilates.png"
                });
            }

            // Krop og bevægelse - Tirsdag 11:30–12:30
            if (!context.Activities.Any(a => a.Title == "Krop og bevægelse"))
            {
                context.Activities.Add(new Activity
                {
                    Title = "Krop og bevægelse",
                    // 2025-12-16 11:30 local → 10:30 UTC
                    StartUtc = new DateTimeOffset(2025, 12, 16, 10, 30, 0, TimeSpan.Zero),
                    // 12:30 local → 11:30 UTC
                    EndUtc = new DateTimeOffset(2025, 12, 16, 11, 30, 0, TimeSpan.Zero),
                    Address = "Ringparkens Beboerhus, Søndre Ringvej 51 C, 4000 Roskilde",
                    Cancelled = false,
                    Description = "Blid træning med fokus på kropsholdning og bevægelse.",
                    IsRecurring = true,
                    Tag = "Træning",
                    Rrule = "FREQ=WEEKLY;BYDAY=TU",
                    Link = "https://if-sir98.dk/#krop-og-bevaegelse",
                    Image = "krop-og-bevaegelse.png"
                });
            }

            // Styrketræning (Lindelunden) - Tirsdag 14:00–16:30
            if (!context.Activities.Any(a => a.Title == "Styrketræning (Lindelunden)" && a.Rrule == "FREQ=WEEKLY;BYDAY=TU"))
            {
                context.Activities.Add(new Activity
                {
                    Title = "Styrketræning (Lindelunden)",
                    // 2025-12-16 14:00 local → 13:00 UTC
                    StartUtc = new DateTimeOffset(2025, 12, 16, 13, 0, 0, TimeSpan.Zero),
                    // 16:30 local → 15:30 UTC
                    EndUtc = new DateTimeOffset(2025, 12, 16, 15, 30, 0, TimeSpan.Zero),
                    Address = "Lindelunden 1, Trekroner",
                    Cancelled = false,
                    Description = "Styrketræning med instruktør til stede.",
                    IsRecurring = true,
                    Tag = "Træning",
                    Rrule = "FREQ=WEEKLY;BYDAY=TU",
                    Link = "https://if-sir98.dk/#styrketraening",
                    Image = "styrketraening.png"
                });
            }

            // Varmtvandsgymnastik - Tirsdag 14:00–15:00
            if (!context.Activities.Any(a => a.Title == "Varmtvandsgymnastik"))
            {
                context.Activities.Add(new Activity
                {
                    Title = "Varmtvandsgymnastik",
                    // 2025-12-16 14:00 local → 13:00 UTC
                    StartUtc = new DateTimeOffset(2025, 12, 16, 13, 0, 0, TimeSpan.Zero),
                    // 15:00 local → 14:00 UTC
                    EndUtc = new DateTimeOffset(2025, 12, 16, 14, 0, 0, TimeSpan.Zero),
                    Address = "Lysholdbadet, Hyrdehøj 5B, 4000 Roskilde",
                    Cancelled = false,
                    Description = "Blid vandgymnastik i varmtvandsbassin.",
                    IsRecurring = true,
                    Tag = "Træning",
                    Rrule = "FREQ=WEEKLY;BYDAY=TU",
                    Link = "https://if-sir98.dk/#varmtvandsgymnastik",
                    Image = "varmtvandsgymnastik.png"
                });
            }


            // 🔹 Onsdag (Wednesday)

            // Cirkeltræning - Onsdag 14:15–15:00
            if (!context.Activities.Any(a => a.Title == "Cirkeltræning" && a.Rrule == "FREQ=WEEKLY;BYDAY=WE"))
            {
                context.Activities.Add(new Activity
                {
                    Title = "Cirkeltræning",
                    // 2025-12-17 14:15 local → 13:15 UTC
                    StartUtc = new DateTimeOffset(2025, 12, 17, 13, 15, 0, TimeSpan.Zero),
                    // 15:00 local → 14:00 UTC
                    EndUtc = new DateTimeOffset(2025, 12, 17, 14, 0, 0, TimeSpan.Zero),
                    Address = "RMI, Store Møllevej 5, 4000 Roskilde",
                    Cancelled = false,
                    Description = "Cirkeltræning med skiftende øvelser i stationer.",
                    IsRecurring = true,
                    Tag = "Træning",
                    Rrule = "FREQ=WEEKLY;BYDAY=WE",
                    Link = "https://if-sir98.dk/#cirkeltraening",
                    Image = "cirkeltraening.png"
                });
            }

            // Styrke og sundhedstræning - Onsdag 14:00–16:00
            if (!context.Activities.Any(a => a.Title == "Styrke og sundhedstræning" && a.Rrule == "FREQ=WEEKLY;BYDAY=WE"))
            {
                context.Activities.Add(new Activity
                {
                    Title = "Styrke og sundhedstræning",
                    // 2025-12-17 14:00 local → 13:00 UTC
                    StartUtc = new DateTimeOffset(2025, 12, 17, 13, 0, 0, TimeSpan.Zero),
                    // 16:00 local → 15:00 UTC
                    EndUtc = new DateTimeOffset(2025, 12, 17, 15, 0, 0, TimeSpan.Zero),
                    Address = "RMI, Store Møllevej 5, 4000 Roskilde",
                    Cancelled = false,
                    Description = "Styrke- og sundhedstræning i fællesskab.",
                    IsRecurring = true,
                    Tag = "Træning",
                    Rrule = "FREQ=WEEKLY;BYDAY=WE",
                    Link = "https://if-sir98.dk/#styrke-sundhed",
                    Image = "styrke-sundhed.png"
                });
            }

            // Badminton (Onsdag 16:30–18:00, weekly)
            if (!context.Activities.Any(a => a.Title == "Badminton"))
            {
                context.Activities.Add(new Activity
                {
                    Title = "Badminton",
                    // 2025-12-17 16:30 local (Copenhagen, UTC+1) → 15:30 UTC
                    StartUtc = new DateTimeOffset(2025, 12, 17, 15, 30, 0, TimeSpan.Zero),
                    // 18:00 local → 17:00 UTC
                    EndUtc = new DateTimeOffset(2025, 12, 17, 17, 0, 0, TimeSpan.Zero),
                    Address = "Hedegårdene Skole, Københavnsvej 34, 4000 Roskilde",
                    Image = "badminton.png",
                    Link = "https://if-sir98.dk/#badminton",
                    Cancelled = false,
                    Description = "Badmintontræning for alle niveauer.",
                    IsRecurring = true,
                    Tag = "Træning",
                    Rrule = "FREQ=WEEKLY;BYDAY=WE"
                });
            }


            // 🔹 Torsdag (Thursday)

            // Ud i det fri - Torsdag i ulige uger 10:00–12:00
            if (!context.Activities.Any(a => a.Title == "Ud i det fri"))
            {
                context.Activities.Add(new Activity
                {
                    Title = "Ud i det fri",
                    // 2025-12-18 10:00 local → 09:00 UTC
                    StartUtc = new DateTimeOffset(2025, 12, 18, 9, 0, 0, TimeSpan.Zero),
                    // 12:00 local → 11:00 UTC
                    EndUtc = new DateTimeOffset(2025, 12, 18, 11, 0, 0, TimeSpan.Zero),
                    Address = "Boserup Skov",
                    Cancelled = false,
                    Description = "Gåtur og aktiviteter i naturen, hver anden torsdag.",
                    IsRecurring = true,
                    Tag = "Træning",
                    // Every 2 weeks on Thursday
                    Rrule = "FREQ=WEEKLY;INTERVAL=2;BYDAY=TH",
                    Link = "https://if-sir98.dk/#ud-i-det-fri",
                    Image = "ud-i-det-fri.png"
                });
            }

            // Svømning - Torsdag 14:45–15:30
            if (!context.Activities.Any(a => a.Title == "Svømning"))
            {
                context.Activities.Add(new Activity
                {
                    Title = "Svømning",
                    // 2025-12-18 14:45 local → 13:45 UTC
                    StartUtc = new DateTimeOffset(2025, 12, 18, 13, 45, 0, TimeSpan.Zero),
                    // 15:30 local → 14:30 UTC
                    EndUtc = new DateTimeOffset(2025, 12, 18, 14, 30, 0, TimeSpan.Zero),
                    Address = "Sct. Jørgens Badet, Helligkorsvej 42 C, 4000 Roskilde",
                    Cancelled = false,
                    Description = "Svømmetræning i varmtvandsbassin. Omklædning fra 14:35.",
                    IsRecurring = true,
                    Tag = "Træning",
                    Rrule = "FREQ=WEEKLY;BYDAY=TH",
                    Link = "https://if-sir98.dk/#svomning",
                    Image = "svomning.png"
                });
            }


            // 🔹 Fredag (Friday)

            // Cirkeltræning - Fredag 10:15–11:00
            if (!context.Activities.Any(a => a.Title == "Cirkeltræning" && a.Rrule == "FREQ=WEEKLY;BYDAY=FR"))
            {
                context.Activities.Add(new Activity
                {
                    Title = "Cirkeltræning",
                    // 2025-12-19 10:15 local → 09:15 UTC
                    StartUtc = new DateTimeOffset(2025, 12, 19, 9, 15, 0, TimeSpan.Zero),
                    // 11:00 local → 10:00 UTC
                    EndUtc = new DateTimeOffset(2025, 12, 19, 10, 0, 0, TimeSpan.Zero),
                    Address = "RMI, Store Møllevej 5, 4000 Roskilde",
                    Cancelled = false,
                    Description = "Formiddagscirkeltræning med styrke og puls.",
                    IsRecurring = true,
                    Tag = "Træning",
                    Rrule = "FREQ=WEEKLY;BYDAY=FR",
                    Link = "https://if-sir98.dk/#cirkeltraening",
                    Image = "cirkeltraening.png"
                });
            }

            // Styrke og sundhedstræning - Fredag 10:00–12:00
            if (!context.Activities.Any(a => a.Title == "Styrke og sundhedstræning" && a.Rrule == "FREQ=WEEKLY;BYDAY=FR"))
            {
                context.Activities.Add(new Activity
                {
                    Title = "Styrke og sundhedstræning",
                    // 2025-12-19 10:00 local → 09:00 UTC
                    StartUtc = new DateTimeOffset(2025, 12, 19, 9, 0, 0, TimeSpan.Zero),
                    // 12:00 local → 11:00 UTC
                    EndUtc = new DateTimeOffset(2025, 12, 19, 11, 0, 0, TimeSpan.Zero),
                    Address = "RMI, Store Møllevej 5, 4000 Roskilde",
                    Cancelled = false,
                    Description = "Styrke- og sundhedstræning til en god start på weekenden.",
                    IsRecurring = true,
                    Tag = "Træning",
                    Rrule = "FREQ=WEEKLY;BYDAY=FR",
                    Link = "https://if-sir98.dk/#styrke-sundhed",
                    Image = "styrke-sundhed.png"
                });
            }

            // Fodbold/badminton - Fredag 12:00–13:30
            if (!context.Activities.Any(a => a.Title == "Fodbold/badminton"))
            {
                context.Activities.Add(new Activity
                {
                    Title = "Fodbold/badminton",
                    // 2025-12-19 12:00 local → 11:00 UTC
                    StartUtc = new DateTimeOffset(2025, 12, 19, 11, 0, 0, TimeSpan.Zero),
                    // 13:30 local → 12:30 UTC
                    EndUtc = new DateTimeOffset(2025, 12, 19, 12, 30, 0, TimeSpan.Zero),
                    Address = "Kildegårdshallen, Kildegården 9, 4000 Roskilde",
                    Cancelled = false,
                    Description = "Boldspil med både fodbold og badminton.",
                    IsRecurring = true,
                    Tag = "Træning",
                    Rrule = "FREQ=WEEKLY;BYDAY=FR",
                    Link = "https://if-sir98.dk/#fodbold-badminton",
                    Image = "fodbold-badminton.png"
                });
            }


            // 🔹 Lørdag & Søndag (Saturday & Sunday)

            // Styrketræning (Lindelunden) - Lørdag & Søndag 13:00–17:00
            if (!context.Activities.Any(a => a.Title == "Styrketræning (Lindelunden)" && a.Rrule == "FREQ=WEEKLY;BYDAY=SA,SU"))
            {
                context.Activities.Add(new Activity
                {
                    Title = "Styrketræning (Lindelunden)",
                    // Base: Saturday 2025-12-20 13:00 local → 12:00 UTC
                    StartUtc = new DateTimeOffset(2025, 12, 20, 12, 0, 0, TimeSpan.Zero),
                    // 17:00 local → 16:00 UTC
                    EndUtc = new DateTimeOffset(2025, 12, 20, 16, 0, 0, TimeSpan.Zero),
                    Address = "Lindelunden 1, Trekroner",
                    Cancelled = false,
                    Description = "Åbent træningslokale i weekenden.",
                    IsRecurring = true,
                    Tag = "Træning",
                    Rrule = "FREQ=WEEKLY;BYDAY=SA,SU",
                    Link = "https://if-sir98.dk/#styrketraening",
                    Image = "styrketraening.png"
                });
            }


            await context.SaveChangesAsync();
        }
    }
}
