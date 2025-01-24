﻿using GDC.EventHost.Shared.Event;
using GDC.EventHost.Shared.SeriesAsset;
using System.ComponentModel.DataAnnotations;
using static GDC.EventHost.Shared.Enums;

namespace GDC.EventHost.Shared.Series
{
    public class SeriesDetailDto
    {
        public Guid Id { get; set; }

        [Display(Name = "Title")]
        public required string Title { get; set; }

        [Display(Name = "Description")]
        public string? Description { get; set; }

        [Display(Name = "Long Description")]
        public string? LongDescription { get; set; }

        [Display(Name = "Start Date")]
        public DateTime? StartDate { get; set; }

        [Display(Name = "End Date")]
        public DateTime? EndDate { get; set; }

        [Display(Name = "Status Id")]
        public required StatusEnum StatusId { get; set; }

        [Display(Name = "Status Value")]
        public string? StatusValue { get; set; }

        public List<EventDetailDto> Events { get; set; } = [];

        public List<SeriesAssetDto> SeriesAssets { get; set; } = [];
    }
}