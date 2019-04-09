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
    public class TodoDataAccess : MySqlDataAccessBase
    {
        public TodoDataAccess(string connectionString) : base(connectionString)
        {
            FluentMapper.Initialize(config =>
            {
                config.AddMap(new TodoMap());
            });
        }

        internal async Task<List<TodoModel>> GetTodoItems()
        {
            var query =
                @"select * from `todo`";

            return await QueryAsync<TodoModel>(query);

        }
        internal async Task<List<TodoModel>> GetTodoItems(string group)
        {

            var query =
                @"select * from `todo` where todoGroup=@Group";

            var param = new
            {
                Group = group,
            };

            return await QueryAsync<TodoModel>(query, param);
        }
        internal async Task CreateTodoItem(string group, string item)
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
