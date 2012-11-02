using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading;
using System.Threading.Tasks;
using Xunit;
using Xunit.Extensions;

namespace ExpressRunner.TestProject
{
    public class ExampleTests
    {
        [Fact]
        public void Should_fail_always()
        {
            Assert.Equal("text", "wrong text");
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

        [Theory]
        [InlineData(1)]
        [InlineData(2)]
        public void Should_support_theory(int input)
        {
            Assert.Equal(1, input);
        }

        [Fact]
        public void Should_take_one_second_to_finish()
        {
            Thread.Sleep(1000);
            Assert.True(true);
        }
    }
}
