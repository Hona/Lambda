using Dapper.FluentMap.Mapping;
using LambdaUI.Models;
using LambdaUI.Models.Bot;
using LambdaUI.Models.Simply;

namespace LambdaUI.Data.Mapping
{
    internal class SimplyHightowerMap : EntityMap<SimplyHightowerModel>
    {
        public SimplyHightowerMap()
        {
            Map(hightower => hightower.SteamId).ToColumn("steamid");
            Map(hightower => hightower.Nickname).ToColumn("nickname");
            Map(hightower => hightower.Points).ToColumn("points");
            Map(hightower => hightower.Seen).ToColumn("seen");
            Map(hightower => hightower.Deaths).ToColumn("deaths");
            Map(hightower => hightower.Kills).ToColumn("kills");
            Map(hightower => hightower.Assists).ToColumn("assists");
            Map(hightower => hightower.Backstabs).ToColumn("backstabs");
            Map(hightower => hightower.Headshots).ToColumn("headshots");
            Map(hightower => hightower.Feigns).ToColumn("feigns");
            Map(hightower => hightower.MerKills).ToColumn("merkills");
            Map(hightower => hightower.MerLevel).ToColumn("merlvl");
            Map(hightower => hightower.MonKills).ToColumn("monkills");
            Map(hightower => hightower.MonLevel).ToColumn("monlvl");
            Map(hightower => hightower.HHHKills).ToColumn("hhhkills");
            Map(hightower => hightower.PlayTime).ToColumn("playtime");
            Map(hightower => hightower.FlagCaptures).ToColumn("flagcaptures");
            Map(hightower => hightower.FlagDefends).ToColumn("flagdefends");
            Map(hightower => hightower.CapCaptures).ToColumn("capcaptures");
            Map(hightower => hightower.CapDefends).ToColumn("capdefends");
            Map(hightower => hightower.RoundsPlayed).ToColumn("roundsplayed");
            Map(hightower => hightower.DominationsGood).ToColumn("dominationsgood");
            Map(hightower => hightower.DominationsBad).ToColumn("dominationsbad");
            Map(hightower => hightower.Deflects).ToColumn("deflects");
            Map(hightower => hightower.Streak).ToColumn("streak");
        }
    }
}