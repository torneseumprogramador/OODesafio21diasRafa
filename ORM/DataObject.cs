using System.Data;

namespace ORM
{
    public sealed class DataObject
    {
        public bool FieldKey { get; set; }
        public string FieldName { get; set; }
        public SqlDbType FieldType { get; set; }
        public object FieldValue { get; set; }
    }
}
