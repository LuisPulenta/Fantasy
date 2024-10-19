using Fantasy.Backend.Data;
using Fantasy.Backend.Helpers;
using Fantasy.Backend.Repositories.Interfaces;
using Fantasy.Shared.DTOs;
using Fantasy.Shared.Entities;
using Fantasy.Shared.Helpers;
using Fantasy.Shared.Responses;
using Microsoft.EntityFrameworkCore;

namespace Fantasy.Backend.Repositories.Implementations;

public class TournamentsRepository : GenericRepository<Tournament>, ITournamentsRepository
{
    private readonly DataContext _context;
    private readonly IFilesHelper _filesHelper;

    public TournamentsRepository(DataContext context, IFilesHelper filesHelper) : base(context)
    {
        _context = context;
        _filesHelper = filesHelper;
    }

    public async Task<ActionResponse<Tournament>> AddAsync(TournamentDTO tournamentDTO)
    {
        var tournament = new Tournament
        {
            IsActive = false,
            Name = tournamentDTO.Name,
            Remarks = tournamentDTO.Remarks,
            TournamentTeams = new List<TournamentTeam>()
        };

        //Foto

        if (tournamentDTO.Image != null)
        {
            byte[] imageArray = Convert.FromBase64String(tournamentDTO.Image!);
            var stream = new MemoryStream(imageArray);
            var guid = Guid.NewGuid().ToString();
            var file = $"{guid}.jpg";
            var folder = "wwwroot\\images\\tournaments";
            var fullPath = $"~/images/tournaments/{file}";
            var response = _filesHelper.UploadPhoto(stream, folder, file);

            if (response)
            {
                tournament.Image = fullPath;
            }
            else
            {
                tournament.Image = string.Empty;
            }
        }
        else
        {
            tournament.Image = string.Empty;
        }

        _context.Add(tournament);
        try
        {
            await _context.SaveChangesAsync();
            return new ActionResponse<Tournament>
            {
                WasSuccess = true,
                Result = tournament
            };
        }
        catch (DbUpdateException)
        {
            return new ActionResponse<Tournament>
            {
                WasSuccess = false,
                Message = "ERR003"
            };
        }
        catch (Exception exception)
        {
            return new ActionResponse<Tournament>
            {
                WasSuccess = false,
                Message = exception.Message
            };
        }
    }

    public async Task<IEnumerable<Tournament>> GetComboAsync()
    {
        return await _context.Tournaments
            .Where(x => x.IsActive)
            .OrderBy(x => x.Name)
            .ToListAsync();
    }

    public override async Task<ActionResponse<IEnumerable<Tournament>>> GetAsync(PaginationDTO pagination)
    {
        var queryable = _context.Tournaments
            .Include(x => x.Matches!)
            .Include(x => x.TournamentTeams!)
            .ThenInclude(x => x.Team)
            .AsQueryable();

        if (!string.IsNullOrWhiteSpace(pagination.Filter))
        {
            queryable = queryable.Where(x => x.Name.ToLower().Contains(pagination.Filter.ToLower()));
        }

        return new ActionResponse<IEnumerable<Tournament>>
        {
            WasSuccess = true,
            Result = await queryable
                .OrderBy(x => x.Name)
                .Paginate(pagination)
                .ToListAsync()
        };
    }

    public override async Task<ActionResponse<Tournament>> GetAsync(int id)
    {
        var tournament = await _context.Tournaments
             .Include(x => x.TournamentTeams!)
             .ThenInclude(x => x.Team)
             .FirstOrDefaultAsync(c => c.Id == id);

        if (tournament == null)
        {
            return new ActionResponse<Tournament>
            {
                WasSuccess = false,
                Message = "ERR001"
            };
        }

        return new ActionResponse<Tournament>
        {
            WasSuccess = true,
            Result = tournament
        };
    }

    public async Task<ActionResponse<int>> GetTotalRecordsAsync(PaginationDTO pagination)
    {
        var queryable = _context.Tournaments.AsQueryable();

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

    public async Task<ActionResponse<Tournament>> UpdateAsync(TournamentDTO tournamentDTO)
    {
        var currentTournament = await _context.Tournaments.FindAsync(tournamentDTO.Id);
        if (currentTournament == null)
        {
            return new ActionResponse<Tournament>
            {
                WasSuccess = false,
                Message = "ERR005"
            };
        }

        //Foto

        if (tournamentDTO.Image != null)
        {
            byte[] imageArray = Convert.FromBase64String(tournamentDTO.Image!);
            var stream = new MemoryStream(imageArray);
            var guid = Guid.NewGuid().ToString();
            var file = $"{guid}.jpg";
            var folder = "wwwroot\\images\\tournaments";
            var fullPath = $"~/images/tournaments/{file}";
            var response = _filesHelper.UploadPhoto(stream, folder, file);

            if (response)
            {
                currentTournament.Image = fullPath;
            }
            else
            {
                currentTournament.Image = string.Empty;
            }
        }
        else
        {
            currentTournament.Image = string.Empty;
        }

        currentTournament.Name = tournamentDTO.Name;
        currentTournament.IsActive = tournamentDTO.IsActive;
        currentTournament.Remarks = tournamentDTO.Remarks;

        _context.Update(currentTournament);
        try
        {
            await _context.SaveChangesAsync();
            return new ActionResponse<Tournament>
            {
                WasSuccess = true,
                Result = currentTournament
            };
        }
        catch (DbUpdateException)
        {
            return new ActionResponse<Tournament>
            {
                WasSuccess = false,
                Message = "ERR003"
            };
        }
        catch (Exception exception)
        {
            return new ActionResponse<Tournament>
            {
                WasSuccess = false,
                Message = exception.Message
            };
        }
    }
}

