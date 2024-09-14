using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace SkycavePlayerService.Exceptions
{
    public class PlayerExistException : ArgumentException
    {
        public PlayerExistException()
        { }
        public PlayerExistException(string message) : base(message)
        { }
    }
}
