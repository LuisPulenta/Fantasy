using Fantasy.Backend.UnitsOfWork.Interfaces;
using Fantasy.Shared.Entities;
using Fantasy.Shared.Enums;
using Fantasy.Shared.Helpers;
using Microsoft.EntityFrameworkCore;

namespace Fantasy.Backend.Data;

public class SeedDb
{
    private readonly DataContext _context;
    private readonly IFilesHelper _filesHelper;
    private readonly IUsersUnitOfWork _usersUnitOfWork;

    public SeedDb(DataContext context,IFilesHelper filesHelper, IUsersUnitOfWork usersUnitOfWork)
    {
        _context = context;
        _filesHelper = filesHelper;
        _usersUnitOfWork = usersUnitOfWork;
    }

    public async Task SeedAsync()
    {
        await _context.Database.EnsureCreatedAsync();
        await CheckCountriesAsync();
        await CheckTeamsAsync();
        await CheckRolesAsync();
        await CheckUsersAsync();
        await CheckTournamentsAsync();
        await CheckGroupsAsync();
    }

    //------------------------------------------------------------------------------------------------------
    private async Task CheckUsersAsync()
    {
        await CheckUserAsync("Luis", "Núñez", "luis@yopmail.com", "156 814 963", "luis.jpg",UserType.Admin);
        await CheckUserAsync("Juan", "Zuluaga", "zulu@yopmail.com", "322 311 4620", "juanzuluaga.jpg", UserType.User);
        await CheckUserAsync("Ledys", "Bedoya", "ledys@yopmail.com", "322 311 4620", "ledysbedoya.jpg", UserType.User);
        await CheckUserAsync("Brad", "Pitt", "brad@yopmail.com", "322 311 4620", "brad.jpg", UserType.User);
        await CheckUserAsync("Angelina", "Jolie", "angelina@yopmail.com", "322 311 4620", "angelina.jpg", UserType.User);
        await CheckUserAsync("Bob", "Marley", "bob@yopmail.com", "322 311 4620", "bob.jpg", UserType.User);
        await CheckUserAsync("Celia", "Cruz", "celia@yopmail.com", "322 311 4620", "celia.jpg", UserType.Admin);
        await CheckUserAsync("Fredy", "Mercury", "fredy@yopmail.com", "322 311 4620", "fredy.jpg", UserType.User);
        await CheckUserAsync("Hector", "Lavoe", "hector@yopmail.com", "322 311 4620", "hector.jpg", UserType.User);
        await CheckUserAsync("Liv", "Taylor", "liv@yopmail.com", "322 311 4620", "liv.jpg", UserType.User);
        await CheckUserAsync("Otep", "Shamaya", "otep@yopmail.com", "322 311 4620", "otep.jpg", UserType.User);
        await CheckUserAsync("Ozzy", "Osbourne", "ozzy@yopmail.com", "322 311 4620", "ozzy.jpg", UserType.User);
        await CheckUserAsync("Selena", "Quintanilla", "selena@yopmail.com", "322 311 4620", "selena.jpg", UserType.User);

        await CheckUserAsync("Gabriel", "Batistuta", "batistuta@yopmail.com", "322 311 4620", "batistuta.jpg", UserType.User);
        await CheckUserAsync("Roger", "Federer", "federer@yopmail.com", "322 311 4620", "federer.jpg", UserType.User);
        await CheckUserAsync("Mario", "Kempes", "kempes@yopmail.com", "322 311 4620", "kempes.jpg", UserType.User);
        await CheckUserAsync("Diego", "Maradona", "maradona@yopmail.com", "322 311 4620", "maradona.jpg", UserType.User);
        await CheckUserAsync("Lionel", "Messi", "messi@yopmail.com", "322 311 4620", "messi.jpg", UserType.User);
        await CheckUserAsync("Rafael", "Nadal", "nadal@yopmail.com", "322 311 4620", "nadal.jpg", UserType.User);
        await CheckUserAsync("Pablo", "Lacuadri", "pablo@yopmail.com", "322 311 4620", "pablo.jpg", UserType.Admin);
    }

