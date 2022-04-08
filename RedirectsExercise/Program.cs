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
            // remove duplicates
            routes = routes.Distinct();

            // process routes into a nested array from the delimiter
            string delimiter = " -> ";
            List<string[]> routePageData = new List<string[]>();
            foreach (var (route, i) in routes.Select((value, i) => (value, i)))
            {
                routePageData.Add(route.Split(delimiter));
            }

            // process page data here
            string lastPagePrevious = ""; // last page from the previous route
            List<string[]> processedPageData = new List<string[]>();
            foreach (var (routePages, i) in routePageData.Select((value, i) => (value, i)))
            {
                string lastPage = ""; // last page navigated
                foreach ((string page, int j) in routePages.Select((value2, j) => (value2, j)))
                {
                    if (page == lastPagePrevious && j == 0) {
                        // this is where data will be joined from the previous route and the redirected route
                        // add this joined data to the processPageData List
                    }

                    // if no other processing happens, add routePages to processedPageData by default

                    lastPage = page;
                }
                lastPagePrevious = lastPage;
            }

            //
            // Console.WriteLine: prints interal processed pages for debugging
            //
            foreach (string[] routePages in routePageData)
            {
                foreach (string page in routePages)
                {
                    Console.WriteLine("page: " + page);
                }
            }

            // pack nested array into single string array using the delimiter
            string[] packedRouteData = new string[routePageData.Count()];
            foreach (var (routePages, i) in routePageData.Select((value, i) => (value, i)))
            {
                string processedRoute = "";
                foreach (string page in routePages)
                {
                    processedRoute += page + delimiter;
                }

                // put processed delimiters into new string array
                packedRouteData[i] = processedRoute.Remove(processedRoute.Count() - delimiter.Count(), delimiter.Count());
            }

            IEnumerable<string> newRoutes = (IEnumerable<string>) packedRouteData;
            return newRoutes;
        }

        private bool checkCircularReference(IEnumerable<string> routes) {
            // check if there is a circular reference in the routes
            return true;
        }
    }
}