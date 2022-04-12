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
                "/test3 -> /test4",
                "/test2 -> /test3",
                "/test1 -> /test2",
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
            List<string[]> routeData = new List<string[]>();
            foreach (var (route, i) in routes.Select((value, i) => (value, i)))
            {
                routeData.Add(route.Split(delimiter));
            }

            // check for circular reference exception
            checkCircularReference(routeData);

            // process route redirects
            List<string[]> processedRouteData = BulkProcessRedirect(routeData);
            //List<string[]> processedRouteData = BulkProcessRedirectOrdered(routeData);

            // Show debug messages
            foreach (string[] routePages in processedRouteData)
            {
                foreach (string page in routePages)
                {
                    Console.WriteLine("page: " + page);
                }
            }

            // pack nested array into single string array using the delimiter
            string[] packedRouteData = new string[processedRouteData.Count()];
            foreach (var (routePages, i) in processedRouteData.Select((value, i) => (value, i)))
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

        private List<string[]> BulkProcessRedirect(List<string[]> routeData)
        {
            List<string[]> processedRouteData = new List<string[]>(routeData);
            List<string[]> singleProcessedData = null;
            while ((singleProcessedData = SingleProcessRedirect(processedRouteData)) != null)
            {
                processedRouteData = singleProcessedData;
            }

            return processedRouteData;
        }

        /// <summary>
        ///
        /// </summary>
        /// <returns>The processed List after first found redirect (if no redirect, returns null)</returns>
        private List<string[]> SingleProcessRedirect(List<string[]> routeData)
        {
            List<string[]> processedRouteData = new List<string[]>(routeData);
            List<string> firstValues = new List<string>();
            bool redirectFound = false;

            // list of first values using corresponding index (perhaps this can be optimized further with linq)
            foreach (var routePages in routeData)
            {
                firstValues.Add(routePages.First());
            }

            // iterate through each route's last page looking for a redirect
            foreach (var (routePages, i) in routeData.Select((value, i) => (value, i)))
            {
                string lastPage = routePages.Last();

                var firstIndex = firstValues.IndexOf(lastPage);
                if (!redirectFound && firstIndex != -1 && firstIndex != i)
                {
                    // redirect found! (flag as done)
                    redirectFound = true;
                    Console.WriteLine("lastpage (" + lastPage + ") found at first element index: " + firstIndex);

                    if (i < firstIndex)
                    {
                        processedRouteData.RemoveAt(firstIndex);
                        processedRouteData.RemoveAt(i);
                    }
                    else
                    {
                        processedRouteData.RemoveAt(i);
                        processedRouteData.RemoveAt(firstIndex);
                    }
                    processedRouteData.Add(routePages.Union(routeData[firstIndex]).ToArray());
                    break;
                }
            }

            if (!redirectFound) return null;

            return processedRouteData;
        }

        private List<string[]> BulkProcessRedirectOrdered(List<string[]> routeData)
        {
            string lastPagePrevious = ""; // last page from the previous route
            List<string[]> processedRouteData = new List<string[]>();
            foreach (var (routePages, i) in routeData.Select((value, i) => (value, i)))
            {
                string lastPage = ""; // last page navigated
                List<string> processedRoutePages = new List<string>();
                foreach ((string page, int j) in routePages.Select((value2, j) => (value2, j)))
                {

                    if (page == lastPagePrevious && j == 0) {

                        // NOTE: this only works for routes that follow after the previous route
                        // That being at index "processedRouteData.Count - 1" of the processed data
                        // To get this to work more thoroughly, perhaps use method calls checking all routes?

                        // Console.WriteLine("redirected route found!");

                        // this is where data joins the previous route and the redirected route
                        processedRoutePages =
                            ((string[])processedRouteData[processedRouteData.Count - 1])
                            .Union(routeData[i]).ToList();
                        processedRouteData.RemoveAt(processedRouteData.Count - 1);
                        processedRouteData.Add(processedRoutePages.ToArray());

                        // flag lastPagePrevious as having been redirected
                        lastPagePrevious = "redirected";
                        lastPage = processedRoutePages[processedRoutePages.Count - 1];
                        // Console.WriteLine("redidirected last page: " + lastPage);
                        break;
                    }

                    processedRoutePages.Add(page);

                    lastPage = page;
                }

                // if no previous data manipulation happened, add pages to processedRouteData
                if (lastPagePrevious != "redirected") processedRouteData.Add(processedRoutePages.ToArray());

                lastPagePrevious = lastPage;
            }

            return processedRouteData;
        }

        private void checkCircularReference(List<string[]> routes) {
            // throw exception if circular reference
            // this is technically incomplete, as it relies on internal input redirect order
            // wondering if I should rework SingleProcessRedirect to use something other than Union

            List<string> visited = new List<string>();

            foreach (var (routePage, i) in routes.Select((value, i) => (value, i)))
            {
                foreach (var page in routePage)
                {
                    if (visited.Contains(page)) {}
                    visited.Add(page);
                }

                // quick solution (looking for reversed routes)
                string[] reversedRoute = routePage.Reverse().ToArray();
                if (routes.Any( r => {
                        int index = routes.IndexOf(r);
                        return Enumerable.SequenceEqual(r, reversedRoute) && index != i;
                    }))
                {
                    throw new System.Exception("Circular Exception");
                }
            }

            // (a possible solution: use an array index crawler that scans through redirects)
            // (goal: mimick website redirects and check if specific route location was visited twice)
        }
    }
}