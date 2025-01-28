using System;

namespace OtpGenerator.DataExchange.Attributes
{
    [AttributeUsage(AttributeTargets.Property, AllowMultiple = false)]
    public class ExportAttribute : Attribute
    {
        public string Name { get; set; }
        public int Order { get; set; }

        public ExportAttribute(string name, int order) : base()
        {
            Name = name;
            Order = order;
        }
    }
}
