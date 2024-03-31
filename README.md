# Udup
> Unified documentation (???) project - just some buzzwords that make it look like this word stands for something.

## General Information
- Generate nice living documentation from code

## How it should work?
To use this tool you will need to first, run analyzer on your solution (can be manually, or automate it) and ship result file with as embedded resource.


## Road map

- [x] Display events and event handlers in a diagram
- [x] Find a way to collect information about places where events are dispatched
  - [ ] Get trace of all invocations that leads to event dispatch (e.g. Controller => Service => AggregateRoot => Method => Event)
- [x] Find a way to generate udup data with building app (maybe use source generator?)
- [ ] Split gathering data to build time and processing it at runtime
  - [ ] Analyze codebase and save result to file (Roslyn)
  - [ ] Process file to generate documentation at runtime
- [ ] Add selectors to events and event handlers in configuration