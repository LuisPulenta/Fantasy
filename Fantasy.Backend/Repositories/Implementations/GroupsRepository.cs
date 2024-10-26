using Fantasy.Backend.Data;
using Fantasy.Backend.Helpers;
using Fantasy.Backend.Repositories.Interfaces;
using Fantasy.Shared.DTOs;
using Fantasy.Shared.Entities;
using Fantasy.Shared.Helpers;
using Fantasy.Shared.Responses;
using Microsoft.EntityFrameworkCore;

namespace Fantasy.Backend.Repositories.Implementations;

public class GroupsRepository : GenericRepository<Group>, IGroupsRepository
{
    private readonly DataContext _context;
    private readonly IFilesHelper _filesHelper;
    private readonly IUsersRepository _usersRepository;

    public GroupsRepository(DataContext context, IFilesHelper filesHelper, IUsersRepository usersRepository) : base(context)
    {
        _context = context;
        _filesHelper = filesHelper;
        _usersRepository = usersRepository;
    }

    //----------------------------------------------------------------------------------------------------
    public override async Task<ActionResponse<IEnumerable<Group>>> GetAsync(PaginationDTO pagination)
    {
        var queryable = _context.Groups
            .Include(x => x.Members!)
            .ThenInclude(x => x.User)
            .Include(x => x.Tournament)
            .AsQueryable();
        queryable = queryable.Where(x => x.Members!.Any(x => x.User.Email == pagination.Email));

        if (!string.IsNullOrWhiteSpace(pagination.Filter))
        {
            queryable = queryable.Where(x => x.Name.ToLower().Contains(pagination.Filter.ToLower()));
        }

        return new ActionResponse<IEnumerable<Group>>
        {
            WasSuccess = true,
            Result = await queryable
                .OrderBy(x => x.Name)
                .Paginate(pagination)
                .ToListAsync()
        };
    }

    //----------------------------------------------------------------------------------------------------
    public override async Task<ActionResponse<Group>> GetAsync(int id)
    {
        var group = await _context.Groups
            .Include(x => x.Members!)
            .ThenInclude(x => x.User)
            .Include(x => x.Tournament)
            .FirstOrDefaultAsync(c => c.Id == id);

        if (group == null)
        {
            return new ActionResponse<Group>
            {
                WasSuccess = false,
                Message = "ERR001"
            };
        }

        return new ActionResponse<Group>
        {
            WasSuccess = true,
            Result = group
        };
    }

    //----------------------------------------------------------------------------------------------------
    public async Task<ActionResponse<Group>> AddAsync(GroupDTO groupDTO)
    {
        var admin = await _usersRepository.GetUserAsync(groupDTO.AdminId);
        if (admin == null)
        {
            return new ActionResponse<Group>
            {
                WasSuccess = false,
                Message = "ERR013"
            };
        }

        var tournament = await _context.Tournaments.FindAsync(groupDTO.TournamentId);
        if (tournament == null)
        {
            return new ActionResponse<Group>
            {
                WasSuccess = false,
                Message = "ERR009"
            };
        }

        var code = string.Empty;
        var exists = true;
        do
        {
            code = Guid.NewGuid().ToString().Substring(0, 6).ToUpper();
            var currentGroup = await _context.Groups.FirstOrDefaultAsync(x => x.Code == code);
            exists = currentGroup != null;
        } while (exists);

        var group = new Group
        {
            Admin = admin,
            Tournament = tournament,
            Code = code,
            IsActive = true,
            Name = groupDTO.Name,
            Remarks = groupDTO.Remarks,
            Members = [
                new UserGroup { User = admin, IsActive = true },
            ]
        };

        //Foto

        if (groupDTO.Image != null)
        {
            byte[] imageArray = Convert.FromBase64String(groupDTO.Image!);
            var stream = new MemoryStream(imageArray);
            var guid = Guid.NewGuid().ToString();
            var file = $"{guid}.jpg";
            var folder = "wwwroot\\images\\groups";
            var fullPath = $"~/images/groups/{file}";
            var response = _filesHelper.UploadPhoto(stream, folder, file);

            if (response)
            {
                group.Image = fullPath;
            }
            else
            {
                group.Image = string.Empty;
            }
        }
        else
        {
            group.Image = string.Empty;
        }

        _context.Add(group);
        try
        {
            await _context.SaveChangesAsync();
            return new ActionResponse<Group>
            {
                WasSuccess = true,
                Result = group
            };
        }
        catch (DbUpdateException)
        {
            return new ActionResponse<Group>
            {
                WasSuccess = false,
                Message = "ERR003"
            };
        }
        catch (Exception exception)
        {
            return new ActionResponse<Group>
            {
                WasSuccess = false,
                Message = exception.Message
            };
        }
    }

