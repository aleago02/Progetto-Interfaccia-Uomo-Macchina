using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations.Schema;
using System.ComponentModel.DataAnnotations;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Template.Services.Shared
{
    public class Requests
    {
        /*
        [Key]
        [DatabaseGenerated(DatabaseGeneratedOption.Identity)]
        public int Id { get; set; }
        */

        [Key]
        public int Id { get; set; }
        [ForeignKey(nameof(Id))]
        [InverseProperty("Requests")]

        public Boolean request { get; set; }
    }
}
