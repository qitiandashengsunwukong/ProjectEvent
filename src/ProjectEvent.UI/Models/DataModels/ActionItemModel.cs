﻿using ProjectEvent.UI.Controls.Base;
using ProjectEvent.UI.Types;
using System;
using System.Collections.Generic;
using System.Text;

namespace ProjectEvent.UI.Models.DataModels
{
    public class ActionItemModel
    {
        public int ID { get; set; }
        public string ActionName { get; set; }
        public IconTypes Icon { get; set; }
        public int Index { get; set; }
        public ActionType ActionType { get; set; }
        public int ParentID { get; set; }
    }
}
