using System;

namespace ORM
{
    public sealed class TableAttribute : Attribute
    {
        public bool Key { get; set; }
        public string Name { get; set; }
    }
}
