namespace LambdaUI.Models.Simply
{
    public class SimplyHightowerModel
    {
        public string SteamId { get; set; }
        public string Nickname { get; set; }
        public decimal Points { get; set; }
        public int Seen { get; set; }
        public int Deaths { get; set; }
        public int Kills { get; set; }
        public int Assists { get; set; }
        public int Backstabs { get; set; }
        public int Headshots { get; set; }
        public int Feigns { get; set; }
        public int MerKills { get; set; }
        public int MerLevel { get; set; }
        public int MonKills { get; set; }
        public int MonLevel { get; set; }
        public int HHHKills { get; set; }
        public int PlayTime { get; set; }
        public int FlagCaptures { get; set; }
        public int FlagDefends { get; set; }
        public int CapCaptures { get; set; }

        public int CapDefends { get; set; }
        public int RoundsPlayed { get; set; }

        public int DominationsGood { get; set; }
        public int DominationsBad { get; set; }

        public int Deflects { get; set; }
        public int Streak { get; set; }
    }
}