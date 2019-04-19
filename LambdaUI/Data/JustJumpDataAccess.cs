using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Dapper.FluentMap;
using LambdaUI.Data.Mapping;
using LambdaUI.Models.Simply;

namespace LambdaUI.Data
{
    public class JustJumpDataAccess : MySqlDataAccessBase
    {
        public JustJumpDataAccess(string connectionString) : base(connectionString)
        {
            FluentMapper.Initialize(config => { config.AddMap(new JustJumpTimesMap()); });
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
            var result = (await QueryAsync<JustJumpMapTimeModel>(query, param)).ToList();
            return result;
        }
        internal async Task<List<JustJumpMapTimeModel>> GetRecentRecords(int count)
        {

            var query =
                $@"select * from `Times` where timestamp>0 order by timestamp desc limit {count}";

            var result = (await QueryAsync<JustJumpMapTimeModel>(query)).ToList();


            return result;
        }
    }
}
