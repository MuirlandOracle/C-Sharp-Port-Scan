# C-Sharp-Port-Scan
## Simple C# Port Scanner
**Note:** This tool is still very much in Beta. It was put together very quickly (as my first C# program) for a very specific use-case and so will need further work (plus cleanup) before being put to more general use. It's being committed just now for version control and so I don't forget about it.


## To-do:
- [ ] Find a slightly better method of threading in C# that doesn't involve setting a static upper-limit on the number of threads (tried `async` and `Parallel.ForEach()` already)
- [ ] Find and add an actual argparsing library (keeping it static)
