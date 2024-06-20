﻿using Auth.Database.Contexts;
using Crosscutting;
using Crosscutting.TransactionHandling;
using DiscordBot.Application.Interface;
using DiscordBot.Domain;
using Microsoft.EntityFrameworkCore.Storage;

namespace DiscordBot.Application.Implementation
{
    public class DiscordBotCommandImplementation : IDiscordBotCommandImplementation
    {
        private readonly IUnitOfWork<AuthDbContext> _UoW;
        private readonly IDiscordBotCommandRepository _command;
        public DiscordBotCommandImplementation(IUnitOfWork<AuthDbContext> uoW, IDiscordBotCommandRepository command)
        {
            _UoW = uoW;
            _command = command;
        }

        async Task<string> IDiscordBotCommandImplementation.GetStaffLicense(DiscordModelDto model)
        {
            try
            {
                await _UoW.CreateTransaction(System.Data.IsolationLevel.Serializable);
                var result = await _command.GetStaffLicense(model);
                await _UoW.Commit();
                return result;
            }
            catch (Exception ex)
            {
                await _UoW.Rollback();
                throw new Exception(ex.Message);
            }
        }

        async Task<string> IDiscordBotCommandImplementation.UpdateDiscordAndRole(DiscordModelDto model)
        {
            try
            {
                await _UoW.CreateTransaction(System.Data.IsolationLevel.Serializable);
                var result = await _command.UpdateDiscordAndRole(model);
                await _UoW.Commit();
                return result;
            }
            catch (Exception ex)
            {
                await _UoW.Rollback();
                throw new Exception(ex.Message);
            }
        }

        async Task<string> IDiscordBotCommandImplementation.UpdateHwid(DiscordModelDto model)
        {
            var rollback = false;
            try
            {
                rollback = await new HwidDomain().HwidChecker(model.Hwid);
                await _UoW.CreateTransaction(System.Data.IsolationLevel.Serializable);
                var result = await _command.UpdateHwid(model);
                await _UoW.Commit();
                return result;
            }
            catch (Exception ex)
            {
                if (rollback)
                {
                    await _UoW.Rollback();
                }

                throw new Exception(ex.Message);
            }
        }
    }
}
