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

        internal async Task<List<JustJumpMapTimeModel>> GetMapTimesAsync(int classValue, string mapName)
        {
            var query =
                @"select * from `Times` where timestamp>0 and `class`=@ClassValue and `map` like CONCAT('%', @MapName, '%')";
            var param = new
            {
                ClassValue = classValue,
                MapName = mapName
            };
            var result = (await QueryAsync<JustJumpMapTimeModel>(query, param)).ToList();
            return result;
        }

        internal async Task<List<JustJumpMapTimeModel>> GetRecentRecordsAsync(int count)
        {
            var query =
                $@"select * from `Times` where timestamp>0 order by timestamp desc limit {count}";

            var result = (await QueryAsync<JustJumpMapTimeModel>(query)).ToList();


            return result;
        }


        internal async Task<List<JumpRankModel>> GetTopDemoAsync(int count) => await GetTopAsync("dem", count);

        internal async Task<List<JumpRankModel>> GetTopSollyAsync(int count) => await GetTopAsync("sol", count);

        internal async Task<List<JumpRankModel>> GetTopConcAsync(int count) => await GetTopAsync("conc", count);

        internal async Task<List<JumpRankModel>> GetTopEngiAsync(int count) => await GetTopAsync("eng", count);

        internal async Task<List<JumpRankModel>> GetTopPyroAsync(int count) => await GetTopAsync("pyro", count);

        internal async Task<List<JumpRankModel>> GetTopOverallAsync(int count) => await GetTopAsync("general", count);

        private async Task<List<JumpRankModel>> GetTopAsync(string type, int count)
        {
            var query = $@"select * from JumpRanks order by {type} desc limit {count}";
            var result = (await QueryAsync<JumpRankModel>(query)).ToList();
            return result;
        }
    }
}