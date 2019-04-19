using System.Collections.Generic;
using System.Threading.Tasks;
using Dapper.FluentMap;
using LambdaUI.Data.Mapping;
using LambdaUI.Models;
using LambdaUI.Models.Bot;

namespace LambdaUI.Data
{
    public class TodoDataAccess : MySqlDataAccessBase
    {
        public TodoDataAccess(string connectionString) : base(connectionString)
        {
            FluentMapper.Initialize(config => { config.AddMap(new TodoMap()); });
        }

        internal async Task<List<TodoModel>> GetTodoItemsAsync()
        {
            var query =
                @"select * from `todo`";

            return await QueryAsync<TodoModel>(query);
        }

        internal async Task<List<TodoModel>> GetTodoItemsAsync(string group)
        {
            var query =
                @"select * from `todo` where todoGroup=@Group";

            var param = new
            {
                Group = group
            };

            return await QueryAsync<TodoModel>(query, param);
        }

        internal async Task CreateTodoItemAsync(string group, string item)
        {
            var query =
                @"INSERT INTO `todo`(`todoGroup`, `todoItem`) VALUES (@Group, @Item);";

            var param = new
            {
                Group = group,
                Item = item
            };

            await ExecuteAsync(query, param);
        }
    }
}