    //------------------------------------------------------------------------------------------------------
    private async Task<User> CheckUserAsync(string firstName, string lastName, string email, string phone, string image, UserType userType)
    {
        var user = await _usersUnitOfWork.GetUserAsync(email);
        if (user == null)
        {
            var country = await _context.Countries.FirstOrDefaultAsync(x => x.Name == "Argentina");
            if (country == null)
            {
                country = await _context.Countries.FirstOrDefaultAsync();
            }

            user = new User
            {
                FirstName = firstName,
                LastName = lastName,
                Email = email,
                UserName = email,
                PhoneNumber = phone,
                Country=country!,
                UserType = userType,
                Photo = $"~/images/users/{image}"
            };

            await _usersUnitOfWork.AddUserAsync(user, "123456");
            await _usersUnitOfWork.AddUserToRoleAsync(user, userType.ToString());

            var token = await _usersUnitOfWork.GenerateEmailConfirmationTokenAsync(user);
            await _usersUnitOfWork.ConfirmEmailAsync(user, token);
        }
        return user;
    }

    //------------------------------------------------------------------------------------------------------
    private async Task CheckTournamentsAsync()
    {
        if (!_context.TournamentTeams.Any())
        {
            var colombia = await _context.Teams.FirstOrDefaultAsync(x => x.Name == "Colombia")!;
            var peru = await _context.Teams.FirstOrDefaultAsync(x => x.Name == "Peru");
            var ecuador = await _context.Teams.FirstOrDefaultAsync(x => x.Name == "Ecuador");
            var venezuela = await _context.Teams.FirstOrDefaultAsync(x => x.Name == "Venezuela");
            var brazil = await _context.Teams.FirstOrDefaultAsync(x => x.Name == "Brazil");
            var argentina = await _context.Teams.FirstOrDefaultAsync(x => x.Name == "Argentina");
            var uruguay = await _context.Teams.FirstOrDefaultAsync(x => x.Name == "Uruguay");
            var chile = await _context.Teams.FirstOrDefaultAsync(x => x.Name == "Chile");
            var bolivia = await _context.Teams.FirstOrDefaultAsync(x => x.Name == "Bolivia");
            var paraguay = await _context.Teams.FirstOrDefaultAsync(x => x.Name == "Paraguay");

            var unitedStates = await _context.Teams.FirstOrDefaultAsync(x => x.Name == "United States");
            var canada = await _context.Teams.FirstOrDefaultAsync(x => x.Name == "Canada");
            var mexico = await _context.Teams.FirstOrDefaultAsync(x => x.Name == "Mexico");
            var panama = await _context.Teams.FirstOrDefaultAsync(x => x.Name == "Panama");
            var costaRica = await _context.Teams.FirstOrDefaultAsync(x => x.Name == "Costa Rica ");
            var honduras = await _context.Teams.FirstOrDefaultAsync(x => x.Name == "Honduras");
            var jamaica = await _context.Teams.FirstOrDefaultAsync(x => x.Name == "Jamaica");
            var guatemala = await _context.Teams.FirstOrDefaultAsync(x => x.Name == "Guatemala");
            var barbados = await _context.Teams.FirstOrDefaultAsync(x => x.Name == "Barbados");
            var dominica = await _context.Teams.FirstOrDefaultAsync(x => x.Name == "Dominica");

            var name = "Copa América - 2025";
            var imagePath = string.Empty;
            var filePath = $"{Environment.CurrentDirectory}\\Images\\Tournaments\\{name}.png";
            
            //if (File.Exists(filePath))
            //{
            //    var fileBytes = File.ReadAllBytes(filePath);
            //    imagePath = await _fileStorage.SaveFileAsync(fileBytes, "jpg", "tournaments");
            //}
                       

            if (File.Exists(filePath))
            {
                var fileBytes = File.ReadAllBytes(filePath);
                var stream = new MemoryStream(fileBytes);
                var file = $"{name}.jpg";
                var folder = "wwwroot\\images\\tournaments";
                var fullPath = $"~/images/tournaments/{file}";
                var response = _filesHelper.UploadPhoto(stream, folder, file);

                if (response)
                {
                    imagePath = fullPath;
                }
                else
                {
                    imagePath = string.Empty;
                }
            }

            var copaAmerica = new Tournament
            {
                IsActive = true,
                Name = name,
                Image = imagePath,
                TournamentTeams =
                [
                    new TournamentTeam { Team = colombia! },
                new TournamentTeam { Team = peru! },
                new TournamentTeam { Team = ecuador! },
                new TournamentTeam { Team = venezuela! },
                new TournamentTeam { Team = brazil! },
                new TournamentTeam { Team = argentina! },
                new TournamentTeam { Team = uruguay! },
                new TournamentTeam { Team = chile! },
                new TournamentTeam { Team = bolivia! },
                new TournamentTeam { Team = paraguay! },
                new TournamentTeam { Team = unitedStates! },
                new TournamentTeam { Team = canada! },
            ],
                Matches =
                [
                    new Match { Date = DateTime.Today.AddDays(1).AddHours(18).ToUniversalTime(), IsActive = true, Local = colombia!, Visitor = peru! },
                new Match { Date = DateTime.Today.AddDays(1).AddHours(21).ToUniversalTime(), IsActive = true, Local = ecuador!, Visitor = canada! },
                new Match { Date = DateTime.Today.AddDays(2).AddHours(18).ToUniversalTime(), IsActive = true, Local = brazil!, Visitor = chile! },
                new Match { Date = DateTime.Today.AddDays(2).AddHours(21).ToUniversalTime(), IsActive = true, Local = bolivia!, Visitor = uruguay! },
                new Match { Date = DateTime.Today.AddDays(3).AddHours(18).ToUniversalTime(), IsActive = true, Local = argentina!, Visitor = unitedStates! },
                new Match { Date = DateTime.Today.AddDays(3).AddHours(21).ToUniversalTime(), IsActive = true, Local = venezuela!, Visitor = paraguay! },

                new Match { Date = DateTime.Today.AddDays(4).AddHours(18).ToUniversalTime(), IsActive = true, Local = canada!, Visitor = colombia! },
                new Match { Date = DateTime.Today.AddDays(4).AddHours(21).ToUniversalTime(), IsActive = true, Local = peru!, Visitor = ecuador! },
                new Match { Date = DateTime.Today.AddDays(5).AddHours(18).ToUniversalTime(), IsActive = true, Local = uruguay!, Visitor = chile! },
                new Match { Date = DateTime.Today.AddDays(5).AddHours(21).ToUniversalTime(), IsActive = true, Local = chile!, Visitor = bolivia! },
                new Match { Date = DateTime.Today.AddDays(6).AddHours(18).ToUniversalTime(), IsActive = true, Local = argentina!, Visitor = paraguay! },
                new Match { Date = DateTime.Today.AddDays(6).AddHours(21).ToUniversalTime(), IsActive = true, Local = unitedStates!, Visitor = venezuela! },

                new Match { Date = DateTime.Today.AddDays(7).AddHours(19).ToUniversalTime(), IsActive = true, Local = peru!, Visitor = canada! },
                new Match { Date = DateTime.Today.AddDays(7).AddHours(19).ToUniversalTime(), IsActive = true, Local = colombia!, Visitor = ecuador! },
                new Match { Date = DateTime.Today.AddDays(8).AddHours(19).ToUniversalTime(), IsActive = true, Local = chile!, Visitor = uruguay! },
                new Match { Date = DateTime.Today.AddDays(8).AddHours(19).ToUniversalTime(), IsActive = true, Local = bolivia!, Visitor = brazil! },
                new Match { Date = DateTime.Today.AddDays(9).AddHours(19).ToUniversalTime(), IsActive = true, Local = unitedStates!, Visitor = paraguay! },
                new Match { Date = DateTime.Today.AddDays(9).AddHours(19).ToUniversalTime(), IsActive = true, Local = argentina!, Visitor = venezuela! },
            ]
            };

            name = "Copa Oro - 2025";
            imagePath = string.Empty;
            filePath = $"{Environment.CurrentDirectory}\\Images\\Tournaments\\{name}.png";
            //if (File.Exists(filePath))
            //{
            //    var fileBytes = File.ReadAllBytes(filePath);
            //    imagePath = await _fileStorage.SaveFileAsync(fileBytes, "jpg", "tournaments");
            //}

            if (File.Exists(filePath))
            {
                var fileBytes = File.ReadAllBytes(filePath);
                var stream = new MemoryStream(fileBytes);
                var file = $"{name}.jpg";
                var folder = "wwwroot\\images\\tournaments";
                var fullPath = $"~/images/tournaments/{file}";
                var response = _filesHelper.UploadPhoto(stream, folder, file);

                if (response)
                {
                    imagePath = fullPath;
                }
                else
                {
                    imagePath = string.Empty;
                }
            }

            var copaOro = new Tournament
            {
                IsActive = true,
                Name = name,
                Image = imagePath,
                TournamentTeams =
                [
                    new TournamentTeam { Team = unitedStates! },
                new TournamentTeam { Team = canada! },
                new TournamentTeam { Team = mexico! },
                new TournamentTeam { Team = panama! },
                new TournamentTeam { Team = costaRica! },
                new TournamentTeam { Team = honduras! },
                new TournamentTeam { Team = jamaica! },
                new TournamentTeam { Team = guatemala! },
                new TournamentTeam { Team = barbados! },
                new TournamentTeam { Team = dominica! },
                new TournamentTeam { Team = colombia! },
                new TournamentTeam { Team = uruguay! },
            ]
            };

            _context.Tournaments.Add(copaAmerica);
            _context.Tournaments.Add(copaOro);
            await _context.SaveChangesAsync();
        }
    }

