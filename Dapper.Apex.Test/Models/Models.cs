using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Dapper.Apex.Test.Models
{
    public abstract class BaseModel { }

    [Table("Model1")]
    public class ModelX : BaseModel
    {
        public int Id { get; set; }
        public string Prop1 { get; set; }
        [ReadOnly]
        public string Prop2 { get; set; }
        public string Prop3 { get; set; }
        [Computed]
        public int Prop4 { get; set; }
    }

    public class Model2
    {
        [Key]
        public int Model2Id { get; set; }
        public string Prop1 { get; set; }
    }

    public class Model3
    {
        [ExplicitKey]
        public Guid Id1 { get; set; }
        [ExplicitKey]
        public Guid Id2 { get; set; }
        public string Prop1 { get; set; }
    }

    public class Model4 : ModelX
    {
    }
}
