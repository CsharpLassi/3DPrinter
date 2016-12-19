using System;
using System.Linq;
using System.Collections.Generic;

namespace DSVG
{
    public class WindowInterface
    {
        public List<WindowInterface> Interfaces { get; set; }

        public string Name { get; set; }



        public WindowInterface()
        {
            Name = "Unknown";


            Interfaces = new List<WindowInterface>();
        }

        public virtual void OnInteract(string input)
        {
        }

        public virtual bool CanChangeWindow(string input)
        {
            return true;
        }


    }
}