    //------------------------------------------------------------------------------------------------------
    private async Task CheckRolesAsync()
    {
        await _usersUnitOfWork.CheckRoleAsync(UserType.Admin.ToString());
        await _usersUnitOfWork.CheckRoleAsync(UserType.User.ToString());
    }

        private async Task CheckCountriesAsync()
    {
        if (!_context.Countries.Any())
        {
            var countriesSQLScript = File.ReadAllText("Data\\Countries.sql");
            await _context.Database.ExecuteSqlRawAsync(countriesSQLScript);
        }
    }

    //------------------------------------------------------------------------------------------------------
    private async Task CheckTeamsAsync()
    {
        if (!_context.Teams.Any())
        {
            foreach (var country in _context.Countries)
            {
                if (country.Name == "Argentina")
                {
                    _context.Teams.Add(new Team { Name = "Talleres", Country = country! });
                    _context.Teams.Add(new Team { Name = "Belgrano", Country = country! });
                    _context.Teams.Add(new Team { Name = "Instituto", Country = country! });
                    _context.Teams.Add(new Team { Name = "River Plate", Country = country! });
                    _context.Teams.Add(new Team { Name = "Boca Juniors", Country = country! });
                }



                var imagePath = string.Empty;
                var filePath = $"{Environment.CurrentDirectory}\\Images\\Flags\\{country.Name}.png";

                if (File.Exists(filePath))
                {
                    var fileBytes = File.ReadAllBytes(filePath);
                    var stream = new MemoryStream(fileBytes);
                    var file = $"{country.Name}.jpg";
                    var folder = "wwwroot\\images\\teams";
                    var fullPath = $"~/images/teams/{file}";
                    var response = _filesHelper.UploadPhoto(stream, folder, file);

                    if (response)
                    {
                        imagePath = fullPath;
                    }
                    else
                    {
                        imagePath = string.Empty;
                    }
                }
                _context.Teams.Add(new Team { Name = country.Name, Country = country!, Image = imagePath });
            }

            await _context.SaveChangesAsync();
        }
    }

