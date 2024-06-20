﻿using Admin.Application.Interface.ExtendLicense;
using Auth.Database.Contexts;
using Crosscutting.TransactionHandling;
using Microsoft.Extensions.Logging;
using System.Data;

namespace Admin.Application.Implementation.ExtendLicense;

public class AdminExtendLicensesImplementation : IAdminExtendLicensesImplementation
{
    private readonly ILogger<AdminExtendLicensesImplementation> _logger;
    private readonly IUnitOfWork<AuthDbContext> _UoW;
    private readonly IAdminExtendLicensesRepository _admin;

    public AdminExtendLicensesImplementation(IUnitOfWork<AuthDbContext> uoW, IAdminExtendLicensesRepository admin, ILogger<AdminExtendLicensesImplementation> logger)
    {
        _UoW = uoW;
        _admin = admin;
        _logger = logger;
    }

    async Task<string> IAdminExtendLicensesImplementation.ExtendLicense(int minutesToExtend, string? discordId)
    {
        try
        {
            await _UoW.CreateTransaction(IsolationLevel.Serializable);
            var result = await _admin.ExtendLicense(minutesToExtend, discordId);
            await _UoW.Commit();
            return result;
        }
        catch (Exception ex)
        {
            _logger.LogError(ex.Message);
            await _UoW.Rollback();
            return ex.Message;
        }
    }
}