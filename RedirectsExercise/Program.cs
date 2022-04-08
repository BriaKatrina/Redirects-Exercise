using System;
using System.Collections.Generic;
using System.Linq;

namespace RedirectsExercise
{
    public class Program {
        private static void Main()
        {
            MyRouteAnalyzer routeAnalyzer = new MyRouteAnalyzer();
            IEnumerable<string> routes = new string[]  // this is temp input data
            {
                "/home",
                "/home",
                "/our-ceo.html -> /about-us.html",
                "/about-us.html -> /about",
                "/product-1.html -> /seo"
            };

            foreach (var route in routes)
            {
                Console.WriteLine(route);
            }

            Console.WriteLine();
            Console.WriteLine("route pages: ");

            // this is where you pass in string inputs for the routes
            IEnumerable<string> newRoutes = routeAnalyzer.Process(routes);

            Console.WriteLine();
            Console.WriteLine("new routes: ");

            foreach (var route in newRoutes)
            {
                Console.WriteLine(route);
            }
        }
    }

    public class MyRouteAnalyzer : RouteAnalyzer
    {
        public IEnumerable<string> Process(IEnumerable<string> routes)
        {
            string delimiter = " -> ";
            IEnumerable<string> newRoutes = new string[routes.Count()];

            // process routes into a nested array from delimiter
            string[][] routePageData = new string[routes.Count()][];
            foreach (var (route, i) in routes.Select((value, i) => (value, i)))
            {
                routePageData[i] = route.Split(delimiter);
            }

            // process pages data here
            string[][] processedRoutePageData = routePageData // fluent api calls?
                                                    .ToArray();

            //
            // Console.WriteLine: prints interal processed pages for debugging
            //
            foreach (string[] routePages in processedRoutePageData)
            {
                foreach (string page in routePages)
                {
                    Console.WriteLine("page: " + page);
                }
            }

            // pack nested array into single string array using delimiter
            string[] packedRouteData = new string[processedRoutePageData.Count()];
            foreach (var (routePages, i) in processedRoutePageData.Select((value, i) => (value, i)))
            {
                string processedRoute = "";
                foreach (string page in routePages)
                {
                    processedRoute += page + delimiter;
                }

                // put processed delimiters into new string array
                packedRouteData[i] = processedRoute.Remove(processedRoute.Count() - delimiter.Count(), delimiter.Count());
            }

            newRoutes = (IEnumerable<string>) packedRouteData;
            return newRoutes;
        }

        private bool isCircularReference(IEnumerable<string> routes) {
            // check if there is a circular reference in the routes
            return false;
        }
    }
}