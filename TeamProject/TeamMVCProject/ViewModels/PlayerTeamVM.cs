using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;

namespace TeamMVCProject.ViewModels
{
    public class PlayerTeamVM
    {
        public int ID { get; set; }
        public string Name { get; set; }
        public bool isAssigned { get; set; }
    }
}