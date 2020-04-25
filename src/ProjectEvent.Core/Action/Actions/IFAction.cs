﻿using ProjectEvent.Core.Action.Models;
using ProjectEvent.Core.Action.Types;
using System;
using System.Collections.Generic;
using System.Text;
using System.Threading.Tasks;

namespace ProjectEvent.Core.Action.Actions
{
    public class IFAction : IAction
    {
        public Task<ActionResultModel> GenerateAction(int taskID, int actionID, BaseParameterModel parameter)
        {
            var task = new Task<ActionResultModel>(() =>
            {
                bool isPass = false;
                var p = parameter as IFActionParameterModel;
                ActionResultValueModel left, right;
                //获取左输入
                left = GetActionInput(taskID, p.LeftInput);
                //获取右输入
                right = GetActionInput(taskID, p.RightInput);


                switch (p.Condition)
                {
                    case Types.IFActionConditionType.Equal:
                        isPass = EqualOrUnEqual(left, right);
                        break;
                    case Types.IFActionConditionType.UnEqual:
                        isPass = EqualOrUnEqual(left, right, false);
                        break;
                }

                if (isPass)
                {
                    ActionTask.InvokeAction(taskID, p.PassActions, actionID);
                }
                else
                {
                    ActionTask.InvokeAction(taskID, p.NoPassActions, actionID);
                }

                var result = new ActionResultModel();
                result.ID = actionID;
                result.Result = new Dictionary<int, ActionResultValueModel>();
                result.Result.Add((int)CommonResultKeyType.Status, new ActionResultValueModel()
                {
                    Type = ActoinResultValueType.BOOL,
                    Value = isPass
                });
                return result;
            });
            return task;

        }

        private bool EqualOrUnEqual(ActionResultValueModel left, ActionResultValueModel right, bool equal = true)
        {
            try
            {
                if (left.Type == ActoinResultValueType.TEXT)
                {
                    return equal ? left.Value.ToString() == right.Value.ToString() : left.Value.ToString() != right.Value.ToString();
                }
                else if (left.Type == ActoinResultValueType.NUMBER)
                {
                    return equal ? (double)left.Value == (double)right.Value : (double)left.Value != (double)right.Value;
                }
                else if (left.Type == ActoinResultValueType.BOOL)
                {
                    return equal ? (bool)left.Value == (bool)right.Value : (bool)left.Value != (bool)right.Value;
                }
                else
                {
                    return equal ? left.Value == right.Value : left.Value != right.Value;
                }
            }
            catch
            {
                return false;
            }
        }

        private ActionResultValueModel GetActionInput(int taskID, BaseIFActionInputModel ip)
        {
            Types.IFActionInputType type = ip.InputType;
            var result = new ActionResultValueModel();
            result.Type = ActoinResultValueType.TEXT;
            result.Value = string.Empty;

            if (type == Types.IFActionInputType.Text)
            {
                result.Type = ActoinResultValueType.TEXT;
                result.Value = (ip as IFActionTextInputModel).Value;
            }
            else if (type == Types.IFActionInputType.ActionResult)
            {
                var actionResult = ActionTaskResulter.GetActionResult(taskID, (ip as IFActionResultInputModel).ActionID);
                if (actionResult != null)
                {
                    if (actionResult.Result.ContainsKey((ip as IFActionResultInputModel).ResultKey))
                    {
                        return actionResult.Result[(ip as IFActionResultInputModel).ResultKey];
                    }
                }
            }

            return result;
        }
    }
}