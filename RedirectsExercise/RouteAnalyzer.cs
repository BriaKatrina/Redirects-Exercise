using System;
using System.Collections.Generic;

namespace RedirectsExercise {
    interface RouteAnalyzer
    {
        IEnumerable<string> Process(IEnumerable<string> routes);
    }
}