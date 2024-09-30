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
        await CheckUserAsync("Luis", "Núñez", "luis@yopmail.com", "156 814 963", UserType.Admin);
    }

    private async Task CheckRolesAsync()
    {
        await _usersUnitOfWork.CheckRoleAsync(UserType.Admin.ToString());
        await _usersUnitOfWork.CheckRoleAsync(UserType.User.ToString());
    }

    private async Task<User> CheckUserAsync(string firstName, string lastName, string email, string phone, UserType userType)
    {
        var user = await _usersUnitOfWork.GetUserAsync(email);
        if (user == null)
        {
            var country = await _context.Countries.FirstOrDefaultAsync(x => x.Name == "Argentina");
            user = new User
            {
                FirstName = firstName,
                LastName = lastName,
                Email = email,
                UserName = email,
                PhoneNumber = phone,
                Country = country!,
                UserType = userType,
            };

            await _usersUnitOfWork.AddUserAsync(user, "123456");
            await _usersUnitOfWork.AddUserToRoleAsync(user, userType.ToString());

            var token = await _usersUnitOfWork.GenerateEmailConfirmationTokenAsync(user);
            await _usersUnitOfWork.ConfirmEmailAsync(user, token);
        }

        return user;
    }


    private async Task CheckCountriesAsync()
    {
        if (!_context.Countries.Any())
        {
            var countriesSQLScript = File.ReadAllText("Data\\Countries.sql");
            await _context.Database.ExecuteSqlRawAsync(countriesSQLScript);
        }
    }

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

}
