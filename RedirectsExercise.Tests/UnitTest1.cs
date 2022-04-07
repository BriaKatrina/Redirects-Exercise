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
            IEnumerable<string> testRoutes = new string[]  // this is temp data
            {
                "/home",
                "/our-ceo.html -> /about-us.html",
                "/about-us.html -> /about",
                "/product-1.html -> /seo"
            };

            IEnumerable<string> testNewRoutes = new string[]  // this is temp data
            {
                "/home",
                "/our-ceo.html -> /about-us.html -> /about",
                "/product-1.html -> /seo"
            };

            MyRouteAnalyzer myRouteAnalyzer = new MyRouteAnalyzer();
            IEnumerable<string> newRoutes = myRouteAnalyzer.Process(testRoutes);

            Assert.Equal(newRoutes, testNewRoutes);
        }
    }