using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace XiMPLib.xType {
    public interface IRange<T> {
        T Start { get; }
        T End { get; }
        bool contains(T value);
        bool contains(IRange<T> range);
    }

    public class DateRange : IRange<DateTime> {
        public DateTime Start { get; set; }
        public DateTime End { get; set; }

        public DateRange(DateTime start, DateTime end) {
            Start = start;
            End = end;
        }

        public bool contains(DateTime value) {
            return (Start <= value) && (value <= End);
        }

        public bool contains(IRange<DateTime> range) {
            return (Start <= range.Start) && (range.End <= End);
        }
    }

    public class TimeRange : IRange<TimeSpan> {
        public TimeRange(TimeSpan start, TimeSpan end) {
            Start = start;
            End = end;
        }

        public TimeRange(string range) {
            setRange(range);
        }

        public TimeSpan Start { get; set; }
        public TimeSpan End { get; set; }


        public void setRange(string range) {
            if (string.IsNullOrEmpty(range))
                return;
            string[] timeTokens = range.Split('-');
            Start = new TimeSpan(int.Parse(timeTokens[0].Substring(0, 2)), int.Parse(timeTokens[0].Substring(2)), 00);
            if (timeTokens.Length > 1)
                End = new TimeSpan(int.Parse(timeTokens[1].Substring(0, 2)), int.Parse(timeTokens[1].Substring(2)), 00);
        }

        public bool contains(TimeSpan value) {
            return (Start <= value) && (value <= End);
        }

        public bool contains(IRange<TimeSpan> range) {
            return (Start <= range.Start) && (range.End <= End);
        }

    }

    public class FloatRange : IRange<float> {
        public float Start { get; set; }
        public float End { get; set; }

        public FloatRange(float from, float to) {
            Start = from;
            End = to;
        }

        public bool contains(float value) {
            return this.Start <= value && value <= this.End;
        }

        public bool contains(IRange<float> range) {
            return (Start <= range.Start) && (range.End <= End);
        }

        public override string ToString() {
            return string.Format("{0:F1}-{1:F1}", Start, End);
        }
    }

    public class IntRange : IRange<int> {
        public int Start { get; set; }
        public int End { get; set; }

        public IntRange(int from, int to) {
            Start = from;
            End = to;
        }

        public bool contains(int value) {
            return this.Start <= value && value <= this.End;
        }
        public bool contains(IRange<int> range) {
            return (Start <= range.Start) && (range.End <= End);
        }
        public override string ToString() {
            return string.Format("{0}-{1}", Start, End);
        }
    }

}
