using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations.Schema;
using System.ComponentModel.DataAnnotations;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Template.Services.Shared
{
    internal class Days
    {
        [Key]
        [DatabaseGenerated(DatabaseGeneratedOption.Identity)]
        public Guid Id_User { get; set; }
        public DataType Day { get; set; }
        public float HSmartWorking { get; set; }
        public float HHoliday { get; set; }

    }
}
