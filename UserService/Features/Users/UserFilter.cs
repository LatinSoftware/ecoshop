using System.Linq.Expressions;
using FluentResults;
using MediatR;
using UserService.Abstractions;
using UserService.Database;
using UserService.Entities;
using UserService.Extensions;
using UserService.Models;
using UserService.Shared;

namespace UserService.Features.Users;

public sealed class UserFilter
{
    public sealed class Query : IQuery<PaginationResponse<UserModel>>
    {
        public string? Name { get; set; } = string.Empty;
        public string? Email { get; set; } = string.Empty;
        public string? City { get; set; } = string.Empty;
        public string? Country { get; set; } = string.Empty;
        public string? SortBy { get; set; } = string.Empty;
        public string? SortOrder { get; set; } = string.Empty;
        public int Page { get; set; } = 1;
        public int PageSize { get; set; } = 10;
    }

    public sealed class Handler(ApplicationContext context) : IQueryHandler<Query, PaginationResponse<UserModel>>
    {
        public async Task<Result<PaginationResponse<UserModel>>> Handle(Query request, CancellationToken cancellationToken)
        {
            var query = context.Users.AsQueryable();

            if (!string.IsNullOrEmpty(request.Name))
                query = query.Where(u => u.Name.ToLower().Contains(request.Name.ToLower()));

            if (!string.IsNullOrEmpty(request.Email))
                query = query.Where(u => u.Email.Equals(new Email(request.Email)));

            if (!string.IsNullOrEmpty(request.City))
                query = query.Where(u => u.Address.City.ToLower().Contains(request.City.ToLower()));

            if (!string.IsNullOrEmpty(request.Country))
                query = query.Where(u => u.Address.Country.ToLower().Contains(request.Country.ToLower()));



            if (request.SortOrder?.ToLower() == "desc")
            {
                query = query.OrderByDescending(GetSortProperty(request));
            }
            else
            {
                query = query.OrderBy(GetSortProperty(request));
            }

            var userResponseQuery = query.Select(u => new UserModel
            {
                Id = u.Id.Value,
                Name = u.Name,
                Email = u.Email.Value,
                Role = u.Role,
                Address = new Address(u.Address.Street, u.Address.Sector, u.Address.City, u.Address.Country)
            });

            Func<int, string> generatePageUrl = page => $"/api/users?page={page}&pageSize={request.PageSize}";

            var users = await PaginationResponse<UserModel>.CreateAsync(userResponseQuery, request.Page, request.PageSize, generatePageUrl);

            return users;
        }

        private static Expression<Func<User, object>> GetSortProperty(Query request)
        {
            return request.SortBy?.ToLower() switch
            {
                "name" => user => user.Name,
                "email" => user => user.Email.Value,
                "city" => user => user.Address.City,
                "country" => user => user.Address.Country,
                _ => user => user.CreatedAt
            };
        }
    }

    public sealed class Endpoint : IEndpoint
    {
        public void MapEndpoint(IEndpointRouteBuilder app)
        {
            app.MapGet("users", async ([AsParameters]Query query, ISender sender) =>
            {
                var result = await sender.Send(query);
                return Results.Ok(result.ToApiResponse());
            });
        }
    }
}
