using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations.Schema;
using System.ComponentModel.DataAnnotations;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Template.Services.Shared
{
    public class UserDayDetail
    {

        [Key]
        [DatabaseGenerated(DatabaseGeneratedOption.Identity)]
        public int Id { get; set; }

        public Guid UserId { get; set; }
        [ForeignKey(nameof(UserId))]
        public User User { get; set; }


        public DateOnly Day { get; set; }
        public decimal HSmartWorking { get; set; }
        public decimal HHoliday { get; set; }

        [InverseProperty(nameof(Request.UserDayDetail))]
        public virtual ICollection<Request> Requests { get; set; }
    }
}
