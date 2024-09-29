using Fantasy.Shared.Entities;
using Fantasy.Shared.Helpers;
using Microsoft.EntityFrameworkCore;

namespace Fantasy.Backend.Data;

public class SeedDb
{
    private readonly DataContext _context;
    private readonly IFilesHelper _filesHelper;

    public SeedDb(DataContext context,IFilesHelper filesHelper)
    {
        _context = context;
        _filesHelper = filesHelper;
    }

    public async Task SeedAsync()
    {
        await _context.Database.EnsureCreatedAsync();
        await CheckCountriesAsync();
        await CheckTeamsAsync();
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
