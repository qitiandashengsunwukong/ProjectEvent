﻿using System;
using System.Collections.Generic;
using System.Text;

namespace ProjectEvent.Core.Action.Types
{
    public enum ActionType
    {
        /// <summary>
        /// 写文件
        /// </summary>
        WriteFile = 1,
        /// <summary>
        /// 删除文件
        /// </summary>
        DeleteFile = 2,
        /// <summary>
        /// 判断
        /// </summary>
        IF = 3,
        /// <summary>
        /// HTTP GET
        /// </summary>
        HttpGet = 4,
    }
}
