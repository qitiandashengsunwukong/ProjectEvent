﻿using ProjectEvent.Core.Condition.Types;
using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.Text;
using System.Timers;

namespace ProjectEvent.Core.Services
{
    public class TimerService : ITimerService
    {
        private Dictionary<int, Timer> _timers;
        private Dictionary<int, int> _timerRunCount;
        public TimerService()
        {
            _timers = new Dictionary<int, Timer>();
            _timerRunCount = new Dictionary<int, int>();
        }

        public void StartNew(int id, System.Action action, DateTime dateTime, TimeChangedRepetitionType repetitionType)
        {
            //转换时间
            var trueTime = new DateTime(dateTime.Year, dateTime.Month, dateTime.Day, dateTime.Hour, dateTime.Minute, 0);

            if (repetitionType == TimeChangedRepetitionType.Day)
            {
                //每天
                trueTime = new DateTime(DateTime.Now.Year, DateTime.Now.Month, DateTime.Now.Day, dateTime.Hour, dateTime.Minute, 0);
                if (trueTime < DateTime.Now)
                {
                    //当前已超过时间，调整为明天
                    trueTime = trueTime.AddDays(1);
                }
                StartNew(id, action, trueTime.Subtract(DateTime.Now).TotalSeconds, 1, () =>
                   {
                       //进行下一个周期
                       StartNew(id, action, dateTime, repetitionType);
                   });
            }
            else if (repetitionType == TimeChangedRepetitionType.Week)
            {
                //每周

                //今天星期数字
                int dayWeekNum = (int)DateTime.Now.DayOfWeek;
                //目标星期数字
                int inWeekNum = (int)dateTime.DayOfWeek;
                var time = DateTime.Now.AddDays(dayWeekNum == 0 ? dayWeekNum + inWeekNum : dayWeekNum + 7);
                trueTime = new DateTime(time.Year, time.Month, time.Day, dateTime.Hour, dateTime.Minute, 0);
                StartNew(id, action, trueTime.Subtract(DateTime.Now).TotalSeconds, 1, () =>
                {
                    //进行下一个周期
                    StartNew(id, action, dateTime, repetitionType);
                });
            }
            else
            {
                //指定日期
                if (DateTime.Now < trueTime)
                {
                    var s = trueTime.Subtract(DateTime.Now).TotalSeconds;
                    //可以执行
                    StartNew(id, action, s, 1);
                }
                else
                {
                    //已经超过指定日期不再执行
                    Debug.WriteLine("指定的日期已超过，无法再执行。");
                }
            }

        }



        public void StartNew(int id, System.Action action, double seconds, int num = 0, System.Action timerClosedAction = null)
        {

            if (_timers.ContainsKey(id) || seconds <= 0)
            {
                return;
            }
            var timer = new Timer();
            timer.Interval = seconds * 1000;

            Debug.WriteLine("任务在：" + seconds + "秒后执行");
            timer.Elapsed += (e, c) =>
            {
                int nextCount = GetRunCount(id) + 1;
                SetRunCount(id);

                if (num > 0 && nextCount > num)
                {
                    Close(id);
                }

                action?.Invoke();
                timerClosedAction?.Invoke();
            };

            _timerRunCount.Add(id, 0);
            _timers.Add(id, timer);

            timer.Start();
        }

        public void Close(int id)
        {
            if (_timers.ContainsKey(id))
            {
                var timer = _timers[id];
                timer.Stop();
                timer.Dispose();
                _timers.Remove(id);
            }
            if (_timerRunCount.ContainsKey(id))
            {
                _timerRunCount.Remove(id);
            }
        }

        /// <summary>
        /// 获取timer运行次数
        /// </summary>
        /// <param name="id"></param>
        /// <returns></returns>
        public int GetRunCount(int id)
        {
            int count = 0;

            if (_timerRunCount.ContainsKey(id))
            {
                return _timerRunCount[id];
            }
            return count;
        }

        /// <summary>
        /// 设置timer运行次数
        /// </summary>
        /// <param name="id">timer id</param>
        /// <param name="count">次数，默认为0，叠加一次</param>
        private void SetRunCount(int id, int count = 0)
        {
            if (_timerRunCount.ContainsKey(id))
            {
                if (count == 0)
                {
                    count = _timerRunCount[id] + 1;
                }
                _timerRunCount[id] = count;
            }
        }




    }
}
