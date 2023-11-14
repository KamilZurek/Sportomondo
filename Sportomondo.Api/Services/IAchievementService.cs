﻿using Sportomondo.Api.Models;
using Sportomondo.Api.Requests;

namespace Sportomondo.Api.Services
{
    public interface IAchievementService
    {
        Task<IEnumerable<Achievement>> GetAllAsync(bool onlyMine);
        Task<int> CreateAsync(CreateAchievementRequest request);
        Task DeleteAsync(int id);
        Task<string> CheckAsync();
    }
}