    //------------------------------------------------------------------------------------------------------
    private async Task CheckGroupsAsync()
    {
        if (!_context.Groups.Any())
        {
            var zulu = await _context.Users.FirstOrDefaultAsync(x => x.UserName == "zulu@yopmail.com");
            var ledys = await _context.Users.FirstOrDefaultAsync(x => x.UserName == "ledys@yopmail.com");
            var brad = await _context.Users.FirstOrDefaultAsync(x => x.UserName == "brad@yopmail.com");
            var angelina = await _context.Users.FirstOrDefaultAsync(x => x.UserName == "angelina@yopmail.com");
            var bob = await _context.Users.FirstOrDefaultAsync(x => x.UserName == "bob@yopmail.com");
            var celia = await _context.Users.FirstOrDefaultAsync(x => x.UserName == "celia@yopmail.com");
            var fredy = await _context.Users.FirstOrDefaultAsync(x => x.UserName == "fredy@yopmail.com");
            var hector = await _context.Users.FirstOrDefaultAsync(x => x.UserName == "hector@yopmail.com");
            var liv = await _context.Users.FirstOrDefaultAsync(x => x.UserName == "liv@yopmail.com");
            var otep = await _context.Users.FirstOrDefaultAsync(x => x.UserName == "otep@yopmail.com");
            var ozzy = await _context.Users.FirstOrDefaultAsync(x => x.UserName == "ozzy@yopmail.com");
            var selena = await _context.Users.FirstOrDefaultAsync(x => x.UserName == "selena@yopmail.com");

            var luis = await _context.Users.FirstOrDefaultAsync(x => x.UserName == "luis@yopmail.com");
            var pablo = await _context.Users.FirstOrDefaultAsync(x => x.UserName == "pablo@yopmail.com");
            var messi = await _context.Users.FirstOrDefaultAsync(x => x.UserName == "messi@yopmail.com");
            var maradona = await _context.Users.FirstOrDefaultAsync(x => x.UserName == "maradona@yopmail.com");
            var batistuta = await _context.Users.FirstOrDefaultAsync(x => x.UserName == "batistuta@yopmail.com");
            var kempes = await _context.Users.FirstOrDefaultAsync(x => x.UserName == "kempes@yopmail.com");
            var federer = await _context.Users.FirstOrDefaultAsync(x => x.UserName == "federer@yopmail.com");
            var nadal = await _context.Users.FirstOrDefaultAsync(x => x.UserName == "nadal@yopmail.com");

            var copaAmerica = await _context.Tournaments.FirstOrDefaultAsync(x => x.Name == "Copa América - 2025");

            var zuluGroup = new Group
            {
                Admin = zulu!,
                Code = Guid.NewGuid().ToString().Substring(0, 6).ToUpper(),
                Name = "Gupo Zulu Copa America",
                Remarks = "Valor COP$50,000. Primer puesto 70% del premio, segundo puesto 30% del premio",
                Tournament = copaAmerica!,
                Image = copaAmerica?.Image,
                IsActive = true,
                Members =
                [
                    new UserGroup { IsActive = true, User = zulu! },
                new UserGroup { IsActive = true, User = ledys! },
                new UserGroup { IsActive = true, User = brad! },
                new UserGroup { IsActive = true, User = angelina! },
                new UserGroup { IsActive = true, User = bob! },
                new UserGroup { IsActive = true, User = celia! },
                new UserGroup { IsActive = true, User = fredy! },
                new UserGroup { IsActive = true, User = selena! },
            ],
            };
            _context.Add(zuluGroup);

            var selenaGroup = new Group
            {
                Admin = selena!,
                Code = Guid.NewGuid().ToString().Substring(0, 6).ToUpper(),
                Name = "Gupo Selena Copa America",
                Remarks = "Valor USD$30.00. Primer puesto 80% del premio, segundo puesto 20% del premio",
                Tournament = copaAmerica!,
                Image = copaAmerica?.Image,
                IsActive = true,
                Members =
                [
                    new UserGroup { IsActive = true, User = zulu! },
                new UserGroup { IsActive = true, User = celia! },
                new UserGroup { IsActive = true, User = fredy! },
                new UserGroup { IsActive = true, User = hector! },
                new UserGroup { IsActive = true, User = liv! },
                new UserGroup { IsActive = true, User = otep! },
                new UserGroup { IsActive = true, User = ozzy! },
                new UserGroup { IsActive = true, User = selena! },
            ],
            };
            _context.Add(selenaGroup);

            var luisGroup = new Group
            {
                Admin = luis!,
                Code = Guid.NewGuid().ToString().Substring(0, 6).ToUpper(),
                Name = "Gupo Luis Copa America",
                Remarks = "Valor COP$150,000. Primer puesto 80% del premio, segundo puesto 20% del premio",
                Tournament = copaAmerica!,
                Image = copaAmerica?.Image,
                IsActive = true,
                Members =
                [
                    new UserGroup { IsActive = true, User = luis! },
                new UserGroup { IsActive = true, User = pablo! },
                new UserGroup { IsActive = true, User = messi! },
                new UserGroup { IsActive = true, User = maradona! },
                new UserGroup { IsActive = true, User = batistuta! },
                new UserGroup { IsActive = true, User = kempes! },
                new UserGroup { IsActive = true, User = federer! },
                new UserGroup { IsActive = true, User = nadal! },
            ],
            };
            _context.Add(luisGroup);

            await _context.SaveChangesAsync();
        }
    }
    
    //------------------------------------------------------------------------------------------------------
    private async Task CheckPredictionsAsync()
    {
        if (!_context.Predictions.Any())
        {
            var random = new Random();
            var predictions = new List<Prediction>();
            var groups = await _context.Groups
                .Include(x => x.Tournament)
                .ThenInclude(x => x.Matches)
                .Include(x => x.Members)
                .ToListAsync();
            foreach (var group in groups)
            {
                foreach (var match in group.Tournament.Matches!)
                {
                    foreach (var member in group.Members!)
                    {
                        predictions.Add(new Prediction
                        {
                            GoalsLocal = random.Next(4),
                            GoalsVisitor = random.Next(4),
                            Group = group,
                            Match = match,
                            Tournament = group.Tournament,
                            User = member.User,
                        });
                    }
                }
            }
            _context.AddRange(predictions);
            await _context.SaveChangesAsync();
        }
    }


}
