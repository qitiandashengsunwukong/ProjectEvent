﻿using ProjectEvent.Core.Action.Models;
using ProjectEvent.Core.Action.Types;
using ProjectEvent.Core.Helper;
using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.IO;
using System.Text;
using System.Threading;
using System.Threading.Tasks;

namespace ProjectEvent.Core.Action.Actions
{
    public class WriteFileAction : IAction
    {
        public Task<ActionResultModel> GenerateAction(int taskID, int actionID, object parameter)
        {
            var task = new Task<ActionResultModel>(() =>
            {
                var p = ObjectConvert.Get<WriteFileActionParameterModel>(parameter);
                var result = new ActionResultModel();
                result.ID = actionID;
                result.Result = new Dictionary<int, string>();
                result.Result.Add((int)CommonResultKeyType.IsSuccess, "false");
                p.FilePath = ActionTaskResulter.GetActionResultsString(taskID, p.FilePath);
                p.Content = ActionTaskResulter.GetActionResultsString(taskID, p.Content);

                Debug.WriteLine("write file:" + p.FilePath);
                //Thread.Sleep(5000);
                try
                {
                    File.WriteAllText(p.FilePath, p.Content);
                    result.Result[(int)CommonResultKeyType.IsSuccess] = "true";
                }
                catch
                {
                }
                return result;
            });
            return task;
        }
    }
}
