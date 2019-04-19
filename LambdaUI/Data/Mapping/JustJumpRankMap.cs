using Dapper.FluentMap.Mapping;
using LambdaUI.Models;
using LambdaUI.Models.Bot;
using LambdaUI.Models.Simply;

namespace LambdaUI.Data.Mapping
{
    internal class JustJumpRankMap : EntityMap<JumpRankModel>
    {
        public JustJumpRankMap()
        {
            Map(rank => rank.UniqueId).ToColumn("uniqueID");
            Map(rank => rank.SteamId).ToColumn("steamID");
            Map(rank => rank.Name).ToColumn("name");
            Map(rank => rank.Overall).ToColumn("overall");
            Map(rank => rank.Soldier).ToColumn("sol");
            Map(rank => rank.Demoman).ToColumn("dem");
            Map(rank => rank.Conc).ToColumn("conc");
            Map(rank => rank.Engi).ToColumn("eng");
            Map(rank => rank.Pyro).ToColumn("pyro");
        }
    }
}