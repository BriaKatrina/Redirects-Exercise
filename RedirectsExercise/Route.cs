using System;

namespace RedirectsExercise
{
    public class Route
    {
        public string Path { get; set; }
        public bool Redirect { get; set; }

        public Route(string path, bool redirect)
        {
            Path = path;
            Redirect = redirect;
        }
    }
}