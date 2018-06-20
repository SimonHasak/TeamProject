using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Linq;
using System.Web;

namespace TeamMVCProject.Models
{
    public class Player
    {

        public int ID { get; set; }

        [Required]
        public string Name { get; set; }
        public string Description { get; set; }



        /// <summary>
        /// Contains teams.
        /// </summary>
        public virtual ICollection<Team> Teams { get; set; }
    }
}