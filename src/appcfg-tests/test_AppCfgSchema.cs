using System;
using appcfg;
using NUnit.Framework;

namespace appcfg_tests
{
    [TestFixture]
    public class test_AppCfgSchema
    {
        [Test]
        public void Schema_with_valid_routes() {
            var routes = new[] {
                new Route("r1", "r1Alias,r1Alias2"),
                new Route("r2", "r2Alias") 
            };

            var _ = new AppCfgSchema("config1.json", routes);
        }
        
        
        [Test]
        public void Schema_with_invalid_routes() {
            var routes = new[] {
                new Route("thesame", "r1Alias,r1Alias2"),
                new Route("thesame", "r2Alias") 
            };
            Assert.Throws<InvalidOperationException>(() => new AppCfgSchema("config1.json", routes));
            
            routes = new[] {
                new Route("r1", "r1Alias,thesame"),
                new Route("r2", "thesame") 
            };
            Assert.Throws<InvalidOperationException>(() => new AppCfgSchema("config1.json", routes));
        }
    }
}