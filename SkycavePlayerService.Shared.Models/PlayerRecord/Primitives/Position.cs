using System.Text.RegularExpressions;

namespace SkycavePlayerService.Shared.Models.PlayerRecord.Primitives
{
    public class Position : ValueObjectBase<string>
    {
        public Position(string position) : base(position)
        { }

        protected override bool IsValid(string position)
        {
            if (string.IsNullOrEmpty(position))
            {
                return false;
            }

            else if (PositionFormatIsNotValid(position))
            {
                return false;   
            }

            return true;
        }

        private Boolean PositionFormatIsNotValid(string position)
        {
            string pattern = @"^\((-?\d+)(,-?\d+)*\)$"; 

            if (Regex.IsMatch(position, pattern))
            {
                return false;
            }
            else
            {
                return true;
            }
        }

    }
}
