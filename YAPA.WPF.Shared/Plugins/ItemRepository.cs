﻿using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;

namespace YAPA
{
    public class ItemRepository
    {
        private DatabaseContext context;

        public ItemRepository()
        {
            context = new DatabaseContext(Path.Combine(Environment.GetFolderPath(Environment.SpecialFolder.MyDocuments), "YAPA2", "Dashboard"));
        }

        public void CompletePomodoro()
        {
            var date = DateTime.Now.Date;
            var pomodoro = context.Pomodoros.SingleOrDefault(p => p.DateTime == date);
            if (pomodoro == null)
            {
                pomodoro = new PomodoroEntity() { Count = 1, DateTime = date };
                context.Pomodoros.Add(pomodoro);
            }
            else
            {
                pomodoro.Count = pomodoro.Count + 1;
            }
            context.SaveChanges();
        }

        public int CompletedToday()
        {
            var date = DateTime.Now.Date;
            var todays = context.Pomodoros.SingleOrDefault(p => p.DateTime == date);
            if (todays == null)
            {
                return 0;
            }
            else
            {
                return todays.Count;
            }
        }

        public IEnumerable<PomodoroEntity> GetPomodoros()
        {
            var days = 190;
            var today = DateTime.Now.Date.Date;
            var fromDate = today.AddDays(-days);
            var emptyPomodoros = Enumerable.Range(0, days + 1).Select(x => new PomodoroEntity() { Count = 0, DateTime = fromDate.AddDays(x) }).ToList();
            var capturedPomodoros = context.Pomodoros.Where(x => x.DateTime >= fromDate).ToList();

            var joinedPomodoros = capturedPomodoros.Union(emptyPomodoros)
                .GroupBy(c => c.DateTime.Date, c => c.Count,
                    (time, ints) => new PomodoroEntity() { DateTime = time, Count = ints.Sum(x => x) });

            return joinedPomodoros.OrderBy(x => x.DateTime.Date);
        }

        public void Add(PomodoroEntity pomodoroEntity)
        {
            context.Pomodoros.Add(pomodoroEntity);
            context.SaveChanges();
        }
    }

    public enum PomodoroLevelEnum
    {
        Level0 = 0,
        Level1 = 1,
        Level2 = 2,
        Level3 = 3,
        Level4 = 4,
    }
}