using Newtonsoft.Json;

namespace LambdaUI.Models.Tempus.Activity
{
    public class RecordInfo4
    {
        [JsonProperty(PropertyName = "server_id")]
        public int ServerId { get; set; }

        [JsonProperty(PropertyName = "user_id")]
        public int UserId { get; set; }

        [JsonProperty(PropertyName = "zone_id")]
        public int ZoneId { get; set; }

        [JsonProperty(PropertyName = "class")]
        public int Class { get; set; }

        [JsonProperty(PropertyName = "date")]
        public double Date { get; set; }

        [JsonProperty(PropertyName = "duration")]
        public double Duration { get; set; }

        [JsonProperty(PropertyName = "id")]
        public int Id { get; set; }
        public string ClassString()
        {
            switch (Class)
            {
                case 4:
                    return "D";
                case 3:
                    return "S";
                default:
                    return Class.ToString();
            }
        }
    }
}