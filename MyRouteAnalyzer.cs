using System;
using System.Data;
using System.Data.SqlTypes;

namespace redirects_exercises
{
    class MyRouteAnalyzer : RouteAnalyzer
    {
        private static void Main()
        {
            MyRouteAnalyzer routeAnalyzer = new MyRouteAnalyzer();
            IEnumerable<string> routes = new string[]  // this is temp data
            {
                "/home",
                "/our-ceo.html -> /about-us.html",
                "/about-us.html -> /about",
                "/product-1.html -> /seo"
            };

            foreach (var route in routes)
            {
                Console.WriteLine(route);
            }

            // this is where you pass in string inputs for the routes
            IEnumerable<string> newRoutes = routeAnalyzer.Process(routes);

            foreach (var route in newRoutes)
            {
                Console.WriteLine(route);
            }
        }

        public IEnumerable<string> Process(IEnumerable<string> routes)
        {
            IEnumerable<string> newRoutes = new string[] {};

            // the route processing will be done here.
            return newRoutes;
        }
    }
}