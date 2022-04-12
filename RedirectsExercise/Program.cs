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
            // IEnumerable<string> routes = new string[]  // this is temp input data
            // {
            //     "/home",
            //     "/about -> /about-us.html",
            //     "/about-us.html -> /about",
            //     "/product-1.html -> /seo"
            // };

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

                //
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
            bool redirectFound = false;
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
                    if (redirectRoute != null)
                    {
                        redirectFound = true;
                        int redirectIndex = routeData.IndexOf(redirectRoute);

                        int iLast = i;
                        for (int _i = i; _i < routeData.Count; _i++)
                        {
                            if (!routeData[_i].Redirect)
                            {
                                iLast = _i;
                                _i = routeData.Count;
                            }
                        }

                        Console.WriteLine(iLast - i + 1);
                        List<Route> routesToInsert = routeData.GetRange(i, iLast - i + 1);

                        foreach (Route r in routesToInsert)
                        {
                            Console.WriteLine("r: " + r.Path);
                        }

                        // Yes! Redirect found!
                        if (routesToInsert.Count > 0 && redirectIndex != i)
                        {
                            redirectRoute.Redirect = true;

                            routeData.InsertRange(redirectIndex + 1, routesToInsert);

                            foreach (Route r in routesToInsert)
                            {
                                Console.Write(r.Path + ", ");

                                r.Redirect = true;
                                routeData.Remove(r);
                            }
                            routesToInsert.Last().Redirect = false;
                            routeData.Remove(route);

                            Console.WriteLine("/*****");

                            return true;
                        }
                    }
                }

                isFirst = !route.Redirect;
            }

            return redirectFound;
        }

        private void CheckCircularReference(List<Route> routeData)
        {

        }
    }
}