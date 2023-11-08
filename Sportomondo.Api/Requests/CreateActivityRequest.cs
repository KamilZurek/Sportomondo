﻿using Sportomondo.Api.Models;
using System.ComponentModel.DataAnnotations;

namespace Sportomondo.Api.Requests
{
    public class CreateActivityRequest : ActivityRequest //trzeba eksrea walidacje do wiekszosci pól
    { 
        [Required]
        public ActivityType Type { get; set; }
    }
}
