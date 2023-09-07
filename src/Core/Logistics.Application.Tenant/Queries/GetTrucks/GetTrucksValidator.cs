﻿using FluentValidation;

namespace Logistics.Application.Tenant.Queries;

internal sealed class GetTrucksValidator : AbstractValidator<GetTrucksQuery>
{
    public GetTrucksValidator()
    {
        RuleFor(i => i.Page)
            .GreaterThanOrEqualTo(0);
        
        RuleFor(i => i.PageSize)
            .GreaterThanOrEqualTo(1);
    }
}
