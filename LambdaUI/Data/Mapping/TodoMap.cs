using System;
using System.Collections.Generic;
using System.Text;
using Dapper.FluentMap.Mapping;
using LambdaUI.Models;

namespace LambdaUI.Data.Mapping
{
    class TodoMap : EntityMap<TodoModel>
    {
        public TodoMap()
        {
            Map(todo => todo.Group).ToColumn("todoGroup");
            Map(todo => todo.Item).ToColumn("todoItem");
            Map(todo => todo.Completeted).ToColumn("completed");
        }
    }
}
