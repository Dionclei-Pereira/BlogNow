﻿using System.ComponentModel.DataAnnotations;

namespace BlogMVC.Models.ViewModels {
    public class CreateViewModel {
        public string? Owner { get; set; }
        public DateTime Date { get; set; }
        public string? err { get; set; }
        [Required]
        [StringLength(150, MinimumLength = 5 ,ErrorMessage ="{0} must be between {2} and {1} length")]
        public string? Message { get; set; }
    }
}
