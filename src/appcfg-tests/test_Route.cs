using System;
using appcfg;
using NUnit.Framework;

namespace appcfg_tests
{
    [TestFixture]
    public class test_Route
    {
        [Test]
        public void Route_with_all_params_distinct()
        {
            new Route("r").Param("p1", "p1Alias,p1Alias2").Param("p2", "p2Alias");
        }
        
        [Test]
        public void Route_with_not_all_params_distinct()
        {
            Assert.Throws<InvalidOperationException>(() => new Route("r").Param("thesame", "p1Alias,p1Alias2").Param("thesame", "p2Alias"));
            Assert.Throws<InvalidOperationException>(() => new Route("r").Param("p1", "p1Alias,thesame").Param("p2", "thesame"));
        }
        
        
        [Test]
        public void Route_with_catchall_at_end()
        {
            new Route("r").Param("p1").Param("p2").Param("*",valueType:ValueTypes.String);
        }
        
        [Test]
        public void Route_with_catchall_not_at_end()
        {
            Assert.Throws<InvalidOperationException>(() => new Route("r").Param("p1").Param("*",valueType:ValueTypes.String).Param("p2"));
        }


        [Test]
        public void Catch_all_param_needs_to_have_value_type()
        {
            Assert.Throws<InvalidOperationException>(() => new Route("r").Param("p1").Param("*"));
        }
    }
}