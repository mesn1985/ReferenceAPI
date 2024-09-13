using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace SkycavePlayerService.Exceptions
{
    public class PlayerDoesNotExistException : ArgumentException
    {
        public PlayerDoesNotExistException(string message) :base(message)
        { }
    }
}
