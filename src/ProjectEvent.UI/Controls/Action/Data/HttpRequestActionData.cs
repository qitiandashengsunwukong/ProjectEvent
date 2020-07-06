﻿using ProjectEvent.Core.Action.Types;
using ProjectEvent.Core.Net.Types;
using ProjectEvent.UI.Controls.Action.Models;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace ProjectEvent.UI.Controls.Action.Data
{
    public class HttpRequestActionData
    {
        public static List<ComBoxModel> PamramsTypes = new List<ComBoxModel>()
        {
            new ComBoxModel()
            {
                ID = (int)ParamsType.Json,
                DisplayName = "Json"
            },
            new ComBoxModel()
            {
                ID = (int)ParamsType.FormData,
                DisplayName = "Form"
            },

        };
        public static ComBoxModel GetPamramsType(int id)
        {
            return PamramsTypes.Where(m => m.ID == id).FirstOrDefault();
        }

        public static List<ComBoxModel> MethodTypes = new List<ComBoxModel>()
        {
            new ComBoxModel()
            {
                ID = (int)MethodType.GET,
                DisplayName = "GET"
            },
            new ComBoxModel()
            {
                ID = (int)MethodType.POST,
                DisplayName = "POST"
            },

        };
        public static ComBoxModel GetMethodType(int id)
        {
            return MethodTypes.Where(m => m.ID == id).FirstOrDefault();
        }
    }
}
