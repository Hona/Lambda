using System;
using System.Collections.Generic;
using System.Text;
using Dapper.FluentMap.Mapping;
using LambdaUI.Models;

namespace LambdaUI.Data.Mapping
{
    class ConfigMap : EntityMap<ConfigModel>
    {
        public ConfigMap()
        {
            Map(config => config.Key).ToColumn("key");
            Map(config => config.Value).ToColumn("value");
        }
    }
}
