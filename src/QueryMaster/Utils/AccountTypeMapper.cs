#region License

/*
Copyright (c) 2015 Betson Roy

Permission is hereby granted, free of charge, to any person
obtaining a copy of this software and associated documentation
files (the "Software"), to deal in the Software without
restriction, including without limitation the rights to use,
copy, modify, merge, publish, distribute, sublicense, and/or sell
copies of the Software, and to permit persons to whom the
Software is furnished to do so, subject to the following
conditions:

The above copyright notice and this permission notice shall be
included in all copies or substantial portions of the Software.

THE SOFTWARE IS PROVIDED "AS IS", WITHOUT WARRANTY OF ANY KIND,
EXPRESS OR IMPLIED, INCLUDING BUT NOT LIMITED TO THE WARRANTIES
OF MERCHANTABILITY, FITNESS FOR A PARTICULAR PURPOSE AND
NONINFRINGEMENT. IN NO EVENT SHALL THE AUTHORS OR COPYRIGHT
HOLDERS BE LIABLE FOR ANY CLAIM, DAMAGES OR OTHER LIABILITY,
WHETHER IN AN ACTION OF CONTRACT, TORT OR OTHERWISE, ARISING
FROM, OUT OF OR IN CONNECTION WITH THE SOFTWARE OR THE USE OR
OTHER DEALINGS IN THE SOFTWARE.
*/

#endregion

using System;
using System.Collections.Generic;
using System.Linq;

namespace QueryMaster.Utils
{
    internal class AccountTypeMapper
    {
        private static readonly List<Tuple<int, char, AccountType>> AccountTypes =
            new List<Tuple<int, char, AccountType>>();

        private static AccountTypeMapper _obj;

        private AccountTypeMapper()
        {
            AccountTypes.Add(new Tuple<int, char, AccountType>(0, 'I', AccountType.Invalid));
            AccountTypes.Add(new Tuple<int, char, AccountType>(1, 'U', AccountType.Individual));
            AccountTypes.Add(new Tuple<int, char, AccountType>(2, 'M', AccountType.MultiSeat));
            AccountTypes.Add(new Tuple<int, char, AccountType>(3, 'G', AccountType.GameServer));
            AccountTypes.Add(new Tuple<int, char, AccountType>(4, 'A', AccountType.AnonGameServer));
            AccountTypes.Add(new Tuple<int, char, AccountType>(5, 'P', AccountType.Pending));
            AccountTypes.Add(new Tuple<int, char, AccountType>(6, 'C', AccountType.ContentServer));
            AccountTypes.Add(new Tuple<int, char, AccountType>(7, 'g', AccountType.Clan));
            AccountTypes.Add(new Tuple<int, char, AccountType>(8, 'T', AccountType.Chat));
            AccountTypes.Add(new Tuple<int, char, AccountType>(9, ' ', AccountType.P2PSuperSeeder));
            AccountTypes.Add(new Tuple<int, char, AccountType>(10, 'a', AccountType.AnonUser));
        }

        internal static AccountTypeMapper Instance
        {
            get
            {
                if (_obj == null) _obj = new AccountTypeMapper();
                return _obj;
            }
        }

        internal AccountType this[char character]
        {
            get
            {
                if (character == 'c' || character == 'L')
                    character = 'T';
                if (AccountTypes.Where(x => x.Item2 == character).Count() > 0)
                    return AccountTypes.Where(x => x.Item2 == character).First().Item3;
                return AccountType.Invalid;
            }
        }

        internal char this[AccountType type]
        {
            get
            {
                if (AccountTypes.Where(x => x.Item3 == type).Count() > 0)
                    return AccountTypes.Where(x => x.Item3 == type).First().Item2;
                return 'I';
            }
        }
    }
}