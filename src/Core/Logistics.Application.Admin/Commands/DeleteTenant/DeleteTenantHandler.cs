﻿using Logistics.Application.Core;
using Logistics.Domain.Persistence;
using Logistics.Domain.Services;
using Logistics.Shared;

namespace Logistics.Application.Admin.Commands;

internal sealed class DeleteTenantHandler : RequestHandler<DeleteTenantCommand, ResponseResult>
{
    private readonly ITenantDatabaseService _tenantDatabase;
    private readonly IMainRepository _repository;

    public DeleteTenantHandler(
        ITenantDatabaseService tenantDatabase,
        IMainRepository repository)
    {
        _tenantDatabase = tenantDatabase;
        _repository = repository;
    }

    protected override async Task<ResponseResult> HandleValidated(DeleteTenantCommand req, CancellationToken cancellationToken)
    {
        var tenant = await _repository.GetAsync<Domain.Entities.Tenant>(req.Id!);

        if (tenant == null)
            return ResponseResult.CreateError("Could not find the tenant");

        var deleted = await _tenantDatabase.DeleteDatabaseAsync(tenant.ConnectionString!);

        if (!deleted)
            return ResponseResult.CreateError("Could not delete the tenant's database");

        _repository.Delete(tenant);
        await _repository.UnitOfWork.CommitAsync();
        return ResponseResult.CreateSuccess();
    }
}
