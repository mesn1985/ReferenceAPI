using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace SkycavePlayerService.Shared.Models.PlayerRecord.Primitives
{
    public class AccessToken : ValueObjectBase<string>
    {
        public AccessToken(string accessTokenString) : base(accessTokenString)
        { }

        /// <summary>
        /// validates if access token is either null (means the player is inactive), or have a value with a  length
        /// larger then 0.
        /// </summary>
        /// <param name="accessTokenString"></param>
        /// <returns></returns>
        protected override bool IsValid(string accessTokenString)
        {
            if (accessTokenString == null)
            {
                return true;
            }
            else if (accessTokenString.Length == 0)
            {
                return false;
            }

            return true;
        }
    }
}
