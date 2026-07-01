using Microsoft.AspNetCore.Mvc;
using Users.API.DTOs;
using Users.API.Profiles;
using Users.Domain.Enums;
using Users.Domain.Repositories;
using Users.Domain.Services;

namespace Users.API.Endpoints
{
    public static class UserEndpoints
    {
        public static void MapUserEndpoints(this IEndpointRouteBuilder routes)
        {
            var group = routes.MapGroup("/api/users").WithTags("Users");

            group.MapPost("/auth", async (LoginRequest dto, UserService _userService, CancellationToken cancellationToken) =>
            {
                try
                {
                    var token = await _userService.Auth(dto.Email, dto.Password, cancellationToken);

                    return Results.Ok(new LoginResponse { Email = dto.Email, Token = token });
                }
                catch (UnauthorizedAccessException) { return Results.Unauthorized(); }
                catch (Exception ex) { return Results.BadRequest(new ProblemDetails { Detail = ex.Message }); }
            });

            group.MapPost("/create", async (CreateUserRequest dto, Mapper _mapper, IUserRepository _repository, UserService _service, CancellationToken cancellationToken) =>
            {
                if (await _repository.GetAsync(dto.Email, cancellationToken) is not null) return Results.BadRequest("The e-mail inserted has already been registered.");

                var user = _mapper.Map(dto);

                await _service.Add(user, cancellationToken);

                var userCreated = await _repository.GetAsync(user.Id, cancellationToken);

                var response = _mapper.Map(userCreated!);

                return Results.Created($"/api/users/{response.Id}", response);
            })
            .RequireAuthorization(nameof(Policy.Admin));

            group.MapGet("/", async (Mapper _mapper, IUserRepository _repository, CancellationToken cancellationToken) =>
            {
                var users = await _repository.GetAsync(cancellationToken);

                var response = _mapper.Map(users);

                return Results.Ok(response);
            })
            .RequireAuthorization(nameof(Policy.Admin));

            group.MapGet("/{id:int}", async (int id, Mapper _mapper, IUserRepository _repository, CancellationToken cancellationToken) =>
            {
                var user = await _repository.GetAsync(id, cancellationToken);

                if (user == null) return Results.NotFound();

                var response = _mapper.Map(user);

                return Results.Ok(response);
            })
            .RequireAuthorization(nameof(Policy.Admin));

            group.MapGet("/{email}", async (string email, Mapper _mapper, IUserRepository _repository, CancellationToken cancellationToken) =>
            {
                var user = await _repository.GetAsync(email, cancellationToken);

                if (user == null) return Results.NotFound();

                var response = _mapper.Map(user);

                return Results.Ok(response);
            })
            .RequireAuthorization(nameof(Policy.Admin));
        }
    }
}
