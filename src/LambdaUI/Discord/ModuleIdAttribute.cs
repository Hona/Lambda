using System;
using System.Collections.Generic;
using System.Text;

namespace LambdaUI.Discord
{
    class ModuleIdAttribute : Attribute
    {
        public int Id { get; }
        public ModuleIdAttribute(int id)
        {
            Id = id;
        }
    }
}
