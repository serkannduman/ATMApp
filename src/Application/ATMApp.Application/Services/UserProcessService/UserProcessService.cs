﻿using ATMApp.Application.Models.DTOs;
using ATMApp.Application.Services.MailService;
using ATMApp.Domain.Entities;
using ATMApp.Domain.Repositories;
using AutoMapper;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using ATMApp.Domain.Enums;
using ATMApp.Domain.StaticClass;

namespace ATMApp.Application.Services.UserProcessService
{
    public class UserProcessService : IUserProcessService
    {

        private readonly IMapper _mapper;
        private readonly IUserRepo _userRepo;
        private readonly IMailService _mailService;
        private readonly IUserProcessRepo _userProcessRepo;

        public UserProcessService(IMapper mapper, IUserRepo userRepo, IMailService mailService, IUserProcessRepo userProcessRepo)
        {
            _mapper = mapper;
            _userRepo = userRepo;
            _mailService = mailService;
            _userProcessRepo = userProcessRepo;
        }

        public async Task<UserProcess> AddUserProcess(ProcessDTO model, int userID)
        {
            var user = await _userRepo.GetDefault(x => x.Id == userID);

            StaticMailModel.successMail.ToEmail = user.EmailAddress;
            StaticMailModel.failedMail.ToEmail = user.EmailAddress;

            UserProcess newUserProcess = new UserProcess();
            newUserProcess.Process = model.Process;
            newUserProcess.Amount = model.Amout;

            var userBalance = await GetUserBalance(userID);

            if(newUserProcess.Process == Process.Withdraw)
            {
                if (userBalance < model.Amout)
                {
                    await _mailService.SendEmail(StaticMailModel.failedMail);
                    return null;
                }
                else
                {
                    await _mailService.SendEmail(StaticMailModel.successMail);
                }     
            }
            else
            {
                await _mailService.SendEmail(StaticMailModel.successMail);
            }

            user.UserProcesses.Add(newUserProcess);
            await _userRepo.Update(user);
            return newUserProcess;

        }

        public async Task<decimal> GetUserBalance(int userId)
        {
            //var user = _userRepo.GetDefault(x => x.Id == userId);
            var userBalanceList = await _userProcessRepo.GetDefaults(x => x.UserId == userId);

            decimal userBalance = 0;
            foreach (var userProcess in userBalanceList)
            {
                if (userProcess.Process == Process.Deposit)
                    userBalance += userProcess.Amount;
                else
                    userBalance -= userProcess.Amount;
            }

            return userBalance;
        }
    }
}
