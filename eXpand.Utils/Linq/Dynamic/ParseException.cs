using System;

namespace eXpand.Utils.Linq.Dynamic
{
    public sealed class ParseException : Exception
    {
        readonly int position;

        public ParseException(string message, int position)
            : base(message)
        {
            this.position = position;
        }

        public int Position
        {
            get { return position; }
        }

        public override string ToString()
        {
            return string.Format(Res.ParseExceptionFormat, Message, position);
        }
    }
}