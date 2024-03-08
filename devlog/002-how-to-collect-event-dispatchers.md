Problem: Collect information about components that dispatch events to allow to display it in a diagram as a source of event

Don't know if I can do it with current approach, so googled and found this presentation https://www.youtube.com/watch?v=hf8hzGb2C6E&ab_channel=NDCConferences
Looks like I should use c# analyzers for this. Question is: can it analyze source at runtime? If not, then I need to collect this information at build time and serve it from API in response.
Will try to make it live, but if not, static file generated with build will be also fine, I think even so, other information that should be collected at runtime could use other methods.

As for now, I will dive into Roslyn and learn the basics.

I started with https://learn.microsoft.com/en-us/dotnet/csharp/roslyn-sdk/

The goal is to recreate actual functionality with Roslyn and add 'event dispatchers' to diagram, to see where they are coming from (both internal events and external events from message broker)

Found that https://www.youtube.com/watch?v=0oXAZQBHH70&ab_channel=NDCConferences and it shows how to start with code.
