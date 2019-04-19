using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Dapper.FluentMap;
using LambdaUI.Data.Mapping;
using LambdaUI.Models.Simply;

namespace LambdaUI.Data.Access.Simply
{
    public class SimplyHightowerDataAccess : MySqlDataAccessBase
    {
        public SimplyHightowerDataAccess(string connectionString) : base(connectionString)
        {
            FluentMapper.Initialize(config => { config.AddMap(new SimplyHightowerMap()); });
        }
        internal async Task<List<SimplyHightowerModel>> GetTopHightowerRank(int count)
        {

            var query = $@"select * from players order by points desc limit {count}";

            var result = Enumerable.ToList<SimplyHightowerModel>((await QueryAsync<SimplyHightowerModel>(query)));

            return result;
        }
    }
}
