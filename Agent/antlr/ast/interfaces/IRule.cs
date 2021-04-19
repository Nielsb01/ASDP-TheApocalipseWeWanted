﻿using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Agent.antlr.ast
{
    interface IRule : INode
    {
        public string SettingName { get; set; }

        public string Value { get; set; }

    }
}
