﻿using System;
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
            List<string[]> routeData = new List<string[]>();
            foreach (var (route, i) in routes.Select((value, i) => (value, i)))
            {
                routeData.Add(route.Split(delimiter));
            }

            // process page data here (note: only currently works for one redirect, needs to be fixed)
            string lastPagePrevious = ""; // last page from the previous route
            List<string[]> processedRouteData = new List<string[]>();
            foreach (var (routePages, i) in routeData.Select((value, i) => (value, i)))
            {
                string lastPage = ""; // last page navigated
                List<string> processedRoutePages = new List<string>();
                foreach ((string page, int j) in routePages.Select((value2, j) => (value2, j)))
                {

                    if (page == lastPagePrevious && j == 0) {
                        // this is where data joins the previous route and the redirected route
                        Console.WriteLine("redirected route found!");
                        processedRoutePages = ((string[])routeData[i - 1]).Union(routeData[i]).ToList();
                        processedRouteData.RemoveAt(i - 1);
                        processedRouteData.Add(processedRoutePages.ToArray());
                        lastPage = "redirected";
                        break;
                    }

                    // if no other processing happened, add page to processedRoutePages
                    processedRoutePages.Add(page);

                    lastPage = page;
                }

                if (lastPage != "redirected") processedRouteData.Add(processedRoutePages.ToArray());

                lastPagePrevious = lastPage;
            }

            //
            // Console.WriteLine: prints interal processed pages for debugging
            //
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

        private bool checkCircularReference(IEnumerable<string> routes) {
            // check if there is a circular reference in the routes
            return true;
        }
    }
}