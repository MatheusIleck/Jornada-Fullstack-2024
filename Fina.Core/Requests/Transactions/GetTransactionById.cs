﻿using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Fina.Core.Requests.Transactions
{
    internal class GetTransactionById : Request
    {
        public long Id { get; set; }
    }
}
