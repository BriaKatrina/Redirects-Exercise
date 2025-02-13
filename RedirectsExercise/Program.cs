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
                "/test3 -> /test4",
                "/test2 -> /test3",
                "/test1 -> /test2",
                "/product-1.html -> /product-2.html -> /seo"
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

            // process routes into Route objects from the delimiter
            string delimiter = " -> ";
            List<Route> routeData = new List<Route>();
            foreach (var (route, i) in routes.Select((value, i) => (value, i)))
            {
                string[] nestedRoutes = route.Split(delimiter);
                foreach (string nestedPath in nestedRoutes)
                {
                    routeData.Add(new Route(nestedPath, true));
                }
                routeData.Last().Redirect = false;
            }

            // check for circular reference exception
            CheckCircularReference(routeData);

            // process route redirects
            BulkProcessRedirect(routeData);

            // Show debug messages
            foreach (Route route in routeData)
            {
                Console.WriteLine("Path: " + route.Path + ", Redirect: " + route.Redirect);
            }

            // pack route paths into a string list with the delimiter
            List<string> packedRouteData = new List<string>();
            int packedRouteIndex = 0; // index for new list
            string packedRoute = ""; // iteratively adds paths to
            foreach (var (route, i) in routeData.Select((value, i) => (value, i)))
            {
                packedRoute += route.Path + delimiter;

                if (!route.Redirect)
                {
                    packedRouteData.Add(packedRoute.Remove(packedRoute.Count() - delimiter.Count(), delimiter.Count()));
                    packedRoute = "";
                    packedRouteIndex++;
                }
            }

            return (IEnumerable<string>) packedRouteData;
        }

        private void BulkProcessRedirect(List<Route> routeData)
        {
            bool redirected = true; // let's assume that it need redirecting
            while (redirected)
            {
                redirected = SingleProcessRedirect(routeData);
            }
        }

        private bool SingleProcessRedirect(List<Route> routeData)
        {
            bool isFirst = true;
            for (int i = 0; i < routeData.Count; i++)
            {
                Route route = routeData[i];

                if (isFirst && route.Redirect)
                {
                    // search for redirect
                    Route redirectRoute = routeData
                        .Where(r => !r.Redirect && r.Path == route.Path)
                        .SingleOrDefault();

                    // redirect found...?
                    if (redirectRoute != null && !Object.ReferenceEquals(redirectRoute, route))
                    {
                        // yes, redirect found!
                        int redirectIndex = routeData.IndexOf(redirectRoute);
                        int iLast = FindLastIndexFromRoutes(i, routeData);
                        List<Route> routesToInsert = routeData.GetRange(i, iLast - i + 1);

                        routeData.InsertRange(redirectIndex + 1, routesToInsert);

                        foreach (Route r in routesToInsert)
                        {
                            r.Redirect = true;
                            routeData.Remove(r);
                        }
                        redirectRoute.Redirect = true;
                        routesToInsert.Last().Redirect = false;
                        routeData.Remove(route);

                        return true;
                    }
                }

                isFirst = !route.Redirect;
            }

            return false;
        }

        private void CheckCircularReference(List<Route> routeData)
        {
            List<Route> firstRoutes = new List<Route>(); // find first routes of redirect sequences
            bool isFirst = true;
            foreach (var (route, i) in routeData.Select((value, i) => (value, i)))
            {
                if (isFirst && route.Redirect) {
                    firstRoutes.Add(route);
                }

                isFirst = !route.Redirect;
            }

            for (int i = 0; i < routeData.Count; i++)
            {
                Route route = routeData[i];

                if (!route.Redirect) // is last route in sequence, see if it redirects
                {
                    Route matchingRoute = firstRoutes
                        .Where(r => r.Path == route.Path)
                        .FirstOrDefault();

                    if (matchingRoute != null) // found redirect!
                    {
                        int matchingRouteFirstIndex = routeData.IndexOf(matchingRoute);
                        int matchingRouteLastIndex = FindLastIndexFromRoutes(matchingRouteFirstIndex, routeData);
                        Route newRoute = routeData[matchingRouteLastIndex];

                        Route newMatchingRoute = firstRoutes
                            .Where(r => r.Path == newRoute.Path)
                            .FirstOrDefault();

                        if (newMatchingRoute != null) // ...another redirect found!
                        {
                            int newMatchingRouteFirstIndex = routeData.IndexOf(newMatchingRoute);
                            int newMatchingRouteLastIndex = FindLastIndexFromRoutes(newMatchingRouteFirstIndex, routeData);
                            Route potentialCircularRoute = routeData[newMatchingRouteLastIndex];

                            if (newMatchingRouteLastIndex == i) // okay, this is a circular loop
                            {
                                throw new System.Exception("Circular Exception");
                            }
                        }
                    }
                }
            }
        }

        private int FindLastIndexFromRoutes(int startIndex, List<Route> routeData)
        {
            int iLast = startIndex;
            for (int _i = startIndex; _i < routeData.Count; _i++)
            {
                if (!routeData[_i].Redirect)
                {
                    iLast = _i; // this will be the new last index of the redirected route
                    break;
                }
            }
            return iLast;
        }
    }
}