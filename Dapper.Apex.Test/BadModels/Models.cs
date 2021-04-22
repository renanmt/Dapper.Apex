using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Dapper.Apex.Test.BadModels
{
    public class NoPrimaryKeyModel
    {
        public Guid XXX { get; set; }
    }

    public class DualPrimaryKeyModel
    {
        public int Id { get; set; }
        [ExplicitKey]
        public Guid XXX { get; set; }
    }

    public class DualPrimaryKeyModelUsingKeyAttr
    {
        [Key]
        public int XXX { get; set; }
        [ExplicitKey]
        public Guid YYY { get; set; }
    }

    public class MultipleSurrogateKeyModel
    {
        [Key]
        public int XXX { get; set; }
        [Key]
        public int YYY { get; set; }
    }
}
