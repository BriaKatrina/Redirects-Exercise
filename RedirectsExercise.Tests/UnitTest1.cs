using System;
using System.Security.Authentication;
using System.Collections.Generic;
using System.Linq;
using Xunit;
using RedirectsExercise;

namespace RedirectsExercise.Tests;

public class UnitTest1
    {
        // note:
        // this is all just temp tests to make sure Xunit works

        [Fact]
        public void TestOrderedRoutes()
        {
            IEnumerable<string> testRoutes = new string[]  // this is temp input data
            {
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

            newRoutes = newRoutes.OrderBy(s => s, StringComparer.CurrentCultureIgnoreCase);
            testNewRoutes = testNewRoutes.OrderBy(s => s, StringComparer.CurrentCultureIgnoreCase);

            Assert.Equal(newRoutes, testNewRoutes);
        }

        [Fact]
        public void TestUnorderedRoutes()
        {
            IEnumerable<string> testRoutes = new string[]  // this is temp input data
            {
                "/home",
                "/about-us.html -> /about",
                "/home",
                "/our-ceo.html -> /about-us.html",
                "/test3 -> /test4",
                "/test2 -> /test3",
                "/test1 -> /test2",
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

            newRoutes = newRoutes.OrderBy(s => s, StringComparer.CurrentCultureIgnoreCase);
            testNewRoutes = testNewRoutes.OrderBy(s => s, StringComparer.CurrentCultureIgnoreCase);

            Assert.Equal(newRoutes, testNewRoutes);
        }

        [Fact]
        public void TestCircularException()
        {
            IEnumerable<string> testRoutes = new string[]  // this is temp input data
            {
                "/home",
                "/about-us.html -> /about",
                "/about -> /about-us.html",
                "/product-1.html -> /seo"
            };

            MyRouteAnalyzer myRouteAnalyzer = new MyRouteAnalyzer();

            var exception = Assert.Throws<Exception>(() => myRouteAnalyzer.Process(testRoutes));
            Assert.Equal("Circular Exception", exception.Message);
        }
    }