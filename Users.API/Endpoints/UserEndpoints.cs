using Microsoft.AspNetCore.Mvc;
using Users.API.DTOs;
using Users.API.Profiles;
using Users.Domain.Enums;
using Users.Domain.Repositories;
using Users.Domain.Services;
using Users.Infra.CrossCutting.Security;

namespace Users.API.Endpoints
{
    public static class UserEndpoints
    {
        public static void MapUserEndpoints(this IEndpointRouteBuilder routes)
        {
            var group = routes.MapGroup("/api/users").WithTags("Users");

            group.MapPost("/create", async (CreateUserRequest dto, Mapper _mapper, IUserRepository _repository, CancellationToken cancellationToken) =>
            {
                if (await _repository.GetAsync(dto.Email, cancellationToken) is not null) return Results.BadRequest("The e-mail inserted has already been registered.");

                var user = _mapper.Map(dto);

                await _repository.AddAsync(user);

                //Aqui preciso implementar a adição do usuário num service, pra poder fazer transação e publicar o evento junto
                //await publishEndpoint.Publish(new UserCreatedEvent(user.Id, user.Email, user.Name, user.RoleId));

                var response = _mapper.Map(user);

                return Results.Created($"/api/users/{response.Id}", response);
            });


            group.MapPost("/auth", async (LoginRequest dto, Mapper _mapper, UserService _userService, JwtService _jwtService, CancellationToken cancellationToken) =>
            {
                try
                {
                    var user = await _userService.Auth(dto.Email, dto.Password, cancellationToken);

                    var token = _jwtService.GenerateToken(user.Email, user.Name, user.Role!.Name);

                    return Results.Ok(new LoginResponse { Email = user.Email, Token = token });
                }
                catch (UnauthorizedAccessException) { return Results.Unauthorized(); }
                catch (Exception ex) { return Results.BadRequest(new ProblemDetails { Detail = ex.Message }); }
            });


            group.MapGet("/", async (Mapper _mapper, IUserRepository _repository, CancellationToken cancellationToken) =>
            {
                var users = await _repository.GetAsync(cancellationToken);

                var response = _mapper.Map(users);

                return Results.Ok(response);
            })
            .RequireAuthorization(nameof(Policy.Admin));

            group.MapGet("/{id:int}", async (int id, Mapper _mapper, IUserRepository _repository, CancellationToken cancellationToken) =>
            {
                var user = await _repository.GetAsync(id);

                if (user == null) return Results.NotFound();

                var response = _mapper.Map(user);

                return Results.Ok(response);
            })
            .RequireAuthorization(nameof(Policy.Admin));

            group.MapGet("/{email}", async (string email, Mapper _mapper, IUserRepository _repository, CancellationToken cancellationToken) =>
            {
                var user = await _repository.GetAsync(email);

                if (user == null) return Results.NotFound();

                var response = _mapper.Map(user);

                return Results.Ok(response);
            })
            .RequireAuthorization(nameof(Policy.Admin));

            group.MapPut("/{id:int}", async (int id, UpdateUserRequest dto, Mapper _mapper, IUserRepository _repository, PasswordService _passwordService, CancellationToken cancellationToken) =>
            {
                var user = await _repository.GetAsync(id);

                if (user == null) return Results.NotFound();

                if (!string.IsNullOrEmpty(dto.Email))
                {
                    var userEmailRegistered = await _repository.GetAsync(dto.Email, cancellationToken);

                    if (userEmailRegistered != null && userEmailRegistered.Id != dto.Id) return Results.Conflict(new { Mensagem = "The e-mail inserted has already been registered." });
                }

                var passwordHash = dto.Password is null ? null : _passwordService.CreateHash(dto.Password);

                user.Update(dto.Name, dto.Email, passwordHash, dto.RoleId);

                await _repository.UpdateAsync(user, cancellationToken);

                return Results.NoContent();
            })
            .RequireAuthorization(nameof(Policy.Admin));

            group.MapDelete("/{id}/deactivate", async (int id, IUserRepository _repository, CancellationToken cancellationToken) =>
            {
                var user = await _repository.GetAsync(id, cancellationToken);

                if (user == null) return Results.NotFound();

                await _repository.DeactivateAsync(id, cancellationToken);

                return Results.Ok(new { message = "The user was successfully deactiveted." });
            })
            .RequireAuthorization(nameof(Policy.Admin));
        }
    }
}
