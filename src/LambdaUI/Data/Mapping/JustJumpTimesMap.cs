using Dapper.FluentMap.Mapping;
using LambdaUI.Models.Simply;

namespace LambdaUI.Data.Mapping
{
    internal class JustJumpTimesMap : EntityMap<JustJumpMapTimeModel>
    {
        public JustJumpTimesMap()
        {
            Map(times => times.UniqueId).ToColumn("uniqueID");
            Map(times => times.SteamId).ToColumn("steamID");
            Map(times => times.Map).ToColumn("map");
            Map(times => times.Name).ToColumn("name");
            Map(times => times.RunTime).ToColumn("runtime");
            Map(times => times.Class).ToColumn("class");
            Map(times => times.TimeStamp).ToColumn("timestamp");
        }
    }
}