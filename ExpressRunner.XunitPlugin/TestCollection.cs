using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using ExpressRunner.Api;

namespace ExpressRunner.XunitPlugin
{
    public class TestCollection : KeyedCollection<string, Test>
    {
        protected override string GetKeyForItem(Test item)
        {
            return item.UniqueId;
        }
    }
}
