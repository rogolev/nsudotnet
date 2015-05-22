using System;
using System.Collections.Generic;

namespace Rogolev.Nsudotnet.TaskSheduler
{
    internal class CronExpression
    {
        private readonly int _month;
        private readonly int _dayOfWeek;
        private readonly int _dayOfMonth;
        private readonly int _hour;
        private readonly int _min;

        private readonly int _leapYear;
        private const int MaxMonthsOfYear = 12;
        private const int MaxDaysOfMonth = 31;
        private const int MaxDaysOfWeek = 7;
        private const int MaxHours = 23;
        private const int MaxMins = 59;

        
        public CronExpression(string expression)
        {
            _leapYear = DateTime.Now.Year;
            while (!DateTime.IsLeapYear(_leapYear))
                _leapYear++;

            string[] expressionComponents = expression.Split(' ');
            if (expressionComponents.Length != 5)
                throw new InvalidCronExpressionException();
            assignValue(expressionComponents[0], out _month);
            assignValue(expressionComponents[1], out _dayOfMonth);
            assignValue(expressionComponents[2], out _hour);
            assignValue(expressionComponents[3], out _min);
            assignValue(expressionComponents[4], out _dayOfWeek);
            if (!ValuesAreValid())
                throw new InvalidCronExpressionException();
            if (_dayOfWeek == 0)
                _dayOfWeek = MaxDaysOfWeek;
        }

        public DateTime GetNexDateTime(DateTime currentDateTime)
        {
            DateTime nextDateTime;
            int year = currentDateTime.Year;
            int month = currentDateTime.Month;
            int day = currentDateTime.Day;
            int hour = currentDateTime.Hour;
            int min = currentDateTime.Minute;

            SortedSet<int> validDays =  GetValidDays(currentDateTime);

            if ((_month != -1) && (_month != month))
            {
                if (month > _month)
                    year++;
                month = _month;
                day = validDays.Min;
                hour = _hour != -1 ? _hour : 0;
                min = _min != -1 ? _min : 0;
            }
            else if (((_dayOfMonth != -1) || (_dayOfWeek != -1)) && (!validDays.Contains(day)))
            {
                if (day > validDays.Max)
                {
                    nextDateTime = currentDateTime.AddMonths(1);
                    year = nextDateTime.Year;
                    month = nextDateTime.Month;
                    validDays = GetValidDays(nextDateTime);
                    day = validDays.Min;
                }
                else
                {
                    foreach (int x in validDays)
                        if (x > day)
                        {
                            day = x;
                            break;
                        }
                }
                hour = _hour != -1 ? _hour : 0;
                min = _min != -1 ? _min : 0;
            }
            else if ((_hour != -1) && (_hour != hour))
            {
                if (hour > _hour)
                {
                    nextDateTime = currentDateTime.AddDays(1);
                    year = nextDateTime.Year;
                    month = nextDateTime.Month;
                    day = nextDateTime.Day;
                }
                hour = _hour;
                min = _min != -1 ? _min : 0;
            }
            else if ((_min != -1) && (_min != min))
            {
                if (min > _min)
                {
                    nextDateTime = currentDateTime.AddHours(1);
                    year = nextDateTime.Year;
                    month = nextDateTime.Month;
                    day = nextDateTime.Day;
                    hour = nextDateTime.Hour;
                }
                min = _min;
            }

            return new DateTime(year, month, day, hour, min, 1);
        }

        private SortedSet<int> GetValidDays(DateTime dateTime)
        {
            var validDays = new SortedSet<int>();
            int year = dateTime.Year;
            int month = dateTime.Month;
            if (_dayOfWeek != -1)
            {
                int firstDayOfWeekInCurrentMonth = (int)(new DateTime(year, month, 1).DayOfWeek);

                int firstValidDay;
                if (firstDayOfWeekInCurrentMonth > _dayOfWeek)
                    firstValidDay = (MaxDaysOfWeek - firstDayOfWeekInCurrentMonth) + _dayOfWeek + 1;
                else
                    firstValidDay = _dayOfWeek - firstDayOfWeekInCurrentMonth + 1;

                int validDate = firstValidDay;
                while (validDate < DateTime.DaysInMonth(year, month))
                {
                    validDays.Add(validDate);
                    validDate += MaxDaysOfWeek;
                }
            }
            if (_dayOfMonth != -1)
                validDays.Add(_dayOfMonth);
            return validDays;
        }

        private void assignValue(string expressionComponent, out int value)
        {
            if (int.TryParse(expressionComponent, out value))
            {
                
            }
            else if (expressionComponent.Equals("*"))
                value = -1;
            else
                throw new InvalidCronExpressionException();
        }

        private bool ValuesAreValid()
        {
            return (MonthIsValid() && DayOfMonthIsValid() && HourIsValid() && MinIsValid() && DayOfWeekIsValid());
        }

        private bool DayOfWeekIsValid()
        {
            return !(_dayOfWeek < -1 || _dayOfWeek > MaxDaysOfWeek);
        }

        private bool MinIsValid()
        {
            return !(_min < -1 || _min > MaxMins);
        }

        private bool HourIsValid()
        {
            return !(_hour < -1 || _hour > MaxHours);
        }

        private bool DayOfMonthIsValid()
        {
            return
                !(_dayOfMonth < -1 || (_month == -1 && _dayOfMonth > MaxDaysOfMonth) ||
                  (_month != -1 && _dayOfMonth > DateTime.DaysInMonth(_leapYear, _month)));
        }

        private bool MonthIsValid()
        {
            return !(_month < -1 || _month > MaxMonthsOfYear);
        }


    }

    public class InvalidCronExpressionException : Exception
    {
        
    };

    
}