    //----------------------------------------------------------------------------------------------------
    public async Task<ActionResponse<int>> GetTotalRecordsAsync(PaginationDTO pagination)
    {
        var queryable = _context.Groups.AsQueryable();
        queryable = queryable.Where(x => x.Members!.Any(x => x.User.Email == pagination.Email));

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

    //----------------------------------------------------------------------------------------------------
    public async Task<ActionResponse<Group>> UpdateAsync(GroupDTO groupDTO)
    {
        var currentGroup = await _context.Groups.FindAsync(groupDTO.Id);
        if (currentGroup == null)
        {
            return new ActionResponse<Group>
            {
                WasSuccess = false,
                Message = "ERR014"
            };
        }

        //Foto

        if (groupDTO.Image != null)
        {
            byte[] imageArray = Convert.FromBase64String(groupDTO.Image!);
            var stream = new MemoryStream(imageArray);
            var guid = Guid.NewGuid().ToString();
            var file = $"{guid}.jpg";
            var folder = "wwwroot\\images\\groups";
            var fullPath = $"~/images/groups/{file}";
            var response = _filesHelper.UploadPhoto(stream, folder, file);

            if (response)
            {
                currentGroup.Image = fullPath;
            }
            else
            {
                currentGroup.Image = string.Empty;
            }
        }

        currentGroup.Name = groupDTO.Name;
        currentGroup.IsActive = groupDTO.IsActive;
        currentGroup.Remarks = groupDTO.Remarks;

        _context.Update(currentGroup);
        try
        {
            await _context.SaveChangesAsync();
            return new ActionResponse<Group>
            {
                WasSuccess = true,
                Result = currentGroup
            };
        }
        catch (DbUpdateException)
        {
            return new ActionResponse<Group>
            {
                WasSuccess = false,
                Message = "ERR003"
            };
        }
        catch (Exception exception)
        {
            return new ActionResponse<Group>
            {
                WasSuccess = false,
                Message = exception.Message
            };
        }
    }

    //----------------------------------------------------------------------------------------------------
    public async Task<ActionResponse<Group>> GetAsync(string code)
    {
        var group = await _context.Groups.FirstOrDefaultAsync(x => x.Code == code);

        if (group == null)
        {
            return new ActionResponse<Group>
            {
                WasSuccess = false,
                Message = "ERR001"
            };
        }

        return new ActionResponse<Group>
        {
            WasSuccess = true,
            Result = group
        };
    }

    //----------------------------------------------------------------------------------------------------
    public async Task CheckPredictionsForAllMatchesAsync(int id)
    {
        var group = await _context.Groups
            .Include(x => x.Members)
            .FirstOrDefaultAsync(x => x.Id == id);
        if (group == null)
        {
            return;
        }

        var tournament = await _context.Tournaments
            .Include(x => x.Matches)
            .FirstOrDefaultAsync(x => x.Id == group.TournamentId);
        if (tournament == null)
        {
            return;
        }

        var newPredictions = new List<Prediction>();
        foreach (var userGroup in group.Members!)
        {
            foreach (var match in tournament!.Matches!)
            {
                var prediction = await _context.Predictions.FirstOrDefaultAsync(x => x.GroupId == group.Id &&
                                                                                     x.Match.Id == match.Id &&
                                                                                     x.UserId == userGroup.UserId &&
                                                                                     x.TournamentId == tournament.Id);
                if (prediction == null)
                {
                    newPredictions.Add(new Prediction
                    {
                        Group = group,
                        Match = match,
                        Tournament = tournament,
                        User = userGroup.User,
                        UserId = userGroup.UserId,
                    });
                }
            }
        }

        if (newPredictions.Count > 0)
        {
            _context.AddRange(newPredictions);
            await _context.SaveChangesAsync();
        }
    }

    //----------------------------------------------------------------------------------------------------
    public async Task<ActionResponse<IEnumerable<Group>>> GetAllAsync()
    {
        var groups = await _context.Groups
            .Include(x => x.Admin)
            .Include(x => x.Tournament)
            .Where(x => x.IsActive)
            .OrderBy(x => x.Name)
            .ToListAsync();

        return new ActionResponse<IEnumerable<Group>>
        {
            WasSuccess = true,
            Result = groups
        };
    }
}
