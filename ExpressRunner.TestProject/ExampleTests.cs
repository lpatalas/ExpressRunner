using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Xunit;

namespace ExpressRunner.TestProject
{
    public class ExampleTests
    {
        [Fact]
        public void Should_fail_always()
        {
            Assert.True(false);
        }

        [Fact]
        public void Should_pass_always()
        {
            Assert.True(true);
        }

        [Fact]
        public void Should_pass_also()
        {
            Assert.True(true);
        }
    }
}
