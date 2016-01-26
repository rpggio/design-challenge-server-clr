using System;
using System.Text.RegularExpressions;

namespace DCS.Contracts
{
    public class Stage
    {
        private int _number;

        public static Stage Zero
        {
            get { return new Stage(0); }
        }

        public static Stage First
        {
            get {  return new Stage(1); }
        }

        public int Number
        {
            get { return _number; }
            set { _number = value; }
        }

        public string Name
        {
            get { return string.Format("stage{0:D3}", _number); }
        }

        public string FriendlyName
        {
            get { return "Stage " + Number; }
        }

        public static Stage Parse(string name)
        {
            var match = Regex.Match(name, @"stage(\d+)");
            var createMatchException =
                new Func<Exception>(() => new Exception(string.Format("Could not parse stage number {0}", name)));
            if (!match.Success)
            {
                throw createMatchException();
            }
            int stageNumber;
            if (!int.TryParse(match.Groups[1].Value, out stageNumber))
            {
                throw createMatchException();
            }
            return new Stage(stageNumber);
        }

        public Stage(int number)
        {
            _number = number;
        }

        public static Stage operator +(Stage stage, int value)
        {
            return new Stage(stage.Number + value);
        }

        public static Stage operator -(Stage stage, int value)
        {
            return stage + -value;
        }

        public override string ToString()
        {
            return Name;
        }
    }
}