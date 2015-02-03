﻿using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Fluor.ProjectSwitcher.Class;

namespace Fluor.ProjectSwitcher.Message
{
    public class MessageUpdateSelectedTile
    {
        public object Sender { get; set; }
        public Project SelectedTile { get; set; }
        public bool IsNewTile { get; set; }

        public MessageUpdateSelectedTile(Project selectedTile, bool isNewTile, object sender)
        {
            SelectedTile = selectedTile;
            IsNewTile = isNewTile;
            Sender = sender;
        }
    }
}
