using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Dapper;
using Dapper.FluentMap;
using LambdaUI.Data.Mapping;
using LambdaUI.Models;

namespace LambdaUI.Data
{
    public class ConfigDataAccess : MySqlDataAccessBase
    {
        public ConfigDataAccess(string connectionString) : base(connectionString)
        {
            FluentMapper.Initialize(config =>
            {
                config.AddMap(new ConfigMap());
            });
        }
        internal async Task<List<ConfigModel>> GetAllConfig()
        {
            var query =
                @"select * from `config`";

            return await QueryAsync<ConfigModel>(query);

        }
        internal async Task<List<ConfigModel>> GetConfig(string key)
        {
            var query =
                @"select * from `config` where `key`=@Key";

            var param = new
            {
                Key = key,
            };

            return await QueryAsync<ConfigModel>(query, param);

        }
        internal async Task CreateNewConfigEntry(string key, string value)
        {

            var query =
                @"INSERT INTO `config` (`key`, `value`) VALUES(@Key, @Value) ON DUPLICATE KEY UPDATE Value = @Value";

            var param = new
            {
                Key = key,
                Value = value
            };

            await ExecuteAsync(query, param);
        }
    }
}
