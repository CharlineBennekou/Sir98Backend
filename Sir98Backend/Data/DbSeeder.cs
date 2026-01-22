using Microsoft.EntityFrameworkCore;
using Sir98Backend.Models;

namespace Sir98Backend.Data
{
    public static class DbSeeder
    {
        public static async Task SeedAsync(IApplicationBuilder app)
        {
            using var scope = app.ApplicationServices.CreateScope();
            var context = scope.ServiceProvider.GetRequiredService<AppDbContext>();

            // ---------------------------
            // Seed Instructors
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
                    new Instructor
                    {
                        Email = "Marianne",
                        Number = "+45000000",
                        FirstName = "Marianne",
                        Image = "/images/instructors/Marianne.jpg",
                        Activities = new List<Activity>()
                    },
                    new Instructor
                    {
                        Email = "Mette",
                        Number = "+45000000",
                        FirstName = "Mette",
                        Image = "/images/instructors/Mette.jpg",
                        Activities = new List<Activity>()
                    },
                };

                context.Instructors.AddRange(instructors);
                await context.SaveChangesAsync();
            }

            // ---------------------------
            // Seed Activities (ONLY if table is empty)
            // All times in UTC
            // Week starting Monday 2025-12-15
            // ---------------------------
            if (!context.Activities.Any())
            {
                // Load all instructors once and map by FirstName
                var instructorsByName = await context.Instructors
                    .ToDictionaryAsync(i => i.FirstName);

                List<Instructor> I(params string[] names) =>
                    names.Select(n => instructorsByName[n]).ToList();

                context.Activities.AddRange(

                    // 🔹 Mandag (Monday)

                    // Yoga - Mandag 10:00–11:00 | Yoga: Helle
                    new Activity
                    {
                        Title = "Yoga",
                        StartUtc = new DateTimeOffset(2025, 12, 15, 9, 0, 0, TimeSpan.Zero),
                        EndUtc = new DateTimeOffset(2025, 12, 15, 10, 0, 0, TimeSpan.Zero),
                        Address = "Makers Corner, Penselstrøget 66, i lokalet \"Pulsen\"",
                        Cancelled = false,
                        Description = "Rolig yogatræning med fokus på smidighed og balance.",
                        IsRecurring = true,
                        Tag = "Træning",
                        Rrule = "FREQ=WEEKLY;BYDAY=MO",
                        Link = "https://if-sir98.dk/#yoga",
                        Image = "yoga.png",
                        Instructors = I("Helle")
                    },

                    // Spinning - Mandag 14:30–15:00 | Spinning: Piether
                    new Activity
                    {
                        Title = "Spinning",
                        StartUtc = new DateTimeOffset(2025, 12, 15, 13, 30, 0, TimeSpan.Zero),
                        EndUtc = new DateTimeOffset(2025, 12, 15, 14, 0, 0, TimeSpan.Zero),
                        Address = "RMI, Store Møllevej 5, 4000 Roskilde",
                        Cancelled = false,
                        Description = "Intens cykeltræning med høj puls.",
                        IsRecurring = true,
                        Tag = "Træning",
                        Rrule = "FREQ=WEEKLY;BYDAY=MO",
                        Link = "https://if-sir98.dk/#spinning",
                        Image = "spinning.png",
                        Instructors = I("Piether")
                    },

                    // Styrke og sundhedstræning - Mandag 14:30–16:00 | Lars + Helle
                    new Activity
                    {
                        Title = "Styrke og sundhedstræning",
                        StartUtc = new DateTimeOffset(2025, 12, 15, 13, 30, 0, TimeSpan.Zero),
                        EndUtc = new DateTimeOffset(2025, 12, 15, 15, 0, 0, TimeSpan.Zero),
                        Address = "RMI, Store Møllevej 5, 4000 Roskilde",
                        Cancelled = false,
                        Description = "Styrke- og konditionstræning med fokus på sundhed.",
                        IsRecurring = true,
                        Tag = "Træning",
                        Rrule = "FREQ=WEEKLY;BYDAY=MO",
                        Link = "https://if-sir98.dk/#styrke-sundhed",
                        Image = "styrke-sundhed.png",
                        Instructors = I("Lars", "Helle")
                    },

                    // Styrketræning (Lindelunden) - MO,WE,FR 14:00–17:00 | Lillian
                    new Activity
                    {
                        Title = "Styrketræning (Lindelunden)",
                        StartUtc = new DateTimeOffset(2025, 12, 15, 13, 0, 0, TimeSpan.Zero),
                        EndUtc = new DateTimeOffset(2025, 12, 15, 16, 0, 0, TimeSpan.Zero),
                        Address = "Lindelunden 1, Trekroner",
                        Cancelled = false,
                        Description = "Åbent styrketræningslokale med mulighed for egen træning.",
                        IsRecurring = true,
                        Tag = "Træning",
                        Rrule = "FREQ=WEEKLY;BYDAY=MO,WE,FR",
                        Link = "https://if-sir98.dk/#styrketraening",
                        Image = "styrketraening.png",
                        Instructors = I("Lillian")
                    },

                    // 🔹 Tirsdag (Tuesday)

                    // Pilates - Tirsdag 10:00–10:45 | Lillian
                    new Activity
                    {
                        Title = "Pilates",
                        StartUtc = new DateTimeOffset(2025, 12, 16, 9, 0, 0, TimeSpan.Zero),
                        EndUtc = new DateTimeOffset(2025, 12, 16, 9, 45, 0, 0, TimeSpan.Zero),
                        Address = "Bevægelsesrum, Pulsen på Makers Corner, Penselstrøget 66, Roskilde",
                        Cancelled = false,
                        Description = "Skånsom træning med fokus på core og kropskontrol.",
                        IsRecurring = true,
                        Tag = "Træning",
                        Rrule = "FREQ=WEEKLY;BYDAY=TU",
                        Link = "https://if-sir98.dk/#pilates",
                        Image = "pilates.png",
                        Instructors = I("Lillian")
                    },

                    // Krop og bevægelse - Tirsdag 11:30–12:30 | Lillian + Marianne
                    new Activity
                    {
                        Title = "Krop og bevægelse",
                        StartUtc = new DateTimeOffset(2025, 12, 16, 10, 30, 0, TimeSpan.Zero),
                        EndUtc = new DateTimeOffset(2025, 12, 16, 11, 30, 0, TimeSpan.Zero),
                        Address = "Ringparkens Beboerhus, Søndre Ringvej 51 C, 4000 Roskilde",
                        Cancelled = false,
                        Description = "Blid træning med fokus på kropsholdning og bevægelse.",
                        IsRecurring = true,
                        Tag = "Træning",
                        Rrule = "FREQ=WEEKLY;BYDAY=TU",
                        Link = "https://if-sir98.dk/#krop-og-bevaegelse",
                        Image = "krop-og-bevaegelse.png",
                        Instructors = I("Lillian", "Marianne")
                    },

                    // Styrketræning (Lindelunden) - Tirsdag 14:00–16:30 | Lillian
                    new Activity
                    {
                        Title = "Styrketræning (Lindelunden)",
                        StartUtc = new DateTimeOffset(2025, 12, 16, 13, 0, 0, TimeSpan.Zero),
                        EndUtc = new DateTimeOffset(2025, 12, 16, 15, 30, 0, TimeSpan.Zero),
                        Address = "Lindelunden 1, Trekroner",
                        Cancelled = false,
                        Description = "Styrketræning med instruktør til stede.",
                        IsRecurring = true,
                        Tag = "Træning",
                        Rrule = "FREQ=WEEKLY;BYDAY=TU",
                        Link = "https://if-sir98.dk/#styrketraening",
                        Image = "styrketraening.png",
                        Instructors = I("Lillian")
                    },

                    // Varmtvandsgymnastik - Tirsdag 14:00–15:00 | Mette + Margit
                    new Activity
                    {
                        Title = "Varmtvandsgymnastik",
                        StartUtc = new DateTimeOffset(2025, 12, 16, 13, 0, 0, TimeSpan.Zero),
                        EndUtc = new DateTimeOffset(2025, 12, 16, 14, 0, 0, TimeSpan.Zero),
                        Address = "Lysholdbadet, Hyrdehøj 5B, 4000 Roskilde",
                        Cancelled = false,
                        Description = "Blid vandgymnastik i varmtvandsbassin.",
                        IsRecurring = true,
                        Tag = "Træning",
                        Rrule = "FREQ=WEEKLY;BYDAY=TU",
                        Link = "https://if-sir98.dk/#varmtvandsgymnastik",
                        Image = "varmtvandsgymnastik.png",
                        Instructors = I("Mette", "Margit")
                    },

                    // 🔹 Onsdag (Wednesday)

                    // Cirkeltræning - Onsdag 14:15–15:00 | Sarah + Lars + Helle
                    new Activity
                    {
                        Title = "Cirkeltræning",
                        StartUtc = new DateTimeOffset(2025, 12, 17, 13, 15, 0, TimeSpan.Zero),
                        EndUtc = new DateTimeOffset(2025, 12, 17, 14, 0, 0, TimeSpan.Zero),
                        Address = "RMI, Store Møllevej 5, 4000 Roskilde",
                        Cancelled = false,
                        Description = "Cirkeltræning med skiftende øvelser i stationer.",
                        IsRecurring = true,
                        Tag = "Træning",
                        Rrule = "FREQ=WEEKLY;BYDAY=WE",
                        Link = "https://if-sir98.dk/#cirkeltraening",
                        Image = "cirkeltraening.png",
                        Instructors = I("Sarah", "Lars", "Helle")
                    },

                    // Styrke og sundhedstræning - Onsdag 14:00–16:00 | Lars + Helle
                    new Activity
                    {
                        Title = "Styrke og sundhedstræning",
                        StartUtc = new DateTimeOffset(2025, 12, 17, 13, 0, 0, TimeSpan.Zero),
                        EndUtc = new DateTimeOffset(2025, 12, 17, 15, 0, 0, TimeSpan.Zero),
                        Address = "RMI, Store Møllevej 5, 4000 Roskilde",
                        Cancelled = false,
                        Description = "Styrke- og sundhedstræning i fællesskab.",
                        IsRecurring = true,
                        Tag = "Træning",
                        Rrule = "FREQ=WEEKLY;BYDAY=WE",
                        Link = "https://if-sir98.dk/#styrke-sundhed",
                        Image = "styrke-sundhed.png",
                        Instructors = I("Lars", "Helle")
                    },

                    // Badminton - Onsdag 16:30–18:00 | Lars + Margit
                    new Activity
                    {
                        Title = "Badminton",
                        StartUtc = new DateTimeOffset(2025, 12, 17, 15, 30, 0, TimeSpan.Zero),
                        EndUtc = new DateTimeOffset(2025, 12, 17, 17, 0, 0, TimeSpan.Zero),
                        Address = "Hedegårdene Skole, Københavnsvej 34, 4000 Roskilde",
                        Image = "badminton.png",
                        Link = "https://if-sir98.dk/#badminton",
                        Cancelled = false,
                        Description = "Badmintontræning for alle niveauer.",
                        IsRecurring = true,
                        Tag = "Træning",
                        Rrule = "FREQ=WEEKLY;BYDAY=WE",
                        Instructors = I("Lars", "Margit")
                    },

                    // 🔹 Torsdag (Thursday)

                    // Ud i det fri - hver anden torsdag 10:00–12:00 | Lars + Margit + Kim
                    new Activity
                    {
                        Title = "Ud i det fri",
                        StartUtc = new DateTimeOffset(2025, 12, 18, 9, 0, 0, TimeSpan.Zero),
                        EndUtc = new DateTimeOffset(2025, 12, 18, 11, 0, 0, TimeSpan.Zero),
                        Address = "Boserup Skov",
                        Cancelled = false,
                        Description = "Gåtur og aktiviteter i naturen, hver anden torsdag.",
                        IsRecurring = true,
                        Tag = "Træning",
                        Rrule = "FREQ=WEEKLY;INTERVAL=2;BYDAY=TH",
                        Link = "https://if-sir98.dk/#ud-i-det-fri",
                        Image = "ud-i-det-fri.png",
                        Instructors = I("Lars", "Margit", "Kim")
                    },

                    // Svømning - Torsdag 14:45–15:30 | Egon
                    new Activity
                    {
                        Title = "Svømning",
                        StartUtc = new DateTimeOffset(2025, 12, 18, 13, 45, 0, TimeSpan.Zero),
                        EndUtc = new DateTimeOffset(2025, 12, 18, 14, 30, 0, TimeSpan.Zero),
                        Address = "Sct. Jørgens Badet, Helligkorsvej 42 C, 4000 Roskilde",
                        Cancelled = false,
                        Description = "Svømmetræning i varmtvandsbassin. Omklædning fra 14:35.",
                        IsRecurring = true,
                        Tag = "Træning",
                        Rrule = "FREQ=WEEKLY;BYDAY=TH",
                        Link = "https://if-sir98.dk/#svomning",
                        Image = "svomning.png",
                        Instructors = I("Egon")
                    },

                    // 🔹 Fredag (Friday)

                    // Cirkeltræning - Fredag 10:15–11:00 | Sarah + Lars + Helle
                    new Activity
                    {
                        Title = "Cirkeltræning",
                        StartUtc = new DateTimeOffset(2025, 12, 19, 9, 15, 0, TimeSpan.Zero),
                        EndUtc = new DateTimeOffset(2025, 12, 19, 10, 0, 0, TimeSpan.Zero),
                        Address = "RMI, Store Møllevej 5, 4000 Roskilde",
                        Cancelled = false,
                        Description = "Formiddagscirkeltræning med styrke og puls.",
                        IsRecurring = true,
                        Tag = "Træning",
                        Rrule = "FREQ=WEEKLY;BYDAY=FR",
                        Link = "https://if-sir98.dk/#cirkeltraening",
                        Image = "cirkeltraening.png",
                        Instructors = I("Sarah", "Lars", "Helle")
                    },

                    // Styrke og sundhedstræning - Fredag 10:00–12:00 | Lars + Helle
                    new Activity
                    {
                        Title = "Styrke og sundhedstræning",
                        StartUtc = new DateTimeOffset(2025, 12, 19, 9, 0, 0, TimeSpan.Zero),
                        EndUtc = new DateTimeOffset(2025, 12, 19, 11, 0, 0, TimeSpan.Zero),
                        Address = "RMI, Store Møllevej 5, 4000 Roskilde",
                        Cancelled = false,
                        Description = "Styrke- og sundhedstræning til en god start på weekenden.",
                        IsRecurring = true,
                        Tag = "Træning",
                        Rrule = "FREQ=WEEKLY;BYDAY=FR",
                        Link = "https://if-sir98.dk/#styrke-sundhed",
                        Image = "styrke-sundhed.png",
                        Instructors = I("Lars", "Helle")
                    },

                    // Fodbold/badminton - Fredag 12:00–13:30 | Margit
                    new Activity
                    {
                        Title = "Fodbold/badminton",
                        StartUtc = new DateTimeOffset(2025, 12, 19, 11, 0, 0, TimeSpan.Zero),
                        EndUtc = new DateTimeOffset(2025, 12, 19, 12, 30, 0, TimeSpan.Zero),
                        Address = "Kildegårdshallen, Kildegården 9, 4000 Roskilde",
                        Cancelled = false,
                        Description = "Boldspil med både fodbold og badminton.",
                        IsRecurring = true,
                        Tag = "Træning",
                        Rrule = "FREQ=WEEKLY;BYDAY=FR",
                        Link = "https://if-sir98.dk/#fodbold-badminton",
                        Image = "fodbold-badminton.png",
                        Instructors = I("Margit")
                    },

                    // 🔹 Lørdag & Søndag (Saturday & Sunday)

                    // Styrketræning (Lindelunden) - Lørdag & Søndag 13:00–17:00 | Lillian
                    new Activity
                    {
                        Title = "Styrketræning (Lindelunden)",
                        StartUtc = new DateTimeOffset(2025, 12, 20, 12, 0, 0, TimeSpan.Zero),
                        EndUtc = new DateTimeOffset(2025, 12, 20, 16, 0, 0, TimeSpan.Zero),
                        Address = "Lindelunden 1, Trekroner",
                        Cancelled = false,
                        Description = "Åbent træningslokale i weekenden.",
                        IsRecurring = true,
                        Tag = "Træning",
                        Rrule = "FREQ=WEEKLY;BYDAY=SA,SU",
                        Link = "https://if-sir98.dk/#styrketraening",
                        Image = "styrketraening.png",
                        Instructors = I("Lillian")
                    }
                );

                await context.SaveChangesAsync();
            }
        }
    }
}
