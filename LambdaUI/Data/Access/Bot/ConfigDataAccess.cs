using System.Collections.Generic;
using System.Threading.Tasks;
using Dapper.FluentMap;
using LambdaUI.Data.Mapping;
using LambdaUI.Models.Bot;

namespace LambdaUI.Data.Access.Bot
{
    public class ConfigDataAccess : MySqlDataAccessBase
    {
        public ConfigDataAccess(string connectionString) : base(connectionString)
        {
            FluentMapper.Initialize(config => { config.AddMap(new ConfigMap()); });
        }

        internal async Task<List<ConfigModel>> GetAllConfigAsync()
        {
            var query =
                @"select * from `config`";

            return await QueryAsync<ConfigModel>(query);
        }

        internal async Task<List<ConfigModel>> GetConfigAsync(string key)
        {
            var query =
                @"select * from `config` where `key`=@Key";

            var param = new
            {
                Key = key
            };

            return await QueryAsync<ConfigModel>(query, param);
        }

        internal async Task CreateNewConfigEntryAsync(string key, string value)
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