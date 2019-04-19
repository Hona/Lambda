using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Dapper.FluentMap;
using LambdaUI.Data.Mapping;
using LambdaUI.Models.Simply;

namespace LambdaUI.Data.Access.Simply
{
    public class JustJumpDataAccess : MySqlDataAccessBase
    {
        public JustJumpDataAccess(string connectionString) : base(connectionString)
        {
            FluentMapper.Initialize(config =>
            {
                config.AddMap(new JustJumpTimesMap());
                config.AddMap(new JustJumpRankMap());
            });
        }

        internal async Task<List<string>> QueryAsync(string query) => await QueryAsync<string>(query);
        internal async Task<List<JustJumpMapTimeModel>> GetMapTimes(int classValue, string mapName)
        {

            var query =
                @"select * from `Times` where timestamp>0 and `class`=@ClassValue and `map` like CONCAT('%', @MapName, '%')";
            var param = new
            {
                ClassValue = classValue,
                MapName = mapName
            };
            var result = Enumerable.ToList<JustJumpMapTimeModel>((await QueryAsync<JustJumpMapTimeModel>(query, param)));
            return result;
        }
        internal async Task<List<JustJumpMapTimeModel>> GetRecentRecords(int count)
        {

            var query =
                $@"select * from `Times` where timestamp>0 order by timestamp desc limit {count}";

            var result = Enumerable.ToList<JustJumpMapTimeModel>((await QueryAsync<JustJumpMapTimeModel>(query)));


            return result;
        }


        internal async Task<List<JumpRankModel>> GetTopDemo(int count)
        {
            return await GetTop("dem", count);
        }

        internal async Task<List<JumpRankModel>> GetTopSolly(int count)
        {
            return await GetTop("sol", count);
        }

        internal async Task<List<JumpRankModel>> GetTopConc(int count)
        {
            return await GetTop("conc", count);
        }

        internal async Task<List<JumpRankModel>> GetTopEngi(int count)
        {
            return await GetTop("eng", count);
        }

        internal async Task<List<JumpRankModel>> GetTopPyro(int count)
        {
            return await GetTop("pyro", count);
        }

        internal async Task<List<JumpRankModel>> GetTopOverall(int count)
        {
            return await GetTop("general", count);
        }

        private async Task<List<JumpRankModel>> GetTop(string type, int count)
        {
            var query = $@"select * from JumpRanks order by {type} desc limit {count}";
            var result = Enumerable.ToList<JumpRankModel>((await QueryAsync<JumpRankModel>(query)));
            return result;
        }
    }
}
