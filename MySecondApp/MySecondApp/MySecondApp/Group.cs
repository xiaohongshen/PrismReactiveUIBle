﻿using System;
using System.Collections.ObjectModel;


namespace MySecondApp
{
    public class Group<T> : ObservableCollection<T>
    {
        public Group(string name, string shortName = "")
        {
            this.Name = name;
            this.ShortName = shortName;
        }


        public string Name { get; }
        public string ShortName { get; }
    }
}

