﻿using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Editor.Engine
{
    internal class ListItemLevel
    {
        public Models Model { get; set; }

        public override string ToString()
        {
            return Model.Name;
        }
    }
}
