using System;
using System.Collections.Generic;
using System.Text;
using Newtonsoft.Json;

namespace LambdaUI.Models.Tempus.DetailedMapList
{
    public class Videos
    {
        [JsonProperty(PropertyName = "soldier")] public string Soldier { get; set; }
        [JsonProperty(PropertyName = "demoman")] public string Demoman { get; set; }
    }

    public class TierInfo
    {
        [JsonProperty(PropertyName = "3")] public int Soldier { get; set; }
        [JsonProperty(PropertyName = "4")] public int Demoman { get; set; }
    }

    public class ZoneCounts
    {
        [JsonProperty(PropertyName = "checkpoint")] public int Checkpoint { get; set; }
        [JsonProperty(PropertyName = "map")] public int Map { get; set; }
        [JsonProperty(PropertyName = "linear")] public int Linear { get; set; }
        [JsonProperty(PropertyName = "special")] public int Special { get; set; }
        [JsonProperty(PropertyName = "map_end")] public int MapEnd { get; set; }
        [JsonProperty(PropertyName = "course_end")] public int? CourseEnd { get; set; }
        [JsonProperty(PropertyName = "bonus")] public int? Bonus { get; set; }
        [JsonProperty(PropertyName = "misc")] public int? Misc { get; set; }
        [JsonProperty(PropertyName = "trick")] public int? Trick { get; set; }
        [JsonProperty(PropertyName = "course")] public int? Course { get; set; }
        [JsonProperty(PropertyName = "bonus_end")] public int? BonusEnd { get; set; }
    }

    public class Author
    {
        [JsonProperty(PropertyName = "map_id")] public int MapId { get; set; }
        [JsonProperty(PropertyName = "name")] public string Name { get; set; }
        [JsonProperty(PropertyName = "id")] public int Id { get; set; }
    }

    public class DetailedMapOverviewModel
    {
        [JsonProperty(PropertyName = "name")] public string Name { get; set; }
        [JsonProperty(PropertyName = "videos")] public Videos Videos { get; set; }
        [JsonProperty(PropertyName = "tier_info")] public TierInfo TierInfo { get; set; }
        [JsonProperty(PropertyName = "zone_counts")] public ZoneCounts ZoneCounts { get; set; }
        [JsonProperty(PropertyName = "authors")] public List<Author> Authors { get; set; }
        [JsonProperty(PropertyName = "id")] public int Id { get; set; }
    }
}
