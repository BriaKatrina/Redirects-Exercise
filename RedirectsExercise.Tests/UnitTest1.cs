using System;
using System.Collections.Generic;
using Xunit;
using RedirectsExercise;

namespace RedirectsExercise.Tests;

public class UnitTest1
    {
        // note:
        // this is all just temp tests to make sure Xunit works

        [Fact]
        public void TestRoutes()
        {
            IEnumerable<string> testRoutes = new string[]  // this is temp input data
            {
                "/home",
                "/home",
                "/our-ceo.html -> /about-us.html",
                "/about-us.html -> /about",
                "/test1 -> /test2",
                "/test2 -> /test3",
                "/test3 -> /test4",
                "/product-1.html -> /seo"
            };

            IEnumerable<string> testNewRoutes = new string[]  // this is temp data
            {
                "/home",
                "/our-ceo.html -> /about-us.html -> /about",
                "/test1 -> /test2 -> /test3 -> /test4",
                "/product-1.html -> /seo"
            };

            MyRouteAnalyzer myRouteAnalyzer = new MyRouteAnalyzer();
            IEnumerable<string> newRoutes = myRouteAnalyzer.Process(testRoutes);

            Assert.Equal(newRoutes, testNewRoutes);
        }
    }