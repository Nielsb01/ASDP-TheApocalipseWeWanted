﻿using System;
using System.Collections.Generic;
using System.ComponentModel;

namespace Agent.Models
{
    public abstract class Configuration
    {
        protected Dictionary<string, string> _settings;

        public Dictionary<string, string> Settings
        {
            get => _settings;
            set => _settings = value;
        }
        
        public string GetSetting(string setting)
        {
            return _settings[setting];
        }
    }
    
    
}