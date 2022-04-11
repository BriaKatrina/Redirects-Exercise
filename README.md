# Redirects-Exercise

This exercise tests a custom route analyzer, processing a series of strings in an IEnumerable collection.

---

**Issues being worked on in data-refactoring:**
* Finding a more thorough system for redirecting routes (not just the beginnings/ends of the route strings).
* Cleaner and easier to read/edit code, using new Route objects in a list.
* Optimizing data processing (such as limiting garbage collection from processed List and data objects)

The solution will use a Route object containing properties for the route (stored as string Path) and if it redirects to the next route in the list (stored as bool Redirect). Check the Issues tab for more info.
