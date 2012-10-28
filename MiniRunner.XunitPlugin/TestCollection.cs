using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using MiniRunner.Api;

namespace MiniRunner.XunitPlugin
{
    public class TestCollection : KeyedCollection<string, Test>
    {
        protected override string GetKeyForItem(Test item)
        {
            return item.UniqueId;
        }
    }
}
