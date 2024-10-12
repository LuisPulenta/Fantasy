using Fantasy.Backend.Data;
using Fantasy.Backend.Helpers;
using Fantasy.Backend.Repositories.Interfaces;
using Fantasy.Shared.DTOs;
using Fantasy.Shared.Entities;
using Fantasy.Shared.Helpers;
using Fantasy.Shared.Responses;
using Microsoft.EntityFrameworkCore;

namespace Fantasy.Backend.Repositories.Implementations;

public class TeamsRepository : GenericRepository<Team>, ITeamsRepository
{
    private readonly DataContext _context;
    private readonly IFilesHelper _filesHelper;

    public TeamsRepository(DataContext context, IFilesHelper filesHelper) : base(context)
    {
        _context = context;
        _filesHelper = filesHelper;
    }

    public async Task<ActionResponse<Team>> AddAsync(TeamDTO teamDTO)
    {
        var country = await _context.Countries.FindAsync(teamDTO.CountryId);
        if (country == null)
        {
            return new ActionResponse<Team>
            {
                WasSuccess = false,
                Message = "ERR004"
            };
        }

        var team = new Team
        {
            Country = country,
            Name = teamDTO.Name,
        };

        //Foto

        if (teamDTO.Image != null)
        {
            byte[] imageArray = Convert.FromBase64String(teamDTO.Image!);
            var stream = new MemoryStream(imageArray);
            var guid = Guid.NewGuid().ToString();
            var file = $"{guid}.jpg";
            var folder = "wwwroot\\images\\teams";
            var fullPath = $"~/images/teams/{file}";
            var response = _filesHelper.UploadPhoto(stream, folder, file);

            if (response)
            {
                team.Image = fullPath;
            }
            else
            {
                team.Image = string.Empty;
            }
        }
        else
        {
            team.Image = string.Empty;
        }

        _context.Add(team);
        try
        {
            await _context.SaveChangesAsync();
            return new ActionResponse<Team>
            {
                WasSuccess = true,
                Result = team
            };
        }
        catch (DbUpdateException)
        {
            return new ActionResponse<Team>
            {
                WasSuccess = false,
                Message = "ERR003"
            };
        }
        catch (Exception exception)
        {
            return new ActionResponse<Team>
            {
                WasSuccess = false,
                Message = exception.Message
            };
        }
    }

    public async Task<IEnumerable<Team>> GetComboAsync(int countryId)
    {
        return await _context.Teams
            .Where(x => x.CountryId == countryId)
            .OrderBy(x => x.Name)
            .ToListAsync();
    }

    public async Task<ActionResponse<Team>> UpdateAsync(TeamDTO teamDTO)
    {
        var currentTeam = await _context.Teams.FindAsync(teamDTO.Id);
        if (currentTeam == null)
        {
            return new ActionResponse<Team>
            {
                WasSuccess = false,
                Message = "ERR005"
            };
        }

        var country = await _context.Countries.FindAsync(teamDTO.CountryId);
        if (country == null)
        {
            return new ActionResponse<Team>
            {
                WasSuccess = false,
                Message = "ERR004"
            };
        }

        //Foto

        if (teamDTO.Image != null)
        {
            byte[] imageArray = Convert.FromBase64String(teamDTO.Image!);
            var stream = new MemoryStream(imageArray);
            var guid = Guid.NewGuid().ToString();
            var file = $"{guid}.jpg";
            var folder = "wwwroot\\images\\teams";
            var fullPath = $"~/images/teams/{file}";
            var response = _filesHelper.UploadPhoto(stream, folder, file);

            if (response)
            {
                currentTeam.Image = fullPath;
            }
            else
            {
                currentTeam.Image = string.Empty;
            }
        }

        currentTeam.Country = country;
        currentTeam.Name = teamDTO.Name;

        _context.Update(currentTeam);
        try
        {
            await _context.SaveChangesAsync();
            return new ActionResponse<Team>
            {
                WasSuccess = true,
                Result = currentTeam
            };
        }
        catch (DbUpdateException)
        {
            return new ActionResponse<Team>
            {
                WasSuccess = false,
                Message = "ERR003"
            };
        }
        catch (Exception exception)
        {
            return new ActionResponse<Team>
            {
                WasSuccess = false,
                Message = exception.Message
            };
        }
    }

    public override async Task<ActionResponse<IEnumerable<Team>>> GetAsync()
    {
        var teams = await _context.Teams
            .Include(x => x.Country)
            .OrderBy(x => x.Name)
            .ToListAsync();
        return new ActionResponse<IEnumerable<Team>>
        {
            WasSuccess = true,
            Result = teams
        };
    }

    public override async Task<ActionResponse<Team>> GetAsync(int id)
    {
        var team = await _context.Teams
             .Include(x => x.Country)
             .FirstOrDefaultAsync(c => c.Id == id);

        if (team == null)
        {
            return new ActionResponse<Team>
            {
                WasSuccess = false,
                Message = "ERR001"
            };
        }

        return new ActionResponse<Team>
        {
            WasSuccess = true,
            Result = team
        };
    }

    public override async Task<ActionResponse<IEnumerable<Team>>> GetAsync(PaginationDTO pagination)
    {
        var queryable = _context.Teams
            .Include(x => x.Country)
            .AsQueryable();

        if (!string.IsNullOrWhiteSpace(pagination.Filter))
        {
            queryable = queryable.Where(x => x.Name.ToLower().Contains(pagination.Filter.ToLower()));
        }

        return new ActionResponse<IEnumerable<Team>>
        {
            WasSuccess = true,
            Result = await queryable
                .OrderBy(x => x.Name)
                .Paginate(pagination)
                .ToListAsync()
        };
    }

    public async Task<ActionResponse<int>> GetTotalRecordsAsync(PaginationDTO pagination)
    {
        var queryable = _context.Teams.AsQueryable();

        if (!string.IsNullOrWhiteSpace(pagination.Filter))
        {
            queryable = queryable.Where(x => x.Name.ToLower().Contains(pagination.Filter.ToLower()));
        }

        double count = await queryable.CountAsync();
        return new ActionResponse<int>
        {
            WasSuccess = true,
            Result = (int)count
        };
    }

}
