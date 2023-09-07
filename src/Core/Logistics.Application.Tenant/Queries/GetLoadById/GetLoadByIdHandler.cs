﻿using Logistics.Application.Tenant.Mappers;
using Logistics.Models;

namespace Logistics.Application.Tenant.Queries;

internal sealed class GetLoadByIdHandler : RequestHandler<GetLoadByIdQuery, ResponseResult<LoadDto>>
{
    private readonly ITenantRepository _tenantRepository;

    public GetLoadByIdHandler(ITenantRepository tenantRepository)
    {
        _tenantRepository = tenantRepository;
    }

    protected override async Task<ResponseResult<LoadDto>> HandleValidated(
        GetLoadByIdQuery req, CancellationToken cancellationToken)
    {
        var loadEntity = await _tenantRepository.GetAsync<Load>(req.Id);

        if (loadEntity == null)
            return ResponseResult<LoadDto>.CreateError("Could not find the specified cargo");

        var assignedDispatcher = await _tenantRepository.GetAsync<Employee>(loadEntity.AssignedDispatcherId);

        var loadDto = loadEntity.ToDto();
        loadDto.AssignedDispatcherName = assignedDispatcher?.GetFullName();
        loadDto.AssignedTruckNumber = loadEntity.AssignedTruck?.TruckNumber;
        loadDto.AssignedTruckId = loadDto.AssignedTruckId;
        return ResponseResult<LoadDto>.CreateSuccess(loadDto);
    }
}
