﻿using System;
using System.Collections.Generic;
using System.Linq;
using Business.Repositories;
using DataAccess.Models;
using Legacy.Models;
using Legacy.Repositories;
using Legacy.Services;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using Purchases.Helpers;

namespace Purchases.Controllers
{
    [Route("api/[controller]")]
    [ApiController]
    public class LegacyGraphController : ControllerBase
    {
        // Accounts
        private readonly ILegacyMonthlyAccountStatusRepository _monthlyAccountStatusRepository;

        // Postings
        private readonly ISubCategoryRepository _subCategoryRepository;
        private readonly ILegacySummaryRepository _summaryRepository;
        private readonly ILegacyGraphRepository _graphRepository;

        // Mixed
        private readonly ILegacySumupService _sumupService;

        public LegacyGraphController(ILegacySumupService sumupService, ILegacySummaryRepository summaryRepository, ILegacyMonthlyAccountStatusRepository monthlyAccountStatusRepository, ILegacyGraphRepository graphRepository, ISubCategoryRepository subCategoryRepository)
        {
            _sumupService = sumupService;
            _summaryRepository = summaryRepository;
            _monthlyAccountStatusRepository = monthlyAccountStatusRepository;
            _graphRepository = graphRepository;
            _subCategoryRepository = subCategoryRepository;
        }

        [HttpGet("Sumup")]
        public ActionResult Sumup()
        {
            var userId = HttpContext.GetUserId();

            var sumup = _sumupService.Sumup(userId);

            return Ok(sumup);
        }

        [HttpGet("Summary")]
        public ActionResult Summary()
        {
            var userId = HttpContext.GetUserId();

            var subcategories = _subCategoryRepository.GetList(userId)
                .Select(sc => new { sc.Name, Subcategory_id = sc.Id, Category_id = sc.CategoryId } )
                .GroupBy(x => x.Category_id)
                .ToDictionary(x => x.Key);

            var (categories, summary) = _summaryRepository.Summary(userId);

            return Ok(new
            {
                categories,
                subcategories,
                summary
            });
        }

        [HttpGet("NumDailyPurchases")]
        public ActionResult NumDailyPurchases()
        {
            var userId = HttpContext.GetUserId();

            var dailyPurchases = _graphRepository.GetDailyPurchases(userId);

            return Ok(dailyPurchases);
        }

        [HttpGet("SumPerDay")]
        public ActionResult SumPerDay()
        {
            var userId = HttpContext.GetUserId();

            var sumPerDay = _graphRepository.GetMonthlyAverageDailyPurchases(userId);

            return Ok(sumPerDay);
        }

        [HttpGet("MonthlyAccountStatus")]
        public ActionResult Get()
        {
            var userId = HttpContext.GetUserId();

            var (categories, status) = _monthlyAccountStatusRepository.MonthlyAccountStatus(userId);

            return Ok(new
            {
                Categories = categories,
                status
            });
        }

        [HttpGet("MonthlyStatus")]
        public ActionResult Get(int year, int month)
        {
            var userId = HttpContext.GetUserId();

            var monthlyStatuses = _graphRepository.GetMonthlyStatus(userId, year, month);

            return Ok(monthlyStatuses);
        }
    }
}